using System.Net;
using System.Text.RegularExpressions;

namespace WebApplication1.Shared.Middleware;

public partial class SqlInjectionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SqlInjectionMiddleware> _logger;

    public SqlInjectionMiddleware(RequestDelegate next, ILogger<SqlInjectionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 只检查 POST/PUT/DELETE 请求
        if (context.Request.Method is "POST" or "PUT" or "DELETE" or "PATCH")
        {
            context.Request.EnableBuffering();
            
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (!string.IsNullOrEmpty(body))
            {
                if (ContainsSqlInjection(body))
                {
                    _logger.LogWarning("SQL injection attempt detected from {IP}: {Body}", 
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

        // 检查查询字符串
        if (context.Request.QueryString.HasValue)
        {
            var queryString = context.Request.QueryString.Value;
            if (ContainsSqlInjection(queryString))
            {
                _logger.LogWarning("SQL injection attempt detected in query from {IP}: {Query}", 
                    context.Connection.RemoteIpAddress, queryString);
                
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

        await _next(context);
    }

    private static bool ContainsSqlInjection(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        // 常见SQL注入模式
        var patterns = new[]
        {
            @"(\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER|EXEC|EXECUTE|UNION|DECLARE)\b)",
            @"(--|;|/\*|\*/|@@|@)",
            @"(\b(OR|AND)\b\s+\d+\s*=\s*\d+)",
            @"(\b(OR|AND)\b\s+['""]?\w+['""]?\s*=\s*['""]?\w+['""]?)",
            @"(';\s*(DROP|DELETE|INSERT|UPDATE))",
            @"(\bCHAR\b\s*\()",
            @"(\bCONCAT\b\s*\()",
            @"(\bSUBSTRING\b\s*\()",
            @"(\bEXEC\b\s*\()",
            @"(\bXP_)",
            @"(\bSP_)",
            @"(\b0x[0-9a-fA-F]+)",
            @"(\bWAITFOR\b\s+\bDELAY\b)",
            @"(\bBENCHMARK\b\s*\()",
            @"(\bSLEEP\b\s*\()",
            @"(\bLOAD_FILE\b\s*\()",
            @"(\bINTO\b\s+\bOUTFILE\b)",
            @"(\bINTO\b\s+\bDUMPFILE\b)"
        };

        foreach (var pattern in patterns)
        {
            if (SqlInjectionRegex().IsMatch(input))
            {
                return true;
            }
        }

        return false;
    }

    [GeneratedRegex(@"(\b(SELECT|INSERT|UPDATE|DELETE|DROP|CREATE|ALTER|EXEC|EXECUTE|UNION|DECLARE)\b)|(--|;|/\*|\*/|@@|@)|(\b(OR|AND)\b\s+\d+\s*=\s*\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex SqlInjectionRegex();
}
