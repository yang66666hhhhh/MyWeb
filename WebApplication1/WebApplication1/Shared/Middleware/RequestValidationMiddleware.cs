using System.Text;
using System.Text.Json;

namespace WebApplication1.Shared.Middleware;

public class RequestValidationMiddleware(RequestDelegate next, ILogger<RequestValidationMiddleware> logger)
{
    private static readonly HashSet<string> SuspiciousPatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        "<script",
        "javascript:",
        "onerror=",
        "onload=",
        "onclick=",
        "onmouseover=",
        "expression(",
        "url(",
        "eval(",
        "document.cookie",
        "document.write",
        "window.location",
        "alert(",
        "confirm(",
        "prompt("
    };

    public async Task InvokeAsync(HttpContext context)
{
        // 验证请求头
        if (!ValidateHeaders(context))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { code = 400, message = "无效的请求头" });
            return;
        }

        // 验证查询参数
        if (!ValidateQueryString(context))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { code = 400, message = "无效的查询参数" });
            return;
        }

        // 验证请求体（仅对 POST/PUT/PATCH）
        if (HttpMethods.IsPost(context.Request.Method) ||
            HttpMethods.IsPut(context.Request.Method) ||
            HttpMethods.IsPatch(context.Request.Method))
        {
            if (!await ValidateRequestBody(context))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new { code = 400, message = "无效的请求体" });
                return;
            }
        }

        await next(context);
    }

    private bool ValidateHeaders(HttpContext context)
    {
        // 检查 Content-Type
        if (context.Request.ContentLength > 0 &&
            !context.Request.ContentType?.StartsWith("application/json") == true &&
            !context.Request.ContentType?.StartsWith("multipart/form-data") == true &&
            !context.Request.ContentType?.StartsWith("application/x-www-form-urlencoded") == true)
        {
            logger.LogWarning("Invalid Content-Type: {ContentType}", context.Request.ContentType);
            return false;
        }

        // 检查自定义头长度
        foreach (var header in context.Request.Headers)
        {
            if (header.Value.ToString().Length > 8192)
            {
                logger.LogWarning("Header too long: {Key}", header.Key);
                return false;
            }
        }

        return true;
    }

    private bool ValidateQueryString(HttpContext context)
    {
        foreach (var param in context.Request.Query)
        {
            var value = param.Value.ToString();
            if (ContainsSuspiciousContent(value))
            {
                logger.LogWarning("Suspicious query parameter: {Key}={Value}", param.Key, value);
                return false;
            }
        }
        return true;
    }

    private async Task<bool> ValidateRequestBody(HttpContext context)
    {
        if (context.Request.Body == null || context.Request.ContentLength == 0)
            return true;

        context.Request.EnableBuffering();

        using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        if (string.IsNullOrWhiteSpace(body))
            return true;

        // 检查 JSON 格式
        try
        {
            JsonDocument.Parse(body);
        }
        catch (JsonException)
        {
            logger.LogWarning("Invalid JSON body");
            return false;
        }

        // 检查可疑内容
        if (ContainsSuspiciousContent(body))
        {
            logger.LogWarning("Suspicious content in request body");
            return false;
        }

        return true;
    }

    private static bool ContainsSuspiciousContent(string content)
    {
        if (string.IsNullOrEmpty(content))
            return false;

        var lowerContent = content.ToLowerInvariant();
        return SuspiciousPatterns.Any(pattern => lowerContent.Contains(pattern.ToLowerInvariant()));
    }
}
