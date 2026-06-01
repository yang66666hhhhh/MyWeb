using Microsoft.Extensions.Primitives;

namespace WebApplication1.Shared.Middleware;

public class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // 安全响应头
        var headers = context.Response.Headers;

        // 防止点击劫持
        headers["X-Frame-Options"] = "SAMEORIGIN";

        // 防止 MIME 类型嗅探
        headers["X-Content-Type-Options"] = "nosniff";

        // XSS 保护
        headers["X-XSS-Protection"] = "1; mode=block";

        // 引用策略
        headers["Referrer-Policy"] = "strict-origin-when-cross-origin";

        // 权限策略
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

        // 内容安全策略 (CSP)
        if (!headers.ContainsKey("Content-Security-Policy"))
        {
            headers["Content-Security-Policy"] = string.Join("; ", new[]
            {
                "default-src 'self'",
                "script-src 'self' 'unsafe-inline' 'unsafe-eval'",
                "style-src 'self' 'unsafe-inline'",
                "img-src 'self' data: blob: https:",
                "font-src 'self' data:",
                "connect-src 'self' ws: wss: http: https:",
                "frame-ancestors 'self'",
                "base-uri 'self'",
                "form-action 'self'"
            });
        }

        // 严格传输安全 (HSTS)
        if (context.Request.IsHttps)
        {
            headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
        }

        // 移除服务器信息
        headers.Remove("Server");
        headers.Remove("X-Powered-By");

        await next(context);
    }
}
