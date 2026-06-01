using System.Net;
using System.Text.Json;
using WebApplication1.Shared.Common;

namespace WebApplication1.Shared.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "未处理的异常: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        
        var (statusCode, message) = exception switch
        {
            ArgumentException argEx => (HttpStatusCode.BadRequest, argEx.Message),
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, "无权限执行此操作"),
            KeyNotFoundException => (HttpStatusCode.NotFound, "资源不存在"),
            InvalidOperationException opEx => (HttpStatusCode.BadRequest, opEx.Message),
            _ => (HttpStatusCode.InternalServerError, "服务器内部错误，请稍后重试")
        };

        context.Response.StatusCode = (int)statusCode;
        
        var result = ApiResult.Fail(message, (int)statusCode);
        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsJsonAsync(result, jsonOptions);
    }
}
