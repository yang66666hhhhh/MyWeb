using Microsoft.Extensions.Localization;

namespace WebApplication1.Shared.Localization;

public class SharedResource
{
    private readonly IStringLocalizer _localizer;

    public SharedResource(IStringLocalizer<SharedResource> localizer)
    {
        _localizer = localizer;
    }

    public string this[string name] => _localizer[name];
    public string this[string name, params object[] arguments] => _localizer[name, arguments];
}

public static class SharedResourceExtensions
{
    public static string Localize(this IStringLocalizer localizer, string key, params object[] arguments)
    {
        return arguments.Length > 0 ? localizer[key, arguments] : localizer[key];
    }
}
