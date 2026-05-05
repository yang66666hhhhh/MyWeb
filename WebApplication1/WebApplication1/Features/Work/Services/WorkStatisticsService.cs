using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services.Interfaces;

namespace WebApplication1.Features.Work.Services;

public class WorkStatisticsService : IWorkStatisticsService
{
    private readonly AppDbContext _context;
    private readonly ILogger<WorkStatisticsService> _logger;

    public WorkStatisticsService(AppDbContext context, ILogger<WorkStatisticsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<WorkStatisticsOverviewDto> GetOverviewAsync(WorkStatisticsQueryDto query, CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var logsQuery = _context.WorkLogs.AsNoTracking().AsQueryable();
        if (query.StartDate.HasValue)
            logsQuery = logsQuery.Where(x => x.WorkDate >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            logsQuery = logsQuery.Where(x => x.WorkDate <= query.EndDate.Value);
        if (query.ProjectId.HasValue)
            logsQuery = logsQuery.Where(x => x.ProjectId == query.ProjectId.Value);

        var stats = await logsQuery
            .GroupBy(_ => 1)
            .Select(g => new
            {
                TotalLogs = g.Count(),
                TotalHours = g.Sum(x => x.TotalHours),
                TodayLogs = g.Count(x => x.WorkDate == today),
                TodayHours = g.Where(x => x.WorkDate == today).Sum(x => x.TotalHours),
                MissingCount = g.Count(x => x.Status == Shared.Enums.WorkLogStatus.MissingData),
                PendingCount = g.Count(x => x.Status == Shared.Enums.WorkLogStatus.PendingSupplement)
            })
            .FirstOrDefaultAsync(cancellationToken);

        var totalProjects = await _context.WorkProjects.AsNoTracking().CountAsync(cancellationToken);
        var totalDevices = await _context.WorkDevices.AsNoTracking().CountAsync(cancellationToken);

        return new WorkStatisticsOverviewDto
        {
            TotalLogs = stats?.TotalLogs ?? 0,
            TotalHours = stats?.TotalHours ?? 0,
            TotalProjects = totalProjects,
            TotalDevices = totalDevices,
            TodayLogs = stats?.TodayLogs ?? 0,
            TodayHours = stats?.TodayHours ?? 0,
            MissingDataCount = stats?.MissingCount ?? 0,
            PendingSupplementCount = stats?.PendingCount ?? 0
        };
    }

    public async Task<List<WorkStatisticsDailyHoursDto>> GetDailyHoursAsync(WorkStatisticsQueryDto query, CancellationToken cancellationToken = default)
    {
        var startDate = query.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        var endDate = query.EndDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var q = _context.WorkLogs.AsNoTracking().Where(x => x.WorkDate >= startDate && x.WorkDate <= endDate);

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
        var q = _context.WorkLogs.Include(x => x.Project).AsNoTracking().AsQueryable();

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
        var q = _context.WorkLogItems.Include(x => x.TaskType).AsNoTracking().AsQueryable();

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
            .AsNoTracking()
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
