using TechcoreMicroservices.BookOrderService.Application;
using TechcoreMicroservices.BookOrderService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddApplication();

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(opt =>
    {
        opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Orders API",
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