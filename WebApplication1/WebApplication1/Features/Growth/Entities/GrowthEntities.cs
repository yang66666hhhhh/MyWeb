using WebApplication1.Shared;

namespace WebApplication1.Features.Growth.Entities;

public class Skill : EntityBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Level { get; set; } = 1;
    public int TargetLevel { get; set; } = 5;
    public int ExperiencePoints { get; set; }
    public string? Description { get; set; }
    public string? Tags { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Goal : EntityBase
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Priority { get; set; } = 2;
    public int Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int Progress { get; set; }
    public string? Tags { get; set; }
}

public class YearPlan : EntityBase
{
    public Guid UserId { get; set; }
    public int Year { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Status { get; set; }
    public int Progress { get; set; }
    public string? Tags { get; set; }
}

public class MonthlyReview : EntityBase
{
    public Guid UserId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Achievements { get; set; }
    public string? Challenges { get; set; }
    public string? LessonsLearned { get; set; }
    public string? NextMonthGoals { get; set; }
    public int Rating { get; set; }
    public string? Tags { get; set; }
}

public class LearningPath : EntityBase
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public int Status { get; set; }
    public int Progress { get; set; }
    public int EstimatedHours { get; set; }
    public int ActualHours { get; set; }
    public string? Tags { get; set; }
}

public class Course : EntityBase
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Platform { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int Status { get; set; }
    public int Progress { get; set; }
    public int TotalLessons { get; set; }
    public int CompletedLessons { get; set; }
    public string? Instructor { get; set; }
    public string? Url { get; set; }
    public string? Tags { get; set; }
}

public class FitnessRecord : EntityBase
{
    public Guid UserId { get; set; }
    public string ExerciseType { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public int CaloriesBurned { get; set; }
    public string? Notes { get; set; }
    public DateTime ExerciseDate { get; set; }
    public string? Tags { get; set; }
}

public class SleepRecord : EntityBase
{
    public Guid UserId { get; set; }
    public DateTime BedTime { get; set; }
    public DateTime WakeTime { get; set; }
    public int DurationMinutes { get; set; }
    public int Quality { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }
}

public class MoodRecord : EntityBase
{
    public Guid UserId { get; set; }
    public int MoodLevel { get; set; }
    public string? MoodType { get; set; }
    public string? Notes { get; set; }
    public DateTime RecordDate { get; set; }
    public string? Tags { get; set; }
}

public class ReadingBook : EntityBase
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Author { get; set; }
    public string? Category { get; set; }
    public int Status { get; set; }
    public int Progress { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? FinishDate { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }
}

public class FocusSession : EntityBase
{
    public Guid UserId { get; set; }
    public string? TaskTitle { get; set; }
    public int DurationMinutes { get; set; }
    public int PlannedMinutes { get; set; }
    public int Status { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? Notes { get; set; }
    public string? Tags { get; set; }
}
