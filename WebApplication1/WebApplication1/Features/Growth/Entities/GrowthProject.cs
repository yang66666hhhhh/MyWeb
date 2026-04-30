using WebApplication1.Shared;

namespace WebApplication1.Features.Growth.Entities;

public class GrowthProject : EntityBase
{
    public Guid? UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public int Progress { get; set; }

    public int TaskCount { get; set; }

    public GrowthProjectStatus Status { get; set; } = GrowthProjectStatus.InProgress;

    public GrowthProjectType Type { get; set; } = GrowthProjectType.Personal;
}

public enum GrowthProjectStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Suspended = 3
}

public enum GrowthProjectType
{
    Personal = 0,
    Study = 1,
    Work = 2,
    Other = 3
}