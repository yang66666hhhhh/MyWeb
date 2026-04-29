using WebApplication1.Shared.Common;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Features.Work.Dtos;

public class WorkLogDto
{
    public Guid Id { get; set; }
    public DateOnly WorkDate { get; set; }
    public string WeekDay { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public List<Guid> DeviceIds { get; set; } = new();
    public List<string> DeviceNames { get; set; } = new();
    public List<Guid> TaskTypeIds { get; set; } = new();
    public List<string> TaskTypeNames { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string? OriginalContent { get; set; }
    public string? Summary { get; set; }
    public decimal TotalHours { get; set; }
    public WorkLogStatus Status { get; set; }
    public WorkLogSourceType SourceType { get; set; }
    public Guid? ImportBatchId { get; set; }
    public string? Remark { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateWorkLogDto
{
    public DateOnly WorkDate { get; set; }
    public string WeekDay { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public List<Guid> DeviceIds { get; set; } = new();
    public List<Guid> TaskTypeIds { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string? OriginalContent { get; set; }
    public string? Summary { get; set; }
    public decimal? TotalHours { get; set; }
    public WorkLogStatus Status { get; set; } = WorkLogStatus.Normal;
    public WorkLogSourceType SourceType { get; set; } = WorkLogSourceType.Manual;
    public string? Remark { get; set; }
}

public class UpdateWorkLogDto : CreateWorkLogDto
{
    public Guid Id { get; set; }
}

public class WorkLogQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public DateOnly? WorkDate { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? DeviceId { get; set; }
    public Guid? TaskTypeId { get; set; }
    public WorkLogSourceType? SourceType { get; set; }
    public WorkLogStatus? Status { get; set; }
}
