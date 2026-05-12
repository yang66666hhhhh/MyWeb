using System.ComponentModel.DataAnnotations;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Dtos;

public class WeeklyPlanDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int Year { get; set; }
    public int WeekNumber { get; set; }
    public string WeekCode { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public string Goals { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public decimal TotalHours { get; set; }
    public int Status { get; set; }
    public List<WeeklyPlanTaskDto> Tasks { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class WeeklyPlanTaskDto
{
    public Guid Id { get; set; }
    public Guid WeeklyPlanId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Priority { get; set; }
    public int Status { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WeeklyPlanQueryDto : PageQueryDto
{
    public int? Year { get; set; }
    public int? WeekNumber { get; set; }
    public int? Status { get; set; }
    public string? Keyword { get; set; }
}

public class CreateWeeklyPlanDto
{
    [Required(ErrorMessage = "年份不能为空")]
    public int Year { get; set; }

    [Required(ErrorMessage = "周数不能为空")]
    [Range(1, 53, ErrorMessage = "周数必须在1-53之间")]
    public int WeekNumber { get; set; }

    public string Goals { get; set; } = string.Empty;

    public int Status { get; set; }
}

public class UpdateWeeklyPlanDto
{
    public string? Goals { get; set; }

    public string? Summary { get; set; }

    public int? Status { get; set; }
}

public class CreateWeeklyPlanTaskDto
{
    [Required(ErrorMessage = "任务标题不能为空")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int Priority { get; set; } = 2;

    public decimal? EstimatedHours { get; set; }
}

public class UpdateWeeklyPlanTaskDto
{
    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? Priority { get; set; }

    public int? Status { get; set; }

    public decimal? EstimatedHours { get; set; }

    public decimal? ActualHours { get; set; }
}
