using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Shared.Common;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services.Interfaces;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Authorize]
[Route("api/work/devices")]
public class WorkDevicesController(IWorkDeviceService deviceService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<WorkDeviceDto>>>> GetPage(
        [FromQuery] WorkDeviceQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await deviceService.GetPageAsync(query, cancellationToken);
        return Ok(ApiResult<PageResult<WorkDeviceDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkDeviceDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await deviceService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return NotFound(ApiResult.Fail("设备不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WorkDeviceDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<WorkDeviceDto>>> Create(
        [FromBody] CreateWorkDeviceDto input,
        CancellationToken cancellationToken)
    {
        var result = await deviceService.CreateAsync(input, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<WorkDeviceDto>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkDeviceDto>>> Update(
        Guid id,
        [FromBody] UpdateWorkDeviceDto input,
        CancellationToken cancellationToken)
    {
        var result = await deviceService.UpdateAsync(id, input, cancellationToken);
        if (result == null)
            return NotFound(ApiResult.Fail("设备不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WorkDeviceDto>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await deviceService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("设备不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult.Success("删除成功"));
    }
}
