using System.ComponentModel.DataAnnotations;
using WebApplication1.Enums;

namespace WebApplication1.Dtos.DailyPlans;

public class DailyPlanQueryDto
{
    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DailyPlanStatus? Status { get; set; }

    public string? Keyword { get; set; }

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
}
