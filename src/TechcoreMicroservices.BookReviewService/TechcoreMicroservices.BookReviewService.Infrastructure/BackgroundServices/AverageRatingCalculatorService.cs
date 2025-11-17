using DnsClient.Internal;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TechcoreMicroservices.BookReviewService.Application.Common.Settings;
using TechcoreMicroservices.BookReviewService.Domain.MongoEntities;

namespace TechcoreMicroservices.BookReviewService.Infrastructure.BackgroundServices;

public class AverageRatingCalculatorService : BackgroundService
{
    private readonly IMongoClient _mongoClient;
    private readonly MongoSettings _mongoSettings;

    private readonly IDistributedCache _distributedCache;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly ILogger<AverageRatingCalculatorService> _logger;

    private readonly TimeSpan _period = TimeSpan.FromMinutes(5);

    public AverageRatingCalculatorService(IMongoClient mongoClient,
        IDistributedCache distributedCache,
        IOptions<MongoSettings> options,
        ILogger<AverageRatingCalculatorService> logger)
    {
        _mongoClient = mongoClient;
        _distributedCache = distributedCache;
        _mongoSettings = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AverageRatingCalculatorService started.");

        while(!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var database = _mongoClient.GetDatabase(_mongoSettings.DatabaseName);
                var collection = database.GetCollection<BookReview>("reviews");

                var pipeline = collection.Aggregate()
                    .Group(r => r.BookId, g => new AverageRatingDto(
                        g.Key, 
                        g.Average(x => x.Rating))
                );

                var ratings = await pipeline.ToListAsync(stoppingToken);
                foreach(var rating in ratings)
                {
                    var key = $"rating:{rating.BookId}";

                    var avgStr = rating.Average.ToString("F2", CultureInfo.InvariantCulture);

                    await WriteInRedis(key, avgStr, TimeSpan.FromMinutes(10), stoppingToken);

                    _logger.LogInformation($"Updated {key} = {avgStr}");
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calculating/storing average ratings. Will retry after delay.");
            }

            try
            {
                await Task.Delay(_period, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("AverageRatingCalculatorService stoped");
    }

    private async Task WriteInRedis(string key, string avgStr, TimeSpan tts, CancellationToken cancellationToken)
    {
        var options = new DistributedCacheEntryOptions();
        options.SetAbsoluteExpiration(tts);

        var json = JsonSerializer.Serialize(avgStr, _jsonSerializerOptions);
        await _distributedCache.SetStringAsync(key, json, cancellationToken);
    }

    public record AverageRatingDto(Guid BookId, double Average);
}
