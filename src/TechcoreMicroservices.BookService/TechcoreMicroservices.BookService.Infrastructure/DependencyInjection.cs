using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.HttpServices;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence.Dapper;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence.Redis;
using TechcoreMicroservices.BookService.Infrastructure.Data;
using TechcoreMicroservices.BookService.Infrastructure.Data.Cache;
using TechcoreMicroservices.BookService.Infrastructure.Data.Repositories.Dapper;
using TechcoreMicroservices.BookService.Infrastructure.Data.Repositories.EFCore;
using Microsoft.AspNetCore.Identity;
using TechcoreMicroservices.BookService.Domain.Entities.Identity;
using MassTransit;
using TechcoreMicroservices.BookService.Infrastructure.RabbitMQ.Consumers;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence.Kafka;
using TechcoreMicroservices.BookService.Infrastructure.Kafka;
using TechcoreMicroservices.BookService.Contracts.Responses.Book;

namespace TechcoreMicroservices.BookService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDbContext(services, configuration);

        AddIdentity(services);

        AddEfCoreRepositories(services);
        AddDapperRepositories(services);

        AddRedisCaching(services, configuration);

        AddMassTransit(services, configuration);

        AddKafkaProducer<BookResponse>(services);

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

    private static void AddEfCoreRepositories(IServiceCollection services)
    {
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<IBookRepository, BookRepository>();
    }

    private static void AddDapperRepositories(IServiceCollection services)
    {
        services.AddScoped<IBookDapperRepository, BookDapperRepository>();
    }

    private static void AddRedisCaching(IServiceCollection services, IConfiguration configuration)
    {
        // Получение настроек Redis из конфигурации
        var redisConnectionString = configuration["Redis:RedisConnectionString"];
        var instanceName = configuration["Redis:InstanceName"];

        // Регистрация кэша Redis
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = instanceName;
        });

        services.AddScoped<ICacheService, RedisDistributedCacheService>();
    }

    private static void AddIdentity(IServiceCollection services)
    {
        services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }

    private static void AddMassTransit(IServiceCollection services, IConfiguration configuration)
    {
        // Регистрация MassTransit
        var rabbitUsername = configuration["RabbitMqSettings:Username"];
        var rabbitPassword = configuration["RabbitMqSettings:Password"];

        services.AddMassTransit(options =>
        {
            options.AddConsumer<CheckBookAvailabilityConsumer>();
            options.AddConsumer<BookReservatorConsumer>();

            options.UsingRabbitMq((context, config) =>
            {
                config.Host("localhost", 5672, "/", cfg =>
                {
                    cfg.Username(rabbitUsername!);
                    cfg.Password(rabbitPassword!);
                });

                config.ConfigureEndpoints(context);
            });
        });
    }

    private static void AddKafkaProducer<TMessage>(IServiceCollection services)
    {
        services.AddSingleton<IKafkaProducer<TMessage>, KafkaProducer<TMessage>>();
    }
}
