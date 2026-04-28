using System.Security.Claims;
using WebApplication1.Dtos.Users;

namespace WebApplication1.Services.Interfaces;

public interface IAuthService
{
    string CreateAccessToken(string username);

    IReadOnlyList<string> GetAccessCodes(ClaimsPrincipal principal);

    UserInfoDto? GetUserInfo(ClaimsPrincipal principal);

    UserInfoDto? ValidateUser(string username, string password);
}
