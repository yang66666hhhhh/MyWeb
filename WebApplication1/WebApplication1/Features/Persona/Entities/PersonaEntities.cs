using WebApplication1.Shared;

namespace WebApplication1.Features.Persona.Entities;

public class CodeRepository : EntityBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Url { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = true;
    public int Stars { get; set; }
    public string? Tags { get; set; }
}

public class Issue : EntityBase
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Repository { get; set; } = string.Empty;
    public int Status { get; set; }
    public int Priority { get; set; }
    public string? Assignee { get; set; }
    public string? Labels { get; set; }
}

public class Pipeline : EntityBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Repository { get; set; } = string.Empty;
    public string Branch { get; set; } = string.Empty;
    public int Status { get; set; }
    public string? TriggerType { get; set; }
    public string? Steps { get; set; }
    public DateTime? LastRunAt { get; set; }
}

public class DesignAsset : EntityBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Tags { get; set; }
}

public class Prototype : EntityBase
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Project { get; set; } = string.Empty;
    public string? PreviewUrl { get; set; }
    public int Status { get; set; }
    public string? Tags { get; set; }
}

public class TeacherCourse : EntityBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Code { get; set; } = string.Empty;
    public int Semester { get; set; }
    public int Year { get; set; }
    public int StudentCount { get; set; }
    public int Status { get; set; }
    public string? Tags { get; set; }
}

public class TeacherStudent : EntityBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? StudentId { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Course { get; set; }
    public int Grade { get; set; }
    public string? Tags { get; set; }
}
