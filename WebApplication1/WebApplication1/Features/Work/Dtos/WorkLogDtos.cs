using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Dtos;

public class IndustryTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Industry { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public List<TemplateFieldDto> Fields { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class TemplateFieldDto
{
    public Guid Id { get; set; }
    public Guid TemplateId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string FieldLabel { get; set; } = string.Empty;
    public int FieldType { get; set; }
    public string? Options { get; set; }
    public bool IsRequired { get; set; }
    public int Sort { get; set; }
    public string? DefaultValue { get; set; }
}

public class CreateTemplateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Industry { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public List<CreateTemplateFieldDto> Fields { get; set; } = new();
}

public class CreateTemplateFieldDto
{
    public string FieldName { get; set; } = string.Empty;
    public string FieldLabel { get; set; } = string.Empty;
    public int FieldType { get; set; }
    public string? Options { get; set; }
    public bool IsRequired { get; set; }
    public int Sort { get; set; }
    public string? DefaultValue { get; set; }
}

public class WorkLogDynamicValueDto
{
    public Guid Id { get; set; }
    public Guid WorkLogId { get; set; }
    public Guid FieldId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string? StringValue { get; set; }
    public decimal? NumberValue { get; set; }
    public DateOnly? DateValue { get; set; }
}

public class WorkLogDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public DateOnly WorkDate { get; set; }
    public string WeekDay { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? OriginalContent { get; set; }
    public string? Summary { get; set; }
    public decimal TotalHours { get; set; }
    public int Status { get; set; }
    public int SourceType { get; set; }
    public Guid? TemplateId { get; set; }
    public string? TemplateName { get; set; }
    public string? Remark { get; set; }
    public List<WorkLogDynamicValueDto> DynamicValues { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class WorkLogQueryDto : PageQueryDto
{
    public DateOnly? WorkDate { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? TemplateId { get; set; }
    public string? Keyword { get; set; }
    public int? Status { get; set; }
    public int? SourceType { get; set; }
}

public class CreateWorkLogDto
{
    public DateOnly WorkDate { get; set; }
    public Guid ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? OriginalContent { get; set; }
    public string? Summary { get; set; }
    public decimal TotalHours { get; set; }
    public string? Remark { get; set; }
    public Guid? TemplateId { get; set; }
    public List<DynamicFieldInput> DynamicValues { get; set; } = new();
}

public class UpdateWorkLogDto
{
    public DateOnly? WorkDate { get; set; }
    public Guid? ProjectId { get; set; }
    public string? Title { get; set; }
    public string? OriginalContent { get; set; }
    public string? Summary { get; set; }
    public decimal? TotalHours { get; set; }
    public int? Status { get; set; }
    public string? Remark { get; set; }
    public Guid? TemplateId { get; set; }
    public List<DynamicFieldInput> DynamicValues { get; set; } = new();
}

public class DynamicFieldInput
{
    public string FieldName { get; set; } = string.Empty;
    public string? StringValue { get; set; }
    public decimal? NumberValue { get; set; }
    public DateOnly? DateValue { get; set; }
}