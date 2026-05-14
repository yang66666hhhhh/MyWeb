using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth.Authorization;

public interface IUserAccessContextService
{
    Task<UserAccessContext?> GetAsync(Guid userId, CancellationToken cancellationToken = default);
}

public sealed record UserAccessContext(
    Guid UserId,
    string RoleCode,
    int RoleLevel,
    IReadOnlySet<string> PersonaCodes,
    string PlanCode,
    IReadOnlySet<string> FeatureCodes);

public class UserAccessContextService(AppDbContext db) : IUserAccessContextService
{
    public async Task<UserAccessContext?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await db.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new { x.Id, x.Roles })
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return null;
        }

        var roleCode = GetHighestRole(user.Roles);
        var roleLevel = GetRoleLevel(roleCode);

        var personaCodes = await db.UserPersonas
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.PersonaType!.IsActive)
            .Select(x => x.PersonaType!.Code)
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var planCode = await db.UserSubscriptions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive && (x.ExpireAt == null || x.ExpireAt > now))
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.Plan.Code)
            .FirstOrDefaultAsync(cancellationToken) ?? "Free";

        var planFeatures = await db.PlanFeatures
            .AsNoTracking()
            .Where(x => x.Plan.Code == planCode && x.Feature.IsEnabled)
            .Select(x => x.Feature.Code)
            .ToListAsync(cancellationToken);

        var personaFeatures = await db.PersonaFeatures
            .AsNoTracking()
            .Where(x => x.Feature.IsEnabled &&
                db.UserPersonas.Any(up =>
                    up.UserId == userId &&
                    up.PersonaType!.IsActive &&
                    up.PersonaType.Code == x.PersonaCode))
            .Select(x => x.Feature.Code)
            .ToListAsync(cancellationToken);

        return new UserAccessContext(
            user.Id,
            roleCode,
            roleLevel,
            personaCodes.ToHashSet(StringComparer.OrdinalIgnoreCase),
            planCode,
            planFeatures.Union(personaFeatures, StringComparer.OrdinalIgnoreCase)
                .ToHashSet(StringComparer.OrdinalIgnoreCase));
    }

    private static string GetHighestRole(string roles)
    {
        var list = roles.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(r => r.ToLowerInvariant())
            .ToHashSet();

        if (list.Contains("owner")) return "owner";
        if (list.Contains("pro")) return "pro";
        return "member";
    }

    private static int GetRoleLevel(string roleCode)
    {
        return roleCode.ToLowerInvariant() switch
        {
            "owner" => 3,
            "pro" => 2,
            _ => 1
        };
    }
}
