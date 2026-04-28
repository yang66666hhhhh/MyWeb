using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common;
using WebApplication1.Dtos.Work;
using WebApplication1.Services.Interfaces.IWork;

namespace WebApplication1.Controllers;

[ApiController]
[Authorize]
[Route("api/work/projects")]
public class WorkProjectsController(IWorkProjectService projectService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<WorkProjectDto>>>> GetPage(
        [FromQuery] WorkProjectQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await projectService.GetPageAsync(query, cancellationToken);
        return Ok(ApiResult<PageResult<WorkProjectDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkProjectDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await projectService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return NotFound(ApiResult.Fail("项目不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WorkProjectDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<WorkProjectDto>>> Create(
        [FromBody] CreateWorkProjectDto input,
        CancellationToken cancellationToken)
    {
        var result = await projectService.CreateAsync(input, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<WorkProjectDto>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkProjectDto>>> Update(
        Guid id,
        [FromBody] UpdateWorkProjectDto input,
        CancellationToken cancellationToken)
    {
        var result = await projectService.UpdateAsync(id, input, cancellationToken);
        if (result == null)
            return NotFound(ApiResult.Fail("项目不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WorkProjectDto>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await projectService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("项目不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult.Success("删除成功"));
    }
}
