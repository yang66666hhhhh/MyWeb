using WebApplication1.Shared;

namespace WebApplication1.Features.Work.Entities;

public class SoftwareAsset : EntityBase
{
    public Guid? UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Version { get; set; }

    public SoftwareAssetType Type { get; set; } = SoftwareAssetType.Other;

    public SoftwareLicenseType LicenseType { get; set; } = SoftwareLicenseType.Free;

    public SoftwareAssetStatus Status { get; set; } = SoftwareAssetStatus.Available;

    public string? Vendor { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public DateOnly? ExpireDate { get; set; }

    public decimal? Cost { get; set; }

    public string? Description { get; set; }

    public string? AssignedTo { get; set; }
}

public enum SoftwareAssetType
{
    IDE = 0,
    DesignTool = 1,
    ApiTool = 2,
    ProjectManagement = 3,
    Communication = 4,
    DevOps = 5,
    Database = 6,
    Other = 99
}

public enum SoftwareLicenseType
{
    Free = 0,
    Paid = 1,
    OpenSource = 2,
    Trial = 3
}

public enum SoftwareAssetStatus
{
    Available = 0,
    InUse = 1,
    Expired = 2,
    Retired = 3
}
