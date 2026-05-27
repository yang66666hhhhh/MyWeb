using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Content.Dtos;
using WebApplication1.Features.Content.Services.Interfaces;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Content.Controllers;

[ApiController]
[Authorize]
[Route("api/content")]
[Tags("Content")]
public class ContentController(IContentService contentService) : BaseApiController
{
    [HttpGet("articles")]
    public async Task<ActionResult<ApiResult<PageResult<ArticleDto>>>> GetArticlePage(
        [FromQuery] ContentQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = IsProOrAbove() ? null : GetCurrentUserId();
        var result = await contentService.GetArticlePageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<ArticleDto>>.Success(result));
    }

    [HttpGet("articles/{id:guid}")]
    public async Task<ActionResult<ApiResult<ArticleDto>>> GetArticleById(Guid id, CancellationToken cancellationToken)
    {
        var result = await contentService.GetArticleByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<ArticleDto>.Fail("文章不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<ArticleDto>.Fail("文章不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<ArticleDto>.Success(result));
    }

    [HttpPost("articles")]
    public async Task<ActionResult<ApiResult<ArticleDto>>> CreateArticle(
        [FromBody] CreateArticleDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await contentService.CreateArticleAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetArticleById), new { id = result.Id }, ApiResult<ArticleDto>.Success(result, "创建成功"));
    }

    [HttpPut("articles/{id:guid}")]
    public async Task<ActionResult<ApiResult<ArticleDto>>> UpdateArticle(
        Guid id,
        [FromBody] UpdateArticleDto input,
        CancellationToken cancellationToken)
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
        return Ok(ApiResult<ArticleDto>.Success(result, "更新成功"));
    }

    [HttpDelete("articles/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteArticle(Guid id, CancellationToken cancellationToken)
    {
        var existing = await contentService.GetArticleByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult.Fail("文章不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此文章"));

        var deleted = await contentService.DeleteArticleAsync(id, cancellationToken);
        return Ok(ApiResult.Success("删除成功"));
    }

    [HttpGet("media")]
    public async Task<ActionResult<ApiResult<PageResult<MediaItemDto>>>> GetMediaItemPage(
        [FromQuery] ContentQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = IsProOrAbove() ? null : GetCurrentUserId();
        var result = await contentService.GetMediaItemPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<MediaItemDto>>.Success(result));
    }

    [HttpGet("media/{id:guid}")]
    public async Task<ActionResult<ApiResult<MediaItemDto>>> GetMediaItemById(Guid id, CancellationToken cancellationToken)
    {
        var result = await contentService.GetMediaItemByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<MediaItemDto>.Fail("媒体文件不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<MediaItemDto>.Fail("媒体文件不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<MediaItemDto>.Success(result));
    }

    [HttpPost("media")]
    public async Task<ActionResult<ApiResult<MediaItemDto>>> CreateMediaItem(
        [FromBody] CreateMediaItemDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await contentService.CreateMediaItemAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetMediaItemById), new { id = result.Id }, ApiResult<MediaItemDto>.Success(result, "创建成功"));
    }

    [HttpPut("media/{id:guid}")]
    public async Task<ActionResult<ApiResult<MediaItemDto>>> UpdateMediaItem(
        Guid id,
        [FromBody] UpdateMediaItemDto input,
        CancellationToken cancellationToken)
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
        return Ok(ApiResult<MediaItemDto>.Success(result, "更新成功"));
    }

    [HttpDelete("media/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteMediaItem(Guid id, CancellationToken cancellationToken)
    {
        var existing = await contentService.GetMediaItemByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult.Fail("媒体文件不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此媒体文件"));

        var deleted = await contentService.DeleteMediaItemAsync(id, cancellationToken);
        return Ok(ApiResult.Success("删除成功"));
    }

    [HttpGet("calendar")]
    public async Task<ActionResult<ApiResult<PageResult<PublishingCalendarDto>>>> GetPublishingCalendarPage(
        [FromQuery] ContentQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = IsProOrAbove() ? null : GetCurrentUserId();
        var result = await contentService.GetPublishingCalendarPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<PublishingCalendarDto>>.Success(result));
    }

    [HttpGet("calendar/{id:guid}")]
    public async Task<ActionResult<ApiResult<PublishingCalendarDto>>> GetPublishingCalendarById(Guid id, CancellationToken cancellationToken)
    {
        var result = await contentService.GetPublishingCalendarByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<PublishingCalendarDto>.Fail("发布日历不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<PublishingCalendarDto>.Fail("发布日历不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<PublishingCalendarDto>.Success(result));
    }

    [HttpPost("calendar")]
    public async Task<ActionResult<ApiResult<PublishingCalendarDto>>> CreatePublishingCalendar(
        [FromBody] CreatePublishingCalendarDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await contentService.CreatePublishingCalendarAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetPublishingCalendarById), new { id = result.Id }, ApiResult<PublishingCalendarDto>.Success(result, "创建成功"));
    }

    [HttpPut("calendar/{id:guid}")]
    public async Task<ActionResult<ApiResult<PublishingCalendarDto>>> UpdatePublishingCalendar(
        Guid id,
        [FromBody] UpdatePublishingCalendarDto input,
        CancellationToken cancellationToken)
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
        return Ok(ApiResult<PublishingCalendarDto>.Success(result, "更新成功"));
    }

    [HttpDelete("calendar/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeletePublishingCalendar(Guid id, CancellationToken cancellationToken)
    {
        var existing = await contentService.GetPublishingCalendarByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult.Fail("发布日历不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此发布日历"));

        var deleted = await contentService.DeletePublishingCalendarAsync(id, cancellationToken);
        return Ok(ApiResult.Success("删除成功"));
    }
}
