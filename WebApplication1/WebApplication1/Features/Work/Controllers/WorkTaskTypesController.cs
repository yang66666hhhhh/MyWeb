using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Shared.Common;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services.Interfaces;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Authorize]
[Route("api/work/task-types")]
public class WorkTaskTypesController(IWorkTaskTypeService taskTypeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<WorkTaskTypeDto>>>> GetPage(
        [FromQuery] WorkTaskTypeQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await taskTypeService.GetPageAsync(query, cancellationToken);
        return Ok(ApiResult<PageResult<WorkTaskTypeDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkTaskTypeDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await taskTypeService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return NotFound(ApiResult.Fail("任务类型不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WorkTaskTypeDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<WorkTaskTypeDto>>> Create(
        [FromBody] CreateWorkTaskTypeDto input,
        CancellationToken cancellationToken)
    {
        var result = await taskTypeService.CreateAsync(input, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<WorkTaskTypeDto>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkTaskTypeDto>>> Update(
        Guid id,
        [FromBody] UpdateWorkTaskTypeDto input,
        CancellationToken cancellationToken)
    {
        var result = await taskTypeService.UpdateAsync(id, input, cancellationToken);
        if (result == null)
            return NotFound(ApiResult.Fail("任务类型不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WorkTaskTypeDto>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await taskTypeService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("任务类型不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult.Success("删除成功"));
    }
}
