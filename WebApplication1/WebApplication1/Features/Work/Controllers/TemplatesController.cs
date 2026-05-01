using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Features.Work.Services.Interfaces;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Authorize]
[Route("api/templates")]
public class TemplatesController(ITemplateService templateService) : ControllerBase
{
    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private bool IsAdmin()
    {
        return User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Any(c => c.Value.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                       c.Value.Equals("super", StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<IndustryTemplateDto>>>> GetPage(
        [FromQuery] PageQueryDto query,
        CancellationToken cancellationToken)
    {
        if (!IsAdmin())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限"));

        var result = await templateService.GetPageAsync(query, cancellationToken);
        return Ok(ApiResult<PageResult<IndustryTemplateDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<IndustryTemplateDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await templateService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<IndustryTemplateDto>.Fail("模板不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<IndustryTemplateDto>.Success(result));
    }

    [HttpGet("{id:guid}/fields")]
    public async Task<ActionResult<ApiResult<List<TemplateFieldDto>>>> GetFields(Guid id, CancellationToken cancellationToken)
    {
        var result = await templateService.GetFieldsAsync(id, cancellationToken);
        return Ok(ApiResult<List<TemplateFieldDto>>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<IndustryTemplateDto>>> Create(
        [FromBody] CreateTemplateDto input,
        CancellationToken cancellationToken)
    {
        if (!IsAdmin())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限创建模板"));

        var result = await templateService.CreateAsync(input, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<IndustryTemplateDto>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<IndustryTemplateDto>>> Update(
        Guid id,
        [FromBody] CreateTemplateDto input,
        CancellationToken cancellationToken)
    {
        if (!IsAdmin())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改模板"));

        var result = await templateService.UpdateAsync(id, input, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<IndustryTemplateDto>.Fail("模板不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<IndustryTemplateDto>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (!IsAdmin())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除模板"));

        var deleted = await templateService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("模板不存在"));
        return Ok(ApiResult.Success("删除成功"));
    }

    [HttpPost("{id:guid}/set-default")]
    public async Task<ActionResult<ApiResult>> SetDefault(Guid id, CancellationToken cancellationToken)
    {
        if (!IsAdmin())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限"));

        var result = await templateService.SetDefaultAsync(id, cancellationToken);
        if (!result)
            return NotFound(ApiResult.Fail("模板不存在"));
        return Ok(ApiResult.Success("已设为默认模板"));
    }
}