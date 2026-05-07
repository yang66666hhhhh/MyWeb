using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Enums;
using WebApplication1.Features.Work.Entities;

namespace WebApplication1.Features.Tasks;

public interface ITaskItemService
{
    Task<PageResult<TaskItemDto>> GetPageAsync(TaskItemQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<TaskItemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskItemDto> CreateAsync(CreateTaskItemDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<TaskItemDto?> UpdateAsync(Guid id, UpdateTaskItemDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskItemDto?> CompleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Guid?> ConvertToWorkLogAsync(ConvertTaskToLogDto input, CancellationToken cancellationToken = default);
}

public class TaskItemService(AppDbContext dbContext, ILogger<TaskItemService> logger) : ITaskItemService
{
    public async Task<PageResult<TaskItemDto>> GetPageAsync(TaskItemQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var tasks = dbContext.Tasks.AsNoTracking();

        if (userId.HasValue)
        {
            tasks = tasks.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.TaskType) && Enum.TryParse<TaskType>(query.TaskType, true, out var taskType))
        {
            tasks = tasks.Where(x => x.Type == taskType);
        }

        if (!string.IsNullOrWhiteSpace(query.Source) && Enum.TryParse<TaskSource>(query.Source, true, out var source))
        {
            tasks = tasks.Where(x => x.Source == source);
        }

        if (query.StartDate.HasValue)
        {
            tasks = tasks.Where(x => x.PlanDate >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            tasks = tasks.Where(x => x.PlanDate <= query.EndDate.Value);
        }

        if (query.Status.HasValue)
        {
            tasks = tasks.Where(x => x.Status == (TaskItemStatus)query.Status.Value);
        }

        if (query.Priority.HasValue)
        {
            tasks = tasks.Where(x => x.Priority == (TaskPriority)query.Priority.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            tasks = tasks.Where(x =>
                x.Title.Contains(keyword) ||
                (x.Description != null && x.Description.Contains(keyword)) ||
                (x.Remark != null && x.Remark.Contains(keyword)));
        }

        var total = await tasks.CountAsync(cancellationToken);
        var items = await tasks
            .OrderByDescending(x => x.PlanDate)
            .ThenBy(x => x.Status)
            .ThenBy(x => x.Priority)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(x => x.Project)
            .ToListAsync(cancellationToken);

        return PageResult<TaskItemDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<TaskItemDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await dbContext.Tasks
            .AsNoTracking()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return task is null ? null : ToDto(task);
    }

    public async Task<TaskItemDto> CreateAsync(CreateTaskItemDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        Guid? projectId = null;
        if (!string.IsNullOrWhiteSpace(input.ProjectId) && Guid.TryParse(input.ProjectId, out var parsedProjectId))
        {
            projectId = parsedProjectId;
        }

        var taskType = Enum.TryParse<TaskType>(input.TaskType, true, out var tt) ? tt : TaskType.Personal;
        var source = Enum.TryParse<TaskSource>(input.Source, true, out var src) ? src : TaskSource.Growth;

        var task = new TaskItem
        {
            UserId = userId,
            PlanDate = input.PlanDate,
            Title = input.Title.Trim(),
            Description = input.Description?.Trim(),
            Type = taskType,
            Source = source,
            ProjectId = projectId,
            Priority = (TaskPriority)input.Priority,
            StartTime = input.StartTime,
            EndTime = input.EndTime,
            EstimatedHours = input.EstimatedHours,
            Remark = input.Remark?.Trim(),
            Status = TaskItemStatus.Pending
        };

        dbContext.Tasks.Add(task);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (task.ProjectId.HasValue)
        {
            await dbContext.Entry(task).Reference(x => x.Project).LoadAsync(cancellationToken);
        }

        return ToDto(task);
    }

    public async Task<TaskItemDto?> UpdateAsync(Guid id, UpdateTaskItemDto input, CancellationToken cancellationToken = default)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (task is null)
        {
            return null;
        }

        if (input.PlanDate.HasValue) task.PlanDate = input.PlanDate.Value;
        if (input.Title is not null) task.Title = input.Title;
        if (input.Description is not null) task.Description = input.Description;
        if (input.TaskType is not null && Enum.TryParse<TaskType>(input.TaskType, out var tt)) task.Type = tt;
        if (input.Source is not null && Enum.TryParse<TaskSource>(input.Source, out var src)) task.Source = src;
        if (input.ProjectId is not null && Guid.TryParse(input.ProjectId, out var updatedProjectId))
        {
            task.ProjectId = updatedProjectId;
        }
        if (input.Priority.HasValue) task.Priority = (TaskPriority)input.Priority.Value;
        if (input.Status.HasValue) task.Status = (TaskItemStatus)input.Status.Value;
        if (input.StartTime is not null) task.StartTime = input.StartTime;
        if (input.EndTime is not null) task.EndTime = input.EndTime;
        if (input.EstimatedHours.HasValue) task.EstimatedHours = input.EstimatedHours;
        if (input.ActualHours.HasValue) task.ActualHours = input.ActualHours;
        if (input.Remark is not null) task.Remark = input.Remark;

        await dbContext.SaveChangesAsync(cancellationToken);

        if (task.ProjectId.HasValue)
        {
            await dbContext.Entry(task).Reference(x => x.Project).LoadAsync(cancellationToken);
        }

        return ToDto(task);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (task is null)
        {
            return false;
        }

        dbContext.Tasks.Remove(task);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<TaskItemDto?> CompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var task = await dbContext.Tasks
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.Status = TaskItemStatus.Completed;
        task.CompletedAt ??= DateTime.UtcNow;
        task.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(task);
    }

    public async Task<Guid?> ConvertToWorkLogAsync(ConvertTaskToLogDto input, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(input.TaskId, out var taskId))
        {
            return null;
        }

        var task = await dbContext.Tasks
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == taskId, cancellationToken);
        if (task is null)
        {
            return null;
        }

        var workLog = new WorkLog
        {
            WorkDate = input.WorkDate,
            Title = task.Title,
            OriginalContent = input.OriginalContent ?? task.Description,
            TotalHours = input.TotalHours ?? task.EstimatedHours ?? 0,
            ProjectId = task.ProjectId ?? Guid.Empty,
            WeekDay = input.WorkDate.DayOfWeek.ToString(),
            Status = WorkLogStatus.Normal,
            SourceType = WorkLogSourceType.PlanConversion
        };

        dbContext.WorkLogs.Add(workLog);
        task.ConvertedWorkLogId = workLog.Id;
        task.Status = TaskItemStatus.Completed;

        await dbContext.SaveChangesAsync(cancellationToken);

        return workLog.Id;
    }

    private static TaskItemDto ToDto(TaskItem task)
    {
        return new TaskItemDto
        {
            Id = task.Id,
            UserId = task.UserId,
            PlanDate = task.PlanDate,
            Title = task.Title,
            Description = task.Description,
            Type = task.Type.ToString(),
            Source = task.Source.ToString(),
            ProjectId = task.ProjectId,
            ProjectName = task.Project?.ProjectName,
            Priority = task.Priority.ToString(),
            Status = task.Status.ToString(),
            StartTime = task.StartTime,
            EndTime = task.EndTime,
            EstimatedHours = task.EstimatedHours,
            ActualHours = task.ActualHours,
            ConvertedWorkLogId = task.ConvertedWorkLogId,
            Remark = task.Remark,
            CompletedAt = task.CompletedAt,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}