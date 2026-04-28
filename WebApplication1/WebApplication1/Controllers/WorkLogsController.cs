using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common;
using WebApplication1.Dtos.Work;
using WebApplication1.Services.Interfaces.IWork;

namespace WebApplication1.Controllers;

[ApiController]
[Authorize]
[Route("api/work/logs")]
public class WorkLogsController(IWorkLogService logService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<WorkLogDto>>>> GetPage(
        [FromQuery] WorkLogQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await logService.GetPageAsync(query, cancellationToken);
        return Ok(ApiResult<PageResult<WorkLogDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkLogDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await logService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return NotFound(ApiResult.Fail("工作日志不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WorkLogDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<WorkLogDto>>> Create(
        [FromBody] CreateWorkLogDto input,
        CancellationToken cancellationToken)
    {
        var result = await logService.CreateAsync(input, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<WorkLogDto>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkLogDto>>> Update(
        Guid id,
        [FromBody] UpdateWorkLogDto input,
        CancellationToken cancellationToken)
    {
        var result = await logService.UpdateAsync(id, input, cancellationToken);
        if (result == null)
            return NotFound(ApiResult.Fail("工作日志不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WorkLogDto>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await logService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("工作日志不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult.Success("删除成功"));
    }
}
