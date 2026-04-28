using WebApplication1.Enums;

namespace WebApplication1.Entities.Work;

public class WorkDevice : EntityBase
{
    public Guid? UserId { get; set; }

    public Guid? ProjectId { get; set; }

    public string DeviceName { get; set; } = string.Empty;

    public string? DeviceCode { get; set; }

    public WorkDeviceType DeviceType { get; set; } = WorkDeviceType.Equipment;

    public string? Description { get; set; }

    public WorkDeviceStatus Status { get; set; } = WorkDeviceStatus.Active;

    public WorkProject? Project { get; set; }
}
