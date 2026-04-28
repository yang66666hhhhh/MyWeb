using System.ComponentModel.DataAnnotations;
using WebApplication1.Enums;

namespace WebApplication1.Dtos.DailyPlans;

public class UpdateDailyPlanDto
{
    [Required]
    public DateOnly PlanDate { get; set; }

    [Required]
    [StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(1, 5)]
    public int Priority { get; set; } = 3;

    public DailyPlanStatus Status { get; set; }

    [StringLength(1000)]
    public string? Remark { get; set; }
}
