using WebApplication1.Shared.Common;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Features.Work.Dtos;

public class WorkImportBatchDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public int TotalRows { get; set; }
    public int SuccessRows { get; set; }
    public int FailedRows { get; set; }
    public int SkippedRows { get; set; }
    public int DuplicateRows { get; set; }
    public WorkImportStatus Status { get; set; }
    public WorkImportStrategy ImportStrategy { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WorkImportBatchQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public WorkImportStatus? Status { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class WorkImportPreviewDto
{
    public int RowNumber { get; set; }
    public string WorkDate { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string DeviceNames { get; set; } = string.Empty;
    public string TaskTypeNames { get; set; } = string.Empty;
    public string OriginalContent { get; set; } = string.Empty;
    public decimal? TotalHours { get; set; }
    public string? Remark { get; set; }
    public WorkImportValidationStatus ValidationStatus { get; set; }
    public string? ErrorMessage { get; set; }
    public int DuplicateStatus { get; set; }
}

public class WorkImportPreviewResultDto
{
    public List<WorkImportPreviewDto> Items { get; set; } = new();
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int WarningRows { get; set; }
    public int ErrorRows { get; set; }
    public int DuplicateRows { get; set; }
}

public class WorkImportConfirmDto
{
    public Guid BatchId { get; set; }
    public WorkImportStrategy ImportStrategy { get; set; } = WorkImportStrategy.SkipDuplicate;
}

public class WorkImportConfirmResultDto
{
    public int SuccessRows { get; set; }
    public int FailedRows { get; set; }
    public int SkippedRows { get; set; }
    public int DuplicateRows { get; set; }
}
