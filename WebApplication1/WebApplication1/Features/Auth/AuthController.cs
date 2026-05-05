using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<ApiResult<LoginResponseDto>>> Login([FromBody] LoginRequestDto input)
    {
        var userInfo = await authService.ValidateUserAsync(input.Username, input.Password);
        if (userInfo is null)
        {
            return Unauthorized(ApiResult<LoginResponseDto>.Fail("用户名或密码错误", StatusCodes.Status401Unauthorized));
        }

        var accessToken = await authService.CreateAccessTokenAsync(userInfo.Username);
        var userId = Guid.Parse(userInfo.UserId);
        var refreshToken = await authService.CreateRefreshTokenAsync(userId, HttpContext.Connection.RemoteIpAddress?.ToString());
        var expireMinutes = 15;

        return Ok(ApiResult<LoginResponseDto>.Success(new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = expireMinutes * 60
        }, "登录成功"));
    }

    [Authorize]
    [HttpGet("codes")]
    public async Task<ActionResult<ApiResult<IReadOnlyList<string>>>> GetAccessCodes()
    {
        return Ok(ApiResult<IReadOnlyList<string>>.Success(await authService.GetAccessCodesAsync(User)));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResult<RefreshTokenResponseDto>>> Refresh([FromBody] RefreshTokenRequestDto input)
    {
        if (string.IsNullOrWhiteSpace(input.RefreshToken))
            return BadRequest(ApiResult<RefreshTokenResponseDto>.Fail("Refresh token is required"));

        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult<RefreshTokenResponseDto>.Fail("Invalid token"));

        if (!await authService.ValidateRefreshTokenAsync(input.RefreshToken, userId.Value))
            return Unauthorized(ApiResult<RefreshTokenResponseDto>.Fail("Invalid or expired refresh token"));

        await authService.RevokeRefreshTokenAsync(input.RefreshToken, userId.Value);

        var user = User.FindFirst(ClaimTypes.Name)?.Value;
        if (string.IsNullOrWhiteSpace(user))
            return Unauthorized(ApiResult<RefreshTokenResponseDto>.Fail("User not found"));

        var newAccessToken = await authService.CreateAccessTokenAsync(user);
        var newRefreshToken = await authService.CreateRefreshTokenAsync(userId.Value, HttpContext.Connection.RemoteIpAddress?.ToString());
        var expireMinutes = 15;

        return Ok(ApiResult<RefreshTokenResponseDto>.Success(new RefreshTokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = expireMinutes * 60
        }));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResult>> Logout()
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue)
        {
            var refreshToken = Request.Headers["X-Refresh-Token"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await authService.RevokeRefreshTokenAsync(refreshToken, userId.Value);
            }
        }

        return Ok(ApiResult.Success("退出成功"));
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }
}
