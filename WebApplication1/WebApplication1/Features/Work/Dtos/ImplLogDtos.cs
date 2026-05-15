using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Dtos;

public class ImplLogDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly WorkDate { get; set; }
    public string WeekDay { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public Guid? ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public decimal TotalHours { get; set; }
    public string? PersonaCode { get; set; }
    public Guid? TemplateId { get; set; }
    public string? TemplateName { get; set; }
    public JsonElement? ExtraData { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ImplLogQueryDto : PageQueryDto
{
    public DateOnly? WorkDate { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Keyword { get; set; }
    public Guid? TemplateId { get; set; }
}

public class CreateImplLogDto
{
    [Required(ErrorMessage = "工作日期不能为空")]
    public DateOnly WorkDate { get; set; }

    [Required(ErrorMessage = "标题不能为空")]
    [MaxLength(200, ErrorMessage = "标题不能超过200个字符")]
    public string Title { get; set; } = string.Empty;

    public Guid? ProjectId { get; set; }

    [MaxLength(200, ErrorMessage = "项目名称不能超过200个字符")]
    public string? ProjectName { get; set; }

    [Range(0, 24, ErrorMessage = "工时必须在0-24之间")]
    public decimal TotalHours { get; set; }

    public Guid? TemplateId { get; set; }

    public JsonElement? ExtraData { get; set; }
}

public class UpdateImplLogDto
{
    public DateOnly? WorkDate { get; set; }

    [MaxLength(200, ErrorMessage = "标题不能超过200个字符")]
    public string? Title { get; set; }

    public Guid? ProjectId { get; set; }

    [MaxLength(200, ErrorMessage = "项目名称不能超过200个字符")]
    public string? ProjectName { get; set; }

    [Range(0, 24, ErrorMessage = "工时必须在0-24之间")]
    public decimal? TotalHours { get; set; }

    public Guid? TemplateId { get; set; }

    public JsonElement? ExtraData { get; set; }
}
