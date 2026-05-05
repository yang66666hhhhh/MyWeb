namespace WebApplication1.Features.Auth.Entities.Subscription;

/// <summary>
/// 功能点定义
/// </summary>
public class Feature
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 订阅计划定义
/// </summary>
public class Plan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;  // Free, Pro, Team
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int BillingCycle { get; set; }  // 0=永久, 30=月付, 365=年付
    public int Sort { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<PlanFeature> PlanFeatures { get; set; } = new List<PlanFeature>();
}

/// <summary>
/// 订阅计划-功能映射
/// </summary>
public class PlanFeature
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PlanId { get; set; }
    public Guid FeatureId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Plan Plan { get; set; } = null!;
    public Feature Feature { get; set; } = null!;
}

/// <summary>
/// Persona-功能映射（Persona 包含哪些功能）
/// </summary>
public class PersonaFeature
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PersonaCode { get; set; } = string.Empty;
    public Guid FeatureId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Feature Feature { get; set; } = null!;
}

/// <summary>
/// 用户订阅记录
/// </summary>
public class UserSubscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid PlanId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? ExpireAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Plan Plan { get; set; } = null!;
}
