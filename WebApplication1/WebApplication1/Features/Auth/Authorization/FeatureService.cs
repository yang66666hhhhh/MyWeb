using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth.Authorization;

public interface IFeatureService
{
    Task<bool> HasFeatureAsync(Guid userId, string featureCode);
    Task<HashSet<string>> GetUserFeaturesAsync(Guid userId);
}

public class FeatureService : IFeatureService
{
    private readonly AppDbContext _db;

    public FeatureService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> HasFeatureAsync(Guid userId, string featureCode)
    {
        var features = await GetUserFeaturesAsync(userId);
        return features.Contains(featureCode);
    }

    public async Task<HashSet<string>> GetUserFeaturesAsync(Guid userId)
    {
        // 获取用户订阅计划
        var subscription = await _db.UserSubscriptions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        var planCode = subscription?.Plan?.Code ?? "Free";

        // 计划功能
        var planFeatures = await _db.PlanFeatures
            .AsNoTracking()
            .Where(x => x.Plan!.Code == planCode)
            .Select(x => x.Feature!.Code)
            .ToListAsync();

        // Persona 功能
        var personaFeatures = await _db.PersonaFeatures
            .AsNoTracking()
            .Where(x => _db.UserPersonas
                .Where(up => up.UserId == userId)
                .Select(up => up.PersonaType!.Code)
                .Contains(x.PersonaCode))
            .Select(x => x.Feature!.Code)
            .ToListAsync();

        return planFeatures.Union(personaFeatures).ToHashSet();
    }
}
