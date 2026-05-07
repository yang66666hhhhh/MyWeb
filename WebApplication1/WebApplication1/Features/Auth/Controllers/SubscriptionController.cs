using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Auth.Entities.Subscription;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth.Controllers;

[ApiController]
[Route("api/subscriptions")]
[Authorize]
[Tags("Subscriptions")]
public class SubscriptionController(AppDbContext context) : BaseApiController
{
    [HttpGet("my")]
    public async Task<ActionResult<ApiResult<UserSubscription?>>> GetMySubscription(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var subscription = await context.UserSubscriptions
            .AsNoTracking()
            .Include(x => x.Plan)
            .Where(x => x.UserId == userId.Value && x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(ct);

        return Ok(ApiResult<UserSubscription?>.Success(subscription));
    }

    [HttpGet("my/features")]
    public async Task<ActionResult<ApiResult<List<string>>>> GetMyFeatures(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var features = await GetUserFeatureCodesAsync(userId.Value, ct);
        return Ok(ApiResult<List<string>>.Success(features.ToList()));
    }

    [HttpPost("subscribe")]
    public async Task<ActionResult<ApiResult>> Subscribe([FromBody] SubscribeRequest request, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var plan = await context.Plans.FindAsync([request.PlanId], ct);
        if (plan == null || !plan.IsActive)
            return BadRequest(ApiResult.Fail("无效的订阅计划"));

        // 停用当前订阅
        var current = await context.UserSubscriptions
            .Where(x => x.UserId == userId.Value && x.IsActive)
            .ToListAsync(ct);
        foreach (var sub in current)
            sub.IsActive = false;

        // 创建新订阅
        var subscription = new UserSubscription
        {
            UserId = userId.Value,
            PlanId = request.PlanId,
            StartAt = DateTime.UtcNow,
            ExpireAt = plan.BillingCycle > 0 ? DateTime.UtcNow.AddDays(plan.BillingCycle) : null,
            IsActive = true
        };
        context.UserSubscriptions.Add(subscription);
        await context.SaveChangesAsync(ct);

        return Ok(ApiResult.Success("订阅成功"));
    }

    [HttpGet("plans")]
    public async Task<ActionResult<ApiResult<List<Plan>>>> GetPlans(CancellationToken ct)
    {
        var plans = await context.Plans
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Sort)
            .ToListAsync(ct);
        return Ok(ApiResult<List<Plan>>.Success(plans));
    }

    [HttpGet("plans/{id:guid}/features")]
    public async Task<ActionResult<ApiResult<List<Feature>>>> GetPlanFeatures(Guid id, CancellationToken ct)
    {
        var features = await context.PlanFeatures
            .AsNoTracking()
            .Where(x => x.PlanId == id)
            .Select(x => x.Feature!)
            .ToListAsync(ct);
        return Ok(ApiResult<List<Feature>>.Success(features));
    }

    private async Task<HashSet<string>> GetUserFeatureCodesAsync(Guid userId, CancellationToken ct)
    {
        var subscription = await context.UserSubscriptions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(ct);

        var planCode = subscription?.Plan?.Code ?? "Free";

        var planFeatures = await context.PlanFeatures
            .AsNoTracking()
            .Where(x => x.Plan!.Code == planCode)
            .Select(x => x.Feature!.Code)
            .ToListAsync(ct);

        var personaFeatures = await context.PersonaFeatures
            .AsNoTracking()
            .Where(x => context.UserPersonas
                .Where(up => up.UserId == userId)
                .Select(up => up.PersonaType!.Code)
                .Contains(x.PersonaCode))
            .Select(x => x.Feature!.Code)
            .ToListAsync(ct);

        return planFeatures.Union(personaFeatures).ToHashSet();
    }
}

public class SubscribeRequest
{
    public Guid PlanId { get; set; }
}
