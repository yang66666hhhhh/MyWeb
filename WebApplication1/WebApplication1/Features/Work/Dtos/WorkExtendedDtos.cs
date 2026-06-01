using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Dtos;

public record OkrDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Objective { get; init; }
    public string? KeyResults { get; init; }
    public int Status { get; init; }
    public int Progress { get; init; }
    public string? StartDate { get; init; }
    public string? EndDate { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateOkrInput
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Objective { get; init; }
    public string? KeyResults { get; init; }
    public string? StartDate { get; init; }
    public string? EndDate { get; init; }
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
    public string? StartDate { get; init; }
    public string? EndDate { get; init; }
    public string? Tags { get; init; }
}

public record RiskItemDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Impact { get; init; }
    public string? Mitigation { get; init; }
    public int Status { get; init; }
    public int Level { get; init; }
    public string? ProjectId { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateRiskItemInput
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Impact { get; init; }
    public string? Mitigation { get; init; }
    public int Level { get; init; } = 2;
    public string? ProjectId { get; init; }
    public string? Tags { get; init; }
}

public record UpdateRiskItemInput
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? Impact { get; init; }
    public string? Mitigation { get; init; }
    public int? Status { get; init; }
    public int? Level { get; init; }
    public string? ProjectId { get; init; }
    public string? Tags { get; init; }
}

public record WorkFileDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string FilePath { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string FileType { get; init; } = string.Empty;
    public string? ProjectId { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateWorkFileInput
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string FilePath { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string FileType { get; init; } = string.Empty;
    public string? ProjectId { get; init; }
    public string? Tags { get; init; }
}

public record UpdateWorkFileInput
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? FilePath { get; init; }
    public long? FileSize { get; init; }
    public string? FileType { get; init; }
    public string? ProjectId { get; init; }
    public string? Tags { get; init; }
}

public class WorkExtendedQueryDto : PageQueryDto
{
    public string? Category { get; init; }
    public int? Status { get; init; }
    public string? Keyword { get; init; }
    public int? Year { get; init; }
    public int? Quarter { get; init; }
}
