using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Kubernetes;
using TechcoreMicroservices.ApiGateway.OcelotGw.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration
        .SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables()
        .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

    builder.Services.AddBearerAuth(builder.Configuration);

    builder.Services.AddOcelot(builder.Configuration)
        .AddKubernetes();
}

var app = builder.Build();
{
    app.Use(async (context, next) =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("=== INCOMING REQUEST HEADERS ===");
        foreach(var header in context.Request.Headers)
        {
            logger.LogInformation($"Header: {header.Key} = {header.Value}");
        }
        logger.LogInformation("=== END HEADERS ===");

        logger.LogInformation($"Method: {context.Request.Method}, Path: {context.Request.Path}");

        await next();
    });

    app.UseAuthentication();
    app.UseAuthorization();

    await app.UseOcelot();

    await app.RunAsync();
}
