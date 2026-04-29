using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.DailyPlans;

[ApiController]
[Authorize]
[Route("api/daily-plans")]
public class DailyPlansController(IDailyPlanService dailyPlanService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<DailyPlanDto>>>> GetPage(
        [FromQuery] DailyPlanQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await dailyPlanService.GetPageAsync(query, cancellationToken);
        return Ok(ApiResult<PageResult<DailyPlanDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<DailyPlanDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await dailyPlanService.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound(ApiResult.Fail("每日计划不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult<DailyPlanDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<DailyPlanDto>>> Create(
        [FromBody] CreateDailyPlanDto input,
        CancellationToken cancellationToken)
    {
        var result = await dailyPlanService.CreateAsync(input, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<DailyPlanDto>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<DailyPlanDto>>> Update(
        Guid id,
        [FromBody] UpdateDailyPlanDto input,
        CancellationToken cancellationToken)
    {
        var result = await dailyPlanService.UpdateAsync(id, input, cancellationToken);
        if (result is null)
        {
            return NotFound(ApiResult.Fail("每日计划不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult<DailyPlanDto>.Success(result, "更新成功"));
    }

    [HttpPatch("{id:guid}/complete")]
    public async Task<ActionResult<ApiResult<DailyPlanDto>>> Complete(Guid id, CancellationToken cancellationToken)
    {
        var result = await dailyPlanService.CompleteAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound(ApiResult.Fail("每日计划不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult<DailyPlanDto>.Success(result, "已完成"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await dailyPlanService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResult.Fail("每日计划不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult.Success("删除成功"));
    }
}
