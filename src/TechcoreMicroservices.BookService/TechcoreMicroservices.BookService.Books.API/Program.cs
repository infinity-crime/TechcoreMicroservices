using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using TechcoreMicroservices.BookService.Application;
using TechcoreMicroservices.BookService.Application.Common.Settings;
using TechcoreMicroservices.BookService.Books.API.Middleware;
using TechcoreMicroservices.BookService.Infrastructure;
using Npgsql;
using Serilog;
using Confluent.Kafka.Extensions.OpenTelemetry;
using Serilog.Sinks.Grafana.Loki;
using TechcoreMicroservices.BookService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration
       .AddJsonFile("/app/config/appsettings.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables();

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

    var serviceName = builder.Configuration["OTelSettings:ServiceName"] ?? "book-service-books";

    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.GrafanaLoki("http://loki:3100")
        .CreateLogger();

    builder.Host.UseSerilog();


    // Инструментирование OpenTelemetry
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

    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();
        app.Logger.LogInformation("------------Migrate succsess!----------------");

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roleNames = { "DefaultUser", "Admin" };

        foreach(var role in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        app.Logger.LogInformation("------------Role succsess!----------------");
    }
    catch (Exception)
    {
        app.Logger.LogError("------------Migrate failed!----------------");
    }

    app.UseMiddleware<ExceptionHandlerMiddleware>();
    app.UseMiddleware<RequestTimingMiddleware>();

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.MapPrometheusScrapingEndpoint();

    app.MapHealthChecks("/healthz");

    try
    {
        Log.Information("Starting service: book-service-books");
        app.Run();
    }
    catch(Exception ex)
    {
        Log.Fatal(ex, "Host terminated unexpectedly");
        throw;
    }
    finally
    {
        Log.CloseAndFlush();
    }
}
