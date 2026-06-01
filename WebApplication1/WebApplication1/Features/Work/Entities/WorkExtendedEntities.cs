using WebApplication1.Shared;

namespace WebApplication1.Features.Work.Entities;

public class Okr : EntityBase
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Objective { get; set; }
    public string? KeyResults { get; set; }
    public int Status { get; set; }
    public int Progress { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Tags { get; set; }
}

public class WorkRisk : EntityBase
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Impact { get; set; }
    public string? Mitigation { get; set; }
    public int Status { get; set; }
    public int Level { get; set; }
    public Guid? ProjectId { get; set; }
    public string? Tags { get; set; }
}

public class WorkFile : EntityBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string FileType { get; set; } = string.Empty;
    public Guid? ProjectId { get; set; }
    public string? Tags { get; set; }
}
