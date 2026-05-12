using WebApplication1.Shared;

namespace WebApplication1.Features.Work.Entities;

public class WeeklyPlan : EntityBase
{
    public Guid UserId { get; set; }

    public int Year { get; set; }

    public int WeekNumber { get; set; }

    public string WeekCode { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string Goals { get; set; } = string.Empty;

    public string? Summary { get; set; }

    public decimal TotalHours { get; set; }

    public WeeklyPlanStatus Status { get; set; } = WeeklyPlanStatus.Draft;

    public ICollection<WeeklyPlanTask> Tasks { get; set; } = new List<WeeklyPlanTask>();
}

public class WeeklyPlanTask : EntityBase
{
    public Guid WeeklyPlanId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public WeeklyPlanTaskPriority Priority { get; set; } = WeeklyPlanTaskPriority.Medium;

    public WeeklyPlanTaskStatus Status { get; set; } = WeeklyPlanTaskStatus.Pending;

    public decimal? EstimatedHours { get; set; }

    public decimal? ActualHours { get; set; }

    public WeeklyPlan WeeklyPlan { get; set; } = null!;
}

public enum WeeklyPlanStatus
{
    Draft = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}

public enum WeeklyPlanTaskPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}

public enum WeeklyPlanTaskStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}
