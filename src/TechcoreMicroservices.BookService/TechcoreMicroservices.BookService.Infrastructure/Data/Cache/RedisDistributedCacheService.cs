using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TechcoreMicroservices.BookService.Application.Common.Interfaces.Persistence.Redis;

namespace TechcoreMicroservices.BookService.Infrastructure.Data.Cache;

public class RedisDistributedCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public RedisDistributedCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _cache.GetStringAsync(key);
        return json is null ? default : JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
    }

    public async Task RemoveAsync(string key) => await _cache.RemoveAsync(key);

    public async Task SetAsync<T>(string key, T value, TimeSpan? tts = null)
    {
        var options = new DistributedCacheEntryOptions();
        if (tts.HasValue)
            options.SetSlidingExpiration(tts.Value);

        var json = JsonSerializer.Serialize(value, _jsonSerializerOptions);
        await _cache.SetStringAsync(key, json, options);
    }
}
