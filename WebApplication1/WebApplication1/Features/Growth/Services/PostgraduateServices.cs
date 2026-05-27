using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Features.Growth.Entities;
using WebApplication1.Features.Growth.Services.Interfaces;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Growth.Services;

public class PostgraduateTaskService(AppDbContext dbContext) : IPostgraduateTaskService
{
    public async Task<PageResult<PostgraduateTaskDto>> GetPageAsync(PostgraduateTaskQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var tasks = dbContext.PostgraduateTasks.AsNoTracking();

        if (userId.HasValue)
        {
            tasks = tasks.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            tasks = tasks.Where(x => x.Title.Contains(keyword) || (x.Description != null && x.Description.Contains(keyword)));
        }

        if (query.Status.HasValue)
        {
            tasks = tasks.Where(x => x.Status == (PostgraduateTaskStatus)query.Status.Value);
        }

        if (query.Type.HasValue)
        {
            tasks = tasks.Where(x => x.Type == (PostgraduateTaskType)query.Type.Value);
        }

        var total = await tasks.CountAsync(cancellationToken);
        var items = await tasks
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<PostgraduateTaskDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<PostgraduateTaskDto?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var tasks = dbContext.PostgraduateTasks.AsNoTracking().Where(x => x.Id == id);
        if (userId.HasValue)
        {
            tasks = tasks.Where(x => x.UserId == userId.Value);
        }

        var task = await tasks.FirstOrDefaultAsync(cancellationToken);
        return task is null ? null : ToDto(task);
    }

    public async Task<PostgraduateTaskDto> CreateAsync(CreatePostgraduateTaskDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var task = new PostgraduateTask
        {
            UserId = userId,
            Title = input.Title,
            Description = input.Description,
            DueDate = input.DueDate,
            Status = (PostgraduateTaskStatus)input.Status,
            Priority = (PostgraduateTaskPriority)input.Priority,
            Type = (PostgraduateTaskType)input.Type
        };

        dbContext.PostgraduateTasks.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(task);
    }

    public async Task<PostgraduateTaskDto?> UpdateAsync(Guid id, UpdatePostgraduateTaskDto input, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var tasks = dbContext.PostgraduateTasks.Where(x => x.Id == id);
        if (userId.HasValue)
        {
            tasks = tasks.Where(x => x.UserId == userId.Value);
        }

        var task = await tasks.FirstOrDefaultAsync(cancellationToken);
        if (task is null) return null;

        if (input.Title is not null) task.Title = input.Title;
        if (input.Description is not null) task.Description = input.Description;
        if (input.DueDate.HasValue) task.DueDate = input.DueDate;
        if (input.Status.HasValue) task.Status = (PostgraduateTaskStatus)input.Status.Value;
        if (input.Priority.HasValue) task.Priority = (PostgraduateTaskPriority)input.Priority.Value;
        if (input.Type.HasValue) task.Type = (PostgraduateTaskType)input.Type.Value;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(task);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var tasks = dbContext.PostgraduateTasks.Where(x => x.Id == id);
        if (userId.HasValue)
        {
            tasks = tasks.Where(x => x.UserId == userId.Value);
        }

        var task = await tasks.FirstOrDefaultAsync(cancellationToken);
        if (task is null) return false;

        dbContext.PostgraduateTasks.Remove(task);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static PostgraduateTaskDto ToDto(PostgraduateTask task) => new()
    {
        Id = task.Id,
        UserId = task.UserId,
        Title = task.Title,
        Description = task.Description,
        DueDate = task.DueDate,
        Status = (int)task.Status,
        Priority = (int)task.Priority,
        Type = (int)task.Type,
        CreatedAt = task.CreatedAt,
        UpdatedAt = task.UpdatedAt
    };
}

public class ExamMistakeService(AppDbContext dbContext) : IExamMistakeService
{
    public async Task<PageResult<ExamMistakeDto>> GetPageAsync(ExamMistakeQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var mistakes = dbContext.ExamMistakes.AsNoTracking();

        if (userId.HasValue)
        {
            mistakes = mistakes.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            mistakes = mistakes.Where(x => x.Question.Contains(keyword) || (x.Explanation != null && x.Explanation.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.Subject))
        {
            mistakes = mistakes.Where(x => x.Subject == query.Subject);
        }

        var total = await mistakes.CountAsync(cancellationToken);
        var items = await mistakes
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<ExamMistakeDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<ExamMistakeDto?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var mistakes = dbContext.ExamMistakes.AsNoTracking().Where(x => x.Id == id);
        if (userId.HasValue)
        {
            mistakes = mistakes.Where(x => x.UserId == userId.Value);
        }

        var mistake = await mistakes.FirstOrDefaultAsync(cancellationToken);
        return mistake is null ? null : ToDto(mistake);
    }

    public async Task<ExamMistakeDto> CreateAsync(CreateExamMistakeDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var mistake = new ExamMistake
        {
            UserId = userId,
            Question = input.Question,
            Answer = input.Answer,
            Explanation = input.Explanation,
            Subject = input.Subject,
            Tags = input.Tags
        };

        dbContext.ExamMistakes.Add(mistake);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(mistake);
    }

    public async Task<ExamMistakeDto?> UpdateAsync(Guid id, UpdateExamMistakeDto input, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var mistakes = dbContext.ExamMistakes.Where(x => x.Id == id);
        if (userId.HasValue)
        {
            mistakes = mistakes.Where(x => x.UserId == userId.Value);
        }

        var mistake = await mistakes.FirstOrDefaultAsync(cancellationToken);
        if (mistake is null) return null;

        if (input.Question is not null) mistake.Question = input.Question;
        if (input.Answer is not null) mistake.Answer = input.Answer;
        if (input.Explanation is not null) mistake.Explanation = input.Explanation;
        if (input.Subject is not null) mistake.Subject = input.Subject;
        if (input.Tags is not null) mistake.Tags = input.Tags;
        if (input.Status.HasValue) mistake.Status = (ExamMistakeStatus)input.Status.Value;
        if (input.ReviewCount.HasValue) mistake.ReviewCount = input.ReviewCount.Value;
        if (input.LastReviewDate.HasValue) mistake.LastReviewDate = input.LastReviewDate;
        if (input.NextReviewDate.HasValue) mistake.NextReviewDate = input.NextReviewDate;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(mistake);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var mistakes = dbContext.ExamMistakes.Where(x => x.Id == id);
        if (userId.HasValue)
        {
            mistakes = mistakes.Where(x => x.UserId == userId.Value);
        }

        var mistake = await mistakes.FirstOrDefaultAsync(cancellationToken);
        if (mistake is null) return false;

        dbContext.ExamMistakes.Remove(mistake);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ExamMistakeDto ToDto(ExamMistake mistake) => new()
    {
        Id = mistake.Id,
        UserId = mistake.UserId,
        Question = mistake.Question,
        Answer = mistake.Answer,
        Explanation = mistake.Explanation,
        Subject = mistake.Subject,
        Tags = mistake.Tags,
        Status = (int)mistake.Status,
        ReviewCount = mistake.ReviewCount,
        LastReviewDate = mistake.LastReviewDate,
        NextReviewDate = mistake.NextReviewDate,
        CreatedAt = mistake.CreatedAt,
        UpdatedAt = mistake.UpdatedAt
    };
}

public class ExamMaterialService(AppDbContext dbContext) : IExamMaterialService
{
    public async Task<PageResult<ExamMaterialDto>> GetPageAsync(ExamMaterialQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var materials = dbContext.ExamMaterials.AsNoTracking();

        if (userId.HasValue)
        {
            materials = materials.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            materials = materials.Where(x => x.Title.Contains(keyword) || (x.Content != null && x.Content.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.Subject))
        {
            materials = materials.Where(x => x.Subject == query.Subject);
        }

        if (query.Type.HasValue)
        {
            materials = materials.Where(x => x.Type == (ExamMaterialType)query.Type.Value);
        }

        var total = await materials.CountAsync(cancellationToken);
        var items = await materials
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<ExamMaterialDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<ExamMaterialDto?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var materials = dbContext.ExamMaterials.AsNoTracking().Where(x => x.Id == id);
        if (userId.HasValue)
        {
            materials = materials.Where(x => x.UserId == userId.Value);
        }

        var material = await materials.FirstOrDefaultAsync(cancellationToken);
        return material is null ? null : ToDto(material);
    }

    public async Task<ExamMaterialDto> CreateAsync(CreateExamMaterialDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var material = new ExamMaterial
        {
            UserId = userId,
            Title = input.Title,
            Content = input.Content,
            Subject = input.Subject,
            Tags = input.Tags,
            Type = (ExamMaterialType)input.Type
        };

        dbContext.ExamMaterials.Add(material);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(material);
    }

    public async Task<ExamMaterialDto?> UpdateAsync(Guid id, UpdateExamMaterialDto input, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var materials = dbContext.ExamMaterials.Where(x => x.Id == id);
        if (userId.HasValue)
        {
            materials = materials.Where(x => x.UserId == userId.Value);
        }

        var material = await materials.FirstOrDefaultAsync(cancellationToken);
        if (material is null) return null;

        if (input.Title is not null) material.Title = input.Title;
        if (input.Content is not null) material.Content = input.Content;
        if (input.Subject is not null) material.Subject = input.Subject;
        if (input.Tags is not null) material.Tags = input.Tags;
        if (input.Type.HasValue) material.Type = (ExamMaterialType)input.Type.Value;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(material);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var materials = dbContext.ExamMaterials.Where(x => x.Id == id);
        if (userId.HasValue)
        {
            materials = materials.Where(x => x.UserId == userId.Value);
        }

        var material = await materials.FirstOrDefaultAsync(cancellationToken);
        if (material is null) return false;

        dbContext.ExamMaterials.Remove(material);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ExamMaterialDto ToDto(ExamMaterial material) => new()
    {
        Id = material.Id,
        UserId = material.UserId,
        Title = material.Title,
        Content = material.Content,
        Subject = material.Subject,
        Tags = material.Tags,
        Type = (int)material.Type,
        CreatedAt = material.CreatedAt,
        UpdatedAt = material.UpdatedAt
    };
}

public class StudentSubjectService(AppDbContext dbContext) : IStudentSubjectService
{
    public async Task<PageResult<StudentSubjectDto>> GetPageAsync(StudentSubjectQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var subjects = dbContext.StudentSubjects.AsNoTracking();

        if (userId.HasValue)
        {
            subjects = subjects.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            subjects = subjects.Where(x => x.Name.Contains(keyword) || (x.Description != null && x.Description.Contains(keyword)));
        }

        if (query.IsActive.HasValue)
        {
            subjects = subjects.Where(x => x.IsActive == query.IsActive.Value);
        }

        var total = await subjects.CountAsync(cancellationToken);
        var items = await subjects
            .OrderBy(x => x.Sort)
            .ThenBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<StudentSubjectDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<StudentSubjectDto?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var subjects = dbContext.StudentSubjects.AsNoTracking().Where(x => x.Id == id);
        if (userId.HasValue)
        {
            subjects = subjects.Where(x => x.UserId == userId.Value);
        }

        var subject = await subjects.FirstOrDefaultAsync(cancellationToken);
        return subject is null ? null : ToDto(subject);
    }

    public async Task<StudentSubjectDto> CreateAsync(CreateStudentSubjectDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var subject = new StudentSubject
        {
            UserId = userId,
            Name = input.Name,
            Description = input.Description,
            Color = string.IsNullOrWhiteSpace(input.Color) ? "blue" : input.Color,
            TargetHours = input.TargetHours,
            Sort = input.Sort,
            IsActive = input.IsActive
        };

        dbContext.StudentSubjects.Add(subject);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(subject);
    }

    public async Task<StudentSubjectDto?> UpdateAsync(Guid id, UpdateStudentSubjectDto input, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var subjects = dbContext.StudentSubjects.Where(x => x.Id == id);
        if (userId.HasValue)
        {
            subjects = subjects.Where(x => x.UserId == userId.Value);
        }

        var subject = await subjects.FirstOrDefaultAsync(cancellationToken);
        if (subject is null) return null;

        if (input.Name is not null) subject.Name = input.Name;
        if (input.Description is not null) subject.Description = input.Description;
        if (input.Color is not null) subject.Color = string.IsNullOrWhiteSpace(input.Color) ? subject.Color : input.Color;
        if (input.TargetHours.HasValue) subject.TargetHours = input.TargetHours.Value;
        if (input.Sort.HasValue) subject.Sort = input.Sort.Value;
        if (input.IsActive.HasValue) subject.IsActive = input.IsActive.Value;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(subject);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var subjects = dbContext.StudentSubjects.Where(x => x.Id == id);
        if (userId.HasValue)
        {
            subjects = subjects.Where(x => x.UserId == userId.Value);
        }

        var subject = await subjects.FirstOrDefaultAsync(cancellationToken);
        if (subject is null) return false;

        dbContext.StudentSubjects.Remove(subject);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static StudentSubjectDto ToDto(StudentSubject subject) => new()
    {
        Id = subject.Id,
        UserId = subject.UserId,
        Name = subject.Name,
        Description = subject.Description,
        Color = subject.Color,
        TargetHours = subject.TargetHours,
        Sort = subject.Sort,
        IsActive = subject.IsActive,
        CreatedAt = subject.CreatedAt,
        UpdatedAt = subject.UpdatedAt
    };
}

public class StudentStudyRecordService(AppDbContext dbContext) : IStudentStudyRecordService
{
    public async Task<PageResult<StudentStudyRecordDto>> GetPageAsync(StudentStudyRecordQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var records = dbContext.StudentStudyRecords.AsNoTracking();

        if (userId.HasValue)
        {
            records = records.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            records = records.Where(x => x.Summary.Contains(keyword) || (x.Remark != null && x.Remark.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.Subject))
        {
            records = records.Where(x => x.Subject == query.Subject);
        }

        if (query.StartDate.HasValue)
        {
            records = records.Where(x => x.RecordDate >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            records = records.Where(x => x.RecordDate <= query.EndDate.Value);
        }

        var total = await records.CountAsync(cancellationToken);
        var items = await records
            .OrderByDescending(x => x.RecordDate)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<StudentStudyRecordDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<StudentStudyRecordDto?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var records = dbContext.StudentStudyRecords.AsNoTracking().Where(x => x.Id == id);
        if (userId.HasValue)
        {
            records = records.Where(x => x.UserId == userId.Value);
        }

        var record = await records.FirstOrDefaultAsync(cancellationToken);
        return record is null ? null : ToDto(record);
    }

    public async Task<StudentStudyRecordDto> CreateAsync(CreateStudentStudyRecordDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var record = new StudentStudyRecord
        {
            UserId = userId,
            Subject = input.Subject,
            Summary = input.Summary,
            RecordDate = input.RecordDate,
            DurationMinutes = input.DurationMinutes,
            TaskTitle = input.TaskTitle,
            TaskId = input.TaskId,
            Remark = input.Remark
        };

        dbContext.StudentStudyRecords.Add(record);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(record);
    }

    public async Task<StudentStudyRecordDto?> UpdateAsync(Guid id, UpdateStudentStudyRecordDto input, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var records = dbContext.StudentStudyRecords.Where(x => x.Id == id);
        if (userId.HasValue)
        {
            records = records.Where(x => x.UserId == userId.Value);
        }

        var record = await records.FirstOrDefaultAsync(cancellationToken);
        if (record is null) return null;

        if (input.Subject is not null) record.Subject = input.Subject;
        if (input.Summary is not null) record.Summary = input.Summary;
        if (input.RecordDate.HasValue) record.RecordDate = input.RecordDate.Value;
        if (input.DurationMinutes.HasValue) record.DurationMinutes = input.DurationMinutes.Value;
        if (input.TaskTitle is not null) record.TaskTitle = input.TaskTitle;
        if (input.TaskId.HasValue) record.TaskId = input.TaskId;
        if (input.Remark is not null) record.Remark = input.Remark;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(record);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var records = dbContext.StudentStudyRecords.Where(x => x.Id == id);
        if (userId.HasValue)
        {
            records = records.Where(x => x.UserId == userId.Value);
        }

        var record = await records.FirstOrDefaultAsync(cancellationToken);
        if (record is null) return false;

        dbContext.StudentStudyRecords.Remove(record);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static StudentStudyRecordDto ToDto(StudentStudyRecord record) => new()
    {
        Id = record.Id,
        UserId = record.UserId,
        Subject = record.Subject,
        Summary = record.Summary,
        RecordDate = record.RecordDate,
        DurationMinutes = record.DurationMinutes,
        TaskTitle = record.TaskTitle,
        TaskId = record.TaskId,
        Remark = record.Remark,
        CreatedAt = record.CreatedAt,
        UpdatedAt = record.UpdatedAt
    };
}
