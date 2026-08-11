using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using StackExchange.Redis;
using System.Text.Json;
using Todo.Model;
using Todo.Services.Interfaces;

namespace Todo.Services.Providers;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly DistributedCacheEntryOptions _defaultOptions;
    private readonly IConnectionMultiplexer _redis;

    public CacheService(IDistributedCache distributedCache, IConnectionMultiplexer redis)
    {
        _distributedCache = distributedCache;
        _redis = redis;
        _defaultOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var cachedValue = await _distributedCache.GetStringAsync(key);
        if (string.IsNullOrEmpty(cachedValue))
            return default;

        return JsonSerializer.Deserialize<T>(cachedValue);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration)
    {
        var options = expiration.HasValue ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration } : _defaultOptions;

        var serializedValue = JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, serializedValue, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
    }

    public async Task RemoveByPrefixAsync(string prefix)
    {
        var database = _redis.GetDatabase();
        var server = _redis.GetServer(_redis.GetEndPoints().First());
        
        // The keys in Redis include the instance name prefix
        var fullPrefix = $"{prefix}*";
        
        // Use SCAN to iterate through keys with the prefix
        var keys = server.Keys(pattern: fullPrefix, pageSize: 100).ToArray();
        
        if (keys.Length > 0)
        {
            await database.KeyDeleteAsync(keys);
        }
    }
}