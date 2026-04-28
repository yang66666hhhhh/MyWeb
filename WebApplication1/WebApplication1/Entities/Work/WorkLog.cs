using WebApplication1.Enums;

namespace WebApplication1.Entities.Work;

public class WorkLog : EntityBase
{
    public Guid? UserId { get; set; }

    public DateOnly WorkDate { get; set; }

    public string WeekDay { get; set; } = string.Empty;

    public Guid ProjectId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? OriginalContent { get; set; }

    public string? Summary { get; set; }

    public decimal TotalHours { get; set; }

    public WorkLogStatus Status { get; set; } = WorkLogStatus.Normal;

    public WorkLogSourceType SourceType { get; set; } = WorkLogSourceType.Manual;

    public Guid? ImportBatchId { get; set; }

    public string? Remark { get; set; }

    public WorkProject? Project { get; set; }

    public ICollection<WorkLogItem> Items { get; set; } = new List<WorkLogItem>();
}

public class WorkLogItem : EntityBase
{
    public Guid WorkLogId { get; set; }

    public string Content { get; set; } = string.Empty;

    public Guid? TaskTypeId { get; set; }

    public Guid? DeviceId { get; set; }

    public int? ProgressPercent { get; set; }

    public decimal? Hours { get; set; }

    public int Sort { get; set; }

    public string? Remark { get; set; }

    public WorkLog WorkLog { get; set; } = null!;

    public WorkTaskType? TaskType { get; set; }

    public WorkDevice? Device { get; set; }
}
