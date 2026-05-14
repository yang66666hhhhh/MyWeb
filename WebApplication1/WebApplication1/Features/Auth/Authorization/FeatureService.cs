namespace WebApplication1.Features.Auth.Authorization;

public interface IFeatureService
{
    Task<bool> HasFeatureAsync(Guid userId, string featureCode);
    Task<HashSet<string>> GetUserFeaturesAsync(Guid userId);
}

public class FeatureService : IFeatureService
{
    private readonly IUserAccessContextService _accessContextService;

    public FeatureService(IUserAccessContextService accessContextService)
    {
        _accessContextService = accessContextService;
    }

    public async Task<bool> HasFeatureAsync(Guid userId, string featureCode)
    {
        var features = await GetUserFeaturesAsync(userId);
        return features.Contains(featureCode);
    }

    public async Task<HashSet<string>> GetUserFeaturesAsync(Guid userId)
    {
        var access = await _accessContextService.GetAsync(userId);
        return access?.FeatureCodes.ToHashSet(StringComparer.OrdinalIgnoreCase)
            ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
}
