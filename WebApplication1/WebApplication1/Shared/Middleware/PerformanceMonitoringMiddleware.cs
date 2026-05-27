using System.Collections.Concurrent;
using System.Diagnostics;

namespace WebApplication1.Shared.Middleware;

public class PerformanceMonitoringMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PerformanceMonitoringMiddleware> _logger;
    private static readonly ConcurrentDictionary<string, PerformanceMetrics> _metrics = new();
    private static readonly TimeSpan _slowQueryThreshold = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan _slowApiThreshold = TimeSpan.FromSeconds(1);

    public PerformanceMonitoringMiddleware(RequestDelegate next, ILogger<PerformanceMonitoringMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = $"{context.Request.Method}:{context.Request.Path}";
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;

            // 更新指标
            var metrics = _metrics.GetOrAdd(endpoint, _ => new PerformanceMetrics());
            metrics.RecordRequest(elapsed);

            // 记录慢API
            if (elapsed > _slowApiThreshold)
            {
                var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                _logger.LogWarning("Slow API detected: {Endpoint} took {ElapsedMs}ms (User: {UserId})", 
                    endpoint, elapsed.TotalMilliseconds, userId);

                // 每100次请求记录一次统计
                if (metrics.TotalRequests % 100 == 0)
                {
                    _logger.LogInformation("Performance stats for {Endpoint}: {@Metrics}", endpoint, new
                    {
                        metrics.TotalRequests,
                        AverageMs = metrics.AverageMs,
                        P95Ms = metrics.P95Ms,
                        P99Ms = metrics.P99Ms,
                        MaxMs = metrics.MaxMs,
                        SlowRequestCount = metrics.SlowRequestCount
                    });
                }
            }

            // 添加性能响应头
            context.Response.Headers["X-Response-Time"] = $"{elapsed.TotalMilliseconds:F2}ms";
        }
    }

    public static PerformanceMetrics? GetMetrics(string endpoint)
    {
        return _metrics.TryGetValue(endpoint, out var metrics) ? metrics : null;
    }

    public static Dictionary<string, PerformanceMetrics> GetAllMetrics()
    {
        return new Dictionary<string, PerformanceMetrics>(_metrics);
    }
}

public class PerformanceMetrics
{
    private readonly object _lock = new();
    private readonly List<double> _responseTimes = new();
    private long _totalRequests;
    private long _slowRequestCount;
    private double _maxMs;

    public long TotalRequests => _totalRequests;
    public long SlowRequestCount => _slowRequestCount;
    public double MaxMs => _maxMs;

    public double AverageMs
    {
        get
        {
            lock (_lock)
            {
                return _responseTimes.Count > 0 ? _responseTimes.Average() : 0;
            }
        }
    }

    public double P95Ms
    {
        get
        {
            lock (_lock)
            {
                if (_responseTimes.Count == 0) return 0;
                var index = (int)Math.Ceiling(_responseTimes.Count * 0.95) - 1;
                return _responseTimes.OrderBy(x => x).ElementAt(Math.Max(0, index));
            }
        }
    }

    public double P99Ms
    {
        get
        {
            lock (_lock)
            {
                if (_responseTimes.Count == 0) return 0;
                var index = (int)Math.Ceiling(_responseTimes.Count * 0.99) - 1;
                return _responseTimes.OrderBy(x => x).ElementAt(Math.Max(0, index));
            }
        }
    }

    public void RecordRequest(TimeSpan elapsed)
    {
        var ms = elapsed.TotalMilliseconds;
        lock (_lock)
        {
            _totalRequests++;
            _responseTimes.Add(ms);
            
            if (ms > _maxMs)
                _maxMs = ms;

            if (elapsed > TimeSpan.FromSeconds(1))
                _slowRequestCount++;

            // 保留最近1000个请求的响应时间
            if (_responseTimes.Count > 1000)
                _responseTimes.RemoveAt(0);
        }
    }
}
