using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Authorize]
[Route("api/work")]
[Tags("Work")]
public class WorkExtendedController(IWorkExtendedService service) : BaseApiController
{
    [HttpGet("okr")]
    public async Task<ActionResult<ApiResult<PageResult<OkrDto>>>> GetOkrs(
        [FromQuery] WorkExtendedQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await service.GetOkrsAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<OkrDto>>.Success(result));
    }

    [HttpPost("okr")]
    public async Task<ActionResult<ApiResult<OkrDto>>> CreateOkr(
        [FromBody] CreateOkrInput input, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await service.CreateOkrAsync(input, userId.Value, ct);
        return Ok(ApiResult<OkrDto>.Success(result));
    }

    [HttpPut("okr/{id:guid}")]
    public async Task<ActionResult<ApiResult<OkrDto>>> UpdateOkr(
        Guid id, [FromBody] UpdateOkrInput input, CancellationToken ct)
    {
        var existing = await service.GetOkrByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult<OkrDto>.Fail("OKR不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(403, ApiResult.Fail("无权限修改此记录"));

        var result = await service.UpdateOkrAsync(id, input, ct);
        return Ok(ApiResult<OkrDto>.Success(result!, "更新成功"));
    }

    [HttpDelete("okr/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteOkr(Guid id, CancellationToken ct)
    {
        var existing = await service.GetOkrByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult.Fail("OKR不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(403, ApiResult.Fail("无权限删除此记录"));

        var success = await service.DeleteOkrAsync(id, ct);
        return HandleDeleteResult(success, "OKR");
    }

    [HttpGet("risks")]
    public async Task<ActionResult<ApiResult<PageResult<RiskItemDto>>>> GetRisks(
        [FromQuery] WorkExtendedQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await service.GetRisksAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<RiskItemDto>>.Success(result));
    }

    [HttpPost("risks")]
    public async Task<ActionResult<ApiResult<RiskItemDto>>> CreateRisk(
        [FromBody] CreateRiskItemInput input, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await service.CreateRiskAsync(input, userId.Value, ct);
        return Ok(ApiResult<RiskItemDto>.Success(result));
    }

    [HttpPut("risks/{id:guid}")]
    public async Task<ActionResult<ApiResult<RiskItemDto>>> UpdateRisk(
        Guid id, [FromBody] UpdateRiskItemInput input, CancellationToken ct)
    {
        var existing = await service.GetRiskByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult<RiskItemDto>.Fail("风险不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(403, ApiResult.Fail("无权限修改此记录"));

        var result = await service.UpdateRiskAsync(id, input, ct);
        return Ok(ApiResult<RiskItemDto>.Success(result!, "更新成功"));
    }

    [HttpDelete("risks/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteRisk(Guid id, CancellationToken ct)
    {
        var existing = await service.GetRiskByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult.Fail("风险不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(403, ApiResult.Fail("无权限删除此记录"));

        var success = await service.DeleteRiskAsync(id, ct);
        return HandleDeleteResult(success, "风险");
    }

    [HttpGet("files")]
    public async Task<ActionResult<ApiResult<PageResult<WorkFileDto>>>> GetFiles(
        [FromQuery] WorkExtendedQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await service.GetFilesAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<WorkFileDto>>.Success(result));
    }

    [HttpPost("files")]
    public async Task<ActionResult<ApiResult<WorkFileDto>>> CreateFile(
        [FromBody] CreateWorkFileInput input, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await service.CreateFileAsync(input, userId.Value, ct);
        return Ok(ApiResult<WorkFileDto>.Success(result));
    }

    [HttpPut("files/{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkFileDto>>> UpdateFile(
        Guid id, [FromBody] UpdateWorkFileInput input, CancellationToken ct)
    {
        var existing = await service.GetFileByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult<WorkFileDto>.Fail("文件不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(403, ApiResult.Fail("无权限修改此记录"));

        var result = await service.UpdateFileAsync(id, input, ct);
        return Ok(ApiResult<WorkFileDto>.Success(result!, "更新成功"));
    }

    [HttpDelete("files/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteFile(Guid id, CancellationToken ct)
    {
        var existing = await service.GetFileByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult.Fail("文件不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(403, ApiResult.Fail("无权限删除此记录"));

        var success = await service.DeleteFileAsync(id, ct);
        return HandleDeleteResult(success, "文件");
    }
}
