using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Analytics;

[ApiController]
[Authorize]
[RequireFeature("ANALYTICS_GROWTH")]
[Route("api/growth/analytics")]
[Tags("Analytics")]
public class AnalyticsController(IAnalyticsService analyticsService) : BaseApiController
{
    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResult<DashboardOverviewDto>>> GetDashboard(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await analyticsService.GetDashboardOverviewAsync(userId.Value, cancellationToken);
        return Ok(ApiResult<DashboardOverviewDto>.Success(result));
    }

    [HttpGet("task-trends")]
    public async Task<ActionResult<ApiResult<List<TaskTrendDto>>>> GetTaskTrends(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await analyticsService.GetTaskTrendsAsync(userId.Value, startDate, endDate, cancellationToken);
        return Ok(ApiResult<List<TaskTrendDto>>.Success(result));
    }

    [HttpGet("task-distribution")]
    public async Task<ActionResult<ApiResult<List<TaskDistributionDto>>>> GetTaskDistribution(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await analyticsService.GetTaskDistributionByTypeAsync(userId.Value, cancellationToken);
        return Ok(ApiResult<List<TaskDistributionDto>>.Success(result));
    }

    [HttpGet("work-vs-growth")]
    public async Task<ActionResult<ApiResult<List<WorkVsGrowthDto>>>> GetWorkVsGrowth(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await analyticsService.GetWorkVsGrowthAsync(userId.Value, cancellationToken);
        return Ok(ApiResult<List<WorkVsGrowthDto>>.Success(result));
    }

    [HttpGet("priority-distribution")]
    public async Task<ActionResult<ApiResult<List<TaskPriorityDistributionDto>>>> GetPriorityDistribution(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await analyticsService.GetTaskPriorityDistributionAsync(userId.Value, cancellationToken);
        return Ok(ApiResult<List<TaskPriorityDistributionDto>>.Success(result));
    }
}
