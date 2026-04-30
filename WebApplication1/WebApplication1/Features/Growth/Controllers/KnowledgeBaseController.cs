using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Features.Growth.Services.Interfaces;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Controllers;

[ApiController]
[Authorize]
[Route("api/knowledge-base")]
public class KnowledgeBaseController(IKnowledgeArticleService articleService) : ControllerBase
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
    public async Task<ActionResult<ApiResult<PageResult<KnowledgeArticleDto>>>> GetPage(
        [FromQuery] KnowledgeArticleQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = IsAdmin() ? null : GetUserId();
        var result = await articleService.GetPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<KnowledgeArticleDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<KnowledgeArticleDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await articleService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<KnowledgeArticleDto>.Fail("文章不存在", StatusCodes.Status404NotFound));

        await articleService.IncrementViewCountAsync(id, cancellationToken);

        var currentUserId = GetUserId();
        if (!IsAdmin() && result.UserId != currentUserId)
            return NotFound(ApiResult<KnowledgeArticleDto>.Fail("文章不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<KnowledgeArticleDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<KnowledgeArticleDto>>> Create(
        [FromBody] CreateKnowledgeArticleDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await articleService.CreateAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<KnowledgeArticleDto>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<KnowledgeArticleDto>>> Update(
        Guid id,
        [FromBody] UpdateKnowledgeArticleDto input,
        CancellationToken cancellationToken)
    {
        var existing = await articleService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult<KnowledgeArticleDto>.Fail("文章不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetUserId();
        if (!IsAdmin() && existing.UserId != currentUserId)
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此文章"));

        var result = await articleService.UpdateAsync(id, input, cancellationToken);
        return Ok(ApiResult<KnowledgeArticleDto>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var existing = await articleService.GetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult.Fail("文章不存在"));

        var currentUserId = GetUserId();
        if (!IsAdmin() && existing.UserId != currentUserId)
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此文章"));

        var deleted = await articleService.DeleteAsync(id, cancellationToken);
        return Ok(ApiResult.Success("删除成功"));
    }
}