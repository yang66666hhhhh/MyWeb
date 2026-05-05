using WebApplication1.Shared;

namespace WebApplication1.Features.Ai.Entities;

public class AiPlan : EntityBase
{
    public Guid? UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public AiPlanType Type { get; set; } = AiPlanType.Daily;

    public AiPlanStatus Status { get; set; } = AiPlanStatus.Pending;

    public string? GeneratedContent { get; set; }

    public string? Remark { get; set; }
}

public class AiReport : EntityBase
{
    public Guid? UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public AiReportType Type { get; set; } = AiReportType.Daily;

    public string? Content { get; set; }

    public string? Remark { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }
}

public class AiChatSession : EntityBase
{
    public Guid? UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? LastMessage { get; set; }

    public int MessageCount { get; set; }
}

public class AiChatMessage : EntityBase
{
    public Guid SessionId { get; set; }

    public AiChatSession? Session { get; set; }

    public AiMessageRole Role { get; set; }

    public string Content { get; set; } = string.Empty;
}

public enum AiPlanType
{
    Daily = 0,
    Weekly = 1,
    Monthly = 2,
    Project = 3,
    Custom = 4
}

public enum AiPlanStatus
{
    Pending = 0,
    Generating = 1,
    Completed = 2,
    Failed = 3
}

public enum AiReportType
{
    Daily = 0,
    Weekly = 1,
    Monthly = 2,
    Project = 3,
    Custom = 4
}

public enum AiMessageRole
{
    User = 0,
    Assistant = 1,
    System = 2
}
