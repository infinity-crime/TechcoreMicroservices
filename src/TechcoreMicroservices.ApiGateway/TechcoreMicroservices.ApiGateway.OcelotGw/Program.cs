using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration.AddEnvironmentVariables(); // добавим возможность перегрузки настроек через env контейнера

    builder.Services.AddOcelot(builder.Configuration);
}

var app = builder.Build();
{
    await app.UseOcelot();

    await app.RunAsync();
}
