using WebApplication1.Shared;

namespace WebApplication1.Features.Work.Entities;

public class WorkTaskType : EntityBase
{
    public Guid? UserId { get; set; }

    public string TypeName { get; set; } = string.Empty;

    public string? TypeCode { get; set; }

    public string? Description { get; set; }

    public int Sort { get; set; }

    public bool Enabled { get; set; } = true;
}
