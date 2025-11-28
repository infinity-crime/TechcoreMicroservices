using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
{
    var jwtSection = builder.Configuration.GetSection("JwtSettings");

    builder.Services.AddHttpClient("books", o =>
    {
        o.BaseAddress = new Uri("http://book-service-books:8081");
    });
    builder.Services.AddHttpClient("reviews", o =>
    {
        o.BaseAddress = new Uri("http://book-review-service:8081");
    });

    builder.Services.AddReverseProxy()
        .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Secret"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var has = context.Request.Headers.ContainsKey("Authorization");
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtAuth");

                logger.LogInformation("OnMessageReceived. Authorization header present: {HasHeader}", has);

                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                   .CreateLogger("JwtAuth");

                logger.LogError(context.Exception, $"Authentication Failed: {context.Exception.Message}");
                if (context.Exception.InnerException != null)
                {
                    logger.LogError($"Inner exception: {context.Exception.InnerException.Message}");
                }

                return Task.CompletedTask;
            }
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Bearer", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.AuthenticationSchemes.Add("Bearer");
        });

        options.AddPolicy("AdminOnly", policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole("Admin");
            policy.AuthenticationSchemes.Add("Bearer");
        });
    });

    // Инструментирование OpenTelemetry
    var serviceName = builder.Configuration["OTelSettings:ServiceName"] ?? "api-gateway";
    var otel = builder.Services.AddOpenTelemetry();

    otel.ConfigureResource(resource => resource
        .AddService(serviceName: serviceName));

    otel.WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddZipkinExporter(options =>
        {
            options.Endpoint = new Uri("http://zipkin:9411/api/v2/spans");
        }));

    otel.WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter());
}

var app = builder.Build();
{
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapReverseProxy();

    app.MapPrometheusScrapingEndpoint();

    app.MapGet("/details/{id}", async (Guid id, IHttpClientFactory httpFactory, HttpContext httpContext) =>
    {
        string? authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
        string? token = null;

        if (!string.IsNullOrWhiteSpace(authHeader))
        {
            const string bearerPrefix = "Bearer ";
            token = authHeader[bearerPrefix.Length..].Trim();
        }

        if (string.IsNullOrEmpty(token))
            return Results.Unauthorized();

        var bookClient = httpFactory.CreateClient("books");
        var reviewClient = httpFactory.CreateClient("reviews");

        var bookRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/books/{id}");
        bookRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var reviewRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/reviews/book/{id}");

        var bookResponse = await bookClient.SendAsync(bookRequest);
        var reviewResponse = await reviewClient.SendAsync(reviewRequest);

        if (!bookResponse.IsSuccessStatusCode)
            return TypedResults.InternalServerError();

        if (!reviewResponse.IsSuccessStatusCode)
            return Results.InternalServerError();

        var book = await bookResponse.Content.ReadFromJsonAsync<object>();
        var reviews = await reviewResponse.Content.ReadFromJsonAsync<object>();

        return Results.Json(new { Book = book, Reviews = reviews });
    });

    app.Run();
}
