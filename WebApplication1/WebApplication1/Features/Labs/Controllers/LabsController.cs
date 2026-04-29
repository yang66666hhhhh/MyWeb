using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Labs.Controllers;

[ApiController]
[Route("api/labs")]
[Authorize]
public class LabsController : ControllerBase
{
    [HttpGet("experiments")]
    public ActionResult<ApiResult<PageResult<object>>> GetExperiments([FromQuery] PageQueryDto query)
    {
        return Ok(ApiResult<PageResult<object>>.Success(new PageResult<object>
        {
            Items = new List<object>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }

    [HttpGet("templates")]
    public ActionResult<ApiResult<PageResult<object>>> GetTemplates([FromQuery] PageQueryDto query)
    {
        return Ok(ApiResult<PageResult<object>>.Success(new PageResult<object>
        {
            Items = new List<object>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }

    [HttpGet("ui-components")]
    public ActionResult<ApiResult<PageResult<object>>> GetUiComponents([FromQuery] PageQueryDto query)
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
