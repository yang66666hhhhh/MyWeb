using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Tasks;

[ApiController]
[Authorize]
[Route("api/tasks")]
[Tags("Tasks")]
public class TasksController(ITaskItemService taskService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<TaskItemDto>>>> GetPage(
        [FromQuery] TaskItemQueryDto query,
        [FromQuery] Guid? userId,
        CancellationToken cancellationToken)
    {
        if (IsProOrAbove() && userId.HasValue)
        {
            var result = await taskService.GetPageAsync(query, userId.Value, cancellationToken);
            return Ok(ApiResult<PageResult<TaskItemDto>>.Success(result));
        }
        var currentUserId = GetCurrentUserId();
        var result2 = await taskService.GetPageAsync(query, currentUserId, cancellationToken);
        return Ok(ApiResult<PageResult<TaskItemDto>>.Success(result2));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<TaskItemDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await taskService.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound(ApiResult<TaskItemDto>.Fail("任务不存在", StatusCodes.Status404NotFound));
        }

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId)
        {
            return NotFound(ApiResult<TaskItemDto>.Fail("任务不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult<TaskItemDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<TaskItemDto>>> Create(
        [FromBody] CreateTaskItemDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await taskService.CreateAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<TaskItemDto>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<TaskItemDto>>> Update(
        Guid id,
        [FromBody] UpdateTaskItemDto input,
        CancellationToken cancellationToken)
    {
        var existing = await taskService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound(ApiResult<TaskItemDto>.Fail("任务不存在", StatusCodes.Status404NotFound));
        }

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此任务"));
        }

        var result = await taskService.UpdateAsync(id, input, cancellationToken);
        if (result is null)
        {
            return NotFound(ApiResult<TaskItemDto>.Fail("任务不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult<TaskItemDto>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var existing = await taskService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return NotFound(ApiResult.Fail("任务不存在", StatusCodes.Status404NotFound));
        }

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此任务"));
        }

        var deleted = await taskService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResult.Fail("任务不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult.Success("删除成功"));
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult<ApiResult<TaskItemDto>>> Complete(Guid id, CancellationToken cancellationToken)
    {
        var result = await taskService.CompleteAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound(ApiResult<TaskItemDto>.Fail("任务不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult<TaskItemDto>.Success(result, "已完成"));
    }

    [HttpPost("convert-to-log")]
    public async Task<ActionResult<ApiResult<object>>> ConvertToLog(
        [FromBody] ConvertTaskToLogDto input,
        CancellationToken cancellationToken)
    {
        var workLogId = await taskService.ConvertToWorkLogAsync(input, cancellationToken);
        if (!workLogId.HasValue)
        {
            return NotFound(ApiResult.Fail("任务不存在"));
        }

        return Ok(ApiResult<object>.Success(new { workLogId }, "已转换为工作日志"));
    }
}
