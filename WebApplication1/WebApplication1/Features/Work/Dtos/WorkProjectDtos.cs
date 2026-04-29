using WebApplication1.Shared.Common;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Features.Work.Dtos;

public class WorkProjectDto
{
    public Guid Id { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string? ProjectCode { get; set; }
    public WorkProjectType ProjectType { get; set; }
    public string? CustomerName { get; set; }
    public string? Description { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public WorkProjectStatus Status { get; set; }
    public int Sort { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateWorkProjectDto
{
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

public class UpdateWorkProjectDto : CreateWorkProjectDto
{
    public Guid Id { get; set; }
}

public class WorkProjectQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public WorkProjectStatus? Status { get; set; }
    public WorkProjectType? ProjectType { get; set; }
}
