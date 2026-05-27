using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Enums;
using WebApplication1.Shared.Services;

namespace WebApplication1.Features.Analytics;

public interface IAnalyticsService
{
    Task<DashboardOverviewDto> GetDashboardOverviewAsync(Guid userId, CancellationToken ct = default);
    Task<List<TaskTrendDto>> GetTaskTrendsAsync(Guid userId, DateOnly startDate, DateOnly endDate, CancellationToken ct = default);
    Task<List<TaskDistributionDto>> GetTaskDistributionByTypeAsync(Guid userId, CancellationToken ct = default);
    Task<List<WorkVsGrowthDto>> GetWorkVsGrowthAsync(Guid userId, CancellationToken ct = default);
    Task<List<TaskPriorityDistributionDto>> GetTaskPriorityDistributionAsync(Guid userId, CancellationToken ct = default);
}

public class AnalyticsService(AppDbContext context, ICacheService cacheService) : IAnalyticsService
{
    public async Task<DashboardOverviewDto> GetDashboardOverviewAsync(Guid userId, CancellationToken ct = default)
    {
        var cacheKey = $"analytics:dashboard:{userId}";
        var cached = await cacheService.GetAsync<DashboardOverviewDto>(cacheKey);
        if (cached != null)
            return cached;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var tasks = context.Tasks.AsNoTracking().Where(x => x.UserId == userId);
        var workLogs = context.WorkLogs.AsNoTracking().Where(x => x.UserId == userId);

        var taskStats = await tasks
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Completed = g.Count(x => x.Status == TaskItemStatus.Completed),
                Pending = g.Count(x => x.Status == TaskItemStatus.Pending || x.Status == TaskItemStatus.InProgress),
                TodayCreated = g.Count(x => x.PlanDate == today),
                TodayCompleted = g.Count(x => x.PlanDate == today && x.Status == TaskItemStatus.Completed)
            })
            .FirstOrDefaultAsync(ct);

        var workStats = await workLogs
            .GroupBy(_ => 1)
            .Select(g => new
            {
                TotalLogs = g.Count(),
                TotalHours = g.Sum(x => x.TotalHours),
                TodayHours = g.Where(x => x.WorkDate == today).Sum(x => x.TotalHours)
            })
            .FirstOrDefaultAsync(ct);

        var completionRate = taskStats?.Total > 0
            ? Math.Round((decimal)taskStats.Completed / taskStats.Total * 100, 1)
            : 0;

        var result = new DashboardOverviewDto
        {
            TotalTasks = taskStats?.Total ?? 0,
            CompletedTasks = taskStats?.Completed ?? 0,
            PendingTasks = taskStats?.Pending ?? 0,
            CompletionRate = completionRate,
            TotalWorkLogs = workStats?.TotalLogs ?? 0,
            TotalWorkHours = workStats?.TotalHours ?? 0,
            TodayTasks = taskStats?.TodayCreated ?? 0,
            TodayCompletedTasks = taskStats?.TodayCompleted ?? 0,
            TodayWorkHours = (int)(workStats?.TodayHours ?? 0)
        };

        await cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
        return result;
    }

    public async Task<List<TaskTrendDto>> GetTaskTrendsAsync(Guid userId, DateOnly startDate, DateOnly endDate, CancellationToken ct = default)
    {
        var cacheKey = $"analytics:trends:{userId}:{startDate}:{endDate}";
        var cached = await cacheService.GetAsync<List<TaskTrendDto>>(cacheKey);
        if (cached != null)
            return cached;

        var tasks = await context.Tasks
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.PlanDate >= startDate && x.PlanDate <= endDate)
            .GroupBy(x => x.PlanDate)
            .Select(g => new TaskTrendDto
            {
                Date = g.Key,
                Created = g.Count(),
                Completed = g.Count(x => x.Status == TaskItemStatus.Completed)
            })
            .OrderBy(x => x.Date)
            .ToListAsync(ct);

        foreach (var t in tasks)
        {
            t.CompletionRate = t.Created > 0 ? Math.Round((decimal)t.Completed / t.Created * 100, 1) : 0;
        }

        await cacheService.SetAsync(cacheKey, tasks, TimeSpan.FromMinutes(10));
        return tasks;
    }

    public async Task<List<TaskDistributionDto>> GetTaskDistributionByTypeAsync(Guid userId, CancellationToken ct = default)
    {
        var cacheKey = $"analytics:distribution:type:{userId}";
        var cached = await cacheService.GetAsync<List<TaskDistributionDto>>(cacheKey);
        if (cached != null)
            return cached;

        var total = await context.Tasks.CountAsync(x => x.UserId == userId, ct);

        var distribution = await context.Tasks
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.Type)
            .Select(g => new TaskDistributionDto
            {
                Type = g.Key.ToString(),
                Count = g.Count()
            })
            .ToListAsync(ct);

        foreach (var d in distribution)
        {
            d.Percentage = total > 0 ? Math.Round((decimal)d.Count / total * 100, 1) : 0;
        }

        await cacheService.SetAsync(cacheKey, distribution, TimeSpan.FromMinutes(10));
        return distribution;
    }

    public async Task<List<WorkVsGrowthDto>> GetWorkVsGrowthAsync(Guid userId, CancellationToken ct = default)
    {
        var cacheKey = $"analytics:workVsGrowth:{userId}";
        var cached = await cacheService.GetAsync<List<WorkVsGrowthDto>>(cacheKey);
        if (cached != null)
            return cached;

        var taskStats = await context.Tasks
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.Source)
            .Select(g => new WorkVsGrowthDto
            {
                Category = g.Key.ToString(),
                TaskCount = g.Count()
            })
            .ToListAsync(ct);

        await cacheService.SetAsync(cacheKey, taskStats, TimeSpan.FromMinutes(10));
        return taskStats;
    }

    public async Task<List<TaskPriorityDistributionDto>> GetTaskPriorityDistributionAsync(Guid userId, CancellationToken ct = default)
    {
        var cacheKey = $"analytics:priority:{userId}";
        var cached = await cacheService.GetAsync<List<TaskPriorityDistributionDto>>(cacheKey);
        if (cached != null)
            return cached;

        var total = await context.Tasks.CountAsync(x => x.UserId == userId, ct);

        var distribution = await context.Tasks
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .GroupBy(x => x.Priority)
            .Select(g => new TaskPriorityDistributionDto
            {
                Priority = g.Key.ToString(),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Count)
            .ToListAsync(ct);

        foreach (var d in distribution)
        {
            d.Percentage = total > 0 ? Math.Round((decimal)d.Count / total * 100, 1) : 0;
        }

        await cacheService.SetAsync(cacheKey, distribution, TimeSpan.FromMinutes(10));
        return distribution;
    }
}