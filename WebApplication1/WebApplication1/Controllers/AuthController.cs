using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Common;
using WebApplication1.Dtos.Auth;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult<ApiResult<LoginResponseDto>> Login([FromBody] LoginRequestDto input)
    {
        var userInfo = authService.ValidateUser(input.Username, input.Password);
        if (userInfo is null)
        {
            return Unauthorized(ApiResult<LoginResponseDto>.Fail("用户名或密码错误", StatusCodes.Status401Unauthorized));
        }

        var accessToken = authService.CreateAccessToken(userInfo.Username);
        return Ok(ApiResult<LoginResponseDto>.Success(new LoginResponseDto
        {
            AccessToken = accessToken,
        }, "登录成功"));
    }

    [Authorize]
    [HttpGet("codes")]
    public ActionResult<ApiResult<IReadOnlyList<string>>> GetAccessCodes()
    {
        return Ok(ApiResult<IReadOnlyList<string>>.Success(authService.GetAccessCodes(User)));
    }

    [Authorize]
    [HttpPost("refresh")]
    public ActionResult<string> Refresh()
    {
        if (string.IsNullOrWhiteSpace(User.Identity?.Name))
        {
            return Unauthorized("Unauthorized");
        }

        return Ok(authService.CreateAccessToken(User.Identity.Name));
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public ActionResult<ApiResult> Logout()
    {
        return Ok(ApiResult.Success("退出成功"));
    }
}
