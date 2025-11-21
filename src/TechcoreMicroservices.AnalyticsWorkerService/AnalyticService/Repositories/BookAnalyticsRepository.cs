using AnalyticService.Models;
using AnalyticService.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalyticService.Repositories;

public class BookAnalyticsRepository : IBookAnalyticsRepository
{
    private readonly IMongoCollection<BookAnalytics> _collection;
    private readonly MongoSettings _settings;

    public BookAnalyticsRepository(IMongoClient mongoClient, IOptions<MongoSettings> options)
    {
        _settings = options.Value;

        _collection = mongoClient.GetDatabase(_settings.DatabaseName)
            .GetCollection<BookAnalytics>("book_analytics");
    }

    public async Task AddAsync(BookAnalytics bookAnalytics, CancellationToken cancellationToken)
    {
        await _collection.InsertOneAsync(bookAnalytics, new InsertOneOptions(), cancellationToken);
    }
}
