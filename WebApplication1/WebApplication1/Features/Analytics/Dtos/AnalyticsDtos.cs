using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Analytics.Dtos;

public class GrowthAnalyticsDto
{
    public int TotalPlans { get; set; }
    public int CompletedPlans { get; set; }
    public int TotalHabits { get; set; }
    public int CompletedHabits { get; set; }
    public decimal CompletionRate { get; set; }
    public List<DailyTrendDto> DailyTrends { get; set; } = new();
    public List<CategoryBreakdownDto> CategoryBreakdown { get; set; } = new();
}

public class WorkAnalyticsDto
{
    public decimal TotalHours { get; set; }
    public int TotalLogs { get; set; }
    public int TotalProjects { get; set; }
    public decimal AverageHoursPerDay { get; set; }
    public List<ProjectHoursDto> ProjectHours { get; set; } = new();
    public List<TaskTypeDistributionDto> TaskTypeDistribution { get; set; } = new();
}

public class TimeAnalyticsDto
{
    public int TotalFocusMinutes { get; set; }
    public int TotalTasks { get; set; }
    public decimal AverageFocusPerDay { get; set; }
    public List<HourlyDistributionDto> HourlyDistribution { get; set; } = new();
    public List<WeeklyTrendDto> WeeklyTrend { get; set; } = new();
}

public class FinanceAnalyticsDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetIncome { get; set; }
    public decimal SavingsRate { get; set; }
    public List<MonthlyFinanceDto> MonthlyTrend { get; set; } = new();
    public List<CategoryBreakdownDto> ExpenseBreakdown { get; set; } = new();
}

public class DailyTrendDto
{
    public string Date { get; set; } = string.Empty;
    public int Value { get; set; }
}

public class CategoryBreakdownDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

public class ProjectHoursDto
{
    public string ProjectId { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public decimal Hours { get; set; }
}

public class TaskTypeDistributionDto
{
    public string TaskTypeId { get; set; } = string.Empty;
    public string TaskTypeName { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public decimal Percentage { get; set; }
}

public class HourlyDistributionDto
{
    public int Hour { get; set; }
    public int Count { get; set; }
}

public class WeeklyTrendDto
{
    public string Week { get; set; } = string.Empty;
    public int FocusMinutes { get; set; }
    public int Tasks { get; set; }
}

public class MonthlyFinanceDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
}

public class AnalyticsQueryDto : PageQueryDto
{
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? GroupBy { get; set; }
}
