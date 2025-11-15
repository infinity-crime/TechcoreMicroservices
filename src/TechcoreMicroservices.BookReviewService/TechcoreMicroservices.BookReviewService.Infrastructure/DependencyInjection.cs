using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechcoreMicroservices.BookReviewService.Application.Common.Interfaces.Persistence;
using TechcoreMicroservices.BookReviewService.Infrastructure.BackgroundServices;
using TechcoreMicroservices.BookReviewService.Infrastructure.Data.Repositories;

namespace TechcoreMicroservices.BookReviewService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddMongoDb(services, configuration);

        AddRedisCaching(services, configuration);

        AddRepositories(services);

        services.AddHostedService<AverageRatingCalculatorService>();

        return services;
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IBookReviewRepository, BookReviewRepository>();
    }

    private static void AddMongoDb(IServiceCollection services, IConfiguration configuration)
    {
        // Регистрация клиента MongoDB
        var mongoConnectionString = configuration["Mongo:MongoConnectionString"];
        var mongoDatabaseName = configuration["Mongo:DatabaseName"];

        services.AddSingleton<IMongoClient>(new MongoClient(mongoConnectionString));
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
    }
}
