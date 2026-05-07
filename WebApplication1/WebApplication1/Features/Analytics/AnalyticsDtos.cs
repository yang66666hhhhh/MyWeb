namespace WebApplication1.Features.Analytics;

public class AnalyticsQueryDto
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
}

public class DashboardOverviewDto
{
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public decimal CompletionRate { get; set; }
    public int TotalWorkLogs { get; set; }
    public decimal TotalWorkHours { get; set; }
    public int TodayTasks { get; set; }
    public int TodayCompletedTasks { get; set; }
    public int TodayWorkHours { get; set; }
}

public class TaskTrendDto
{
    public DateOnly Date { get; set; }
    public int Created { get; set; }
    public int Completed { get; set; }
    public decimal CompletionRate { get; set; }
}

public class TaskDistributionDto
{
    public string Type { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

public class WorkVsGrowthDto
{
    public string Category { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public decimal Hours { get; set; }
}

public class TaskPriorityDistributionDto
{
    public string Priority { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

public class ProductivityByDayDto
{
    public string DayOfWeek { get; set; } = string.Empty;
    public decimal AverageHours { get; set; }
    public int TaskCount { get; set; }
}