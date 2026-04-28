using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common;
using WebApplication1.Dtos.Users;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Authorize]
[Route("api/user")]
public class UserController(IAuthService authService) : ControllerBase
{
    [HttpGet("info")]
    public ActionResult<ApiResult<UserInfoDto>> GetUserInfo()
    {
        var userInfo = authService.GetUserInfo(User);
        if (userInfo is null)
        {
            return Unauthorized(ApiResult<UserInfoDto>.Fail("用户信息不存在", StatusCodes.Status401Unauthorized));
        }

        return Ok(ApiResult<UserInfoDto>.Success(userInfo));
    }
}
