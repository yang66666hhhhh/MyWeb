using WebApplication1.Shared.Common;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Features.Tasks;

public class TaskItemDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public DateOnly PlanDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = "Personal";
    public string Source { get; set; } = "Growth";
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string Priority { get; set; } = "Medium";
    public string Status { get; set; } = "Pending";
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
    public Guid? ConvertedWorkLogId { get; set; }
    public string? Remark { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TaskItemQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public string? TaskType { get; set; }
    public string? Source { get; set; }
    public DateOnly? PlanDate { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid? ProjectId { get; set; }
    public int? Status { get; set; }
    public int? Priority { get; set; }
}

public class CreateTaskItemDto
{
    public DateOnly PlanDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TaskType { get; set; } = "Personal";
    public string Source { get; set; } = "Growth";
    public string? ProjectId { get; set; }
    public int Priority { get; set; } = 2;
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public decimal? EstimatedHours { get; set; }
    public string? Remark { get; set; }
}

public class UpdateTaskItemDto
{
    public DateOnly? PlanDate { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? TaskType { get; set; }
    public string? Source { get; set; }
    public string? ProjectId { get; set; }
    public int? Priority { get; set; }
    public int? Status { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
    public string? Remark { get; set; }
}

public class ConvertTaskToLogDto
{
    public string TaskId { get; set; } = string.Empty;
    public DateOnly WorkDate { get; set; }
    public string? OriginalContent { get; set; }
    public decimal? TotalHours { get; set; }
}