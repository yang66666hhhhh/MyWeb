using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Dtos.Work;
using WebApplication1.Services.Interfaces.IWork;

namespace WebApplication1.Services.Implementations.Work;

public class WorkStatisticsService : IWorkStatisticsService
{
    private readonly AppDbContext _context;

    public WorkStatisticsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WorkStatisticsOverviewDto> GetOverviewAsync(WorkStatisticsQueryDto query, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var logsQuery = _context.WorkLogs.AsQueryable();
        if (query.StartDate.HasValue)
            logsQuery = logsQuery.Where(x => x.WorkDate >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            logsQuery = logsQuery.Where(x => x.WorkDate <= query.EndDate.Value);
        if (query.ProjectId.HasValue)
            logsQuery = logsQuery.Where(x => x.ProjectId == query.ProjectId.Value);

        var todayLogsQuery = logsQuery.Where(x => x.WorkDate == today);

        var totalLogs = await logsQuery.CountAsync(cancellationToken);
        var totalHours = await logsQuery.SumAsync(x => x.TotalHours, cancellationToken);
        var todayLogsCount = await todayLogsQuery.CountAsync(cancellationToken);
        var todayHours = await todayLogsQuery.SumAsync(x => x.TotalHours, cancellationToken);
        var missingCount = await logsQuery.CountAsync(x => x.Status == Enums.WorkLogStatus.MissingData, cancellationToken);
        var pendingCount = await logsQuery.CountAsync(x => x.Status == Enums.WorkLogStatus.PendingSupplement, cancellationToken);

        return new WorkStatisticsOverviewDto
        {
            TotalLogs = totalLogs,
            TotalHours = totalHours,
            TotalProjects = await _context.WorkProjects.CountAsync(cancellationToken),
            TotalDevices = await _context.WorkDevices.CountAsync(cancellationToken),
            TodayLogs = todayLogsCount,
            TodayHours = todayHours,
            MissingDataCount = missingCount,
            PendingSupplementCount = pendingCount
        };
    }

    public async Task<List<WorkStatisticsDailyHoursDto>> GetDailyHoursAsync(WorkStatisticsQueryDto query, CancellationToken cancellationToken = default)
    {
        var startDate = query.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var endDate = query.EndDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var q = _context.WorkLogs.Where(x => x.WorkDate >= startDate && x.WorkDate <= endDate);

        if (query.ProjectId.HasValue)
            q = q.Where(x => x.ProjectId == query.ProjectId.Value);

        var result = await q
            .GroupBy(x => x.WorkDate)
            .Select(g => new WorkStatisticsDailyHoursDto
            {
                Date = g.Key,
                Hours = g.Sum(x => x.TotalHours),
                LogCount = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<List<WorkStatisticsProjectHoursDto>> GetProjectHoursAsync(WorkStatisticsQueryDto query, CancellationToken cancellationToken = default)
    {
        var q = _context.WorkLogs.Include(x => x.Project).AsQueryable();

        if (query.StartDate.HasValue)
            q = q.Where(x => x.WorkDate >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            q = q.Where(x => x.WorkDate <= query.EndDate.Value);
        if (query.ProjectId.HasValue)
            q = q.Where(x => x.ProjectId == query.ProjectId.Value);

        var totalHoursSum = await q.SumAsync(x => x.TotalHours, cancellationToken);

        var result = await q
            .GroupBy(x => x.ProjectId)
            .Select(g => new WorkStatisticsProjectHoursDto
            {
                ProjectId = g.Key,
                ProjectName = g.First().Project != null ? g.First().Project!.ProjectName : "",
                TotalHours = g.Sum(x => x.TotalHours),
                LogCount = g.Count(),
                Percentage = totalHoursSum > 0 ? Math.Round(g.Sum(x => x.TotalHours) / totalHoursSum * 100, 2) : 0
            })
            .OrderByDescending(x => x.TotalHours)
            .ToListAsync(cancellationToken);

        return result;
    }

    public async Task<List<WorkStatisticsTaskTypeDistributionDto>> GetTaskTypeDistributionAsync(WorkStatisticsQueryDto query, CancellationToken cancellationToken = default)
    {
        var q = _context.WorkLogItems.Include(x => x.TaskType).AsQueryable();

        var data = await q
            .Where(x => x.TaskTypeId != null)
            .GroupBy(x => x.TaskTypeId)
            .Select(g => new
            {
                TaskTypeId = g.Key!.Value,
                TaskTypeName = g.First().TaskType != null ? g.First().TaskType!.TypeName : "",
                TotalHours = g.Sum(x => x.Hours ?? 0),
                LogCount = g.Count()
            })
            .OrderByDescending(x => x.TotalHours)
            .ToListAsync(cancellationToken);

        var totalHoursSum = data.Sum(x => x.TotalHours);

        return data.Select(x => new WorkStatisticsTaskTypeDistributionDto
        {
            TaskTypeId = x.TaskTypeId,
            TaskTypeName = x.TaskTypeName,
            TotalHours = x.TotalHours,
            LogCount = x.LogCount,
            Percentage = totalHoursSum > 0 ? Math.Round(x.TotalHours / totalHoursSum * 100, 2) : 0
        }).ToList();
    }

    public async Task<List<WorkStatisticsDeviceRankingDto>> GetDeviceRankingAsync(WorkStatisticsQueryDto query, CancellationToken cancellationToken = default)
    {
        var data = await _context.WorkLogItems
            .Include(x => x.Device)
            .Where(x => x.DeviceId != null)
            .GroupBy(x => x.DeviceId)
            .Select(g => new
            {
                DeviceId = g.Key!.Value,
                DeviceName = g.First().Device != null ? g.First().Device!.DeviceName : "",
                TotalHours = g.Sum(x => x.Hours ?? 0),
                LogCount = g.Count()
            })
            .OrderByDescending(x => x.TotalHours)
            .ToListAsync(cancellationToken);

        return data.Select((x, i) => new WorkStatisticsDeviceRankingDto
        {
            DeviceId = x.DeviceId,
            DeviceName = x.DeviceName,
            TotalHours = x.TotalHours,
            LogCount = x.LogCount,
            Ranking = i + 1
        }).ToList();
    }
}
