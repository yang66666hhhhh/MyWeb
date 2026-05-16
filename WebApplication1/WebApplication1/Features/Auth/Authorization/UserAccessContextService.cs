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
            .Select(x => new FeatureGrant(x.Feature.Code, x.Feature.Category))
            .ToListAsync(cancellationToken);

        if (roleLevel >= 3)
        {
            planFeatures = await db.Features
                .AsNoTracking()
                .Where(x => x.IsEnabled)
                .Select(x => new FeatureGrant(x.Code, x.Category))
                .ToListAsync(cancellationToken);
        }
        else if (roleLevel >= 2 && planFeatures.Count == 0)
        {
            planFeatures = await db.PlanFeatures
                .AsNoTracking()
                .Where(x => x.Plan.Code == "Pro" && x.Feature.IsEnabled)
                .Select(x => new FeatureGrant(x.Feature.Code, x.Feature.Category))
                .ToListAsync(cancellationToken);
        }

        if (planFeatures.Count == 0 && planCode.Equals("Free", StringComparison.OrdinalIgnoreCase))
        {
            planFeatures = await LoadDefaultFreeFeaturesAsync(cancellationToken);
        }

        var personaFeatureCodes = await db.PersonaFeatures
            .AsNoTracking()
            .Where(x => x.Feature.IsEnabled &&
                db.UserPersonas.Any(up =>
                    up.UserId == userId &&
                    up.PersonaType!.IsActive &&
                    up.PersonaType.Code == x.PersonaCode))
            .Select(x => x.Feature.Code)
            .ToListAsync(cancellationToken);

        var effectiveFeatureCodes = ResolveEffectiveFeatureCodes(planFeatures, personaFeatureCodes);

        return new UserAccessContext(
            user.Id,
            roleCode,
            roleLevel,
            personaCodes.ToHashSet(StringComparer.OrdinalIgnoreCase),
            planCode,
            effectiveFeatureCodes);
    }

    private async Task<List<FeatureGrant>> LoadDefaultFreeFeaturesAsync(CancellationToken cancellationToken)
    {
        var fallbackCodes = new[]
        {
            "GROWTH_DAILY_PLAN",
            "GROWTH_HABIT",
            "GROWTH_KNOWLEDGE",
            "WORK_LOG",
            "WORK_TASK",
            "STUDENT_EXAM",
            "STUDENT_LEARNING",
            "STUDENT_MISTAKES",
            "STUDENT_REVIEW",
            "STUDENT_MATERIALS",
            "STUDENT_RECORDS",
            "STUDENT_SUBJECTS",
        };

        var configuredFeatures = await db.Features
            .AsNoTracking()
            .Where(x => x.IsEnabled && fallbackCodes.Contains(x.Code))
            .Select(x => new FeatureGrant(x.Code, x.Category))
            .ToListAsync(cancellationToken);

        var configuredCodes = configuredFeatures
            .Select(x => x.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        configuredFeatures.AddRange(fallbackCodes
            .Where(code => !configuredCodes.Contains(code))
            .Select(code => new FeatureGrant(code, GetDefaultFeatureCategory(code))));

        return configuredFeatures;
    }

    private static HashSet<string> ResolveEffectiveFeatureCodes(
        IEnumerable<FeatureGrant> planFeatures,
        IEnumerable<string> personaFeatureCodes)
    {
        var personaFeatureSet = personaFeatureCodes.ToHashSet(StringComparer.OrdinalIgnoreCase);

        return planFeatures
            .Where(feature =>
            feature.Category.Equals("Persona", StringComparison.OrdinalIgnoreCase)
                ? personaFeatureSet.Contains(feature.Code)
                : true)
            .Select(feature => feature.Code)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
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

    private static string GetDefaultFeatureCategory(string code)
    {
        return code.StartsWith("STUDENT_", StringComparison.OrdinalIgnoreCase) ? "Persona" : string.Empty;
    }

    private sealed record FeatureGrant(string Code, string Category);
}
