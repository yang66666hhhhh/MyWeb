using System.Globalization;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Entities;

namespace WebApplication1.Features.Work.Services;

public interface IWeeklyPlanService
{
    Task<PageResult<WeeklyPlanDto>> GetPageAsync(WeeklyPlanQueryDto query, Guid userId, CancellationToken cancellationToken = default);
    Task<WeeklyPlanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WeeklyPlanDto> CreateAsync(CreateWeeklyPlanDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<WeeklyPlanDto?> UpdateAsync(Guid id, UpdateWeeklyPlanDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WeeklyPlanDto?> AddTaskAsync(Guid planId, CreateWeeklyPlanTaskDto input, CancellationToken cancellationToken = default);
    Task<WeeklyPlanDto?> UpdateTaskAsync(Guid taskId, UpdateWeeklyPlanTaskDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
}

public class WeeklyPlanService(AppDbContext context) : IWeeklyPlanService
{
    public async Task<PageResult<WeeklyPlanDto>> GetPageAsync(WeeklyPlanQueryDto query, Guid userId, CancellationToken cancellationToken = default)
    {
        var q = context.WeeklyPlans
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Include(x => x.Tasks)
            .AsQueryable();

        if (query.Year.HasValue)
            q = q.Where(x => x.Year == query.Year.Value);
        if (query.WeekNumber.HasValue)
            q = q.Where(x => x.WeekNumber == query.WeekNumber.Value);
        if (query.Status.HasValue)
            q = q.Where(x => x.Status == (WeeklyPlanStatus)query.Status.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x => x.Goals.Contains(query.Keyword));

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.WeekNumber)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return PageResult<WeeklyPlanDto>.Create(items.Select(MapToDto).ToList(), total, query.Page, query.PageSize);
    }

    public async Task<WeeklyPlanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.WeeklyPlans
            .AsNoTracking()
            .Include(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<WeeklyPlanDto> CreateAsync(CreateWeeklyPlanDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var (startDate, endDate) = GetWeekDates(input.Year, input.WeekNumber);
        var weekCode = $"{input.Year}-W{input.WeekNumber:D2}";

        var existing = await context.WeeklyPlans
            .AnyAsync(x => x.UserId == userId && x.Year == input.Year && x.WeekNumber == input.WeekNumber, cancellationToken);
        if (existing)
            throw new InvalidOperationException("该周计划已存在");

        var entity = new WeeklyPlan
        {
            UserId = userId,
            Year = input.Year,
            WeekNumber = input.WeekNumber,
            WeekCode = weekCode,
            StartDate = startDate,
            EndDate = endDate,
            Goals = input.Goals,
            Status = (WeeklyPlanStatus)input.Status
        };

        context.WeeklyPlans.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<WeeklyPlanDto?> UpdateAsync(Guid id, UpdateWeeklyPlanDto input, CancellationToken cancellationToken = default)
    {
        var entity = await context.WeeklyPlans
            .Include(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity == null) return null;

        if (input.Goals != null) entity.Goals = input.Goals;
        if (input.Summary != null) entity.Summary = input.Summary;
        if (input.Status.HasValue) entity.Status = (WeeklyPlanStatus)input.Status.Value;

        await context.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.WeeklyPlans.FindAsync([id], cancellationToken);
        if (entity == null) return false;

        context.WeeklyPlans.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<WeeklyPlanDto?> AddTaskAsync(Guid planId, CreateWeeklyPlanTaskDto input, CancellationToken cancellationToken = default)
    {
        var entity = await context.WeeklyPlans
            .Include(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.Id == planId, cancellationToken);
        if (entity == null) return null;

        var task = new WeeklyPlanTask
        {
            WeeklyPlanId = planId,
            Title = input.Title,
            Description = input.Description,
            Priority = (WeeklyPlanTaskPriority)input.Priority,
            EstimatedHours = input.EstimatedHours
        };

        context.WeeklyPlanTasks.Add(task);
        await context.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<WeeklyPlanDto?> UpdateTaskAsync(Guid taskId, UpdateWeeklyPlanTaskDto input, CancellationToken cancellationToken = default)
    {
        var task = await context.WeeklyPlanTasks
            .Include(x => x.WeeklyPlan)
            .ThenInclude(x => x.Tasks)
            .FirstOrDefaultAsync(x => x.Id == taskId, cancellationToken);
        if (task == null) return null;

        if (input.Title != null) task.Title = input.Title;
        if (input.Description != null) task.Description = input.Description;
        if (input.Priority.HasValue) task.Priority = (WeeklyPlanTaskPriority)input.Priority.Value;
        if (input.Status.HasValue) task.Status = (WeeklyPlanTaskStatus)input.Status.Value;
        if (input.EstimatedHours.HasValue) task.EstimatedHours = input.EstimatedHours;
        if (input.ActualHours.HasValue) task.ActualHours = input.ActualHours;

        await context.SaveChangesAsync(cancellationToken);
        return MapToDto(task.WeeklyPlan);
    }

    public async Task<bool> DeleteTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        var task = await context.WeeklyPlanTasks.FindAsync([taskId], cancellationToken);
        if (task == null) return false;

        context.WeeklyPlanTasks.Remove(task);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static (DateOnly start, DateOnly end) GetWeekDates(int year, int week)
    {
        var jan4 = new DateOnly(year, 1, 4);
        var startOfWeek1 = jan4.AddDays(-(int)jan4.DayOfWeek + (DayOfWeek.Monday - DayOfWeek.Monday + 7) % 7);
        if (jan4.DayOfWeek != DayOfWeek.Monday)
            startOfWeek1 = jan4.AddDays(-(int)jan4.DayOfWeek + (int)DayOfWeek.Monday);
        var start = startOfWeek1.AddDays((week - 1) * 7);
        var end = start.AddDays(6);
        return (start, end);
    }

    private static WeeklyPlanDto MapToDto(WeeklyPlan entity)
    {
        return new WeeklyPlanDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Year = entity.Year,
            WeekNumber = entity.WeekNumber,
            WeekCode = entity.WeekCode,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Goals = entity.Goals,
            Summary = entity.Summary,
            TotalHours = entity.TotalHours,
            Status = (int)entity.Status,
            Tasks = entity.Tasks?.Select(t => new WeeklyPlanTaskDto
            {
                Id = t.Id,
                WeeklyPlanId = t.WeeklyPlanId,
                Title = t.Title,
                Description = t.Description,
                Priority = (int)t.Priority,
                Status = (int)t.Status,
                EstimatedHours = t.EstimatedHours,
                ActualHours = t.ActualHours,
                CreatedAt = t.CreatedAt
            }).ToList() ?? new(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
