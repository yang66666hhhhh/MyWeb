using System.Collections.Concurrent;
using System.Net;

namespace WebApplication1.Shared.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, SlidingWindowCounter> _clients = new();

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = $"{context.Request.Method}:{context.Request.Path}";
        var (limit, window) = GetLimit(endpoint);

        if (limit > 0)
        {
            var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var key = $"{clientIp}:{endpoint}";
            var counter = _clients.GetOrAdd(key, _ => new SlidingWindowCounter(window));

            if (!counter.TryIncrement())
            {
                _logger.LogWarning("Rate limit exceeded for {Key}", key);
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    code = 429,
                    message = "请求过于频繁，请稍后再试",
                    success = false
                });
                return;
            }
        }

        await _next(context);
    }

    private static (int limit, TimeSpan window) GetLimit(string endpoint) => endpoint switch
    {
        "POST:/api/auth/login" => (5, TimeSpan.FromMinutes(1)),
        "POST:/api/ai/generate-plan" => (10, TimeSpan.FromMinutes(1)),
        "POST:/api/ai/generate-report" => (10, TimeSpan.FromMinutes(1)),
        "POST:/api/ai/chat" => (20, TimeSpan.FromMinutes(1)),
        _ => (0, TimeSpan.Zero)
    };
}

internal class SlidingWindowCounter
{
    private readonly TimeSpan _window;
    private readonly List<DateTime> _timestamps = new();
    private readonly object _lock = new();

    public SlidingWindowCounter(TimeSpan window)
    {
        _window = window;
    }

    public bool TryIncrement()
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            _timestamps.RemoveAll(t => t < now - _window);
            if (_timestamps.Count >= 1000)
                return false;
            _timestamps.Add(now);
            return true;
        }
    }
}
