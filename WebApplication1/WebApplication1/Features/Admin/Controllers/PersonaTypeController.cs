using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Admin.Dtos;
using WebApplication1.Features.Admin.Services;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Admin.Controllers;

[ApiController]
[Route("api/system/persona-types")]
[Authorize]
[Tags("Admin - Personas")]
public class PersonaTypeController(IPersonaService personaService, ILogger<PersonaTypeController> logger) : BaseApiController
{
    [HttpGet]
    [Authorize(Roles = "pro,owner")]
    public async Task<ActionResult<ApiResult<PageResult<PersonaTypeDto>>>> GetPage([FromQuery] PersonaTypeQueryDto query, CancellationToken cancellationToken)
    {
        try
        {
            var result = await personaService.GetPageAsync(query, cancellationToken);
            return Ok(ApiResult<PageResult<PersonaTypeDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取身份类型列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("all")]
    [Authorize(Roles = "pro,owner")]
    public async Task<ActionResult<ApiResult<List<PersonaTypeDto>>>> GetAll([FromQuery] bool? isActive = true, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await personaService.GetAllAsync(isActive, cancellationToken);
            return Ok(ApiResult<List<PersonaTypeDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取全部身份类型失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<PersonaTypeDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await personaService.GetByIdAsync(id, cancellationToken);
            if (result == null)
                return NotFound(ApiResult<PersonaTypeDto>.Fail("身份类型不存在"));
            return Ok(ApiResult<PersonaTypeDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取身份类型详情失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpPost]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult<PersonaTypeDto>>> Create([FromBody] CreatePersonaTypeDto input, CancellationToken cancellationToken)
    {
        try
        {
            var result = await personaService.CreateAsync(input, cancellationToken);
            logger.LogInformation("创建身份类型成功: {Code}", result.Code);
            return Ok(ApiResult<PersonaTypeDto>.Success(result, "创建成功"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResult<PersonaTypeDto>.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建身份类型失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult<PersonaTypeDto>>> Update(Guid id, [FromBody] UpdatePersonaTypeDto input, CancellationToken cancellationToken)
    {
        try
        {
            var result = await personaService.UpdateAsync(id, input, cancellationToken);
            if (result == null)
                return NotFound(ApiResult<PersonaTypeDto>.Fail("身份类型不存在"));
            logger.LogInformation("更新身份类型成功: {Id}", id);
            return Ok(ApiResult<PersonaTypeDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新身份类型失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var success = await personaService.DeleteAsync(id, cancellationToken);
            if (!success)
                return NotFound(ApiResult.Fail("身份类型不存在"));
            logger.LogInformation("删除身份类型成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResult.Fail(ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除身份类型失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("{id:guid}/menu-items")]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult<List<PersonaMenuItemDto>>>> GetMenuItems(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await personaService.GetMenuItemsAsync(id, cancellationToken);
            return Ok(ApiResult<List<PersonaMenuItemDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取身份菜单项失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpPut("{id:guid}/menu-items")]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult>> SetMenuItems(Guid id, [FromBody] List<PersonaMenuItemDto> items, CancellationToken cancellationToken)
    {
        try
        {
            await personaService.SetMenuItemsAsync(id, items, cancellationToken);
            logger.LogInformation("更新身份菜单项成功: {Id}", id);
            return Ok(ApiResult.Success("菜单项更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新身份菜单项失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }
}
