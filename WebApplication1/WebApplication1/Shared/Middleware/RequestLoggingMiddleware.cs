using System.Diagnostics;
using System.Security.Claims;
using System.Text;

namespace WebApplication1.Shared.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private static readonly HashSet<string> _sensitiveHeaders = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization", "Cookie", "Set-Cookie", "X-API-Key"
    };

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = Guid.NewGuid().ToString("N")[..8];
        context.Items["RequestId"] = requestId;
        context.Response.Headers["X-Request-Id"] = requestId;

        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
        var method = context.Request.Method;
        var path = context.Request.Path;
        var queryString = context.Request.QueryString.ToString();
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var clientIp = GetClientIp(context);

        var requestLog = new
        {
            RequestId = requestId,
            UserId = userId,
            Method = method,
            Path = path,
            QueryString = queryString,
            ClientIp = clientIp,
            UserAgent = userAgent,
            Headers = GetSafeHeaders(context.Request.Headers)
        };

        _logger.LogInformation("HTTP Request: {@Request}", requestLog);

        var stopwatch = Stopwatch.StartNew();
        var originalBodyStream = context.Response.Body;

        try
        {
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            stopwatch.Stop();

            var responseLog = new
            {
                RequestId = requestId,
                UserId = userId,
                Method = method,
                Path = path,
                StatusCode = context.Response.StatusCode,
                ElapsedMs = stopwatch.ElapsedMilliseconds,
                ContentType = context.Response.ContentType
            };

            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                _logger.LogWarning("Slow request detected: {@Response}", responseLog);
            }
            else if (context.Response.StatusCode >= 400)
            {
                _logger.LogWarning("HTTP Error: {@Response}", responseLog);
            }
            else
            {
                _logger.LogInformation("HTTP Response: {@Response}", responseLog);
            }

            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Request failed: {RequestId} {Method} {Path} after {ElapsedMs}ms", 
                requestId, method, path, stopwatch.ElapsedMilliseconds);
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private static string GetClientIp(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
            return forwardedFor.Split(',')[0].Trim();

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
            return realIp;

        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static Dictionary<string, string> GetSafeHeaders(IHeaderDictionary headers)
    {
        var result = new Dictionary<string, string>();
        foreach (var header in headers)
        {
            if (_sensitiveHeaders.Contains(header.Key))
            {
                result[header.Key] = "***REDACTED***";
            }
            else
            {
                result[header.Key] = header.Value.ToString();
            }
        }
        return result;
    }
}
