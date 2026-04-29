using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Ai.Dtos;

public class AiPlanDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public string? GeneratedContent { get; set; }
    public string? Remark { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class AiReportDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? Remark { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class AiChatMessageDto
{
    public string Id { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class AiQueryDto : PageQueryDto
{
    public string? Type { get; set; }
    public string? Keyword { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}

public class GeneratePlanRequest
{
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? TargetDate { get; set; }
}

public class GenerateReportRequest
{
    public string Type { get; set; } = string.Empty;
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
    public string? SessionId { get; set; }
}
