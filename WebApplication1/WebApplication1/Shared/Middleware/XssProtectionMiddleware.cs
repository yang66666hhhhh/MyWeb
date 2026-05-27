using System.Net;
using System.Text.RegularExpressions;

namespace WebApplication1.Shared.Middleware;

public partial class XssProtectionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<XssProtectionMiddleware> _logger;

    public XssProtectionMiddleware(RequestDelegate next, ILogger<XssProtectionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 添加安全响应头
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'; img-src 'self' data: https:; font-src 'self' data:;";

        // 检查请求内容
        if (context.Request.Method is "POST" or "PUT" or "PATCH")
        {
            context.Request.EnableBuffering();
            
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (!string.IsNullOrEmpty(body))
            {
                if (ContainsXssAttempt(body))
                {
                    _logger.LogWarning("XSS attempt detected from {IP}: {Body}", 
                        context.Connection.RemoteIpAddress, body[..Math.Min(body.Length, 200)]);
                    
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        code = 400,
                        message = "请求包含非法字符",
                        success = false
                    });
                    return;
                }
            }
        }

        await _next(context);
    }

    private static bool ContainsXssAttempt(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        // 常见XSS攻击模式
        var patterns = new[]
        {
            @"<script[^>]*>",
            @"javascript:",
            @"on\w+\s*=",
            @"expression\s*\(",
            @"vbscript:",
            @"data:text/html",
            @"<iframe[^>]*>",
            @"<object[^>]*>",
            @"<embed[^>]*>",
            @"<applet[^>]*>",
            @"<form[^>]*>",
            @"<input[^>]*>",
            @"<meta[^>]*>",
            @"<link[^>]*>",
            @"<base[^>]*]",
            @"<!--",
            @"-->",
            @"<svg[^>]*onload",
            @"<img[^>]*onerror",
            @"<body[^>]*onload"
        };

        foreach (var pattern in patterns)
        {
            if (XssPatternRegex().IsMatch(input))
            {
                return true;
            }
        }

        return false;
    }

    [GeneratedRegex(@"<script[^>]*>|javascript:|on\w+\s*=|expression\s*\(|vbscript:|data:text/html|<iframe[^>]*>|<object[^>]*>|<embed[^>]*>|<applet[^>]*>|<svg[^>]*onload|<img[^>]*onerror|<body[^>]*onload", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex XssPatternRegex();
}
