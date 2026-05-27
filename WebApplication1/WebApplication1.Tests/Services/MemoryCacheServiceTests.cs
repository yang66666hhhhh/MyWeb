using Microsoft.Extensions.Caching.Memory;
using WebApplication1.Shared.Services;

namespace WebApplication1.Tests.Services;

public class MemoryCacheServiceTests : IDisposable
{
    private readonly MemoryCache _memoryCache;
    private readonly MemoryCacheService _service;

    public MemoryCacheServiceTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _service = new MemoryCacheService(_memoryCache);
    }

    public void Dispose()
    {
        _memoryCache.Dispose();
    }

    [Fact]
    public void Get_ShouldReturnDefault_WhenKeyNotExists()
    {
        var result = _service.Get<string>("nonexistent");

        Assert.Null(result);
    }

    [Fact]
    public void Set_ShouldStoreValue()
    {
        var key = "test-key";
        var value = "test-value";

        _service.Set(key, value);
        var result = _service.Get<string>(key);

        Assert.Equal(value, result);
    }

    [Fact]
    public void Set_ShouldStoreValueWithExpiration()
    {
        var key = "test-key";
        var value = "test-value";
        var expiration = TimeSpan.FromMinutes(5);

        _service.Set(key, value, expiration);
        var result = _service.Get<string>(key);

        Assert.Equal(value, result);
    }

    [Fact]
    public void Remove_ShouldRemoveValue()
    {
        var key = "test-key";
        var value = "test-value";

        _service.Set(key, value);
        _service.Remove(key);
        var result = _service.Get<string>(key);

        Assert.Null(result);
    }

    [Fact]
    public void Exists_ShouldReturnTrue_WhenKeyExists()
    {
        var key = "test-key";
        var value = "test-value";

        _service.Set(key, value);
        var result = _service.Exists(key);

        Assert.True(result);
    }

    [Fact]
    public void Exists_ShouldReturnFalse_WhenKeyNotExists()
    {
        var result = _service.Exists("nonexistent");

        Assert.False(result);
    }

    [Fact]
    public async Task GetAsync_ShouldReturnValue_WhenKeyExists()
    {
        var key = "test-key";
        var value = "test-value";

        _service.Set(key, value);
        var result = await _service.GetAsync<string>(key);

        Assert.Equal(value, result);
    }

    [Fact]
    public async Task SetAsync_ShouldStoreValue()
    {
        var key = "test-key";
        var value = "test-value";

        await _service.SetAsync(key, value);
        var result = await _service.GetAsync<string>(key);

        Assert.Equal(value, result);
    }

    [Fact]
    public async Task RemoveAsync_ShouldRemoveValue()
    {
        var key = "test-key";
        var value = "test-value";

        await _service.SetAsync(key, value);
        await _service.RemoveAsync(key);
        var result = await _service.GetAsync<string>(key);

        Assert.Null(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenKeyExists()
    {
        var key = "test-key";
        var value = "test-value";

        await _service.SetAsync(key, value);
        var result = await _service.ExistsAsync(key);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenKeyNotExists()
    {
        var result = await _service.ExistsAsync("nonexistent");

        Assert.False(result);
    }

    [Fact]
    public void Get_ShouldReturnCorrectType()
    {
        var key = "test-key";
        var value = new List<string> { "item1", "item2" };

        _service.Set(key, value);
        var result = _service.Get<List<string>>(key);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains("item1", result);
        Assert.Contains("item2", result);
    }

    [Fact]
    public void Set_ShouldOverwriteExistingValue()
    {
        var key = "test-key";
        var value1 = "value1";
        var value2 = "value2";

        _service.Set(key, value1);
        _service.Set(key, value2);
        var result = _service.Get<string>(key);

        Assert.Equal(value2, result);
    }
}
