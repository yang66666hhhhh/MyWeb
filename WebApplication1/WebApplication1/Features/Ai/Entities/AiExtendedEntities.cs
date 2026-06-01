using WebApplication1.Shared;

namespace WebApplication1.Features.Ai.Entities;

public class AutomationWorkflow : EntityBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? TriggerType { get; set; }
    public string? Actions { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastRunAt { get; set; }
}

public class AiInsight : EntityBase
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? Category { get; set; }
    public string? Source { get; set; }
}

public class KnowledgeChatSessionItem : EntityBase
{
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public string? LastMessage { get; set; }
    public int MessageCount { get; set; }
}
