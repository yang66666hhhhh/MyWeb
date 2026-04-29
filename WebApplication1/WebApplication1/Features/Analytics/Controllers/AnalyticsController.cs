using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Analytics.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Analytics.Controllers;

[ApiController]
[Route("api/analytics")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    [HttpGet("growth")]
    public ActionResult<ApiResult<GrowthAnalyticsDto>> GetGrowthAnalytics([FromQuery] AnalyticsQueryDto query)
    {
        return Ok(ApiResult<GrowthAnalyticsDto>.Success(new GrowthAnalyticsDto()));
    }

    [HttpGet("work")]
    public ActionResult<ApiResult<WorkAnalyticsDto>> GetWorkAnalytics([FromQuery] AnalyticsQueryDto query)
    {
        return Ok(ApiResult<WorkAnalyticsDto>.Success(new WorkAnalyticsDto()));
    }

    [HttpGet("time")]
    public ActionResult<ApiResult<TimeAnalyticsDto>> GetTimeAnalytics([FromQuery] AnalyticsQueryDto query)
    {
        return Ok(ApiResult<TimeAnalyticsDto>.Success(new TimeAnalyticsDto()));
    }

    [HttpGet("finance")]
    public ActionResult<ApiResult<FinanceAnalyticsDto>> GetFinanceAnalytics([FromQuery] AnalyticsQueryDto query)
    {
        return Ok(ApiResult<FinanceAnalyticsDto>.Success(new FinanceAnalyticsDto()));
    }

    [HttpGet("dashboard")]
    public ActionResult<ApiResult<object>> GetDashboardSummary([FromQuery] AnalyticsQueryDto query)
    {
        return Ok(ApiResult<object>.Success(new
        {
            Growth = new GrowthAnalyticsDto(),
            Work = new WorkAnalyticsDto(),
            Time = new TimeAnalyticsDto(),
            Finance = new FinanceAnalyticsDto()
        }));
    }
}
