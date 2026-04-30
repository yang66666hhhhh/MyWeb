using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Shared.Common;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services.Interfaces;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Authorize]
[Route("api/work/logs")]
public class WorkLogsController(IWorkLogService logService) : ControllerBase
{
    private Guid? GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private bool IsAdmin()
    {
        return User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Any(c => c.Value.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                       c.Value.Equals("super", StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<WorkLogDto>>>> GetPage(
        [FromQuery] WorkLogQueryDto query,
        [FromQuery] Guid? userId,
        CancellationToken cancellationToken)
    {
        if (IsAdmin() && userId.HasValue)
        {
            var result = await logService.GetPageAsync(query, userId.Value, cancellationToken);
            return Ok(ApiResult<PageResult<WorkLogDto>>.Success(result));
        }
        var currentUserId = GetUserIdFromClaims();
        var result2 = await logService.GetPageAsync(query, currentUserId, cancellationToken);
        return Ok(ApiResult<PageResult<WorkLogDto>>.Success(result2));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkLogDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await logService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return NotFound(ApiResult.Fail("工作日志不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetUserIdFromClaims();
        if (!IsAdmin() && result.UserId != currentUserId)
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
        var userId = GetUserIdFromClaims();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await logService.CreateAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<WorkLogDto>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkLogDto>>> Update(
        Guid id,
        [FromBody] UpdateWorkLogDto input,
        CancellationToken cancellationToken)
    {
        var existing = await logService.GetByIdAsync(id, cancellationToken);
        if (existing == null)
            return NotFound(ApiResult.Fail("工作日志不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetUserIdFromClaims();
        if (!IsAdmin() && existing.UserId != currentUserId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此日志"));
        }

        var result = await logService.UpdateAsync(id, input, cancellationToken);
        if (result == null)
            return NotFound(ApiResult.Fail("工作日志不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WorkLogDto>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var existing = await logService.GetByIdAsync(id, cancellationToken);
        if (existing == null)
            return NotFound(ApiResult.Fail("工作日志不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetUserIdFromClaims();
        if (!IsAdmin() && existing.UserId != currentUserId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此日志"));
        }

        var deleted = await logService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("工作日志不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult.Success("删除成功"));
    }
}