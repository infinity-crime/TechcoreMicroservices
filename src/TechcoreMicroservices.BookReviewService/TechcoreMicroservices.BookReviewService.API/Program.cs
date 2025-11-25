using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using TechcoreMicroservices.BookReviewService.Application;
using TechcoreMicroservices.BookReviewService.Application.Common.Settings;
using TechcoreMicroservices.BookReviewService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();

    builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("Mongo"));

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Reviews API",
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
            .AddService(serviceName: "book-review-service"))
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

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
