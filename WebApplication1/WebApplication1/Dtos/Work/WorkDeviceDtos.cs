using WebApplication1.Enums;

namespace WebApplication1.Dtos.Work;

public class WorkDeviceDto
{
    public Guid Id { get; set; }
    public Guid? ProjectId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string? DeviceCode { get; set; }
    public WorkDeviceType DeviceType { get; set; }
    public string? Description { get; set; }
    public WorkDeviceStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateWorkDeviceDto
{
    public Guid? ProjectId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string? DeviceCode { get; set; }
    public WorkDeviceType DeviceType { get; set; } = WorkDeviceType.Equipment;
    public string? Description { get; set; }
    public WorkDeviceStatus Status { get; set; } = WorkDeviceStatus.Active;
}

public class UpdateWorkDeviceDto : CreateWorkDeviceDto
{
    public Guid Id { get; set; }
}

public class WorkDeviceQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public Guid? ProjectId { get; set; }
    public WorkDeviceType? DeviceType { get; set; }
    public WorkDeviceStatus? Status { get; set; }
}
