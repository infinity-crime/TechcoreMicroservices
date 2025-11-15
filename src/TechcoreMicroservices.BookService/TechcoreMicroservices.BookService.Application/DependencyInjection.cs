using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
        services.AddHttpClient<IBookReviewHttpService, BookReviewHttpService>(client =>
        {
            client.BaseAddress = new Uri("https://localhost:7227");
        });
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
