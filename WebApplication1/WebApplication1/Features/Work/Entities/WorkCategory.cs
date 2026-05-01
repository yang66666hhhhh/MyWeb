using WebApplication1.Shared;

namespace WebApplication1.Features.Work.Entities;

public class WorkCategory : EntityBase
{
    public Guid? TenantId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public int Level { get; set; }

    public Guid? ParentId { get; set; }

    public int Sort { get; set; }

    public bool IsActive { get; set; } = true;

    public WorkCategory? Parent { get; set; }

    public ICollection<WorkCategory> Children { get; set; } = new List<WorkCategory>();

    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}