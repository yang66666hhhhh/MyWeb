using WebApplication1.Shared;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Features.Work.Entities;

public class WorkProject : EntityBase
{
    public Guid? UserId { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public string? ProjectCode { get; set; }

    public WorkProjectType ProjectType { get; set; } = WorkProjectType.Internal;

    public string? CustomerName { get; set; }

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public WorkProjectStatus Status { get; set; } = WorkProjectStatus.Active;

    public int Sort { get; set; }
}
