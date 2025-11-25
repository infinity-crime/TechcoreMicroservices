using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookOrderService.Application.Common.Interfaces.Persistence;
using TechcoreMicroservices.BookOrderService.Contracts.Commands;
using TechcoreMicroservices.BookOrderService.Infrastructure.Data;
using TechcoreMicroservices.BookOrderService.Infrastructure.Data.Repositories;

namespace TechcoreMicroservices.BookOrderService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDbContext(services, configuration);

        AddRepositories(services);

        AddMassTransit(services, configuration);

        return services;
    }

    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        var connectionStringDb = configuration.GetConnectionString("DefaultConnection");

        // Регистрация контекста базы данных с использованием Npgsql
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionStringDb);
        });
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
    }

    private static void AddMassTransit(IServiceCollection services, IConfiguration configuration)
    {
        // Регистрация MassTransit
        var rabbitUsername = configuration["RabbitMqSettings:Username"];
        var rabbitPassword = configuration["RabbitMqSettings:Password"];

        services.AddMassTransit(options =>
        {
            options.UsingRabbitMq((context, config) =>
            {
                config.Host("rabbitmq", 5672, "/", cfg =>
                {
                    cfg.Username(rabbitUsername!);
                    cfg.Password(rabbitPassword!);
                });

                config.ConfigureEndpoints(context);
            });

            options.AddRequestClient<CheckBookAvailabilityCommand>();
        });
    }
}
