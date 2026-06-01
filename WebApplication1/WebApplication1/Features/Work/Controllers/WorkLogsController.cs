using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Shared.Common;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services.Interfaces;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Authorize]
[RequireFeature("WORK_LOG")]
[Route("api/work/logs")]
[Tags("Work - Logs")]
public class WorkLogsController(IWorkLogService logService, ILogger<WorkLogsController> logger) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<WorkLogDto>>>> GetPage(
        [FromQuery] WorkLogQueryDto query,
        [FromQuery] Guid? userId,
        CancellationToken cancellationToken)
    {
        if (IsProOrAbove() && userId.HasValue)
        {
            var result = await logService.GetPageAsync(query, userId.Value, cancellationToken);
            return Ok(ApiResult<PageResult<WorkLogDto>>.Success(result));
        }
        var currentUserId = GetCurrentUserId();
        var result2 = await logService.GetPageAsync(query, currentUserId, cancellationToken);
        return Ok(ApiResult<PageResult<WorkLogDto>>.Success(result2));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkLogDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await logService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return NotFound(ApiResult.Fail("工作日志不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId)
        {
            return NotFound(ApiResult.Fail("工作日志不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult<WorkLogDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<WorkLogDto>>> Create(
        [FromBody] CreateWorkLogDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await logService.CreateAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建工作日志成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<WorkLogDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建工作日志失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkLogDto>>> Update(
        Guid id,
        [FromBody] UpdateWorkLogDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await logService.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                return NotFound(ApiResult.Fail("工作日志不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此日志"));
            }

            var result = await logService.UpdateAsync(id, input, cancellationToken);
            if (result == null)
                return NotFound(ApiResult.Fail("工作日志不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新工作日志成功: {Id}", id);
            return Ok(ApiResult<WorkLogDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新工作日志失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await logService.GetByIdAsync(id, cancellationToken);
            if (existing == null)
                return NotFound(ApiResult.Fail("工作日志不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此日志"));
            }

            var deleted = await logService.DeleteAsync(id, cancellationToken);
            if (!deleted)
                return NotFound(ApiResult.Fail("工作日志不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除工作日志成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除工作日志失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }
}
