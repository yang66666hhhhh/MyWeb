using System.Collections.Concurrent;

namespace WebApplication1.Shared.Middleware;

public class RequestDeduplicationMiddleware(RequestDelegate next, ILogger<RequestDeduplicationMiddleware> logger)
{
    private static readonly ConcurrentDictionary<string, TaskCompletionSource<HttpContext>> _pendingRequests = new();

    public async Task InvokeAsync(HttpContext context)
{
        // 只对 GET 请求进行去重
        if (!HttpMethods.IsGet(context.Request.Method))
        {
            await next(context);
            return;
        }

        var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var requestKey = $"{userId ?? clientIp}:{context.Request.Path}{context.Request.QueryString}";

        var tcs = new TaskCompletionSource<HttpContext>();
        var entry = _pendingRequests.GetOrAdd(requestKey, tcs);

        if (entry != tcs)
        {
            // 已有相同请求在处理，等待其完成
            logger.LogDebug("Deduplicating request: {Key}", requestKey);
            try
            {
                await entry.Task;
                return;
            }
            catch
            {
                // 如果原始请求失败，继续处理当前请求
            }
        }

        try
        {
            await next(context);
            tcs.SetResult(context);
        }
        catch (Exception ex)
        {
            tcs.SetException(ex);
            throw;
        }
        finally
        {
            _pendingRequests.TryRemove(requestKey, out _);
        }
    }
}
