using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Shared.Common;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Authorize]
[Route("api/work/software-assets")]
[Tags("Work - Software Assets")]
public class SoftwareAssetController : BaseApiController
{
    private readonly ISoftwareAssetService _softwareAssetService;

    public SoftwareAssetController(ISoftwareAssetService softwareAssetService)
    {
        _softwareAssetService = softwareAssetService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<SoftwareAssetDto>>>> GetPage(
        [FromQuery] SoftwareAssetQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await _softwareAssetService.GetPageAsync(query, cancellationToken);
        return Ok(ApiResult<PageResult<SoftwareAssetDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<SoftwareAssetDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _softwareAssetService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return NotFound(ApiResult<SoftwareAssetDto>.Fail("软件资产不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<SoftwareAssetDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<SoftwareAssetDto>>> Create(
        [FromBody] CreateSoftwareAssetDto input,
        CancellationToken cancellationToken)
    {
        var result = await _softwareAssetService.CreateAsync(input, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<SoftwareAssetDto>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<SoftwareAssetDto>>> Update(
        Guid id,
        [FromBody] UpdateSoftwareAssetDto input,
        CancellationToken cancellationToken)
    {
        var result = await _softwareAssetService.UpdateAsync(id, input, cancellationToken);
        if (result == null)
            return NotFound(ApiResult<SoftwareAssetDto>.Fail("软件资产不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<SoftwareAssetDto>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _softwareAssetService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("软件资产不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult.Success("删除成功"));
    }
}
