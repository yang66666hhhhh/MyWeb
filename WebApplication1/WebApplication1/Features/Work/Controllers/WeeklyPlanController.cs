using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Shared.Common;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Authorize]
[RequireFeature("WORK_TASK")]
[Route("api/work/weekly-plans")]
[Tags("Work - Weekly Plans")]
public class WeeklyPlanController : BaseApiController
{
    private readonly IWeeklyPlanService _weeklyPlanService;

    public WeeklyPlanController(IWeeklyPlanService weeklyPlanService)
    {
        _weeklyPlanService = weeklyPlanService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<WeeklyPlanDto>>>> GetPage(
        [FromQuery] WeeklyPlanQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await _weeklyPlanService.GetPageAsync(query, userId.Value, cancellationToken);
        return Ok(ApiResult<PageResult<WeeklyPlanDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<WeeklyPlanDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _weeklyPlanService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return NotFound(ApiResult<WeeklyPlanDto>.Fail("周计划不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WeeklyPlanDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<WeeklyPlanDto>>> Create(
        [FromBody] CreateWeeklyPlanDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        try
        {
            var result = await _weeklyPlanService.CreateAsync(input, userId.Value, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<WeeklyPlanDto>.Success(result, "创建成功"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResult<WeeklyPlanDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<WeeklyPlanDto>>> Update(
        Guid id,
        [FromBody] UpdateWeeklyPlanDto input,
        CancellationToken cancellationToken)
    {
        var result = await _weeklyPlanService.UpdateAsync(id, input, cancellationToken);
        if (result == null)
            return NotFound(ApiResult<WeeklyPlanDto>.Fail("周计划不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WeeklyPlanDto>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _weeklyPlanService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("周计划不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult.Success("删除成功"));
    }

    [HttpPost("{id:guid}/tasks")]
    public async Task<ActionResult<ApiResult<WeeklyPlanDto>>> AddTask(
        Guid id,
        [FromBody] CreateWeeklyPlanTaskDto input,
        CancellationToken cancellationToken)
    {
        var result = await _weeklyPlanService.AddTaskAsync(id, input, cancellationToken);
        if (result == null)
            return NotFound(ApiResult<WeeklyPlanDto>.Fail("周计划不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WeeklyPlanDto>.Success(result, "任务已添加"));
    }

    [HttpPut("tasks/{taskId:guid}")]
    public async Task<ActionResult<ApiResult<WeeklyPlanDto>>> UpdateTask(
        Guid taskId,
        [FromBody] UpdateWeeklyPlanTaskDto input,
        CancellationToken cancellationToken)
    {
        var result = await _weeklyPlanService.UpdateTaskAsync(taskId, input, cancellationToken);
        if (result == null)
            return NotFound(ApiResult<WeeklyPlanDto>.Fail("任务不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WeeklyPlanDto>.Success(result, "更新成功"));
    }

    [HttpDelete("tasks/{taskId:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteTask(Guid taskId, CancellationToken cancellationToken)
    {
        var deleted = await _weeklyPlanService.DeleteTaskAsync(taskId, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("任务不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult.Success("删除成功"));
    }
}
