namespace WebApplication1.Shared.Configuration;

public class ApiConfig
{
    public const string SectionName = "Api";
    
    // API 版本
    public string Version { get; set; } = "1.0";
    
    // API 标题
    public string Title { get; set; } = "Personal Growth Management API";
    
    // API 描述
    public string Description { get; set; } = "全栈个人成长与工作管理系统 API";
    
    // 联系信息
    public ContactConfig Contact { get; set; } = new();
    
    // 许可信息
    public LicenseConfig License { get; set; } = new();
    
    // CORS 配置
    public CorsConfig Cors { get; set; } = new();
    
    // 缓存配置
    public CacheConfig Cache { get; set; } = new();
    
    // 分页配置
    public PaginationConfig Pagination { get; set; } = new();
}

public class ContactConfig
{
    public string Name { get; set; } = "API Support";
    public string Email { get; set; } = "support@example.com";
    public string Url { get; set; } = "";
}

public class LicenseConfig
{
    public string Name { get; set; } = "MIT License";
    public string Url { get; set; } = "https://opensource.org/licenses/MIT";
}

public class CorsConfig
{
    public string[] Origins { get; set; } = ["http://localhost:5666", "http://localhost:5173"];
    public string[] Methods { get; set; } = ["GET", "POST", "PUT", "DELETE", "OPTIONS"];
    public string[] Headers { get; set; } = ["*"];
    public bool AllowCredentials { get; set; } = true;
    public int MaxAge { get; set; } = 3600;
}

public class CacheConfig
{
    public bool Enabled { get; set; } = true;
    public int DefaultExpirationMinutes { get; set; } = 30;
    public int SlidingExpirationMinutes { get; set; } = 10;
    public long MaxCacheSize { get; set; } = 100 * 1024 * 1024; // 100MB
}

public class PaginationConfig
{
    public int DefaultPage { get; set; } = 1;
    public int DefaultPageSize { get; set; } = 10;
    public int MaxPageSize { get; set; } = 100;
}
