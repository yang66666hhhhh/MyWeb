using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Analytics.Dtos;
using WebApplication1.Features.Analytics.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Analytics;

public interface IAnalyticsExtendedService
{
    Task<TimeAnalyticsOverviewDto> GetTimeAnalyticsAsync(Guid userId, CancellationToken ct = default);
    Task<List<HourlyDistributionDto>> GetHourlyDistributionAsync(Guid userId, CancellationToken ct = default);
    Task<List<WeeklyTrendDto>> GetWeeklyTrendAsync(Guid userId, CancellationToken ct = default);
    Task<HabitsAnalyticsOverviewDto> GetHabitsAnalyticsAsync(Guid userId, CancellationToken ct = default);
    Task<List<HabitTrendDto>> GetHabitTrendsAsync(Guid userId, CancellationToken ct = default);
    Task<FinanceAnalyticsOverviewDto> GetFinanceAnalyticsAsync(Guid userId, CancellationToken ct = default);
    Task<List<MonthlyFinanceTrendDto>> GetMonthlyFinanceTrendAsync(Guid userId, CancellationToken ct = default);
    Task<List<ExpenseBreakdownDto>> GetExpenseBreakdownAsync(Guid userId, CancellationToken ct = default);
    Task<PageResult<CustomReportDto>> GetCustomReportsAsync(AnalyticsQueryDto query, Guid userId, bool isPro, CancellationToken ct = default);
    Task<CustomReportDto?> GetCustomReportByIdAsync(Guid id, CancellationToken ct = default);
    Task<CustomReportDto> CreateCustomReportAsync(CreateCustomReportInput input, Guid userId, CancellationToken ct = default);
    Task<bool> DeleteCustomReportAsync(Guid id, CancellationToken ct = default);
}

public class AnalyticsExtendedService(AppDbContext context) : IAnalyticsExtendedService
{
    private static string FormatDate(DateOnly d) => d.ToString("yyyy-MM-dd");
    private static string FormatDate(DateTime d) => d.ToString("yyyy-MM-dd");

    public async Task<TimeAnalyticsOverviewDto> GetTimeAnalyticsAsync(Guid userId, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var thirtyDaysAgo = today.AddDays(-30);

        var stats = await context.WorkLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.WorkDate >= thirtyDaysAgo)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                TotalHours = g.Sum(x => x.TotalHours),
                Days = g.Select(x => x.WorkDate).Distinct().Count()
            })
            .FirstOrDefaultAsync(ct);

        var totalHours = (double)(stats?.TotalHours ?? 0);
        var days = stats?.Days ?? 1;
        var dailyWork = days > 0 ? Math.Round(totalHours / days, 1) : 0;

        // 从实际学习记录计算学习时长
        var monthStart = new DateOnly(today.Year, today.Month, 1);
        var totalStudyMinutes = await context.StudentStudyRecords
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.RecordDate >= monthStart)
            .SumAsync(x => x.DurationMinutes, ct);
        var studyDays = await context.StudentStudyRecords
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.RecordDate >= monthStart)
            .Select(x => x.RecordDate)
            .Distinct()
            .CountAsync(ct);
        var dailyLearning = studyDays > 0 ? Math.Round(totalStudyMinutes / 60.0 / studyDays, 1) : 0;

        return new TimeAnalyticsOverviewDto
        {
            DailyWorkHours = dailyWork,
            DailyLearningHours = dailyLearning,
            DailyRestHours = Math.Max(24 - dailyWork - dailyLearning, 0),
            TimeUtilizationRate = Math.Round((dailyWork + dailyLearning) / 24 * 100, 0)
        };
    }

    public async Task<List<HourlyDistributionDto>> GetHourlyDistributionAsync(Guid userId, CancellationToken ct = default)
    {
        var logs = await context.WorkLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new { x.WorkDate, x.TotalHours })
            .ToListAsync(ct);

        return logs
            .GroupBy(x => x.WorkDate.Day % 24)
            .Select(g => new HourlyDistributionDto
            {
                Hour = g.Key,
                Value = Math.Round((double)g.Sum(x => x.TotalHours), 1),
                Category = g.Key < 12 ? "上午" : g.Key < 18 ? "下午" : "晚上"
            })
            .OrderBy(x => x.Hour)
            .ToList();
    }

    public async Task<List<WeeklyTrendDto>> GetWeeklyTrendAsync(Guid userId, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var start = today.AddDays(-42);

        var logs = await context.WorkLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.WorkDate >= start)
            .GroupBy(x => x.WorkDate)
            .Select(g => new { Date = g.Key, Hours = g.Sum(x => x.TotalHours) })
            .OrderBy(x => x.Date)
            .ToListAsync(ct);

        // 获取实际学习记录
        var studyRecords = await context.StudentStudyRecords
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.RecordDate >= start)
            .GroupBy(x => x.RecordDate)
            .Select(g => new { Date = g.Key, Minutes = g.Sum(x => x.DurationMinutes) })
            .ToListAsync(ct);
        var studyDict = studyRecords.ToDictionary(x => x.Date, x => Math.Round(x.Minutes / 60.0, 1));

        return logs.Select(x => new WeeklyTrendDto
        {
            Date = FormatDate(x.Date),
            WorkHours = (double)x.Hours,
            LearningHours = studyDict.GetValueOrDefault(x.Date, 0)
        }).ToList();
    }

    public async Task<HabitsAnalyticsOverviewDto> GetHabitsAnalyticsAsync(Guid userId, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var monthStart = new DateOnly(today.Year, today.Month, 1);

        var habits = await context.Habits
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new { x.Id, x.Status, x.LongestStreak })
            .ToListAsync(ct);

        var activeHabits = habits.Count(x => x.Status == 1);
        var longestStreak = habits.Any() ? habits.Max(x => x.LongestStreak) : 0;

        var habitIds = habits.Select(x => x.Id).ToList();
        var monthlyCheckIns = await context.HabitCheckIns
            .AsNoTracking()
            .Where(x => habitIds.Contains(x.HabitId) && x.CheckInDate >= monthStart)
            .CountAsync(ct);

        var totalExpected = activeHabits * (today.Day);
        var completionRate = totalExpected > 0 ? Math.Round((double)monthlyCheckIns / totalExpected * 100, 0) : 0;

        return new HabitsAnalyticsOverviewDto
        {
            ActiveHabits = activeHabits,
            MonthlyCheckIns = monthlyCheckIns,
            LongestStreak = longestStreak,
            CompletionRate = completionRate
        };
    }

    public async Task<List<HabitTrendDto>> GetHabitTrendsAsync(Guid userId, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var start = today.AddDays(-30);

        var habitIds = await context.Habits
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.Id)
            .ToListAsync(ct);

        var checkIns = await context.HabitCheckIns
            .AsNoTracking()
            .Where(x => habitIds.Contains(x.HabitId) && x.CheckInDate >= start)
            .GroupBy(x => x.CheckInDate)
            .Select(g => new { Date = g.Key, Count = g.Count() })
            .OrderBy(x => x.Date)
            .ToListAsync(ct);

        var activeCount = habitIds.Count;
        return checkIns.Select(x => new HabitTrendDto
        {
            Date = FormatDate(x.Date),
            CheckIns = x.Count,
            CompletionRate = activeCount > 0 ? Math.Round((double)x.Count / activeCount * 100, 0) : 0
        }).ToList();
    }

    public async Task<FinanceAnalyticsOverviewDto> GetFinanceAnalyticsAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var monthStart = $"{now.Year}-{now.Month:D2}";

        var incomes = await context.Incomes
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IncomeDate.StartsWith(monthStart))
            .SumAsync(x => x.Amount, ct);

        var expenses = await context.Expenses
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.ExpenseDate.StartsWith(monthStart))
            .SumAsync(x => x.Amount, ct);

        var balance = incomes - expenses;
        var savingsRate = incomes > 0 ? Math.Round((double)(balance / incomes * 100), 0) : 0;

        return new FinanceAnalyticsOverviewDto
        {
            MonthlyBalance = balance,
            SavingsRate = savingsRate,
            InvestmentReturn = 0,
            BudgetExecution = 0
        };
    }

    public async Task<List<MonthlyFinanceTrendDto>> GetMonthlyFinanceTrendAsync(Guid userId, CancellationToken ct = default)
    {
        var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6).ToString("yyyy-MM");

        var incomes = await context.Incomes
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IncomeDate.CompareTo(sixMonthsAgo) >= 0)
            .Select(x => new { x.IncomeDate, x.Amount })
            .ToListAsync(ct);

        var expenses = await context.Expenses
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.ExpenseDate.CompareTo(sixMonthsAgo) >= 0)
            .Select(x => new { x.ExpenseDate, x.Amount })
            .ToListAsync(ct);

        var incomeByMonth = incomes
            .GroupBy(x => x.IncomeDate.Length >= 7 ? x.IncomeDate[..7] : x.IncomeDate)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

        var expenseByMonth = expenses
            .GroupBy(x => x.ExpenseDate.Length >= 7 ? x.ExpenseDate[..7] : x.ExpenseDate)
            .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

        var months = incomeByMonth.Keys.Union(expenseByMonth.Keys).OrderBy(m => m);
        return months.Select(m => new MonthlyFinanceTrendDto
        {
            Month = m,
            Income = incomeByMonth.GetValueOrDefault(m),
            Expense = expenseByMonth.GetValueOrDefault(m)
        }).ToList();
    }

    public async Task<List<ExpenseBreakdownDto>> GetExpenseBreakdownAsync(Guid userId, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var monthStart = $"{now.Year}-{now.Month:D2}";

        var expenses = await context.Expenses
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.ExpenseDate.StartsWith(monthStart))
            .GroupBy(x => x.Category)
            .Select(g => new { Category = g.Key, Amount = g.Sum(x => x.Amount) })
            .OrderByDescending(x => x.Amount)
            .ToListAsync(ct);

        var total = expenses.Sum(x => x.Amount);
        return expenses.Select(x => new ExpenseBreakdownDto
        {
            Category = x.Category,
            Amount = x.Amount,
            Percentage = total > 0 ? Math.Round((double)(x.Amount / total * 100), 1) : 0
        }).ToList();
    }

    public async Task<PageResult<CustomReportDto>> GetCustomReportsAsync(AnalyticsQueryDto query, Guid userId, bool isPro, CancellationToken ct = default)
    {
        var q = context.CustomReports.AsNoTracking().AsQueryable();
        if (!isPro) q = q.Where(x => x.UserId == userId);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(x => new CustomReportDto
            {
                Id = x.Id.ToString(), Title = x.Name, Description = x.Description,
                Type = x.Tags, CreatedAt = x.CreatedAt.ToString()
            }).ToListAsync(ct);

        return PageResult<CustomReportDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<CustomReportDto?> GetCustomReportByIdAsync(Guid id, CancellationToken ct = default)
    {
        var x = await context.CustomReports.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (x == null) return null;
        return new CustomReportDto
        {
            Id = x.Id.ToString(), Title = x.Name, Description = x.Description,
            Type = x.Tags, CreatedAt = x.CreatedAt.ToString()
        };
    }

    public async Task<CustomReportDto> CreateCustomReportAsync(CreateCustomReportInput input, Guid userId, CancellationToken ct = default)
    {
        var entity = new CustomReport
        {
            UserId = userId, Name = input.Title, Description = input.Description,
            Tags = input.Type
        };
        context.CustomReports.Add(entity);
        await context.SaveChangesAsync(ct);
        return new CustomReportDto
        {
            Id = entity.Id.ToString(), Title = entity.Name, Description = entity.Description,
            Type = entity.Tags, CreatedAt = entity.CreatedAt.ToString()
        };
    }

    public async Task<bool> DeleteCustomReportAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await context.CustomReports.FindAsync([id], ct);
        if (entity == null) return false;
        context.CustomReports.Remove(entity);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
