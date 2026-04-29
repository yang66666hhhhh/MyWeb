using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Platform.Controllers;

[ApiController]
[Route("api/platform")]
[Authorize]
public class PlatformController : ControllerBase
{
    [HttpGet("api-docs")]
    public ActionResult<ApiResult<object>> GetApiDocs()
    {
        return Ok(ApiResult<object>.Success(new
        {
            Version = "1.0.0",
            Title = "Personal OS API",
            Description = "个人数字操作系统 API 文档"
        }));
    }

    [HttpGet("scripts")]
    public ActionResult<ApiResult<PageResult<object>>> GetScripts([FromQuery] PageQueryDto query)
    {
        return Ok(ApiResult<PageResult<object>>.Success(new PageResult<object>
        {
            Items = new List<object>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }

    [HttpGet("integrations")]
    public ActionResult<ApiResult<PageResult<object>>> GetIntegrations([FromQuery] PageQueryDto query)
    {
        return Ok(ApiResult<PageResult<object>>.Success(new PageResult<object>
        {
            Items = new List<object>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }

    [HttpGet("webhooks")]
    public ActionResult<ApiResult<PageResult<object>>> GetWebhooks([FromQuery] PageQueryDto query)
    {
        return Ok(ApiResult<PageResult<object>>.Success(new PageResult<object>
        {
            Items = new List<object>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }
}
