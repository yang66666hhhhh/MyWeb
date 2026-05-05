using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Features.Auth.Services;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth.Controllers;

[ApiController]
[Route("api/role-menus")]
[Authorize]
public class RoleMenuController(RoleMenuService roleMenuService, AppDbContext context) : ControllerBase
{
    [HttpGet("mine")]
    public async Task<ActionResult<ApiResult<List<RoleMenu>>>> GetMyMenus(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Ok(ApiResult<List<RoleMenu>>.Success(new()));

        var user = await context.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId.Value, cancellationToken);

        if (user is null)
            return Ok(ApiResult<List<RoleMenu>>.Success(new()));

        var roleCode = GetHighestRole(user.Roles);

        var personaCodes = await context.UserPersonas
            .Where(x => x.UserId == userId.Value)
            .Select(x => x.PersonaType!.Code)
            .ToListAsync(cancellationToken);

        var availableFeatures = await GetUserAvailableFeaturesAsync(userId.Value, cancellationToken);

        var menus = await roleMenuService.GetMenusForUserAsync(
            userId.Value, roleCode, personaCodes, availableFeatures, cancellationToken);

        return Ok(ApiResult<List<RoleMenu>>.Success(menus));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<List<RoleMenu>>>> GetAll(CancellationToken cancellationToken)
    {
        var menus = await roleMenuService.GetAllAsync(cancellationToken);
        return Ok(ApiResult<List<RoleMenu>>.Success(menus));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<RoleMenu>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var menu = await roleMenuService.GetByIdAsync(id, cancellationToken);
        if (menu is null) return NotFound(ApiResult<RoleMenu>.Fail("菜单不存在"));
        return Ok(ApiResult<RoleMenu>.Success(menu));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<RoleMenu>>> Create([FromBody] RoleMenu input, CancellationToken cancellationToken)
    {
        var result = await roleMenuService.CreateAsync(input, cancellationToken);
        return Ok(ApiResult<RoleMenu>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<RoleMenu>>> Update(Guid id, [FromBody] RoleMenu input, CancellationToken cancellationToken)
    {
        var result = await roleMenuService.UpdateAsync(id, input, cancellationToken);
        if (result is null) return NotFound(ApiResult<RoleMenu>.Fail("菜单不存在"));
        return Ok(ApiResult<RoleMenu>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await roleMenuService.DeleteAsync(id, cancellationToken);
        return success ? Ok(ApiResult.Success("删除成功")) : NotFound(ApiResult.Fail("菜单不存在"));
    }

    private async Task<HashSet<string>> GetUserAvailableFeaturesAsync(Guid userId, CancellationToken ct)
    {
        // 1. 获取用户活跃订阅
        var subscription = await context.UserSubscriptions
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.IsActive)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(ct);

        // 2. 如果没有订阅，默认 Free 计划
        var planCode = subscription?.Plan?.Code ?? "Free";

        // 3. 获取该计划包含的功能
        var planFeatures = await context.PlanFeatures
            .AsNoTracking()
            .Where(x => x.Plan!.Code == planCode)
            .Select(x => x.Feature!.Code)
            .ToListAsync(ct);

        // 4. 获取用户 Persona 包含的功能
        var personaFeatures = await context.PersonaFeatures
            .AsNoTracking()
            .Where(x => context.UserPersonas
                .Where(up => up.UserId == userId)
                .Select(up => up.PersonaType!.Code)
                .Contains(x.PersonaCode))
            .Select(x => x.Feature!.Code)
            .ToListAsync(ct);

        // 5. 合并：计划功能 + Persona功能
        var allFeatures = planFeatures.Union(personaFeatures).ToHashSet();
        return allFeatures;
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private static string GetHighestRole(string roles)
    {
        var list = roles.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(r => r.Trim().ToLower());
        if (list.Contains("owner")) return "owner";
        if (list.Contains("pro")) return "pro";
        return "member";
    }
}
