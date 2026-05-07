using System.ComponentModel.DataAnnotations;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Dtos;

public class WorkLogTemplateDto
{
    public Guid Id { get; set; }
    public string PersonaCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public FieldDefinitionsDto FieldDefinitions { get; set; } = new();
    public bool IsActive { get; set; }
    public int Sort { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class FieldDefinitionsDto
{
    public List<FieldDefinitionDto> Fields { get; set; } = new();
}

public class FieldDefinitionDto
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = "text";
    public List<string>? Options { get; set; }
    public bool Required { get; set; }
    public string? Placeholder { get; set; }
    public List<string>? Fields { get; set; }
}

public class WorkLogTemplateQueryDto : PageQueryDto
{
    public string? PersonaCode { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateWorkLogTemplateDto
{
    [Required(ErrorMessage = "Persona编码不能为空")]
    public string PersonaCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "模板名称不能为空")]
    [MaxLength(100, ErrorMessage = "模板名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "描述不能超过500个字符")]
    public string? Description { get; set; }

    public FieldDefinitionsDto FieldDefinitions { get; set; } = new();

    public bool IsActive { get; set; } = true;

    public int Sort { get; set; }
}

public class UpdateWorkLogTemplateDto
{
    [MaxLength(100, ErrorMessage = "模板名称不能超过100个字符")]
    public string? Name { get; set; }

    [MaxLength(500, ErrorMessage = "描述不能超过500个字符")]
    public string? Description { get; set; }

    public FieldDefinitionsDto? FieldDefinitions { get; set; }

    public bool? IsActive { get; set; }

    public int? Sort { get; set; }
}
