using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Features.Growth.Services.Interfaces;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Controllers;

[ApiController]
[Authorize]
[RequireFeature("GROWTH_SKILL")]
[Route("api/growth/projects")]
[Tags("Growth - Projects")]
public class GrowthProjectsController(IGrowthProjectService projectService, ILogger<GrowthProjectsController> logger) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<GrowthProjectDto>>>> GetPage(
        [FromQuery] GrowthProjectQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await projectService.GetPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<GrowthProjectDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<GrowthProjectDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await projectService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<GrowthProjectDto>.Fail("项目不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId)
            return NotFound(ApiResult<GrowthProjectDto>.Fail("项目不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<GrowthProjectDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<GrowthProjectDto>>> Create(
        [FromBody] CreateGrowthProjectDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await projectService.CreateAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建成长项目成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<GrowthProjectDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建成长项目失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<GrowthProjectDto>>> Update(
        Guid id,
        [FromBody] UpdateGrowthProjectDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await projectService.GetByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult<GrowthProjectDto>.Fail("项目不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId)
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此项目"));

            var result = await projectService.UpdateAsync(id, input, cancellationToken);
            if (result is null)
                return NotFound(ApiResult.Fail("项目不存在"));
            logger.LogInformation("更新成长项目成功: {Id}", id);
            return Ok(ApiResult<GrowthProjectDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新成长项目失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await projectService.GetByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult.Fail("项目不存在"));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId)
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此项目"));

            var deleted = await projectService.DeleteAsync(id, cancellationToken);
            logger.LogInformation("删除成长项目成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除成长项目失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }
}