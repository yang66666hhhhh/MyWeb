using System.Collections.Concurrent;

namespace WebApplication1.Shared.Middleware;

public class OutputCacheMiddleware(RequestDelegate next, ILogger<OutputCacheMiddleware> logger)
{
    private static readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
    private static readonly TimeSpan _defaultDuration = TimeSpan.FromSeconds(30);

    public async Task InvokeAsync(HttpContext context)
{
        // 只缓存 GET 请求
        if (!HttpMethods.IsGet(context.Request.Method))
        {
            await next(context);
            return;
        }

        // 不缓存需要认证的请求
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var cacheKey = $"{context.Request.Path}:{context.Request.QueryString}:{context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value}";
            
            if (_cache.TryGetValue(cacheKey, out var cached) && !cached.IsExpired)
            {
                context.Response.ContentType = cached.ContentType;
                context.Response.StatusCode = cached.StatusCode;
                await context.Response.WriteAsync(cached.Body);
                logger.LogDebug("Cache hit for {CacheKey}", cacheKey);
                return;
            }

            // 捕获响应
            var originalBody = context.Response.Body;
            using var newBody = new MemoryStream();
            context.Response.Body = newBody;

            await next(context);

            newBody.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(newBody).ReadToEndAsync();

            // 缓存成功的响应
            if (context.Response.StatusCode == 200)
            {
                _cache[cacheKey] = new CacheEntry
                {
                    Body = responseBody,
                    ContentType = context.Response.ContentType ?? "application/json",
                    StatusCode = context.Response.StatusCode,
                    ExpiresAt = DateTime.UtcNow.Add(_defaultDuration)
                };
            }

            newBody.Seek(0, SeekOrigin.Begin);
            await newBody.CopyToAsync(originalBody);
        }
        else
        {
            await next(context);
        }
    }

    public static void ClearCache()
    {
        _cache.Clear();
    }

    private class CacheEntry
    {
        public string Body { get; set; } = string.Empty;
        public string ContentType { get; set; } = "application/json";
        public int StatusCode { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    }
}
