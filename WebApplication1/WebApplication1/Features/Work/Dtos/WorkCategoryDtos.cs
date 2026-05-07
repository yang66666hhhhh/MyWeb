using System.ComponentModel.DataAnnotations;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Dtos;

public class WorkCategoryDto
{
    public Guid Id { get; set; }
    public Guid? TenantId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Level { get; set; }
    public Guid? ParentId { get; set; }
    public int Sort { get; set; }
    public bool IsActive { get; set; }
    public List<WorkCategoryDto> Children { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class WorkCategoryQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateWorkCategoryDto
{
    [Required(ErrorMessage = "分类名称不能为空")]
    [MaxLength(100, ErrorMessage = "分类名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "分类编码不能为空")]
    [MaxLength(50, ErrorMessage = "分类编码不能超过50个字符")]
    public string Code { get; set; } = string.Empty;

    public int Level { get; set; }

    public Guid? ParentId { get; set; }

    public int Sort { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UpdateWorkCategoryDto
{
    [MaxLength(100, ErrorMessage = "分类名称不能超过100个字符")]
    public string? Name { get; set; }

    [MaxLength(50, ErrorMessage = "分类编码不能超过50个字符")]
    public string? Code { get; set; }

    public int? Level { get; set; }

    public Guid? ParentId { get; set; }

    public int? Sort { get; set; }

    public bool? IsActive { get; set; }
}
