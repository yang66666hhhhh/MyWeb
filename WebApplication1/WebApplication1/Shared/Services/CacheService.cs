using Microsoft.Extensions.Caching.Memory;

namespace WebApplication1.Shared.Services;

public interface ICacheService
{
    T? Get<T>(string key);
    Task<T?> GetAsync<T>(string key);
    void Set<T>(string key, T value, TimeSpan? expiration = null);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    void Remove(string key);
    Task RemoveAsync(string key);
    bool Exists(string key);
    Task<bool> ExistsAsync(string key);
}

public class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(30);

    public T? Get<T>(string key)
    {
        return memoryCache.TryGetValue(key, out T? value) ? value : default;
    }

    public Task<T?> GetAsync<T>(string key)
    {
        return Task.FromResult(Get<T>(key));
    }

    public void Set<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? DefaultExpiration,
            SlidingExpiration = TimeSpan.FromMinutes(10),
            Priority = CacheItemPriority.Normal
        };

        memoryCache.Set(key, value, options);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        Set(key, value, expiration);
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        memoryCache.Remove(key);
    }

    public Task RemoveAsync(string key)
    {
        Remove(key);
        return Task.CompletedTask;
    }

    public bool Exists(string key)
    {
        return memoryCache.TryGetValue(key, out _);
    }

    public Task<bool> ExistsAsync(string key)
    {
        return Task.FromResult(Exists(key));
    }
}
