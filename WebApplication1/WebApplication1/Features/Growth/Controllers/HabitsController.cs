using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Features.Growth.Services.Interfaces;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Controllers;

[ApiController]
[Authorize]
[RequireFeature("GROWTH_HABIT")]
[Route("api/growth/habits")]
[Tags("Growth - Habits")]
public class HabitsController(IHabitService habitService, ILogger<HabitsController> logger) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<HabitDto>>>> GetPage(
        [FromQuery] HabitQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await habitService.GetPageAsync(query, userId, cancellationToken);
            return Ok(ApiResult<PageResult<HabitDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取习惯列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<HabitDetailDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await habitService.GetByIdAsync(id, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<HabitDetailDto>.Fail("习惯不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && result.UserId != currentUserId)
                return NotFound(ApiResult<HabitDetailDto>.Fail("习惯不存在", StatusCodes.Status404NotFound));

            return Ok(ApiResult<HabitDetailDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取习惯详情失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<HabitDto>>> Create(
        [FromBody] CreateHabitDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await habitService.CreateAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建习惯成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<HabitDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建习惯失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<HabitDto>>> Update(
        Guid id,
        [FromBody] UpdateHabitDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await habitService.GetByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult<HabitDto>.Fail("习惯不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId)
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此习惯"));

            var result = await habitService.UpdateAsync(id, input, cancellationToken);
            if (result is null)
                return NotFound(ApiResult.Fail("习惯不存在"));
            logger.LogInformation("更新习惯成功: {Id}", id);
            return Ok(ApiResult<HabitDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新习惯失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await habitService.GetByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult.Fail("习惯不存在"));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId)
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此习惯"));

            var deleted = await habitService.DeleteAsync(id, cancellationToken);
            logger.LogInformation("删除习惯成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除习惯失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpPost("{id:guid}/check-in")]
    public async Task<ActionResult<ApiResult<HabitDto>>> CheckIn(
        Guid id,
        [FromBody] CheckInDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await habitService.GetByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult<HabitDto>.Fail("习惯不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId)
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限打卡"));

            var result = await habitService.CheckInAsync(id, input, cancellationToken);
            if (result is null)
                return NotFound(ApiResult.Fail("习惯不存在"));
            logger.LogInformation("习惯打卡成功: {Id}", id);
            return Ok(ApiResult<HabitDto>.Success(result, "打卡成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "习惯打卡失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("打卡失败，请稍后重试"));
        }
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<ApiResult<HabitDto>>> UpdateStatus(
        Guid id,
        [FromBody] UpdateHabitDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await habitService.GetByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult<HabitDto>.Fail("习惯不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId)
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改"));

            if (!input.Status.HasValue)
                return BadRequest(ApiResult.Fail("状态值不能为空"));

            var result = await habitService.UpdateStatusAsync(id, input.Status.Value, cancellationToken);
            if (result is null)
                return NotFound(ApiResult.Fail("习惯不存在"));
            logger.LogInformation("更新习惯状态成功: {Id}", id);
            return Ok(ApiResult<HabitDto>.Success(result, "状态更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新习惯状态失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }
}
