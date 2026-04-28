using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Dtos.Users;
using WebApplication1.Services.Interfaces;

namespace WebApplication1.Services.Implementations;

public class AuthService(IConfiguration configuration) : IAuthService
{
    private static readonly IReadOnlyDictionary<string, AppUserDefinition> Users =
        new Dictionary<string, AppUserDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            ["vben"] = new(
                UserId: "10000",
                Username: "vben",
                Password: "123456",
                RealName: "Vben Super",
                Avatar: "https://avatar.vercel.sh/vben",
                HomePath: "/growth/dashboard",
                Roles: ["super"],
                AccessCodes: ["dashboard", "growth"]),
            ["admin"] = new(
                UserId: "10001",
                Username: "admin",
                Password: "123456",
                RealName: "Admin User",
                Avatar: "https://avatar.vercel.sh/admin",
                HomePath: "/growth/dashboard",
                Roles: ["admin"],
                AccessCodes: ["dashboard", "growth"]),
            ["jack"] = new(
                UserId: "10002",
                Username: "jack",
                Password: "123456",
                RealName: "Jack User",
                Avatar: "https://avatar.vercel.sh/jack",
                HomePath: "/growth/dashboard",
                Roles: ["user"],
                AccessCodes: ["dashboard", "growth"]),
        };

    public string CreateAccessToken(string username)
    {
        if (!Users.TryGetValue(username, out var user))
        {
            throw new InvalidOperationException("User does not exist.");
        }

        var jwtSecretKey = configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey is not configured.");
        var jwtIssuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
        var jwtAudience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience is not configured.");
        var expireMinutes = configuration.GetValue("Jwt:ExpireMinutes", 120);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.GivenName, user.RealName),
            new("avatar", user.Avatar),
            new("homePath", user.HomePath),
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = jwtAudience,
            Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
            Issuer = jwtIssuer,
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public IReadOnlyList<string> GetAccessCodes(ClaimsPrincipal principal)
    {
        var username = principal.Identity?.Name;
        return username is not null && Users.TryGetValue(username, out var user)
            ? user.AccessCodes
            : [];
    }

    public UserInfoDto? GetUserInfo(ClaimsPrincipal principal)
    {
        var username = principal.Identity?.Name;
        return username is not null && Users.TryGetValue(username, out var user)
            ? ToUserInfo(user)
            : null;
    }

    public UserInfoDto? ValidateUser(string username, string password)
    {
        if (!Users.TryGetValue(username, out var user))
        {
            return null;
        }

        return string.Equals(user.Password, password, StringComparison.Ordinal)
            ? ToUserInfo(user)
            : null;
    }

    private static UserInfoDto ToUserInfo(AppUserDefinition user)
    {
        return new UserInfoDto
        {
            Avatar = user.Avatar,
            HomePath = user.HomePath,
            RealName = user.RealName,
            Roles = user.Roles,
            UserId = user.UserId,
            Username = user.Username,
        };
    }

    private sealed record AppUserDefinition(
        string UserId,
        string Username,
        string Password,
        string RealName,
        string Avatar,
        string HomePath,
        IReadOnlyList<string> Roles,
        IReadOnlyList<string> AccessCodes);
}
