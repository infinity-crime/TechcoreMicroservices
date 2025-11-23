using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Authentication;
using TechcoreMicroservices.BookService.Application.Authorization.Handlers;
using TechcoreMicroservices.BookService.Application.Authorization.Requirements;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Authentication;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.HttpServices;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services.Identity;
using TechcoreMicroservices.BookService.Application.HttpServices;
using TechcoreMicroservices.BookService.Application.Services;
using TechcoreMicroservices.BookService.Application.Services.Identity;
using TechcoreMicroservices.BookService.Contracts.Responses.BookReview;

namespace TechcoreMicroservices.BookService.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddApplicationServices(services);

        // Для общения с микросервисом отзывов по Http
        AddHttpClientFactory(services);

        AddAuthentication(services, configuration);
        AddAuthorization(services);

        return services;
    }

    private static void AddApplicationServices(IServiceCollection services)
    {
        services.AddScoped<IBookService, TechcoreMicroservices.BookService.Application.Services.BookService>();
        services.AddScoped<IAuthorService, AuthorService>();
        services.AddScoped<IBookDetailsService, BookDetailsService>();
    }

    private static void AddHttpClientFactory(IServiceCollection services)
    {
        var fallback = Policy<HttpResponseMessage>
            .Handle<BrokenCircuitException>()
            .FallbackAsync(fallbackAction: async (ct) =>
            {
                var fallbackReview = new BookReviewResponse("unknown", Guid.Empty, "", 0, "", DateTime.Now);

                var fallbackResponse = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(new List<BookReviewResponse> { fallbackReview }), Encoding.UTF8, "application/json")
                };

                return await Task.FromResult(fallbackResponse);
            },
            onFallbackAsync: (ex) =>
            {
                Console.WriteLine($"Fallback triggered: {ex.Exception.Message}");
                return Task.CompletedTask;
            });

        var circuitBreakerPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

        var retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

        var timeoutPolicy = Policy
            .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(3), TimeoutStrategy.Optimistic);

        var policyWrap = Policy.WrapAsync(fallback, circuitBreakerPolicy, retryPolicy, timeoutPolicy);

        services.AddHttpClient<IBookReviewHttpService, BookReviewHttpService>(client =>
        {
            // ???
            client.BaseAddress = new Uri("https://localhost:7227");
        })
        .AddPolicyHandler(policyWrap);
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false,

                ValidIssuer = configuration["JwtSettings:Issuer"],
                ValidAudience = configuration["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"]!))
            };
        });

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
    }

    private static void AddAuthorization(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("OlderThan18", policy => policy.Requirements.Add(new AgeRequirement(18)));
        });

        services.AddSingleton<IAuthorizationHandler, AgeHandler>();
    }
}
