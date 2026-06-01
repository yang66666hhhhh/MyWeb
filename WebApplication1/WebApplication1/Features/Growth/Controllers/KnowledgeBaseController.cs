using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Features.Growth.Services.Interfaces;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Controllers;

[ApiController]
[Authorize]
[RequireFeature("GROWTH_KNOWLEDGE")]
[Route("api/growth/knowledge-base")]
[Tags("Growth - Knowledge Base")]
public class KnowledgeBaseController(IKnowledgeArticleService articleService, ILogger<KnowledgeBaseController> logger) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<KnowledgeArticleDto>>>> GetPage(
        [FromQuery] KnowledgeArticleQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await articleService.GetPageAsync(query, userId, cancellationToken);
            return Ok(ApiResult<PageResult<KnowledgeArticleDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取知识库文章列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<KnowledgeArticleDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await articleService.GetByIdAsync(id, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<KnowledgeArticleDto>.Fail("文章不存在", StatusCodes.Status404NotFound));

            await articleService.IncrementViewCountAsync(id, cancellationToken);

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && result.UserId != currentUserId)
                return NotFound(ApiResult<KnowledgeArticleDto>.Fail("文章不存在", StatusCodes.Status404NotFound));

            return Ok(ApiResult<KnowledgeArticleDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取知识库文章详情失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<KnowledgeArticleDto>>> Create(
        [FromBody] CreateKnowledgeArticleDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await articleService.CreateAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建知识库文章成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<KnowledgeArticleDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建知识库文章失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<KnowledgeArticleDto>>> Update(
        Guid id,
        [FromBody] UpdateKnowledgeArticleDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await articleService.GetByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult<KnowledgeArticleDto>.Fail("文章不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId)
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此文章"));

            var result = await articleService.UpdateAsync(id, input, cancellationToken);
            if (result is null)
                return NotFound(ApiResult.Fail("文章不存在"));
            logger.LogInformation("更新知识库文章成功: {Id}", id);
            return Ok(ApiResult<KnowledgeArticleDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新知识库文章失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await articleService.GetByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult.Fail("文章不存在"));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId)
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此文章"));

            var deleted = await articleService.DeleteAsync(id, cancellationToken);
            logger.LogInformation("删除知识库文章成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除知识库文章失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }
}
