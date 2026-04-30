using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Shared.Common;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.User;

[ApiController]
[Authorize]
[Route("api/user/profile")]
public class UserProfileController(AppDbContext context) : ControllerBase
{
    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    [HttpGet]
    public ActionResult<ApiResult<UserProfileDto>> GetProfile()
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var user = context.Users.Find(userId.Value);
        if (user == null)
            return NotFound(ApiResult.Fail("用户不存在"));

        return Ok(ApiResult<UserProfileDto>.Success(new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            RealName = user.RealName,
            Avatar = user.Avatar,
            Email = user.Email,
            Phone = user.Phone,
            Roles = user.Roles,
            Status = user.Status,
        }));
    }

    [HttpPut]
    public ActionResult<ApiResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileDto input)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var user = context.Users.Find(userId.Value);
        if (user == null)
            return NotFound(ApiResult.Fail("用户不存在"));

        user.RealName = input.RealName ?? user.RealName;
        user.Avatar = input.Avatar ?? user.Avatar;
        user.Email = input.Email ?? user.Email;
        user.Phone = input.Phone ?? user.Phone;

        context.SaveChanges();

        return Ok(ApiResult<UserProfileDto>.Success(new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            RealName = user.RealName,
            Avatar = user.Avatar,
            Email = user.Email,
            Phone = user.Phone,
            Roles = user.Roles,
            Status = user.Status,
        }, "更新成功"));
    }

    [HttpPost("change-password")]
    public ActionResult<ApiResult> ChangePassword([FromBody] ChangePasswordDto input)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var user = context.Users.Find(userId.Value);
        if (user == null)
            return NotFound(ApiResult.Fail("用户不存在"));

        if (!BCrypt.Net.BCrypt.Verify(input.OldPassword, user.PasswordHash))
        {
            return BadRequest(ApiResult.Fail("原密码错误"));
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.NewPassword);
        context.SaveChanges();

        return Ok(ApiResult.Success("密码修改成功"));
    }
}

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string Roles { get; set; } = string.Empty;
    public AppUserStatus Status { get; set; }
}

public class UpdateProfileDto
{
    public string? RealName { get; set; }
    public string? Avatar { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}

public class ChangePasswordDto
{
    public string OldPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}