using Microsoft.Extensions.Localization;

namespace WebApplication1.Shared.Localization;

public interface ILocalizationService
{
    string GetString(string key);
    string GetString(string key, params object[] arguments);
    string GetString(string key, string culture);
    string GetString(string key, string culture, params object[] arguments);
}

public class LocalizationService(IStringLocalizer<SharedResource> localizer) : ILocalizationService
{
    public string GetString(string key)
    {
        return localizer[key];
    }

    public string GetString(string key, params object[] arguments)
    {
        return localizer[key, arguments];
    }

    public string GetString(string key, string culture)
    {
        // 暂时忽略 culture 参数，使用当前请求的文化
        return localizer[key];
    }

    public string GetString(string key, string culture, params object[] arguments)
    {
        // 暂时忽略 culture 参数，使用当前请求的文化
        return localizer[key, arguments];
    }
}
