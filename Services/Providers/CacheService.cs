using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<CacheService> _logger;

    public CacheService(IDistributedCache distributedCache, IConnectionMultiplexer redis, ILogger<CacheService> logger)
    {
        _distributedCache = distributedCache;
        _redis = redis;
        _logger = logger;
        _defaultOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        _logger.LogDebug("Attempting to retrieve from cache with key: {CacheKey}", key);
        var cachedValue = await _distributedCache.GetStringAsync(key);
        if (string.IsNullOrEmpty(cachedValue))
        {
            _logger.LogDebug("Cache miss for key: {CacheKey}", key);
            return default;
        }

        _logger.LogDebug("Cache hit for key: {CacheKey}", key);
        return JsonSerializer.Deserialize<T>(cachedValue);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration)
    {
        _logger.LogDebug("Setting cache for key: {CacheKey} with expiration: {Expiration} minutes", key, expiration?.TotalMinutes ?? 5);

        var options = expiration.HasValue ? new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration } : _defaultOptions;

        var serializedValue = JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, serializedValue, options);
    }

    public async Task RemoveAsync(string key)
    {
        _logger.LogDebug("Removing cache entry with key: {CacheKey}", key);
        await _distributedCache.RemoveAsync(key);
    }

    public async Task RemoveByPrefixAsync(string prefix)
    {
        _logger.LogInformation("Removing cache entries with prefix: {Prefix}", prefix);
        var database = _redis.GetDatabase();
        var server = _redis.GetServer(_redis.GetEndPoints().First());

        // The keys in Redis include the instance name prefix
        var fullPrefix = $"{prefix}*";

        // Use SCAN to iterate through keys with the prefix
        var keys = server.Keys(pattern: fullPrefix, pageSize: 100).ToArray();

        if (keys.Length > 0)
        {
            _logger.LogInformation("Removing {Count} cache entries with prefix: {Prefix}", keys.Length, prefix);
            await database.KeyDeleteAsync(keys);
        }
        
        _logger.LogDebug("No cache entries found with prefix: {Prefix}", prefix);
    
    }
}