using System.ComponentModel.DataAnnotations;
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
    [Required(ErrorMessage = "工作日期不能为空")]
    public DateOnly WorkDate { get; set; }

    [Required(ErrorMessage = "项目不能为空")]
    public Guid ProjectId { get; set; }

    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(200, ErrorMessage = "标题不能超过200个字符")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000, ErrorMessage = "原始内容不能超过4000个字符")]
    public string? OriginalContent { get; set; }

    [MaxLength(1000, ErrorMessage = "摘要不能超过1000个字符")]
    public string? Summary { get; set; }

    [Range(0, 24, ErrorMessage = "工时必须在0-24之间")]
    public decimal TotalHours { get; set; }

    [MaxLength(1000, ErrorMessage = "备注不能超过1000个字符")]
    public string? Remark { get; set; }

    public Guid? TemplateId { get; set; }
    public List<DynamicFieldInput> DynamicValues { get; set; } = new();
}

public class UpdateWorkLogDto
{
    public DateOnly? WorkDate { get; set; }
    public Guid? ProjectId { get; set; }

    [MaxLength(200, ErrorMessage = "标题不能超过200个字符")]
    public string? Title { get; set; }

    [MaxLength(4000, ErrorMessage = "原始内容不能超过4000个字符")]
    public string? OriginalContent { get; set; }

    [MaxLength(1000, ErrorMessage = "摘要不能超过1000个字符")]
    public string? Summary { get; set; }

    [Range(0, 24, ErrorMessage = "工时必须在0-24之间")]
    public decimal? TotalHours { get; set; }

    public int? Status { get; set; }

    [MaxLength(1000, ErrorMessage = "备注不能超过1000个字符")]
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