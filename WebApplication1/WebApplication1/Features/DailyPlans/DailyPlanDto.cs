using WebApplication1.Shared.Enums;

namespace WebApplication1.Features.DailyPlans;

public class DailyPlanDto
{
    public Guid Id { get; set; }

    public DateOnly PlanDate { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Priority { get; set; }

    public DailyPlanStatus Status { get; set; }

    public string? Remark { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
