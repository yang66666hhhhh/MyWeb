namespace WebApplication1.Shared.Configuration;

public class RateLimitConfig
{
    public const string SectionName = "RateLimit";
    
    public bool Enabled { get; set; } = true;
    
    // 默认限制
    public int DefaultRequestsPerMinute { get; set; } = 60;
    
    // 认证相关
    public int LoginRequestsPerMinute { get; set; } = 5;
    public int RegisterRequestsPerMinute { get; set; } = 3;
    public int RefreshTokenRequestsPerMinute { get; set; } = 10;
    
    // AI 相关
    public int AiRequestsPerMinute { get; set; } = 20;
    
    // 文件上传
    public int UploadRequestsPerMinute { get; set; } = 10;
    
    // 数据修改
    public int CreateRequestsPerMinute { get; set; } = 30;
    public int UpdateRequestsPerMinute { get; set; } = 30;
    public int DeleteRequestsPerMinute { get; set; } = 20;
    
    // 查询
    public int GetRequestsPerMinute { get; set; } = 100;
    
    // 窗口大小
    public int WindowSeconds { get; set; } = 60;
    
    // 自定义端点限制
    public Dictionary<string, EndpointLimit> EndpointLimits { get; set; } = new();
}

public class EndpointLimit
{
    public int Requests { get; set; }
    public int WindowSeconds { get; set; } = 60;
}
