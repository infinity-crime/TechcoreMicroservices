using TechcoreMicroservices.BookService.Application;
using TechcoreMicroservices.BookService.Books.API.Middleware;
using TechcoreMicroservices.BookService.Infrastructure;
using FluentValidation.AspNetCore;
using FluentValidation;
using TechcoreMicroservices.BookService.Application.Common.Settings;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();

    // Читаем конфигурацию с appsettings в наши POCO классы из слоя Application
    builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

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
    });
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
    app.UseAuthorization();
    app.MapControllers();

    app.MapHealthChecks("/healthz");

    app.Run();
}
