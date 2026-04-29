using WebApplication1.Shared;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Features.DailyPlans;

public class DailyPlan : EntityBase
{
    public DateOnly PlanDate { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Priority { get; set; } = 3;

    public DailyPlanStatus Status { get; set; } = DailyPlanStatus.Pending;

    public string? Remark { get; set; }

    public DateTime? CompletedAt { get; set; }
}
