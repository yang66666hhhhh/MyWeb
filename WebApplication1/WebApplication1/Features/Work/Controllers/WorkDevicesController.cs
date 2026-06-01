using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Shared.Common;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services.Interfaces;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Authorize]
[RequireFeature("WORK_DEVICE")]
[Route("api/work/devices")]
[Tags("Work - Devices")]
public class WorkDevicesController(IWorkDeviceService deviceService, ILogger<WorkDevicesController> logger) : ControllerBase
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
        try
        {
            var result = await deviceService.CreateAsync(input, cancellationToken);
            logger.LogInformation("创建工作设备成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<WorkDeviceDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建工作设备失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkDeviceDto>>> Update(
        Guid id,
        [FromBody] UpdateWorkDeviceDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await deviceService.UpdateAsync(id, input, cancellationToken);
            if (result == null)
                return NotFound(ApiResult.Fail("设备不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新工作设备成功: {Id}", id);
            return Ok(ApiResult<WorkDeviceDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新工作设备失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await deviceService.DeleteAsync(id, cancellationToken);
            if (!deleted)
                return NotFound(ApiResult.Fail("设备不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除工作设备成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除工作设备失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }
}
