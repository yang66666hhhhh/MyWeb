using WebApplication1.Shared;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Features.Work.Entities;

public class WorkImportBatch : EntityBase
{
    public Guid? UserId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public int ImportType { get; set; }

    public int TotalRows { get; set; }

    public int SuccessRows { get; set; }

    public int FailedRows { get; set; }

    public int SkippedRows { get; set; }

    public int DuplicateRows { get; set; }

    public WorkImportStatus Status { get; set; } = WorkImportStatus.Pending;

    public WorkImportStrategy ImportStrategy { get; set; } = WorkImportStrategy.SkipDuplicate;

    public DateTime? StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    public string? ErrorMessage { get; set; }

    public ICollection<WorkImportRow> Rows { get; set; } = new List<WorkImportRow>();
}

public class WorkImportRow : EntityBase
{
    public Guid BatchId { get; set; }

    public int RowNumber { get; set; }

    public string? RawDate { get; set; }

    public string? RawWeekDay { get; set; }

    public string? RawProject { get; set; }

    public string? RawDevice { get; set; }

    public string? RawTaskType { get; set; }

    public string? RawContent { get; set; }

    public string? RawHours { get; set; }

    public string? RawRemark { get; set; }

    public DateOnly? ParsedDate { get; set; }

    public decimal? ParsedHours { get; set; }

    public WorkImportValidationStatus ValidationStatus { get; set; } = WorkImportValidationStatus.Valid;

    public string? ErrorMessage { get; set; }

    public int DuplicateStatus { get; set; }

    public Guid? ImportedWorkLogId { get; set; }

    public WorkImportBatch Batch { get; set; } = null!;
}
