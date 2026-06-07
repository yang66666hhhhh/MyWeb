namespace WebApplication1.Features.Shared.Dtos;

public enum ExportFormat
{
    Excel = 0,
    Csv = 1,
    Json = 2
}

public class ExportQueryDto
{
    public ExportFormat Format { get; set; } = ExportFormat.Excel;
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}

public class ImportResultDto
{
    public int TotalRows { get; set; }
    public int SuccessRows { get; set; }
    public int FailedRows { get; set; }
    public int SkippedRows { get; set; }
    public List<ImportErrorDto> Errors { get; set; } = [];
}

public class ImportErrorDto
{
    public int RowNumber { get; set; }
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class ImportPreviewDto
{
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int ErrorRows { get; set; }
    public List<Dictionary<string, object?>> PreviewRows { get; set; } = [];
    public List<ImportErrorDto> Errors { get; set; } = [];
}
