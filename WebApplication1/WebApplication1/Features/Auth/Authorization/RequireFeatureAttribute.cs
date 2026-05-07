using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Data;

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

        var db = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

        // 获取用户活跃订阅的计划
        var subscription = await db.UserSubscriptions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        var planCode = subscription?.Plan?.Code ?? "Free";

        // 检查计划是否包含该功能
        var hasFeatureInPlan = await db.PlanFeatures
            .AsNoTracking()
            .AnyAsync(x => x.Plan!.Code == planCode && x.Feature!.Code == FeatureCode);

        if (hasFeatureInPlan) return; // 计划包含，通过

        // 检查 Persona 是否包含该功能（使用子查询避免 EF Core 类型映射问题）
        var hasFeatureInPersona = await db.PersonaFeatures
            .AsNoTracking()
            .AnyAsync(x => x.Feature!.Code == FeatureCode &&
                db.UserPersonas.Any(up => up.UserId == userId && up.PersonaType!.Code == x.PersonaCode));

        if (!hasFeatureInPersona)
        {
            context.Result = new ForbidResult();
        }
    }
}
