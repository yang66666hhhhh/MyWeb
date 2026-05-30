using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Authorize]
[Route("api/work")]
[Tags("Work")]
public class WorkExtendedController : BaseApiController
{
    // OKR endpoints
    [HttpGet("okr")]
    public async Task<ActionResult<ApiResult<PageResult<OkrDto>>>> GetOkrs(
        [FromQuery] WorkExtendedQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        // TODO: Implement OKR service
        return Ok(ApiResult<PageResult<OkrDto>>.Success(PageResult<OkrDto>.Create(new List<OkrDto>(), 0, query.Page, query.PageSize)));
    }

    [HttpPost("okr")]
    public async Task<ActionResult<ApiResult<OkrDto>>> CreateOkr(
        [FromBody] CreateOkrInput input, CancellationToken ct)
    {
        // TODO: Implement OKR service
        return Ok(ApiResult<OkrDto>.Success(new OkrDto { Id = Guid.NewGuid().ToString(), Title = input.Title }));
    }

    [HttpPut("okr/{id:guid}")]
    public async Task<ActionResult<ApiResult<OkrDto>>> UpdateOkr(
        Guid id, [FromBody] UpdateOkrInput input, CancellationToken ct)
    {
        // TODO: Implement OKR service
        return Ok(ApiResult<OkrDto>.Success(new OkrDto { Id = id.ToString() }));
    }

    [HttpDelete("okr/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteOkr(Guid id, CancellationToken ct)
    {
        // TODO: Implement OKR service
        return Ok(ApiResult.Success("删除成功"));
    }

    // Risk endpoints
    [HttpGet("risks")]
    public async Task<ActionResult<ApiResult<PageResult<RiskItemDto>>>> GetRisks(
        [FromQuery] WorkExtendedQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        // TODO: Implement risk service
        return Ok(ApiResult<PageResult<RiskItemDto>>.Success(PageResult<RiskItemDto>.Create(new List<RiskItemDto>(), 0, query.Page, query.PageSize)));
    }

    [HttpPost("risks")]
    public async Task<ActionResult<ApiResult<RiskItemDto>>> CreateRisk(
        [FromBody] CreateRiskItemInput input, CancellationToken ct)
    {
        // TODO: Implement risk service
        return Ok(ApiResult<RiskItemDto>.Success(new RiskItemDto { Id = Guid.NewGuid().ToString(), Title = input.Title }));
    }

    [HttpPut("risks/{id:guid}")]
    public async Task<ActionResult<ApiResult<RiskItemDto>>> UpdateRisk(
        Guid id, [FromBody] UpdateRiskItemInput input, CancellationToken ct)
    {
        // TODO: Implement risk service
        return Ok(ApiResult<RiskItemDto>.Success(new RiskItemDto { Id = id.ToString() }));
    }

    [HttpDelete("risks/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteRisk(Guid id, CancellationToken ct)
    {
        // TODO: Implement risk service
        return Ok(ApiResult.Success("删除成功"));
    }

    // Files endpoints
    [HttpGet("files")]
    public async Task<ActionResult<ApiResult<PageResult<WorkFileDto>>>> GetFiles(
        [FromQuery] WorkExtendedQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        // TODO: Implement file service
        return Ok(ApiResult<PageResult<WorkFileDto>>.Success(PageResult<WorkFileDto>.Create(new List<WorkFileDto>(), 0, query.Page, query.PageSize)));
    }

    [HttpPost("files")]
    public async Task<ActionResult<ApiResult<WorkFileDto>>> CreateFile(
        [FromBody] CreateWorkFileInput input, CancellationToken ct)
    {
        // TODO: Implement file service
        return Ok(ApiResult<WorkFileDto>.Success(new WorkFileDto { Id = Guid.NewGuid().ToString(), FileName = input.FileName }));
    }

    [HttpPut("files/{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkFileDto>>> UpdateFile(
        Guid id, [FromBody] UpdateWorkFileInput input, CancellationToken ct)
    {
        // TODO: Implement file service
        return Ok(ApiResult<WorkFileDto>.Success(new WorkFileDto { Id = id.ToString() }));
    }

    [HttpDelete("files/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteFile(Guid id, CancellationToken ct)
    {
        // TODO: Implement file service
        return Ok(ApiResult.Success("删除成功"));
    }
}
