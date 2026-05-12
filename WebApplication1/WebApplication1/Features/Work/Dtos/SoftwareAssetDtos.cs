using System.ComponentModel.DataAnnotations;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Dtos;

public class SoftwareAssetDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Version { get; set; }
    public int Type { get; set; }
    public int LicenseType { get; set; }
    public int Status { get; set; }
    public string? Vendor { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public DateOnly? ExpireDate { get; set; }
    public decimal? Cost { get; set; }
    public string? Description { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SoftwareAssetQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public int? Type { get; set; }
    public int? LicenseType { get; set; }
    public int? Status { get; set; }
}

public class CreateSoftwareAssetDto
{
    [Required(ErrorMessage = "软件名称不能为空")]
    [MaxLength(100, ErrorMessage = "名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50, ErrorMessage = "版本不能超过50个字符")]
    public string? Version { get; set; }

    public int Type { get; set; }

    public int LicenseType { get; set; }

    public int Status { get; set; }

    [MaxLength(100, ErrorMessage = "厂商不能超过100个字符")]
    public string? Vendor { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public DateOnly? ExpireDate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "成本必须为正数")]
    public decimal? Cost { get; set; }

    [MaxLength(500, ErrorMessage = "描述不能超过500个字符")]
    public string? Description { get; set; }

    [MaxLength(100, ErrorMessage = "使用人不能超过100个字符")]
    public string? AssignedTo { get; set; }
}

public class UpdateSoftwareAssetDto
{
    [MaxLength(100, ErrorMessage = "名称不能超过100个字符")]
    public string? Name { get; set; }

    [MaxLength(50, ErrorMessage = "版本不能超过50个字符")]
    public string? Version { get; set; }

    public int? Type { get; set; }

    public int? LicenseType { get; set; }

    public int? Status { get; set; }

    [MaxLength(100, ErrorMessage = "厂商不能超过100个字符")]
    public string? Vendor { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public DateOnly? ExpireDate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "成本必须为正数")]
    public decimal? Cost { get; set; }

    [MaxLength(500, ErrorMessage = "描述不能超过500个字符")]
    public string? Description { get; set; }

    [MaxLength(100, ErrorMessage = "使用人不能超过100个字符")]
    public string? AssignedTo { get; set; }
}
