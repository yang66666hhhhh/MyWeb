using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Persona.Dtos;

// Dev DTOs
public record CodeRepositoryDto
{
    public string Id { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Url { get; init; } = string.Empty;
    public string Language { get; init; } = string.Empty;
    public bool IsPublic { get; init; }
    public int Stars { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateCodeRepositoryInput
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Url { get; init; } = string.Empty;
    public string Language { get; init; } = string.Empty;
    public bool IsPublic { get; init; } = true;
    public string? Tags { get; init; }
}

public record IssueDto
{
    public string Id { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Repository { get; init; } = string.Empty;
    public int Status { get; init; }
    public int Priority { get; init; }
    public string? Assignee { get; init; }
    public string? Labels { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateIssueInput
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Repository { get; init; } = string.Empty;
    public int Priority { get; init; } = 2;
    public string? Assignee { get; init; }
    public string? Labels { get; init; }
}

public record PipelineDto
{
    public string Id { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Repository { get; init; } = string.Empty;
    public string Branch { get; init; } = string.Empty;
    public int Status { get; init; }
    public string? TriggerType { get; init; }
    public string? Steps { get; init; }
    public string? LastRunAt { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreatePipelineInput
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Repository { get; init; } = string.Empty;
    public string Branch { get; init; } = string.Empty;
    public string? TriggerType { get; init; }
    public string? Steps { get; init; }
}

// Design DTOs
public record DesignAssetDto
{
    public string Id { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Category { get; init; } = string.Empty;
    public string FileUrl { get; init; } = string.Empty;
    public string FileType { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateDesignAssetInput
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Category { get; init; } = string.Empty;
    public string FileUrl { get; init; } = string.Empty;
    public string FileType { get; init; } = string.Empty;
    public long FileSize { get; init; }
    public string? Tags { get; init; }
}

public record PrototypeDto
{
    public string Id { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Project { get; init; } = string.Empty;
    public string? PreviewUrl { get; init; }
    public int Status { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreatePrototypeInput
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Project { get; init; } = string.Empty;
    public string? PreviewUrl { get; init; }
    public string? Tags { get; init; }
}

// Teacher DTOs
public record TeacherCourseDto
{
    public string Id { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Code { get; init; } = string.Empty;
    public int Semester { get; init; }
    public int Year { get; init; }
    public int StudentCount { get; init; }
    public int Status { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateTeacherCourseInput
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Code { get; init; } = string.Empty;
    public int Semester { get; init; }
    public int Year { get; init; }
    public string? Tags { get; init; }
}

public record TeacherStudentDto
{
    public string Id { get; init; } = string.Empty;
    public string UserId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? StudentId { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Course { get; init; }
    public int Grade { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateTeacherStudentInput
{
    public string Name { get; init; } = string.Empty;
    public string? StudentId { get; init; }
    public string? Email { get; init; }
    public string? Phone { get; init; }
    public string? Course { get; init; }
    public string? Tags { get; init; }
}

// Query DTOs
public class PersonaQueryDto : PageQueryDto
{
    public string? Category { get; init; }
    public int? Status { get; init; }
    public string? Keyword { get; init; }
}
