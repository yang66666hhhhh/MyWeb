using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Shared.Common;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services.Interfaces;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Authorize]
[RequireFeature("WORK_PROJECT")]
[Route("api/work/projects")]
[Tags("Work - Projects")]
public class WorkProjectsController(IWorkProjectService projectService, ILogger<WorkProjectsController> logger) : ControllerBase
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
        try
        {
            var result = await projectService.CreateAsync(input, cancellationToken);
            logger.LogInformation("创建工作项目成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<WorkProjectDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建工作项目失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkProjectDto>>> Update(
        Guid id,
        [FromBody] UpdateWorkProjectDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await projectService.UpdateAsync(id, input, cancellationToken);
            if (result == null)
                return NotFound(ApiResult.Fail("项目不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新工作项目成功: {Id}", id);
            return Ok(ApiResult<WorkProjectDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新工作项目失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await projectService.DeleteAsync(id, cancellationToken);
            if (!deleted)
                return NotFound(ApiResult.Fail("项目不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除工作项目成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除工作项目失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }
}
