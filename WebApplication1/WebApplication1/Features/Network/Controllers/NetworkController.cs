using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Network.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Network.Controllers;

[ApiController]
[Route("api/network")]
[Authorize]
public class NetworkController : ControllerBase
{
    [HttpGet("contacts")]
    public ActionResult<ApiResult<PageResult<ContactDto>>> GetContacts([FromQuery] NetworkQueryDto query)
    {
        return Ok(ApiResult<PageResult<ContactDto>>.Success(new PageResult<ContactDto>
        {
            Items = new List<ContactDto>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }

    [HttpGet("interactions")]
    public ActionResult<ApiResult<PageResult<InteractionDto>>> GetInteractions([FromQuery] NetworkQueryDto query)
    {
        return Ok(ApiResult<PageResult<InteractionDto>>.Success(new PageResult<InteractionDto>
        {
            Items = new List<InteractionDto>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }

    [HttpGet("tags")]
    public ActionResult<ApiResult<List<TagDto>>> GetTags()
    {
        return Ok(ApiResult<List<TagDto>>.Success(new List<TagDto>()));
    }
}
