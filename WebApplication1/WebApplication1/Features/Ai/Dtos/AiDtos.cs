using System.ComponentModel.DataAnnotations;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Features.Ai.Dtos;

public class AiPlanDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public string? GeneratedContent { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AiReportDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? Remark { get; set; }
    public Guid? RelatedProjectId { get; set; }
    public string? RelatedProjectName { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class AiChatMessageDto
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AiChatSessionDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? LastMessage { get; set; }
    public int MessageCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AiQueryDto : PageQueryDto
{
    public string? Type { get; set; }
    public string? Keyword { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public Guid? RelatedProjectId { get; set; }
}

public class GeneratePlanRequest
{
    [Required(ErrorMessage = "计划类型不能为空")]
    public string Type { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "描述不能超过1000个字符")]
    public string? Description { get; set; }

    public string? TargetDate { get; set; }
    public Guid? RelatedProjectId { get; set; }
    public List<string>? IncludeCategories { get; set; }
}

public class GenerateReportRequest
{
    [Required(ErrorMessage = "报告类型不能为空")]
    public string Type { get; set; } = string.Empty;

    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public Guid? RelatedProjectId { get; set; }
    public bool IncludeStatistics { get; set; } = true;
}

public class ChatRequest
{
    [Required(ErrorMessage = "消息内容不能为空")]
    [MaxLength(5000, ErrorMessage = "消息内容不能超过5000个字符")]
    public string Message { get; set; } = string.Empty;

    public Guid? SessionId { get; set; }
    public List<ChatMessageContext>? History { get; set; }
}

public class ChatMessageContext
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class ChatResponse
{
    public Guid? SessionId { get; set; }
    public Guid? MessageId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
