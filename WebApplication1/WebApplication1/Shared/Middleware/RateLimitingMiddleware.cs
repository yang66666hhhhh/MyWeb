using System.Collections.Concurrent;
using System.Net;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace WebApplication1.Shared.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;
    private static readonly ConcurrentDictionary<string, SlidingWindowCounter> _clients = new();

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, IOptions<RateLimitOptions> options)
    {
        _next = next;
        _logger = logger;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = $"{context.Request.Method}:{context.Request.Path}";
        var (limit, window) = GetLimit(endpoint);

        if (limit > 0)
        {
            var clientId = GetClientId(context);
            var key = $"{clientId}:{endpoint}";
            var counter = _clients.GetOrAdd(key, _ => new SlidingWindowCounter(window, limit));

            if (!counter.TryIncrement())
            {
                _logger.LogWarning("Rate limit exceeded for {Key}", key);
                
                var retryAfter = counter.GetRetryAfterSeconds();
                context.Response.Headers["Retry-After"] = retryAfter.ToString();
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    code = 429,
                    message = "请求过于频繁，请稍后再试",
                    retryAfter,
                    success = false
                });
                return;
            }

            // 添加限流响应头
            context.Response.Headers["X-RateLimit-Limit"] = limit.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = counter.GetRemaining().ToString();
            context.Response.Headers["X-RateLimit-Reset"] = counter.GetResetTime().ToString();
        }

        await _next(context);
    }

    private static string GetClientId(HttpContext context)
    {
        // 优先使用用户ID，其次使用IP
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
            return $"user:{userId}";

        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        // 支持反向代理
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
            clientIp = forwardedFor.Split(',')[0].Trim();

        return $"ip:{clientIp}";
    }

    private (int limit, TimeSpan window) GetLimit(string endpoint)
    {
        // 先检查配置
        if (_options.EndpointLimits.TryGetValue(endpoint, out var customLimit))
            return customLimit;

        // 默认限流规则
        return endpoint switch
        {
            // 认证相关 - 严格限流
            var e when e.StartsWith("POST:/api/auth/login") => (5, TimeSpan.FromMinutes(1)),
            var e when e.StartsWith("POST:/api/auth/register") => (3, TimeSpan.FromMinutes(5)),
            var e when e.StartsWith("POST:/api/auth/refresh-token") => (10, TimeSpan.FromMinutes(1)),
            var e when e.StartsWith("POST:/api/auth/forgot-password") => (3, TimeSpan.FromMinutes(15)),

            // AI相关 - 中等限流
            var e when e.StartsWith("POST:/api/ai/") => (20, TimeSpan.FromMinutes(1)),

            // 文件上传 - 严格限流
            var e when e.StartsWith("POST:/api/") && e.Contains("upload") => (10, TimeSpan.FromMinutes(1)),

            // 数据修改 - 中等限流
            var e when e.StartsWith("POST:/api/") => (30, TimeSpan.FromMinutes(1)),
            var e when e.StartsWith("PUT:/api/") => (30, TimeSpan.FromMinutes(1)),
            var e when e.StartsWith("DELETE:/api/") => (20, TimeSpan.FromMinutes(1)),

            // 查询 - 宽松限流
            var e when e.StartsWith("GET:/api/") => (100, TimeSpan.FromMinutes(1)),

            // 其他
            _ => (60, TimeSpan.FromMinutes(1))
        };
    }
}

public class RateLimitOptions
{
    public const string SectionName = "RateLimit";
    public bool Enabled { get; set; } = true;
    public Dictionary<string, (int limit, TimeSpan window)> EndpointLimits { get; set; } = new();
}

internal class SlidingWindowCounter
{
    private readonly TimeSpan _window;
    private readonly int _limit;
    private readonly List<DateTime> _timestamps = new();
    private readonly object _lock = new();

    public SlidingWindowCounter(TimeSpan window, int limit)
    {
        _window = window;
        _limit = limit;
    }

    public bool TryIncrement()
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            _timestamps.RemoveAll(t => t < now - _window);
            if (_timestamps.Count >= _limit)
                return false;
            _timestamps.Add(now);
            return true;
        }
    }

    public int GetRemaining()
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            _timestamps.RemoveAll(t => t < now - _window);
            return Math.Max(0, _limit - _timestamps.Count);
        }
    }

    public int GetRetryAfterSeconds()
    {
        lock (_lock)
        {
            if (_timestamps.Count == 0) return 0;
            var oldest = _timestamps.Min();
            var retryAfter = (oldest + _window - DateTime.UtcNow).TotalSeconds;
            return Math.Max(1, (int)Math.Ceiling(retryAfter));
        }
    }

    public long GetResetTime()
    {
        lock (_lock)
        {
            if (_timestamps.Count == 0) return 0;
            var resetTime = _timestamps.Min() + _window;
            return new DateTimeOffset(resetTime).ToUnixTimeSeconds();
        }
    }
}
