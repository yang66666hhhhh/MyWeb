using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common;
using WebApplication1.Dtos.Work;
using WebApplication1.Services.Interfaces.IWork;

namespace WebApplication1.Controllers;

[ApiController]
[Authorize]
[Route("api/work/statistics")]
public class WorkStatisticsController(IWorkStatisticsService statisticsService) : ControllerBase
{
    [HttpGet("overview")]
    public async Task<ActionResult<ApiResult<WorkStatisticsOverviewDto>>> GetOverview(
        [FromQuery] WorkStatisticsQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await statisticsService.GetOverviewAsync(query, cancellationToken);
        return Ok(ApiResult<WorkStatisticsOverviewDto>.Success(result));
    }

    [HttpGet("daily-hours")]
    public async Task<ActionResult<ApiResult<List<WorkStatisticsDailyHoursDto>>>> GetDailyHours(
        [FromQuery] WorkStatisticsQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await statisticsService.GetDailyHoursAsync(query, cancellationToken);
        return Ok(ApiResult<List<WorkStatisticsDailyHoursDto>>.Success(result));
    }

    [HttpGet("project-hours")]
    public async Task<ActionResult<ApiResult<List<WorkStatisticsProjectHoursDto>>>> GetProjectHours(
        [FromQuery] WorkStatisticsQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await statisticsService.GetProjectHoursAsync(query, cancellationToken);
        return Ok(ApiResult<List<WorkStatisticsProjectHoursDto>>.Success(result));
    }

    [HttpGet("task-type-distribution")]
    public async Task<ActionResult<ApiResult<List<WorkStatisticsTaskTypeDistributionDto>>>> GetTaskTypeDistribution(
        [FromQuery] WorkStatisticsQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await statisticsService.GetTaskTypeDistributionAsync(query, cancellationToken);
        return Ok(ApiResult<List<WorkStatisticsTaskTypeDistributionDto>>.Success(result));
    }

    [HttpGet("device-ranking")]
    public async Task<ActionResult<ApiResult<List<WorkStatisticsDeviceRankingDto>>>> GetDeviceRanking(
        [FromQuery] WorkStatisticsQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await statisticsService.GetDeviceRankingAsync(query, cancellationToken);
        return Ok(ApiResult<List<WorkStatisticsDeviceRankingDto>>.Success(result));
    }
}
