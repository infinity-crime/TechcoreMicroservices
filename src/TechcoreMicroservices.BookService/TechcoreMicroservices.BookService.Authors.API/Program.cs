using FluentValidation;
using FluentValidation.AspNetCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Instrumentation.AspNetCore;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Services;
using TechcoreMicroservices.BookService.Application.Services;
using TechcoreMicroservices.BookService.Authors.API.Middleware;
using TechcoreMicroservices.BookService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddScoped<IAuthorService, AuthorService>();

    // Регистрация FluentValidation и кастомных валидаторов
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

    builder.Services.AddControllers();

    // Регистрация Swagger документации
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Authors API",
            Version = "v1",
            Description = "Simple REST-API made as an internship in Techcore.",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "Kirill Zhestkov",
                Email = "kirillzhestkov78@gmail.com"
            }
        });
    });

    // Инструментирование OpenTelemetry
    var serviceName = builder.Configuration["OTelSettings:ServiceName"] ?? "book-service-authors";
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
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseMiddleware<ExceptionHandlerMiddleware>();

    app.UseHttpsRedirection();
    app.UseAuthorization();

    app.MapPrometheusScrapingEndpoint();

    app.MapControllers();
    app.Run();
}
