using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace WebApplication1.Features.Auth;

public interface IAuthService
{
    Task<string> CreateAccessTokenAsync(string username);

    Task<string> CreateRefreshTokenAsync(Guid userId, string? createdByIp = null);

    Task<bool> ValidateRefreshTokenAsync(string token, Guid userId);

    Task<bool> RevokeRefreshTokenAsync(string token, Guid userId);

    Task<IReadOnlyList<string>> GetAccessCodesAsync(ClaimsPrincipal principal);

    Task<UserInfoDto?> GetUserInfoAsync(ClaimsPrincipal principal);

    Task<UserInfoDto?> ValidateUserAsync(string username, string password);
}

public class UserInfoDto
{
    public string Avatar { get; set; } = string.Empty;

    public string HomePath { get; set; } = "/growth/dashboard";

    public string RealName { get; set; } = string.Empty;

    public IReadOnlyList<string> Roles { get; set; } = [];

    public string UserId { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;
}

public class LoginRequestDto
{
    [Required(ErrorMessage = "用户名不能为空")]
    [MaxLength(50, ErrorMessage = "用户名不能超过50个字符")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "密码不能为空")]
    [MaxLength(100, ErrorMessage = "密码不能超过100个字符")]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public int ExpiresIn { get; set; }
}

public class RefreshTokenRequestDto
{
    [Required(ErrorMessage = "Refresh token不能为空")]
    public string RefreshToken { get; set; } = string.Empty;
}

public class RefreshTokenResponseDto
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public int ExpiresIn { get; set; }
}
