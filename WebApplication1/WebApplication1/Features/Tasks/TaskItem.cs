using WebApplication1.Shared;
using WebApplication1.Shared.Enums;
using WorkProjectEntity = WebApplication1.Features.Work.Entities.WorkProject;

namespace WebApplication1.Features.Tasks;

public class TaskItem : EntityBase
{
    public Guid? UserId { get; set; }

    public DateOnly PlanDate { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public TaskType Type { get; set; } = TaskType.Personal;

    public TaskSource Source { get; set; } = TaskSource.Growth;

    public Guid? ProjectId { get; set; }

    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public TaskItemStatus Status { get; set; } = TaskItemStatus.Pending;

    public string? StartTime { get; set; }

    public string? EndTime { get; set; }

    public decimal? EstimatedHours { get; set; }

    public decimal? ActualHours { get; set; }

    public Guid? ConvertedWorkLogId { get; set; }

    public string? Remark { get; set; }

    public DateTime? CompletedAt { get; set; }

    public WorkProjectEntity? Project { get; set; }
}