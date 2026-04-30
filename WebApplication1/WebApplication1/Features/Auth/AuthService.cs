using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth;

public class AuthService(
    IConfiguration configuration,
    AppDbContext context) : IAuthService
{
    public string CreateAccessToken(string username)
    {
        var user = context.Users.FirstOrDefault(x => x.Username == username);
        if (user == null)
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

        var roles = user.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.GivenName, user.RealName),
            new("avatar", user.Avatar ?? string.Empty),
            new("homePath", "/growth/dashboard"),
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role.Trim())));

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
        if (username is null) return [];
        var user = context.Users.FirstOrDefault(x => x.Username == username);
        return user != null ? new[] { "dashboard", "growth" } : [];
    }

    public UserInfoDto? GetUserInfo(ClaimsPrincipal principal)
    {
        var username = principal.Identity?.Name;
        if (username is null) return null;
        var user = context.Users.FirstOrDefault(x => x.Username == username);
        if (user == null) return null;

        var roles = user.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries);

        return new UserInfoDto
        {
            Avatar = user.Avatar ?? string.Empty,
            HomePath = "/growth/dashboard",
            RealName = user.RealName,
            Roles = roles.Select(r => r.Trim()).ToArray(),
            UserId = user.Id.ToString(),
            Username = user.Username,
        };
    }

    public UserInfoDto? ValidateUser(string username, string password)
    {
        var user = context.Users.FirstOrDefault(x => x.Username == username);
        if (user == null) return null;

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }

        var roles = user.Roles.Split(',', StringSplitOptions.RemoveEmptyEntries);

        return new UserInfoDto
        {
            Avatar = user.Avatar ?? string.Empty,
            HomePath = "/growth/dashboard",
            RealName = user.RealName,
            Roles = roles.Select(r => r.Trim()).ToArray(),
            UserId = user.Id.ToString(),
            Username = user.Username,
        };
    }
}