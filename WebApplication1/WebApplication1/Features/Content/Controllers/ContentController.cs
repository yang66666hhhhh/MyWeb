using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Content.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Content.Controllers;

[ApiController]
[Route("api/content")]
[Authorize]
public class ContentController : ControllerBase
{
    [HttpGet("articles")]
    public ActionResult<ApiResult<PageResult<ArticleDto>>> GetArticles([FromQuery] ContentQueryDto query)
    {
        return Ok(ApiResult<PageResult<ArticleDto>>.Success(new PageResult<ArticleDto>
        {
            Items = new List<ArticleDto>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }

    [HttpGet("media")]
    public ActionResult<ApiResult<PageResult<MediaItemDto>>> GetMediaItems([FromQuery] ContentQueryDto query)
    {
        return Ok(ApiResult<PageResult<MediaItemDto>>.Success(new PageResult<MediaItemDto>
        {
            Items = new List<MediaItemDto>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }

    [HttpGet("calendar")]
    public ActionResult<ApiResult<PageResult<PublishingCalendarDto>>> GetCalendar([FromQuery] ContentQueryDto query)
    {
        return Ok(ApiResult<PageResult<PublishingCalendarDto>>.Success(new PageResult<PublishingCalendarDto>
        {
            Items = new List<PublishingCalendarDto>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }
}
