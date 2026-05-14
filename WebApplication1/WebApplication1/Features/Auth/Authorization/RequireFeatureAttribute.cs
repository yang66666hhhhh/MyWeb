using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApplication1.Features.Auth.Authorization;

/// <summary>
/// Feature 权限检查特性
/// 使用: [RequireFeature("WORK_LOG")]
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class RequireFeatureAttribute : Attribute, IAsyncAuthorizationFilter
{
    public string FeatureCode { get; }

    public RequireFeatureAttribute(string featureCode)
    {
        FeatureCode = featureCode;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var accessContextService = context.HttpContext.RequestServices.GetRequiredService<IUserAccessContextService>();
        var access = await accessContextService.GetAsync(userId, context.HttpContext.RequestAborted);

        if (access is null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        if (!access.FeatureCodes.Contains(FeatureCode))
        {
            context.Result = new ForbidResult();
        }
    }
}
