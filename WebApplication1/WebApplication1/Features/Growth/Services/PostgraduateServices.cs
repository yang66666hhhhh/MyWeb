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

    public async Task<PostgraduateTaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await dbContext.PostgraduateTasks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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

    public async Task<PostgraduateTaskDto?> UpdateAsync(Guid id, UpdatePostgraduateTaskDto input, CancellationToken cancellationToken = default)
    {
        var task = await dbContext.PostgraduateTasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await dbContext.PostgraduateTasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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

    public async Task<ExamMistakeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var mistake = await dbContext.ExamMistakes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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

    public async Task<ExamMistakeDto?> UpdateAsync(Guid id, UpdateExamMistakeDto input, CancellationToken cancellationToken = default)
    {
        var mistake = await dbContext.ExamMistakes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var mistake = await dbContext.ExamMistakes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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

    public async Task<ExamMaterialDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var material = await dbContext.ExamMaterials.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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

    public async Task<ExamMaterialDto?> UpdateAsync(Guid id, UpdateExamMaterialDto input, CancellationToken cancellationToken = default)
    {
        var material = await dbContext.ExamMaterials.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (material is null) return null;

        if (input.Title is not null) material.Title = input.Title;
        if (input.Content is not null) material.Content = input.Content;
        if (input.Subject is not null) material.Subject = input.Subject;
        if (input.Tags is not null) material.Tags = input.Tags;
        if (input.Type.HasValue) material.Type = (ExamMaterialType)input.Type.Value;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(material);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var material = await dbContext.ExamMaterials.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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