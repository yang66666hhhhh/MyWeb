using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Dtos;

// OKR DTOs
public record OkrDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Objective { get; init; } = string.Empty;
    public string KeyResults { get; init; } = string.Empty;
    public int Year { get; init; }
    public int Quarter { get; init; }
    public int Status { get; init; }
    public int Progress { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateOkrInput
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Objective { get; init; } = string.Empty;
    public string KeyResults { get; init; } = string.Empty;
    public int Year { get; init; }
    public int Quarter { get; init; }
    public string? Tags { get; init; }
}

public record UpdateOkrInput
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? Objective { get; init; }
    public string? KeyResults { get; init; }
    public int? Status { get; init; }
    public int? Progress { get; init; }
    public string? Tags { get; init; }
}

// Risk DTOs
public record RiskItemDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Category { get; init; } = string.Empty;
    public int Impact { get; init; }
    public int Probability { get; init; }
    public int Status { get; init; }
    public string? MitigationPlan { get; init; }
    public string? IdentifiedDate { get; init; }
    public string? ResolvedDate { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateRiskItemInput
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Category { get; init; } = string.Empty;
    public int Impact { get; init; } = 2;
    public int Probability { get; init; } = 2;
    public string? MitigationPlan { get; init; }
    public string? IdentifiedDate { get; init; }
    public string? Tags { get; init; }
}

public record UpdateRiskItemInput
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? Category { get; init; }
    public int? Impact { get; init; }
    public int? Probability { get; init; }
    public int? Status { get; init; }
    public string? MitigationPlan { get; init; }
    public string? ResolvedDate { get; init; }
    public string? Tags { get; init; }
}

// File DTOs
public record WorkFileDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string FileUrl { get; init; } = string.Empty;
    public string FileType { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string? Category { get; init; }
    public string? Description { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateWorkFileInput
{
    public string FileName { get; init; } = string.Empty;
    public string FileUrl { get; init; } = string.Empty;
    public string FileType { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string? Category { get; init; }
    public string? Description { get; init; }
    public string? Tags { get; init; }
}

public record UpdateWorkFileInput
{
    public string? FileName { get; init; }
    public string? FileUrl { get; init; }
    public string? FileType { get; init; }
    public long? FileSize { get; init; }
    public string? Category { get; init; }
    public string? Description { get; init; }
    public string? Tags { get; init; }
}

// Query DTOs
public class WorkExtendedQueryDto : PageQueryDto
{
    public string? Category { get; init; }
    public int? Status { get; init; }
    public string? Keyword { get; init; }
    public int? Year { get; init; }
    public int? Quarter { get; init; }
}
