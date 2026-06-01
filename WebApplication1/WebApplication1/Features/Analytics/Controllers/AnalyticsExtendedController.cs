using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Analytics.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Analytics;

[ApiController]
[Authorize]
[Route("api/analytics")]
[Tags("Analytics")]
public class AnalyticsExtendedController(IAnalyticsExtendedService service) : BaseApiController
{
    [HttpGet("time/overview")]
    public async Task<ActionResult<ApiResult<TimeAnalyticsOverviewDto>>> GetTimeOverview(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await service.GetTimeAnalyticsAsync(userId.Value, ct);
        return Ok(ApiResult<TimeAnalyticsOverviewDto>.Success(result));
    }

    [HttpGet("time/hourly-distribution")]
    public async Task<ActionResult<ApiResult<List<HourlyDistributionDto>>>> GetHourlyDistribution(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await service.GetHourlyDistributionAsync(userId.Value, ct);
        return Ok(ApiResult<List<HourlyDistributionDto>>.Success(result));
    }

    [HttpGet("time/weekly-trend")]
    public async Task<ActionResult<ApiResult<List<WeeklyTrendDto>>>> GetWeeklyTrend(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await service.GetWeeklyTrendAsync(userId.Value, ct);
        return Ok(ApiResult<List<WeeklyTrendDto>>.Success(result));
    }

    [HttpGet("habits/overview")]
    public async Task<ActionResult<ApiResult<HabitsAnalyticsOverviewDto>>> GetHabitsOverview(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await service.GetHabitsAnalyticsAsync(userId.Value, ct);
        return Ok(ApiResult<HabitsAnalyticsOverviewDto>.Success(result));
    }

    [HttpGet("habits/trends")]
    public async Task<ActionResult<ApiResult<List<HabitTrendDto>>>> GetHabitTrends(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await service.GetHabitTrendsAsync(userId.Value, ct);
        return Ok(ApiResult<List<HabitTrendDto>>.Success(result));
    }

    [HttpGet("finance/overview")]
    public async Task<ActionResult<ApiResult<FinanceAnalyticsOverviewDto>>> GetFinanceOverview(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await service.GetFinanceAnalyticsAsync(userId.Value, ct);
        return Ok(ApiResult<FinanceAnalyticsOverviewDto>.Success(result));
    }

    [HttpGet("finance/monthly-trend")]
    public async Task<ActionResult<ApiResult<List<MonthlyFinanceTrendDto>>>> GetMonthlyFinanceTrend(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await service.GetMonthlyFinanceTrendAsync(userId.Value, ct);
        return Ok(ApiResult<List<MonthlyFinanceTrendDto>>.Success(result));
    }

    [HttpGet("finance/expense-breakdown")]
    public async Task<ActionResult<ApiResult<List<ExpenseBreakdownDto>>>> GetExpenseBreakdown(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await service.GetExpenseBreakdownAsync(userId.Value, ct);
        return Ok(ApiResult<List<ExpenseBreakdownDto>>.Success(result));
    }

    [HttpGet("reports")]
    public async Task<ActionResult<ApiResult<PageResult<CustomReportDto>>>> GetCustomReports(
        [FromQuery] AnalyticsQueryDto query, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var isPro = IsProOrAbove();
        var result = await service.GetCustomReportsAsync(query, userId.Value, isPro, ct);
        return Ok(ApiResult<PageResult<CustomReportDto>>.Success(result));
    }

    [HttpPost("reports")]
    public async Task<ActionResult<ApiResult<CustomReportDto>>> CreateCustomReport(
        [FromBody] CreateCustomReportInput input, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await service.CreateCustomReportAsync(input, userId.Value, ct);
        return Ok(ApiResult<CustomReportDto>.Success(result));
    }

    [HttpDelete("reports/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteCustomReport(Guid id, CancellationToken ct)
    {
        var success = await service.DeleteCustomReportAsync(id, ct);
        return HandleDeleteResult(success, "报告");
    }

    [HttpGet("ai-insights")]
    public async Task<ActionResult<ApiResult<PageResult<AiInsightDto>>>> GetAiInsights(
        [FromQuery] AnalyticsQueryDto query, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await service.GetAiInsightsAsync(query, userId.Value, ct);
        return Ok(ApiResult<PageResult<AiInsightDto>>.Success(result));
    }

    [HttpPost("ai-insights/generate")]
    public async Task<ActionResult<ApiResult<AiInsightDto>>> GenerateAiInsight(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await service.GenerateAiInsightAsync(userId.Value, ct);
        return Ok(ApiResult<AiInsightDto>.Success(result));
    }
}

public record TimeAnalyticsOverviewDto
{
    public double DailyWorkHours { get; init; }
    public double DailyLearningHours { get; init; }
    public double DailyRestHours { get; init; }
    public double TimeUtilizationRate { get; init; }
}

public record HourlyDistributionDto
{
    public int Hour { get; init; }
    public double Value { get; init; }
    public string Category { get; init; } = string.Empty;
}

public record WeeklyTrendDto
{
    public string Date { get; init; } = string.Empty;
    public double WorkHours { get; init; }
    public double LearningHours { get; init; }
}

public record HabitsAnalyticsOverviewDto
{
    public int ActiveHabits { get; init; }
    public int MonthlyCheckIns { get; init; }
    public int LongestStreak { get; init; }
    public double CompletionRate { get; init; }
}

public record HabitTrendDto
{
    public string Date { get; init; } = string.Empty;
    public int CheckIns { get; init; }
    public double CompletionRate { get; init; }
}

public record FinanceAnalyticsOverviewDto
{
    public decimal MonthlyBalance { get; init; }
    public double SavingsRate { get; init; }
    public decimal InvestmentReturn { get; init; }
    public double BudgetExecution { get; init; }
}

public record MonthlyFinanceTrendDto
{
    public string Month { get; init; } = string.Empty;
    public decimal Income { get; init; }
    public decimal Expense { get; init; }
}

public record ExpenseBreakdownDto
{
    public string Category { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public double Percentage { get; init; }
}

public record CustomReportDto
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Type { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateCustomReportInput
{
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Type { get; init; }
}

public record AiInsightDto
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Content { get; init; }
    public string? Category { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}
