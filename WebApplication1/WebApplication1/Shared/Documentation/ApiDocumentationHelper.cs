using System.Reflection;
using System.Text.Json;

namespace WebApplication1.Shared.Documentation;

public static class ApiDocumentationHelper
{
    public static void AddApiDocumentation(IConfiguration configuration)
    {
        // API 文档配置在 Program.cs 中已完成
    }

    public static Dictionary<string, object> GenerateApiSummary()
    {
        var summary = new Dictionary<string, object>
        {
            ["version"] = "1.0",
            ["title"] = "Personal Growth Management API",
            ["description"] = "全栈个人成长与工作管理系统 API",
            ["modules"] = new Dictionary<string, object>
            {
                ["auth"] = new { name = "认证模块", endpoints = 4, description = "登录/注册/刷新令牌/登出" },
                ["growth"] = new { name = "成长模块", endpoints = 55, description = "习惯/目标/技能/健身/睡眠/心情/阅读/专注" },
                ["work"] = new { name = "工作模块", endpoints = 48, description = "日志/项目/设备/OKR/风险/文件" },
                ["ai"] = new { name = "AI模块", endpoints = 12, description = "助手/规划/报告/自动化/知识问答/洞察" },
                ["analytics"] = new { name = "分析模块", endpoints = 12, description = "时间/习惯/财务分析/自定义报表" },
                ["persona"] = new { name = "身份模块", endpoints = 28, description = "开发者/设计师/教师中心" },
                ["student"] = new { name = "学生模块", endpoints = 18, description = "学习计划/错题/资料/科目/记录" },
                ["assets"] = new { name = "资产模块", endpoints = 15, description = "收入/支出/预算/投资" },
                ["content"] = new { name = "内容模块", endpoints = 15, description = "文章/媒体/日历" },
                ["network"] = new { name = "人脉模块", endpoints = 11, description = "联系人/互动记录" },
                ["system"] = new { name = "系统模块", endpoints = 20, description = "用户/角色/菜单/功能管理" },
            },
            ["security"] = new[]
            {
                "JWT Bearer 认证",
                "角色权限控制 (Member/Pro/Owner)",
                "功能权限控制 (Feature Code)",
                "请求频率限制 (Rate Limiting)",
                "XSS/SQL注入防护",
                "安全响应头 (CSP/HSTS)",
            },
            ["middleware"] = new[]
            {
                "ExceptionHandlingMiddleware - 全局异常处理",
                "SecurityHeadersMiddleware - 安全响应头",
                "RequestValidationMiddleware - 请求验证",
                "XssProtectionMiddleware - XSS防护",
                "SqlInjectionMiddleware - SQL注入防护",
                "RateLimitingMiddleware - 频率限制",
                "OutputCacheMiddleware - 响应缓存",
                "ApiVersionMiddleware - API版本",
                "RequestDeduplicationMiddleware - 请求去重",
                "RequestLoggingMiddleware - 请求日志",
                "PerformanceMonitoringMiddleware - 性能监控",
            },
            ["healthChecks"] = new[]
            {
                "/healthz - 完整健康检查",
                "/healthz/ready - 就绪检查 (数据库)",
                "/healthz/live - 存活检查",
            }
        };

        return summary;
    }

    public static string GetApiDocumentationHtml()
    {
        var summary = GenerateApiSummary();
        var json = JsonSerializer.Serialize(summary, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        return @"<!DOCTYPE html>
<html>
<head>
    <title>API Documentation</title>
    <style>
        body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; margin: 40px; }
        pre { background: #f5f5f5; padding: 20px; border-radius: 8px; overflow-x: auto; }
        h1 { color: #1890ff; }
        h2 { color: #333; margin-top: 30px; }
        .endpoint { background: #e6f7ff; padding: 10px; border-radius: 4px; margin: 10px 0; }
    </style>
</head>
<body>
    <h1>Personal Growth Management API</h1>
    <p>全栈个人成长与工作管理系统 API 文档</p>
    
    <h2>API 概览</h2>
    <pre>" + json + @"</pre>
    
    <h2>快速开始</h2>
    <div class=""endpoint"">
        <strong>POST</strong> /api/auth/login - 用户登录<br>
        <strong>GET</strong> /api/auth/codes - 获取权限码<br>
        <strong>POST</strong> /api/auth/refresh - 刷新令牌
    </div>
    
    <h2>Swagger UI</h2>
    <p>访问 <a href=""/swagger"">/swagger</a> 查看完整的 API 文档</p>
    
    <h2>健康检查</h2>
    <p>
        <a href=""/healthz"">/healthz</a> - 完整健康检查<br>
        <a href=""/healthz/ready"">/healthz/ready</a> - 就绪检查<br>
        <a href=""/healthz/live"">/healthz/live</a> - 存活检查
    </p>
</body>
</html>";
    }
}
