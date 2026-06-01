namespace WebApplication1.Shared.Middleware;

public class ApiVersionMiddleware(RequestDelegate next)
{
    private const string DefaultVersion = "1.0";

    public async Task InvokeAsync(HttpContext context)
    {
        // 添加 API 版本响应头
        context.Response.Headers["X-API-Version"] = DefaultVersion;
        context.Response.Headers["X-Request-Id"] = context.TraceIdentifier;

        // 添加速率限制信息头
        context.Response.Headers["X-RateLimit-Policy"] = "sliding-window";

        await next(context);
    }
}
