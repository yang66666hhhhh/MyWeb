using WebApplication1.Shared;

namespace WebApplication1.Features.Growth.Entities;

public class KnowledgeArticle : EntityBase
{
    public Guid? UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    public string Category { get; set; } = string.Empty;

    public string? Tags { get; set; }

    public int ViewCount { get; set; }

    public bool IsPublished { get; set; } = true;
}

public class PostgraduateTask : EntityBase
{
    public Guid? UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateOnly? DueDate { get; set; }

    public PostgraduateTaskStatus Status { get; set; } = PostgraduateTaskStatus.Pending;

    public PostgraduateTaskPriority Priority { get; set; } = PostgraduateTaskPriority.Medium;

    public PostgraduateTaskType Type { get; set; } = PostgraduateTaskType.Study;
}

public enum PostgraduateTaskStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Overdue = 3
}

public enum PostgraduateTaskPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}

public enum PostgraduateTaskType
{
    Study = 0,
    Practice = 1,
    Review = 2,
    Mock = 3
}

public class ExamMistake : EntityBase
{
    public Guid? UserId { get; set; }

    public string Question { get; set; } = string.Empty;

    public string? Answer { get; set; }

    public string? Explanation { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string? Tags { get; set; }

    public int ReviewCount { get; set; }

    public DateOnly? LastReviewDate { get; set; }

    public DateOnly? NextReviewDate { get; set; }

    public ExamMistakeStatus Status { get; set; } = ExamMistakeStatus.Pending;
}

public enum ExamMistakeStatus
{
    Pending = 0,
    Reviewed = 1,
    Mastered = 2
}

public class ExamMaterial : EntityBase
{
    public Guid? UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string? Tags { get; set; }

    public ExamMaterialType Type { get; set; } = ExamMaterialType.Note;
}

public enum ExamMaterialType
{
    Note = 0,
    Summary = 1,
    Formula = 2,
    Template = 3
}