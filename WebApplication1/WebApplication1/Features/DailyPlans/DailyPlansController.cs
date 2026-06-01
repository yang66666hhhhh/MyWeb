using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.DailyPlans;

[ApiController]
[Authorize]
[RequireFeature("GROWTH_DAILY_PLAN")]
[Route("api/growth/daily-plans")]
[Tags("Daily Plans")]
public class DailyPlansController(IDailyPlanService dailyPlanService, ILogger<DailyPlansController> logger) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<DailyPlanDto>>>> GetPage(
        [FromQuery] DailyPlanQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await dailyPlanService.GetPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<DailyPlanDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<DailyPlanDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await dailyPlanService.GetByIdAsync(id, userId, cancellationToken);
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
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));
            }

            var result = await dailyPlanService.CreateAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建每日计划成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<DailyPlanDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建每日计划失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<DailyPlanDto>>> Update(
        Guid id,
        [FromBody] UpdateDailyPlanDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await dailyPlanService.UpdateAsync(id, input, userId, cancellationToken);
            if (result is null)
            {
                return NotFound(ApiResult.Fail("每日计划不存在", StatusCodes.Status404NotFound));
            }

            logger.LogInformation("更新每日计划成功: {Id}", id);
            return Ok(ApiResult<DailyPlanDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新每日计划失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpPatch("{id:guid}/complete")]
    public async Task<ActionResult<ApiResult<DailyPlanDto>>> Complete(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await dailyPlanService.CompleteAsync(id, userId, cancellationToken);
        if (result is null)
        {
            return NotFound(ApiResult.Fail("每日计划不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult<DailyPlanDto>.Success(result, "已完成"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var deleted = await dailyPlanService.DeleteAsync(id, userId, cancellationToken);
            if (!deleted)
            {
                return NotFound(ApiResult.Fail("每日计划不存在", StatusCodes.Status404NotFound));
            }

            logger.LogInformation("删除每日计划成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除每日计划失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }
}
