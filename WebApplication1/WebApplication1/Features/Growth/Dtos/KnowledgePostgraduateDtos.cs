using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Dtos;

public class KnowledgeArticleDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public int ViewCount { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class KnowledgeArticleQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public string? Category { get; set; }
    public bool? IsPublished { get; set; }
}

public class CreateKnowledgeArticleDto
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public bool IsPublished { get; set; } = true;
}

public class UpdateKnowledgeArticleDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public bool? IsPublished { get; set; }
}

public class PostgraduateTaskDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? DueDate { get; set; }
    public int Status { get; set; }
    public int Priority { get; set; }
    public int Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PostgraduateTaskQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public int? Status { get; set; }
    public int? Type { get; set; }
}

public class CreatePostgraduateTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? DueDate { get; set; }
    public int Status { get; set; } = 0;
    public int Priority { get; set; } = 2;
    public int Type { get; set; } = 0;
}

public class UpdatePostgraduateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateOnly? DueDate { get; set; }
    public int? Status { get; set; }
    public int? Priority { get; set; }
    public int? Type { get; set; }
}

public class ExamMistakeDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string? Answer { get; set; }
    public string? Explanation { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public int Status { get; set; }
    public int ReviewCount { get; set; }
    public DateOnly? LastReviewDate { get; set; }
    public DateOnly? NextReviewDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ExamMistakeQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public string? Subject { get; set; }
}

public class CreateExamMistakeDto
{
    public string Question { get; set; } = string.Empty;
    public string? Answer { get; set; }
    public string? Explanation { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Tags { get; set; }
}

public class UpdateExamMistakeDto
{
    public string? Question { get; set; }
    public string? Answer { get; set; }
    public string? Explanation { get; set; }
    public string? Subject { get; set; }
    public string? Tags { get; set; }
    public int? Status { get; set; }
    public int? ReviewCount { get; set; }
    public DateOnly? LastReviewDate { get; set; }
    public DateOnly? NextReviewDate { get; set; }
}

public class ExamMaterialDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public int Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ExamMaterialQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public string? Subject { get; set; }
    public int? Type { get; set; }
}

public class CreateExamMaterialDto
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Tags { get; set; }
    public int Type { get; set; } = 0;
}

public class UpdateExamMaterialDto
{
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? Subject { get; set; }
    public string? Tags { get; set; }
    public int? Type { get; set; }
}

public class ExamDashboardDto
{
    public List<PostgraduateTaskDto> TodayTasks { get; set; } = new();
    public double WeeklyHours { get; set; }
    public int MistakeCount { get; set; }
    public int MaterialCount { get; set; }
    public int ReviewTaskCount { get; set; }
    public List<SubjectProgressDto> Subjects { get; set; } = new();
    public List<StudyRecordDto> RecentRecords { get; set; } = new();
}

public class SubjectProgressDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Progress { get; set; }
    public double WeeklyHours { get; set; }
    public int TargetHours { get; set; }
    public string Color { get; set; } = "blue";
}

public class StudyRecordDto
{
    public string Id { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string RecordDate { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
}