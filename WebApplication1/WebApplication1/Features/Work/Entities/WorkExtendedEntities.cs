using WebApplication1.Shared;

namespace WebApplication1.Features.Work.Entities;

public class Okr : EntityBase
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Objective { get; set; } = string.Empty;
    public string KeyResults { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Quarter { get; set; }
    public int Status { get; set; }
    public int Progress { get; set; }
    public string? Tags { get; set; }
}

public class RiskItem : EntityBase
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Impact { get; set; }
    public int Probability { get; set; }
    public int Status { get; set; }
    public string? MitigationPlan { get; set; }
    public DateTime? IdentifiedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? Tags { get; set; }
}

public class WorkFile : EntityBase
{
    public Guid UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
}
