using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Content.Dtos;
using WebApplication1.Features.Content.Services.Interfaces;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Content.Controllers;

[ApiController]
[Authorize]
[RequireFeature("GROWTH_KNOWLEDGE")]
[Route("api/content")]
[Tags("Content")]
public class ContentController(IContentService contentService, ILogger<ContentController> logger) : BaseApiController
{
    [HttpGet("articles")]
    public async Task<ActionResult<ApiResult<PageResult<ArticleDto>>>> GetArticlePage(
        [FromQuery] ContentQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await contentService.GetArticlePageAsync(query, userId, cancellationToken);
            return Ok(ApiResult<PageResult<ArticleDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取文章列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("articles/{id:guid}")]
    public async Task<ActionResult<ApiResult<ArticleDto>>> GetArticleById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await contentService.GetArticleByIdAsync(id, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<ArticleDto>.Fail("文章不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
                return NotFound(ApiResult<ArticleDto>.Fail("文章不存在", StatusCodes.Status404NotFound));

            return Ok(ApiResult<ArticleDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取文章详情失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpPost("articles")]
    public async Task<ActionResult<ApiResult<ArticleDto>>> CreateArticle(
        [FromBody] CreateArticleDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await contentService.CreateArticleAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建文章成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetArticleById), new { id = result.Id }, ApiResult<ArticleDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建文章失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("articles/{id:guid}")]
    public async Task<ActionResult<ApiResult<ArticleDto>>> UpdateArticle(
        Guid id,
        [FromBody] UpdateArticleDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await contentService.GetArticleByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult<ArticleDto>.Fail("文章不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此文章"));

            var result = await contentService.UpdateArticleAsync(id, input, cancellationToken);
            if (result is null)
                return NotFound(ApiResult.Fail("文章不存在"));
            logger.LogInformation("更新文章成功: {Id}", id);
            return Ok(ApiResult<ArticleDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新文章失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("articles/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteArticle(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await contentService.GetArticleByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult.Fail("文章不存在"));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此文章"));

            var deleted = await contentService.DeleteArticleAsync(id, cancellationToken);
            logger.LogInformation("删除文章成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除文章失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("media")]
    public async Task<ActionResult<ApiResult<PageResult<MediaItemDto>>>> GetMediaItemPage(
        [FromQuery] ContentQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await contentService.GetMediaItemPageAsync(query, userId, cancellationToken);
            return Ok(ApiResult<PageResult<MediaItemDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取媒体文件列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("media/{id:guid}")]
    public async Task<ActionResult<ApiResult<MediaItemDto>>> GetMediaItemById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await contentService.GetMediaItemByIdAsync(id, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<MediaItemDto>.Fail("媒体文件不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
                return NotFound(ApiResult<MediaItemDto>.Fail("媒体文件不存在", StatusCodes.Status404NotFound));

            return Ok(ApiResult<MediaItemDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取媒体文件详情失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpPost("media")]
    public async Task<ActionResult<ApiResult<MediaItemDto>>> CreateMediaItem(
        [FromBody] CreateMediaItemDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await contentService.CreateMediaItemAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建媒体文件成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetMediaItemById), new { id = result.Id }, ApiResult<MediaItemDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建媒体文件失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("media/{id:guid}")]
    public async Task<ActionResult<ApiResult<MediaItemDto>>> UpdateMediaItem(
        Guid id,
        [FromBody] UpdateMediaItemDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await contentService.GetMediaItemByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult<MediaItemDto>.Fail("媒体文件不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此媒体文件"));

            var result = await contentService.UpdateMediaItemAsync(id, input, cancellationToken);
            if (result is null)
                return NotFound(ApiResult.Fail("媒体文件不存在"));
            logger.LogInformation("更新媒体文件成功: {Id}", id);
            return Ok(ApiResult<MediaItemDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新媒体文件失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("media/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteMediaItem(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await contentService.GetMediaItemByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult.Fail("媒体文件不存在"));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此媒体文件"));

            var deleted = await contentService.DeleteMediaItemAsync(id, cancellationToken);
            logger.LogInformation("删除媒体文件成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除媒体文件失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("calendar")]
    public async Task<ActionResult<ApiResult<PageResult<PublishingCalendarDto>>>> GetPublishingCalendarPage(
        [FromQuery] ContentQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await contentService.GetPublishingCalendarPageAsync(query, userId, cancellationToken);
            return Ok(ApiResult<PageResult<PublishingCalendarDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取发布日历列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("calendar/{id:guid}")]
    public async Task<ActionResult<ApiResult<PublishingCalendarDto>>> GetPublishingCalendarById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await contentService.GetPublishingCalendarByIdAsync(id, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<PublishingCalendarDto>.Fail("发布日历不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
                return NotFound(ApiResult<PublishingCalendarDto>.Fail("发布日历不存在", StatusCodes.Status404NotFound));

            return Ok(ApiResult<PublishingCalendarDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取发布日历详情失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpPost("calendar")]
    public async Task<ActionResult<ApiResult<PublishingCalendarDto>>> CreatePublishingCalendar(
        [FromBody] CreatePublishingCalendarDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await contentService.CreatePublishingCalendarAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建发布日历成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetPublishingCalendarById), new { id = result.Id }, ApiResult<PublishingCalendarDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建发布日历失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("calendar/{id:guid}")]
    public async Task<ActionResult<ApiResult<PublishingCalendarDto>>> UpdatePublishingCalendar(
        Guid id,
        [FromBody] UpdatePublishingCalendarDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var existing = await contentService.GetPublishingCalendarByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult<PublishingCalendarDto>.Fail("发布日历不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此发布日历"));

            var result = await contentService.UpdatePublishingCalendarAsync(id, input, cancellationToken);
            if (result is null)
                return NotFound(ApiResult.Fail("发布日历不存在"));
            logger.LogInformation("更新发布日历成功: {Id}", id);
            return Ok(ApiResult<PublishingCalendarDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新发布日历失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("calendar/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeletePublishingCalendar(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await contentService.GetPublishingCalendarByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult.Fail("发布日历不存在"));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此发布日历"));

            var deleted = await contentService.DeletePublishingCalendarAsync(id, cancellationToken);
            logger.LogInformation("删除发布日历成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除发布日历失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }
}
