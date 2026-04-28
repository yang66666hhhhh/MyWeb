namespace WebApplication1.Dtos.Work;

public class WorkStatisticsQueryDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid? ProjectId { get; set; }
}

public class WorkStatisticsOverviewDto
{
    public int TotalLogs { get; set; }
    public decimal TotalHours { get; set; }
    public int TotalProjects { get; set; }
    public int TotalDevices { get; set; }
    public int TodayLogs { get; set; }
    public decimal TodayHours { get; set; }
    public int MissingDataCount { get; set; }
    public int PendingSupplementCount { get; set; }
}

public class WorkStatisticsDailyHoursDto
{
    public DateOnly Date { get; set; }
    public decimal Hours { get; set; }
    public int LogCount { get; set; }
}

public class WorkStatisticsProjectHoursDto
{
    public Guid ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public decimal TotalHours { get; set; }
    public int LogCount { get; set; }
    public decimal Percentage { get; set; }
}

public class WorkStatisticsTaskTypeDistributionDto
{
    public Guid TaskTypeId { get; set; }
    public string TaskTypeName { get; set; } = string.Empty;
    public decimal TotalHours { get; set; }
    public int LogCount { get; set; }
    public decimal Percentage { get; set; }
}

public class WorkStatisticsDeviceRankingDto
{
    public Guid DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public decimal TotalHours { get; set; }
    public int LogCount { get; set; }
    public int Ranking { get; set; }
}
