using WebApplication1.Shared;

namespace WebApplication1.Features.Work.Entities;

public class WorkLogTemplate : EntityBase
{
    public string PersonaCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string FieldDefinitions { get; set; } = "{}";

    public bool IsActive { get; set; } = true;

    public int Sort { get; set; }

    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}
