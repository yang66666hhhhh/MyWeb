using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Dtos;

public class WorkDailyPlanDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public DateOnly PlanDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public int Priority { get; set; }
    public int Status { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
    public Guid? ConvertedWorkLogId { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class WorkDailyPlanQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public string? PlanDate { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid? ProjectId { get; set; }
    public int? Status { get; set; }
    public int? Priority { get; set; }
}

public class CreateWorkDailyPlanDto
{
    public DateOnly PlanDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? ProjectId { get; set; }
    public int Priority { get; set; } = 2;
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public decimal? EstimatedHours { get; set; }
    public string? Remark { get; set; }
}

public class UpdateWorkDailyPlanDto
{
    public DateOnly? PlanDate { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? ProjectId { get; set; }
    public int? Priority { get; set; }
    public int? Status { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public decimal? EstimatedHours { get; set; }
    public decimal? ActualHours { get; set; }
    public string? Remark { get; set; }
}

public class ConvertToWorkLogDto
{
    public string PlanId { get; set; } = string.Empty;
    public DateOnly WorkDate { get; set; }
    public string? OriginalContent { get; set; }
    public decimal? TotalHours { get; set; }
}