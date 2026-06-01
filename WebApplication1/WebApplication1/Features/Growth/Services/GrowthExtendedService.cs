using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Features.Growth.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Growth.Services;

public interface IGrowthExtendedService
{
    Task<PageResult<SkillDto>> GetSkillsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct);
    Task<SkillDto?> GetSkillByIdAsync(Guid id, CancellationToken ct);
    Task<SkillDto> CreateSkillAsync(CreateSkillInput input, Guid userId, CancellationToken ct);
    Task<SkillDto?> UpdateSkillAsync(Guid id, UpdateSkillInput input, CancellationToken ct);
    Task<bool> DeleteSkillAsync(Guid id, CancellationToken ct);

    Task<PageResult<GoalDto>> GetGoalsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct);
    Task<GoalDto?> GetGoalByIdAsync(Guid id, CancellationToken ct);
    Task<GoalDto> CreateGoalAsync(CreateGoalInput input, Guid userId, CancellationToken ct);
    Task<GoalDto?> UpdateGoalAsync(Guid id, UpdateGoalInput input, CancellationToken ct);
    Task<bool> DeleteGoalAsync(Guid id, CancellationToken ct);

    Task<PageResult<YearPlanDto>> GetYearPlansAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct);
    Task<YearPlanDto?> GetYearPlanByIdAsync(Guid id, CancellationToken ct);
    Task<YearPlanDto> CreateYearPlanAsync(CreateYearPlanInput input, Guid userId, CancellationToken ct);
    Task<YearPlanDto?> UpdateYearPlanAsync(Guid id, UpdateYearPlanInput input, CancellationToken ct);
    Task<bool> DeleteYearPlanAsync(Guid id, CancellationToken ct);

    Task<PageResult<MonthlyReviewDto>> GetMonthlyReviewsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct);
    Task<MonthlyReviewDto?> GetMonthlyReviewByIdAsync(Guid id, CancellationToken ct);
    Task<MonthlyReviewDto> CreateMonthlyReviewAsync(CreateMonthlyReviewInput input, Guid userId, CancellationToken ct);
    Task<MonthlyReviewDto?> UpdateMonthlyReviewAsync(Guid id, UpdateMonthlyReviewInput input, CancellationToken ct);
    Task<bool> DeleteMonthlyReviewAsync(Guid id, CancellationToken ct);

    Task<PageResult<LearningPathDto>> GetLearningPathsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct);
    Task<LearningPathDto?> GetLearningPathByIdAsync(Guid id, CancellationToken ct);
    Task<LearningPathDto> CreateLearningPathAsync(CreateLearningPathInput input, Guid userId, CancellationToken ct);
    Task<LearningPathDto?> UpdateLearningPathAsync(Guid id, UpdateLearningPathInput input, CancellationToken ct);
    Task<bool> DeleteLearningPathAsync(Guid id, CancellationToken ct);

    Task<PageResult<CourseDto>> GetCoursesAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct);
    Task<CourseDto?> GetCourseByIdAsync(Guid id, CancellationToken ct);
    Task<CourseDto> CreateCourseAsync(CreateCourseInput input, Guid userId, CancellationToken ct);
    Task<CourseDto?> UpdateCourseAsync(Guid id, UpdateCourseInput input, CancellationToken ct);
    Task<bool> DeleteCourseAsync(Guid id, CancellationToken ct);

    Task<PageResult<FitnessRecordDto>> GetFitnessRecordsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct);
    Task<FitnessRecordDto?> GetFitnessRecordByIdAsync(Guid id, CancellationToken ct);
    Task<FitnessRecordDto> CreateFitnessRecordAsync(CreateFitnessRecordInput input, Guid userId, CancellationToken ct);
    Task<FitnessRecordDto?> UpdateFitnessRecordAsync(Guid id, UpdateFitnessRecordInput input, CancellationToken ct);
    Task<bool> DeleteFitnessRecordAsync(Guid id, CancellationToken ct);

    Task<PageResult<SleepRecordDto>> GetSleepRecordsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct);
    Task<SleepRecordDto?> GetSleepRecordByIdAsync(Guid id, CancellationToken ct);
    Task<SleepRecordDto> CreateSleepRecordAsync(CreateSleepRecordInput input, Guid userId, CancellationToken ct);
    Task<SleepRecordDto?> UpdateSleepRecordAsync(Guid id, UpdateSleepRecordInput input, CancellationToken ct);
    Task<bool> DeleteSleepRecordAsync(Guid id, CancellationToken ct);

    Task<PageResult<MoodRecordDto>> GetMoodRecordsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct);
    Task<MoodRecordDto?> GetMoodRecordByIdAsync(Guid id, CancellationToken ct);
    Task<MoodRecordDto> CreateMoodRecordAsync(CreateMoodRecordInput input, Guid userId, CancellationToken ct);
    Task<MoodRecordDto?> UpdateMoodRecordAsync(Guid id, UpdateMoodRecordInput input, CancellationToken ct);
    Task<bool> DeleteMoodRecordAsync(Guid id, CancellationToken ct);

    Task<PageResult<ReadingBookDto>> GetReadingBooksAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct);
    Task<ReadingBookDto?> GetReadingBookByIdAsync(Guid id, CancellationToken ct);
    Task<ReadingBookDto> CreateReadingBookAsync(CreateReadingBookInput input, Guid userId, CancellationToken ct);
    Task<ReadingBookDto?> UpdateReadingBookAsync(Guid id, UpdateReadingBookInput input, CancellationToken ct);
    Task<bool> DeleteReadingBookAsync(Guid id, CancellationToken ct);

    Task<PageResult<FocusSessionDto>> GetFocusSessionsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct);
    Task<FocusSessionDto?> GetFocusSessionByIdAsync(Guid id, CancellationToken ct);
    Task<FocusSessionDto> CreateFocusSessionAsync(CreateFocusSessionInput input, Guid userId, CancellationToken ct);
    Task<FocusSessionDto?> UpdateFocusSessionAsync(Guid id, UpdateFocusSessionInput input, CancellationToken ct);
    Task<bool> DeleteFocusSessionAsync(Guid id, CancellationToken ct);
}

public class GrowthExtendedService(AppDbContext db) : IGrowthExtendedService
{
    #region Skills

    public async Task<PageResult<SkillDto>> GetSkillsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.Skills.AsNoTracking();
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var kw = query.Keyword.Trim();
            q = q.Where(x => x.Name.Contains(kw) || (x.Description != null && x.Description.Contains(kw)));
        }
        if (!string.IsNullOrWhiteSpace(query.Category)) q = q.Where(x => x.Category == query.Category);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return PageResult<SkillDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<SkillDto?> GetSkillByIdAsync(Guid id, CancellationToken ct)
    {
        var e = await db.Skills.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : ToDto(e);
    }

    public async Task<SkillDto> CreateSkillAsync(CreateSkillInput input, Guid userId, CancellationToken ct)
    {
        var entity = new Skill
        {
            UserId = userId,
            Name = input.Name,
            Category = input.Category,
            Level = input.Level,
            TargetLevel = input.TargetLevel,
            Description = input.Description,
            Tags = input.Tags
        };
        db.Skills.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<SkillDto?> UpdateSkillAsync(Guid id, UpdateSkillInput input, CancellationToken ct)
    {
        var e = await db.Skills.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return null;
        if (input.Name is not null) e.Name = input.Name;
        if (input.Category is not null) e.Category = input.Category;
        if (input.Level.HasValue) e.Level = input.Level.Value;
        if (input.TargetLevel.HasValue) e.TargetLevel = input.TargetLevel.Value;
        if (input.Description is not null) e.Description = input.Description;
        if (input.Tags is not null) e.Tags = input.Tags;
        if (input.IsActive.HasValue) e.IsActive = input.IsActive.Value;
        await db.SaveChangesAsync(ct);
        return ToDto(e);
    }

    public async Task<bool> DeleteSkillAsync(Guid id, CancellationToken ct)
    {
        var e = await db.Skills.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;
        db.Skills.Remove(e);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static SkillDto ToDto(Skill e) => new()
    {
        Id = e.Id.ToString(),
        UserId = e.UserId.ToString(),
        Name = e.Name,
        Category = e.Category,
        Level = e.Level,
        TargetLevel = e.TargetLevel,
        ExperiencePoints = e.ExperiencePoints,
        Description = e.Description,
        Tags = e.Tags,
        IsActive = e.IsActive,
        CreatedAt = e.CreatedAt.ToString("o")
    };

    #endregion

    #region Goals

    public async Task<PageResult<GoalDto>> GetGoalsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.Goals.AsNoTracking();
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var kw = query.Keyword.Trim();
            q = q.Where(x => x.Title.Contains(kw) || (x.Description != null && x.Description.Contains(kw)));
        }
        if (!string.IsNullOrWhiteSpace(query.Category)) q = q.Where(x => x.Category == query.Category);
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return PageResult<GoalDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<GoalDto?> GetGoalByIdAsync(Guid id, CancellationToken ct)
    {
        var e = await db.Goals.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : ToDto(e);
    }

    public async Task<GoalDto> CreateGoalAsync(CreateGoalInput input, Guid userId, CancellationToken ct)
    {
        var entity = new Goal
        {
            UserId = userId,
            Title = input.Title,
            Description = input.Description,
            Category = input.Category,
            Priority = input.Priority,
            StartDate = ParseDate(input.StartDate),
            DueDate = ParseDate(input.DueDate),
            Tags = input.Tags
        };
        db.Goals.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<GoalDto?> UpdateGoalAsync(Guid id, UpdateGoalInput input, CancellationToken ct)
    {
        var e = await db.Goals.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return null;
        if (input.Title is not null) e.Title = input.Title;
        if (input.Description is not null) e.Description = input.Description;
        if (input.Category is not null) e.Category = input.Category;
        if (input.Priority.HasValue) e.Priority = input.Priority.Value;
        if (input.Status.HasValue)
        {
            e.Status = input.Status.Value;
            if (input.Status.Value == 2) e.CompletedAt = DateTime.UtcNow;
        }
        if (input.StartDate is not null) e.StartDate = ParseDate(input.StartDate);
        if (input.DueDate is not null) e.DueDate = ParseDate(input.DueDate);
        if (input.Progress.HasValue) e.Progress = input.Progress.Value;
        if (input.Tags is not null) e.Tags = input.Tags;
        await db.SaveChangesAsync(ct);
        return ToDto(e);
    }

    public async Task<bool> DeleteGoalAsync(Guid id, CancellationToken ct)
    {
        var e = await db.Goals.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;
        db.Goals.Remove(e);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static GoalDto ToDto(Goal e) => new()
    {
        Id = e.Id.ToString(),
        UserId = e.UserId.ToString(),
        Title = e.Title,
        Description = e.Description,
        Category = e.Category,
        Priority = e.Priority,
        Status = e.Status,
        StartDate = e.StartDate?.ToString("o"),
        DueDate = e.DueDate?.ToString("o"),
        CompletedAt = e.CompletedAt?.ToString("o"),
        Progress = e.Progress,
        Tags = e.Tags,
        CreatedAt = e.CreatedAt.ToString("o")
    };

    #endregion

    #region YearPlans

    public async Task<PageResult<YearPlanDto>> GetYearPlansAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.YearPlans.AsNoTracking();
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var kw = query.Keyword.Trim();
            q = q.Where(x => x.Title.Contains(kw) || (x.Description != null && x.Description.Contains(kw)));
        }
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return PageResult<YearPlanDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<YearPlanDto?> GetYearPlanByIdAsync(Guid id, CancellationToken ct)
    {
        var e = await db.YearPlans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : ToDto(e);
    }

    public async Task<YearPlanDto> CreateYearPlanAsync(CreateYearPlanInput input, Guid userId, CancellationToken ct)
    {
        var entity = new YearPlan
        {
            UserId = userId,
            Year = input.Year,
            Title = input.Title,
            Description = input.Description,
            Category = input.Category,
            Tags = input.Tags
        };
        db.YearPlans.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<YearPlanDto?> UpdateYearPlanAsync(Guid id, UpdateYearPlanInput input, CancellationToken ct)
    {
        var e = await db.YearPlans.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return null;
        if (input.Title is not null) e.Title = input.Title;
        if (input.Description is not null) e.Description = input.Description;
        if (input.Category is not null) e.Category = input.Category;
        if (input.Status.HasValue) e.Status = input.Status.Value;
        if (input.Progress.HasValue) e.Progress = input.Progress.Value;
        if (input.Tags is not null) e.Tags = input.Tags;
        await db.SaveChangesAsync(ct);
        return ToDto(e);
    }

    public async Task<bool> DeleteYearPlanAsync(Guid id, CancellationToken ct)
    {
        var e = await db.YearPlans.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;
        db.YearPlans.Remove(e);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static YearPlanDto ToDto(YearPlan e) => new()
    {
        Id = e.Id.ToString(),
        UserId = e.UserId.ToString(),
        Year = e.Year,
        Title = e.Title,
        Description = e.Description,
        Category = e.Category,
        Status = e.Status,
        Progress = e.Progress,
        Tags = e.Tags,
        CreatedAt = e.CreatedAt.ToString("o")
    };

    #endregion

    #region MonthlyReviews

    public async Task<PageResult<MonthlyReviewDto>> GetMonthlyReviewsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.MonthlyReviews.AsNoTracking();
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var kw = query.Keyword.Trim();
            q = q.Where(x => x.Title.Contains(kw) || (x.Achievements != null && x.Achievements.Contains(kw)));
        }

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return PageResult<MonthlyReviewDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<MonthlyReviewDto?> GetMonthlyReviewByIdAsync(Guid id, CancellationToken ct)
    {
        var e = await db.MonthlyReviews.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : ToDto(e);
    }

    public async Task<MonthlyReviewDto> CreateMonthlyReviewAsync(CreateMonthlyReviewInput input, Guid userId, CancellationToken ct)
    {
        var entity = new MonthlyReview
        {
            UserId = userId,
            Year = input.Year,
            Month = input.Month,
            Title = input.Title,
            Achievements = input.Achievements,
            Challenges = input.Challenges,
            LessonsLearned = input.LessonsLearned,
            NextMonthGoals = input.NextMonthGoals,
            Rating = input.Rating,
            Tags = input.Tags
        };
        db.MonthlyReviews.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<MonthlyReviewDto?> UpdateMonthlyReviewAsync(Guid id, UpdateMonthlyReviewInput input, CancellationToken ct)
    {
        var e = await db.MonthlyReviews.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return null;
        if (input.Title is not null) e.Title = input.Title;
        if (input.Achievements is not null) e.Achievements = input.Achievements;
        if (input.Challenges is not null) e.Challenges = input.Challenges;
        if (input.LessonsLearned is not null) e.LessonsLearned = input.LessonsLearned;
        if (input.NextMonthGoals is not null) e.NextMonthGoals = input.NextMonthGoals;
        if (input.Rating.HasValue) e.Rating = input.Rating.Value;
        if (input.Tags is not null) e.Tags = input.Tags;
        await db.SaveChangesAsync(ct);
        return ToDto(e);
    }

    public async Task<bool> DeleteMonthlyReviewAsync(Guid id, CancellationToken ct)
    {
        var e = await db.MonthlyReviews.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;
        db.MonthlyReviews.Remove(e);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static MonthlyReviewDto ToDto(MonthlyReview e) => new()
    {
        Id = e.Id.ToString(),
        UserId = e.UserId.ToString(),
        Year = e.Year,
        Month = e.Month,
        Title = e.Title,
        Achievements = e.Achievements,
        Challenges = e.Challenges,
        LessonsLearned = e.LessonsLearned,
        NextMonthGoals = e.NextMonthGoals,
        Rating = e.Rating,
        Tags = e.Tags,
        CreatedAt = e.CreatedAt.ToString("o")
    };

    #endregion

    #region LearningPaths

    public async Task<PageResult<LearningPathDto>> GetLearningPathsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.LearningPaths.AsNoTracking();
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var kw = query.Keyword.Trim();
            q = q.Where(x => x.Title.Contains(kw) || (x.Description != null && x.Description.Contains(kw)));
        }
        if (!string.IsNullOrWhiteSpace(query.Category)) q = q.Where(x => x.Category == query.Category);
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return PageResult<LearningPathDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<LearningPathDto?> GetLearningPathByIdAsync(Guid id, CancellationToken ct)
    {
        var e = await db.LearningPaths.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : ToDto(e);
    }

    public async Task<LearningPathDto> CreateLearningPathAsync(CreateLearningPathInput input, Guid userId, CancellationToken ct)
    {
        var entity = new LearningPath
        {
            UserId = userId,
            Title = input.Title,
            Description = input.Description,
            Category = input.Category,
            EstimatedHours = input.EstimatedHours,
            Tags = input.Tags
        };
        db.LearningPaths.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<LearningPathDto?> UpdateLearningPathAsync(Guid id, UpdateLearningPathInput input, CancellationToken ct)
    {
        var e = await db.LearningPaths.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return null;
        if (input.Title is not null) e.Title = input.Title;
        if (input.Description is not null) e.Description = input.Description;
        if (input.Category is not null) e.Category = input.Category;
        if (input.Status.HasValue) e.Status = input.Status.Value;
        if (input.Progress.HasValue) e.Progress = input.Progress.Value;
        if (input.EstimatedHours.HasValue) e.EstimatedHours = input.EstimatedHours.Value;
        if (input.ActualHours.HasValue) e.ActualHours = input.ActualHours.Value;
        if (input.Tags is not null) e.Tags = input.Tags;
        await db.SaveChangesAsync(ct);
        return ToDto(e);
    }

    public async Task<bool> DeleteLearningPathAsync(Guid id, CancellationToken ct)
    {
        var e = await db.LearningPaths.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;
        db.LearningPaths.Remove(e);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static LearningPathDto ToDto(LearningPath e) => new()
    {
        Id = e.Id.ToString(),
        UserId = e.UserId.ToString(),
        Title = e.Title,
        Description = e.Description,
        Category = e.Category,
        Status = e.Status,
        Progress = e.Progress,
        EstimatedHours = e.EstimatedHours,
        ActualHours = e.ActualHours,
        Tags = e.Tags,
        CreatedAt = e.CreatedAt.ToString("o")
    };

    #endregion

    #region Courses

    public async Task<PageResult<CourseDto>> GetCoursesAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.Courses.AsNoTracking();
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var kw = query.Keyword.Trim();
            q = q.Where(x => x.Title.Contains(kw) || (x.Description != null && x.Description.Contains(kw)));
        }
        if (!string.IsNullOrWhiteSpace(query.Category)) q = q.Where(x => x.Category == query.Category);
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return PageResult<CourseDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<CourseDto?> GetCourseByIdAsync(Guid id, CancellationToken ct)
    {
        var e = await db.Courses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : ToDto(e);
    }

    public async Task<CourseDto> CreateCourseAsync(CreateCourseInput input, Guid userId, CancellationToken ct)
    {
        var entity = new Course
        {
            UserId = userId,
            Title = input.Title,
            Description = input.Description,
            Platform = input.Platform,
            Category = input.Category,
            TotalLessons = input.TotalLessons,
            Instructor = input.Instructor,
            Url = input.Url,
            Tags = input.Tags
        };
        db.Courses.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<CourseDto?> UpdateCourseAsync(Guid id, UpdateCourseInput input, CancellationToken ct)
    {
        var e = await db.Courses.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return null;
        if (input.Title is not null) e.Title = input.Title;
        if (input.Description is not null) e.Description = input.Description;
        if (input.Platform is not null) e.Platform = input.Platform;
        if (input.Category is not null) e.Category = input.Category;
        if (input.Status.HasValue) e.Status = input.Status.Value;
        if (input.Progress.HasValue) e.Progress = input.Progress.Value;
        if (input.TotalLessons.HasValue) e.TotalLessons = input.TotalLessons.Value;
        if (input.CompletedLessons.HasValue) e.CompletedLessons = input.CompletedLessons.Value;
        if (input.Instructor is not null) e.Instructor = input.Instructor;
        if (input.Url is not null) e.Url = input.Url;
        if (input.Tags is not null) e.Tags = input.Tags;
        await db.SaveChangesAsync(ct);
        return ToDto(e);
    }

    public async Task<bool> DeleteCourseAsync(Guid id, CancellationToken ct)
    {
        var e = await db.Courses.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;
        db.Courses.Remove(e);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static CourseDto ToDto(Course e) => new()
    {
        Id = e.Id.ToString(),
        UserId = e.UserId.ToString(),
        Title = e.Title,
        Description = e.Description,
        Platform = e.Platform,
        Category = e.Category,
        Status = e.Status,
        Progress = e.Progress,
        TotalLessons = e.TotalLessons,
        CompletedLessons = e.CompletedLessons,
        Instructor = e.Instructor,
        Url = e.Url,
        Tags = e.Tags,
        CreatedAt = e.CreatedAt.ToString("o")
    };

    #endregion

    #region FitnessRecords

    public async Task<PageResult<FitnessRecordDto>> GetFitnessRecordsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.FitnessRecords.AsNoTracking();
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var kw = query.Keyword.Trim();
            q = q.Where(x => x.ExerciseType.Contains(kw) || (x.Notes != null && x.Notes.Contains(kw)));
        }
        if (!string.IsNullOrWhiteSpace(query.StartDate) && DateTime.TryParse(query.StartDate, out var sd))
            q = q.Where(x => x.ExerciseDate >= sd);
        if (!string.IsNullOrWhiteSpace(query.EndDate) && DateTime.TryParse(query.EndDate, out var ed))
            q = q.Where(x => x.ExerciseDate <= ed);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.ExerciseDate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return PageResult<FitnessRecordDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<FitnessRecordDto?> GetFitnessRecordByIdAsync(Guid id, CancellationToken ct)
    {
        var e = await db.FitnessRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : ToDto(e);
    }

    public async Task<FitnessRecordDto> CreateFitnessRecordAsync(CreateFitnessRecordInput input, Guid userId, CancellationToken ct)
    {
        var entity = new FitnessRecord
        {
            UserId = userId,
            ExerciseType = input.ExerciseType,
            DurationMinutes = input.DurationMinutes,
            CaloriesBurned = input.CaloriesBurned,
            Notes = input.Notes,
            ExerciseDate = DateTime.TryParse(input.ExerciseDate, out var d) ? d : DateTime.UtcNow,
            Tags = input.Tags
        };
        db.FitnessRecords.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<FitnessRecordDto?> UpdateFitnessRecordAsync(Guid id, UpdateFitnessRecordInput input, CancellationToken ct)
    {
        var e = await db.FitnessRecords.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return null;
        if (input.ExerciseType is not null) e.ExerciseType = input.ExerciseType;
        if (input.DurationMinutes.HasValue) e.DurationMinutes = input.DurationMinutes.Value;
        if (input.CaloriesBurned.HasValue) e.CaloriesBurned = input.CaloriesBurned.Value;
        if (input.Notes is not null) e.Notes = input.Notes;
        if (input.ExerciseDate is not null && DateTime.TryParse(input.ExerciseDate, out var d)) e.ExerciseDate = d;
        if (input.Tags is not null) e.Tags = input.Tags;
        await db.SaveChangesAsync(ct);
        return ToDto(e);
    }

    public async Task<bool> DeleteFitnessRecordAsync(Guid id, CancellationToken ct)
    {
        var e = await db.FitnessRecords.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;
        db.FitnessRecords.Remove(e);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static FitnessRecordDto ToDto(FitnessRecord e) => new()
    {
        Id = e.Id.ToString(),
        UserId = e.UserId.ToString(),
        ExerciseType = e.ExerciseType,
        DurationMinutes = e.DurationMinutes,
        CaloriesBurned = e.CaloriesBurned,
        Notes = e.Notes,
        ExerciseDate = e.ExerciseDate.ToString("o"),
        Tags = e.Tags,
        CreatedAt = e.CreatedAt.ToString("o")
    };

    #endregion

    #region SleepRecords

    public async Task<PageResult<SleepRecordDto>> GetSleepRecordsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.SleepRecords.AsNoTracking();
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(query.StartDate) && DateTime.TryParse(query.StartDate, out var sd))
            q = q.Where(x => x.BedTime >= sd);
        if (!string.IsNullOrWhiteSpace(query.EndDate) && DateTime.TryParse(query.EndDate, out var ed))
            q = q.Where(x => x.BedTime <= ed);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.BedTime).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return PageResult<SleepRecordDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<SleepRecordDto?> GetSleepRecordByIdAsync(Guid id, CancellationToken ct)
    {
        var e = await db.SleepRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : ToDto(e);
    }

    public async Task<SleepRecordDto> CreateSleepRecordAsync(CreateSleepRecordInput input, Guid userId, CancellationToken ct)
    {
        var bedTime = DateTime.TryParse(input.BedTime, out var bt) ? bt : DateTime.UtcNow;
        var wakeTime = DateTime.TryParse(input.WakeTime, out var wt) ? wt : DateTime.UtcNow;
        var entity = new SleepRecord
        {
            UserId = userId,
            BedTime = bedTime,
            WakeTime = wakeTime,
            DurationMinutes = (int)(wakeTime - bedTime).TotalMinutes,
            Quality = input.Quality,
            Notes = input.Notes,
            Tags = input.Tags
        };
        db.SleepRecords.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<SleepRecordDto?> UpdateSleepRecordAsync(Guid id, UpdateSleepRecordInput input, CancellationToken ct)
    {
        var e = await db.SleepRecords.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return null;
        if (input.BedTime is not null && DateTime.TryParse(input.BedTime, out var bt)) e.BedTime = bt;
        if (input.WakeTime is not null && DateTime.TryParse(input.WakeTime, out var wt)) e.WakeTime = wt;
        e.DurationMinutes = (int)(e.WakeTime - e.BedTime).TotalMinutes;
        if (input.Quality.HasValue) e.Quality = input.Quality.Value;
        if (input.Notes is not null) e.Notes = input.Notes;
        if (input.Tags is not null) e.Tags = input.Tags;
        await db.SaveChangesAsync(ct);
        return ToDto(e);
    }

    public async Task<bool> DeleteSleepRecordAsync(Guid id, CancellationToken ct)
    {
        var e = await db.SleepRecords.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;
        db.SleepRecords.Remove(e);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static SleepRecordDto ToDto(SleepRecord e) => new()
    {
        Id = e.Id.ToString(),
        UserId = e.UserId.ToString(),
        BedTime = e.BedTime.ToString("o"),
        WakeTime = e.WakeTime.ToString("o"),
        DurationMinutes = e.DurationMinutes,
        Quality = e.Quality,
        Notes = e.Notes,
        Tags = e.Tags,
        CreatedAt = e.CreatedAt.ToString("o")
    };

    #endregion

    #region MoodRecords

    public async Task<PageResult<MoodRecordDto>> GetMoodRecordsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.MoodRecords.AsNoTracking();
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(query.StartDate) && DateTime.TryParse(query.StartDate, out var sd))
            q = q.Where(x => x.RecordDate >= sd);
        if (!string.IsNullOrWhiteSpace(query.EndDate) && DateTime.TryParse(query.EndDate, out var ed))
            q = q.Where(x => x.RecordDate <= ed);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.RecordDate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return PageResult<MoodRecordDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<MoodRecordDto?> GetMoodRecordByIdAsync(Guid id, CancellationToken ct)
    {
        var e = await db.MoodRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : ToDto(e);
    }

    public async Task<MoodRecordDto> CreateMoodRecordAsync(CreateMoodRecordInput input, Guid userId, CancellationToken ct)
    {
        var entity = new MoodRecord
        {
            UserId = userId,
            MoodLevel = input.MoodLevel,
            MoodType = input.MoodType,
            Notes = input.Notes,
            RecordDate = DateTime.TryParse(input.RecordDate, out var d) ? d : DateTime.UtcNow,
            Tags = input.Tags
        };
        db.MoodRecords.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<MoodRecordDto?> UpdateMoodRecordAsync(Guid id, UpdateMoodRecordInput input, CancellationToken ct)
    {
        var e = await db.MoodRecords.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return null;
        if (input.MoodLevel.HasValue) e.MoodLevel = input.MoodLevel.Value;
        if (input.MoodType is not null) e.MoodType = input.MoodType;
        if (input.Notes is not null) e.Notes = input.Notes;
        if (input.RecordDate is not null && DateTime.TryParse(input.RecordDate, out var d)) e.RecordDate = d;
        if (input.Tags is not null) e.Tags = input.Tags;
        await db.SaveChangesAsync(ct);
        return ToDto(e);
    }

    public async Task<bool> DeleteMoodRecordAsync(Guid id, CancellationToken ct)
    {
        var e = await db.MoodRecords.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;
        db.MoodRecords.Remove(e);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static MoodRecordDto ToDto(MoodRecord e) => new()
    {
        Id = e.Id.ToString(),
        UserId = e.UserId.ToString(),
        MoodLevel = e.MoodLevel,
        MoodType = e.MoodType,
        Notes = e.Notes,
        RecordDate = e.RecordDate.ToString("o"),
        Tags = e.Tags,
        CreatedAt = e.CreatedAt.ToString("o")
    };

    #endregion

    #region ReadingBooks

    public async Task<PageResult<ReadingBookDto>> GetReadingBooksAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.ReadingBooks.AsNoTracking();
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var kw = query.Keyword.Trim();
            q = q.Where(x => x.Title.Contains(kw) || (x.Author != null && x.Author.Contains(kw)));
        }
        if (!string.IsNullOrWhiteSpace(query.Category)) q = q.Where(x => x.Category == query.Category);
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return PageResult<ReadingBookDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<ReadingBookDto?> GetReadingBookByIdAsync(Guid id, CancellationToken ct)
    {
        var e = await db.ReadingBooks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : ToDto(e);
    }

    public async Task<ReadingBookDto> CreateReadingBookAsync(CreateReadingBookInput input, Guid userId, CancellationToken ct)
    {
        var entity = new ReadingBook
        {
            UserId = userId,
            Title = input.Title,
            Author = input.Author,
            Category = input.Category,
            TotalPages = input.TotalPages,
            Notes = input.Notes,
            Tags = input.Tags
        };
        db.ReadingBooks.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<ReadingBookDto?> UpdateReadingBookAsync(Guid id, UpdateReadingBookInput input, CancellationToken ct)
    {
        var e = await db.ReadingBooks.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return null;
        if (input.Title is not null) e.Title = input.Title;
        if (input.Author is not null) e.Author = input.Author;
        if (input.Category is not null) e.Category = input.Category;
        if (input.Status.HasValue) e.Status = input.Status.Value;
        if (input.Progress.HasValue) e.Progress = input.Progress.Value;
        if (input.TotalPages.HasValue) e.TotalPages = input.TotalPages.Value;
        if (input.CurrentPage.HasValue) e.CurrentPage = input.CurrentPage.Value;
        if (input.StartDate is not null && DateTime.TryParse(input.StartDate, out var sd)) e.StartDate = sd;
        if (input.FinishDate is not null && DateTime.TryParse(input.FinishDate, out var fd)) e.FinishDate = fd;
        if (input.Notes is not null) e.Notes = input.Notes;
        if (input.Tags is not null) e.Tags = input.Tags;
        await db.SaveChangesAsync(ct);
        return ToDto(e);
    }

    public async Task<bool> DeleteReadingBookAsync(Guid id, CancellationToken ct)
    {
        var e = await db.ReadingBooks.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;
        db.ReadingBooks.Remove(e);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static ReadingBookDto ToDto(ReadingBook e) => new()
    {
        Id = e.Id.ToString(),
        UserId = e.UserId.ToString(),
        Title = e.Title,
        Author = e.Author,
        Category = e.Category,
        Status = e.Status,
        Progress = e.Progress,
        TotalPages = e.TotalPages,
        CurrentPage = e.CurrentPage,
        StartDate = e.StartDate?.ToString("o"),
        FinishDate = e.FinishDate?.ToString("o"),
        Notes = e.Notes,
        Tags = e.Tags,
        CreatedAt = e.CreatedAt.ToString("o")
    };

    #endregion

    #region FocusSessions

    public async Task<PageResult<FocusSessionDto>> GetFocusSessionsAsync(GrowthQueryDto query, Guid? userId, CancellationToken ct)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = db.FocusSessions.AsNoTracking();
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var kw = query.Keyword.Trim();
            q = q.Where(x => (x.TaskTitle != null && x.TaskTitle.Contains(kw)) || (x.Notes != null && x.Notes.Contains(kw)));
        }
        if (!string.IsNullOrWhiteSpace(query.StartDate) && DateTime.TryParse(query.StartDate, out var sd))
            q = q.Where(x => x.StartTime >= sd);
        if (!string.IsNullOrWhiteSpace(query.EndDate) && DateTime.TryParse(query.EndDate, out var ed))
            q = q.Where(x => x.StartTime <= ed);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.StartTime).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return PageResult<FocusSessionDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<FocusSessionDto?> GetFocusSessionByIdAsync(Guid id, CancellationToken ct)
    {
        var e = await db.FocusSessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return e is null ? null : ToDto(e);
    }

    public async Task<FocusSessionDto> CreateFocusSessionAsync(CreateFocusSessionInput input, Guid userId, CancellationToken ct)
    {
        var entity = new FocusSession
        {
            UserId = userId,
            TaskTitle = input.TaskTitle,
            PlannedMinutes = input.PlannedMinutes,
            StartTime = DateTime.UtcNow,
            Notes = input.Notes,
            Tags = input.Tags
        };
        db.FocusSessions.Add(entity);
        await db.SaveChangesAsync(ct);
        return ToDto(entity);
    }

    public async Task<FocusSessionDto?> UpdateFocusSessionAsync(Guid id, UpdateFocusSessionInput input, CancellationToken ct)
    {
        var e = await db.FocusSessions.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return null;
        if (input.TaskTitle is not null) e.TaskTitle = input.TaskTitle;
        if (input.PlannedMinutes.HasValue) e.PlannedMinutes = input.PlannedMinutes.Value;
        if (input.DurationMinutes.HasValue) e.DurationMinutes = input.DurationMinutes.Value;
        if (input.Status.HasValue) e.Status = input.Status.Value;
        if (input.EndTime is not null && DateTime.TryParse(input.EndTime, out var et)) e.EndTime = et;
        if (input.Notes is not null) e.Notes = input.Notes;
        if (input.Tags is not null) e.Tags = input.Tags;
        await db.SaveChangesAsync(ct);
        return ToDto(e);
    }

    public async Task<bool> DeleteFocusSessionAsync(Guid id, CancellationToken ct)
    {
        var e = await db.FocusSessions.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (e is null) return false;
        db.FocusSessions.Remove(e);
        await db.SaveChangesAsync(ct);
        return true;
    }

    private static FocusSessionDto ToDto(FocusSession e) => new()
    {
        Id = e.Id.ToString(),
        UserId = e.UserId.ToString(),
        TaskTitle = e.TaskTitle,
        DurationMinutes = e.DurationMinutes,
        PlannedMinutes = e.PlannedMinutes,
        Status = e.Status,
        StartTime = e.StartTime.ToString("o"),
        EndTime = e.EndTime?.ToString("o"),
        Notes = e.Notes,
        Tags = e.Tags,
        CreatedAt = e.CreatedAt.ToString("o")
    };

    #endregion

    private static DateTime? ParseDate(string? s) => DateTime.TryParse(s, out var d) ? d : null;
}
