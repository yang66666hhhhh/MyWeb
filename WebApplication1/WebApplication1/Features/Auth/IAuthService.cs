using System.Security.Claims;

namespace WebApplication1.Features.Auth;

public interface IAuthService
{
    string CreateAccessToken(string username);

    IReadOnlyList<string> GetAccessCodes(ClaimsPrincipal principal);

    UserInfoDto? GetUserInfo(ClaimsPrincipal principal);

    UserInfoDto? ValidateUser(string username, string password);
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
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
}
