using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Dtos;

// Skill DTOs
public record SkillDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public int Level { get; init; }
    public int TargetLevel { get; init; }
    public int ExperiencePoints { get; init; }
    public string? Description { get; init; }
    public string? Tags { get; init; }
    public bool IsActive { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateSkillInput
{
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public int Level { get; init; } = 1;
    public int TargetLevel { get; init; } = 5;
    public string? Description { get; init; }
    public string? Tags { get; init; }
}

public record UpdateSkillInput
{
    public string? Name { get; init; }
    public string? Category { get; init; }
    public int? Level { get; init; }
    public int? TargetLevel { get; init; }
    public string? Description { get; init; }
    public string? Tags { get; init; }
    public bool? IsActive { get; init; }
}

// Goal DTOs
public record GoalDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Category { get; init; } = string.Empty;
    public int Priority { get; init; }
    public int Status { get; init; }
    public string? StartDate { get; init; }
    public string? DueDate { get; init; }
    public string? CompletedAt { get; init; }
    public int Progress { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateGoalInput
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Category { get; init; } = string.Empty;
    public int Priority { get; init; } = 2;
    public string? StartDate { get; init; }
    public string? DueDate { get; init; }
    public string? Tags { get; init; }
}

public record UpdateGoalInput
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? Category { get; init; }
    public int? Priority { get; init; }
    public int? Status { get; init; }
    public string? StartDate { get; init; }
    public string? DueDate { get; init; }
    public int? Progress { get; init; }
    public string? Tags { get; init; }
}

// YearPlan DTOs
public record YearPlanDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public int Year { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Category { get; init; } = string.Empty;
    public int Status { get; init; }
    public int Progress { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateYearPlanInput
{
    public int Year { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Category { get; init; } = string.Empty;
    public string? Tags { get; init; }
}

public record UpdateYearPlanInput
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? Category { get; init; }
    public int? Status { get; init; }
    public int? Progress { get; init; }
    public string? Tags { get; init; }
}

// MonthlyReview DTOs
public record MonthlyReviewDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public int Year { get; init; }
    public int Month { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Achievements { get; init; }
    public string? Challenges { get; init; }
    public string? LessonsLearned { get; init; }
    public string? NextMonthGoals { get; init; }
    public int Rating { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateMonthlyReviewInput
{
    public int Year { get; init; }
    public int Month { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Achievements { get; init; }
    public string? Challenges { get; init; }
    public string? LessonsLearned { get; init; }
    public string? NextMonthGoals { get; init; }
    public int Rating { get; init; }
    public string? Tags { get; init; }
}

public record UpdateMonthlyReviewInput
{
    public string? Title { get; init; }
    public string? Achievements { get; init; }
    public string? Challenges { get; init; }
    public string? LessonsLearned { get; init; }
    public string? NextMonthGoals { get; init; }
    public int? Rating { get; init; }
    public string? Tags { get; init; }
}

// LearningPath DTOs
public record LearningPathDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Category { get; init; } = string.Empty;
    public int Status { get; init; }
    public int Progress { get; init; }
    public int EstimatedHours { get; init; }
    public int ActualHours { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateLearningPathInput
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Category { get; init; } = string.Empty;
    public int EstimatedHours { get; init; }
    public string? Tags { get; init; }
}

public record UpdateLearningPathInput
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? Category { get; init; }
    public int? Status { get; init; }
    public int? Progress { get; init; }
    public int? EstimatedHours { get; init; }
    public int? ActualHours { get; init; }
    public string? Tags { get; init; }
}

// Course DTOs
public record CourseDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Platform { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public int Status { get; init; }
    public int Progress { get; init; }
    public int TotalLessons { get; init; }
    public int CompletedLessons { get; init; }
    public string? Instructor { get; init; }
    public string? Url { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateCourseInput
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Platform { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public int TotalLessons { get; init; }
    public string? Instructor { get; init; }
    public string? Url { get; init; }
    public string? Tags { get; init; }
}

public record UpdateCourseInput
{
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? Platform { get; init; }
    public string? Category { get; init; }
    public int? Status { get; init; }
    public int? Progress { get; init; }
    public int? TotalLessons { get; init; }
    public int? CompletedLessons { get; init; }
    public string? Instructor { get; init; }
    public string? Url { get; init; }
    public string? Tags { get; init; }
}

// Fitness DTOs
public record FitnessRecordDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string ExerciseType { get; init; } = string.Empty;
    public int DurationMinutes { get; init; }
    public int CaloriesBurned { get; init; }
    public string? Notes { get; init; }
    public string ExerciseDate { get; init; } = string.Empty;
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateFitnessRecordInput
{
    public string ExerciseType { get; init; } = string.Empty;
    public int DurationMinutes { get; init; }
    public int CaloriesBurned { get; init; }
    public string? Notes { get; init; }
    public string ExerciseDate { get; init; } = string.Empty;
    public string? Tags { get; init; }
}

public record UpdateFitnessRecordInput
{
    public string? ExerciseType { get; init; }
    public int? DurationMinutes { get; init; }
    public int? CaloriesBurned { get; init; }
    public string? Notes { get; init; }
    public string? ExerciseDate { get; init; }
    public string? Tags { get; init; }
}

// Sleep DTOs
public record SleepRecordDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string BedTime { get; init; } = string.Empty;
    public string WakeTime { get; init; } = string.Empty;
    public int DurationMinutes { get; init; }
    public int Quality { get; init; }
    public string? Notes { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateSleepRecordInput
{
    public string BedTime { get; init; } = string.Empty;
    public string WakeTime { get; init; } = string.Empty;
    public int Quality { get; init; }
    public string? Notes { get; init; }
    public string? Tags { get; init; }
}

public record UpdateSleepRecordInput
{
    public string? BedTime { get; init; }
    public string? WakeTime { get; init; }
    public int? Quality { get; init; }
    public string? Notes { get; init; }
    public string? Tags { get; init; }
}

// Mood DTOs
public record MoodRecordDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public int MoodLevel { get; init; }
    public string? MoodType { get; init; }
    public string? Notes { get; init; }
    public string RecordDate { get; init; } = string.Empty;
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateMoodRecordInput
{
    public int MoodLevel { get; init; }
    public string? MoodType { get; init; }
    public string? Notes { get; init; }
    public string RecordDate { get; init; } = string.Empty;
    public string? Tags { get; init; }
}

public record UpdateMoodRecordInput
{
    public int? MoodLevel { get; init; }
    public string? MoodType { get; init; }
    public string? Notes { get; init; }
    public string? RecordDate { get; init; }
    public string? Tags { get; init; }
}

// Reading DTOs
public record ReadingBookDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Author { get; init; }
    public string? Category { get; init; }
    public int Status { get; init; }
    public int Progress { get; init; }
    public int TotalPages { get; init; }
    public int CurrentPage { get; init; }
    public string? StartDate { get; init; }
    public string? FinishDate { get; init; }
    public string? Notes { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateReadingBookInput
{
    public string Title { get; init; } = string.Empty;
    public string? Author { get; init; }
    public string? Category { get; init; }
    public int TotalPages { get; init; }
    public string? Notes { get; init; }
    public string? Tags { get; init; }
}

public record UpdateReadingBookInput
{
    public string? Title { get; init; }
    public string? Author { get; init; }
    public string? Category { get; init; }
    public int? Status { get; init; }
    public int? Progress { get; init; }
    public int? TotalPages { get; init; }
    public int? CurrentPage { get; init; }
    public string? StartDate { get; init; }
    public string? FinishDate { get; init; }
    public string? Notes { get; init; }
    public string? Tags { get; init; }
}

// Focus DTOs
public record FocusSessionDto
{
    public string Id { get; init; } = string.Empty;
    public string? UserId { get; init; }
    public string? TaskTitle { get; init; }
    public int DurationMinutes { get; init; }
    public int PlannedMinutes { get; init; }
    public int Status { get; init; }
    public string StartTime { get; init; } = string.Empty;
    public string? EndTime { get; init; }
    public string? Notes { get; init; }
    public string? Tags { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateFocusSessionInput
{
    public string? TaskTitle { get; init; }
    public int PlannedMinutes { get; init; } = 25;
    public string? Notes { get; init; }
    public string? Tags { get; init; }
}

public record UpdateFocusSessionInput
{
    public string? TaskTitle { get; init; }
    public int? PlannedMinutes { get; init; }
    public int? DurationMinutes { get; init; }
    public int? Status { get; init; }
    public string? EndTime { get; init; }
    public string? Notes { get; init; }
    public string? Tags { get; init; }
}

// Query DTOs
public class GrowthQueryDto : PageQueryDto
{
    public string? Category { get; init; }
    public int? Status { get; init; }
    public string? Keyword { get; init; }
    public string? StartDate { get; init; }
    public string? EndDate { get; init; }
}
