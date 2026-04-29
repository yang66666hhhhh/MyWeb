using WebApplication1.Shared;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Features.Work.Entities;

public class WorkDailyPlan : EntityBase
{
    public Guid? UserId { get; set; }

    public DateOnly PlanDate { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    public Guid? ProjectId { get; set; }

    public WorkDailyPlanPriority Priority { get; set; } = WorkDailyPlanPriority.Medium;

    public WorkDailyPlanStatus Status { get; set; } = WorkDailyPlanStatus.Pending;

    public string? StartTime { get; set; }

    public string? EndTime { get; set; }

    public decimal? EstimatedHours { get; set; }

    public decimal? ActualHours { get; set; }

    public Guid? ConvertedWorkLogId { get; set; }

    public string? Remark { get; set; }

    public WorkProject? Project { get; set; }
}
