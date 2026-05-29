using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth;

public class AuthService(
    IConfiguration configuration,
    AppDbContext context,
    IUserAccessContextService accessContextService) : IAuthService
{
    public async Task<string> CreateAccessTokenAsync(string username)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Username == username);
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
        var expireMinutes = configuration.GetValue("Jwt:ExpireMinutes", 15);

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

    public async Task<string> CreateRefreshTokenAsync(Guid userId, string? createdByIp = null)
    {
        var refreshToken = new RefreshToken
        {
            UserId = userId,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddDays(configuration.GetValue("Jwt:RefreshTokenExpireDays", 7)),
            CreatedByIp = createdByIp
        };

        context.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        return refreshToken.Token;
    }

    public async Task<bool> ValidateRefreshTokenAsync(string token, Guid userId)
    {
        var refreshToken = await context.RefreshTokens.FirstOrDefaultAsync(x =>
            x.Token == token && x.UserId == userId);

        if (refreshToken == null)
            return false;

        if (refreshToken.IsRevoked)
            return false;

        if (refreshToken.ExpiresAt <= DateTime.UtcNow)
            return false;

        return true;
    }

    public async Task<bool> RevokeRefreshTokenAsync(string token, Guid userId)
    {
        var refreshToken = await context.RefreshTokens.FirstOrDefaultAsync(x =>
            x.Token == token && x.UserId == userId);

        if (refreshToken == null)
            return false;

        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<IReadOnlyList<string>> GetAccessCodesAsync(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return [];

        var accessContext = await accessContextService.GetAsync(userId);
        if (accessContext is null)
            return [];

        return accessContext.FeatureCodes.ToList();
    }

    public async Task<UserInfoDto?> GetUserInfoAsync(ClaimsPrincipal principal)
    {
        var username = principal.Identity?.Name;
        if (username is null) return null;
        var user = await context.Users.FirstOrDefaultAsync(x => x.Username == username);
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

    public async Task<UserInfoDto?> ValidateUserAsync(string username, string password)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Username == username);
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
