using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Route("api/work/impl-logs")]
[Authorize]
[RequireFeature("WORK_LOG")]
[Tags("Work - Implementation Logs")]
public class ImplLogController(IImplLogService implLogService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<ImplLogDto>>>> GetPage(
        [FromQuery] ImplLogQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await implLogService.GetPageAsync(query, userId.Value, cancellationToken);
        return Ok(ApiResult<PageResult<ImplLogDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<ImplLogDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await implLogService.GetByIdAsync(id, userId.Value, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<ImplLogDto>.Fail("实施日志不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<ImplLogDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<ImplLogDto>>> Create(
        [FromBody] CreateImplLogDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await implLogService.CreateAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<ImplLogDto>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<ImplLogDto>>> Update(
        Guid id,
        [FromBody] UpdateImplLogDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await implLogService.UpdateAsync(id, input, userId.Value, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<ImplLogDto>.Fail("实施日志不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<ImplLogDto>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var deleted = await implLogService.DeleteAsync(id, userId.Value, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("实施日志不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult.Success("删除成功"));
    }
}
