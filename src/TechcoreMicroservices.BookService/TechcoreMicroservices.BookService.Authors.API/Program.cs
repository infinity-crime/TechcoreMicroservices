using FluentValidation;
using FluentValidation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TechcoreMicroservices.BookService.Application;
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

    // OpenTelemetry with Zipkin
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource
            .AddService(serviceName: "book-service-authors"))
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddZipkinExporter(options =>
            {
                options.Endpoint = new Uri("http://zipkin:9411/api/v2/spans");
            }));
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
    app.MapControllers();
    app.Run();
}
