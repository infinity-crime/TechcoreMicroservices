using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;
using System.Diagnostics;
using OpenTelemetry.Metrics;
using TechcoreMicroservices.BookService.Application;
using TechcoreMicroservices.BookService.Application.Common.Settings;
using TechcoreMicroservices.BookService.Books.API.Middleware;
using TechcoreMicroservices.BookService.Infrastructure;
using Npgsql;
using Confluent.Kafka.Extensions.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication(builder.Configuration);

    // Читаем конфигурацию с appsettings в наши POCO классы из слоя Application
    builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
    builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaSettings:Book"));

    // Регистрация FluentValidation и кастомных валидаторов
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    // Регистрация проверки работоспособности для K8s
    builder.Services.AddHealthChecks();

    builder.Services.AddControllers();

    // Регистрация Swagger документации
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Books API",
            Version = "v1",
            Description = "Simple REST-API made as an internship in Techcore.",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Kirill Zhestkov",
                Email = "kirillzhestkov78@gmail.com"
            }
        });

        opt.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter the token in the format: Bearer {your_token}"
        });

        opt.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    });

    // Инструментирование OpenTelemetry
    var serviceName = builder.Configuration["OTelSettings:ServiceName"] ?? "book-service-books";
    var otel = builder.Services.AddOpenTelemetry();

    otel.ConfigureResource(resource => resource
        .AddService(serviceName: serviceName));

    otel.WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
        .AddNpgsql()
        .AddConfluentKafkaInstrumentation()
        .AddZipkinExporter(options =>
        {
            options.Endpoint = new Uri("http://zipkin:9411/api/v2/spans");
        }));

    otel.WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddMeter("BookServiceMetrica.Books")
        .AddPrometheusExporter());
}

var app = builder.Build();
{
    if(app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionHandlerMiddleware>();
    app.UseMiddleware<RequestTimingMiddleware>();

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.MapPrometheusScrapingEndpoint();

    app.MapHealthChecks("/healthz");

    app.Run();
}
