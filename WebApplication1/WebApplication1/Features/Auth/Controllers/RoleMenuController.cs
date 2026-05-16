using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Auth.Dtos;
using WebApplication1.Features.Auth.Services;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth.Controllers;

[ApiController]
[Route("api/system/role-menus")]
[Authorize]
[Tags("Admin - Role Menus")]
public class RoleMenuController(
    RoleMenuService roleMenuService,
    IUserAccessContextService accessContextService,
    AppDbContext context) : ControllerBase
{
    [HttpGet("mine")]
    public async Task<ActionResult<ApiResult<List<RoleMenuDto>>>> GetMyMenus(CancellationToken cancellationToken)
    {
        var userId = await GetCurrentExistingUserIdAsync(cancellationToken);
        if (!userId.HasValue)
            return Ok(ApiResult<List<RoleMenuDto>>.Success(new()));

        var access = await accessContextService.GetAsync(userId.Value, cancellationToken);
        if (access is null)
            return Ok(ApiResult<List<RoleMenuDto>>.Success(new()));

        var menus = await roleMenuService.GetMenusForUserAsync(access, cancellationToken);
        var result = menus.Select(x => x.ToDto()).ToList();

        return Ok(ApiResult<List<RoleMenuDto>>.Success(result));
    }

    [HttpGet]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult<List<RoleMenuDto>>>> GetAll(CancellationToken cancellationToken)
    {
        var menus = await roleMenuService.GetAllAsync(cancellationToken);
        return Ok(ApiResult<List<RoleMenuDto>>.Success(menus.Select(x => x.ToDto()).ToList()));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult<RoleMenuDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var menu = await roleMenuService.GetByIdAsync(id, cancellationToken);
        if (menu is null) return NotFound(ApiResult<RoleMenuDto>.Fail("菜单不存在"));
        return Ok(ApiResult<RoleMenuDto>.Success(menu.ToDto()));
    }

    [HttpPost]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult<RoleMenuDto>>> Create([FromBody] UpsertRoleMenuDto input, CancellationToken cancellationToken)
    {
        var result = await roleMenuService.CreateAsync(input.ToEntity(), cancellationToken);
        return Ok(ApiResult<RoleMenuDto>.Success(result.ToDto(), "创建成功"));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult<RoleMenuDto>>> Update(Guid id, [FromBody] UpsertRoleMenuDto input, CancellationToken cancellationToken)
    {
        var result = await roleMenuService.UpdateAsync(id, input.ToEntity(), cancellationToken);
        if (result is null) return NotFound(ApiResult<RoleMenuDto>.Fail("菜单不存在"));
        return Ok(ApiResult<RoleMenuDto>.Success(result.ToDto(), "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var success = await roleMenuService.DeleteAsync(id, cancellationToken);
        return success ? Ok(ApiResult.Success("删除成功")) : NotFound(ApiResult.Fail("菜单不存在"));
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private async Task<Guid?> GetCurrentExistingUserIdAsync(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId.HasValue && await context.Users.AnyAsync(x => x.Id == userId.Value, cancellationToken))
        {
            return userId;
        }

        var username = User.Identity?.Name;
        if (string.IsNullOrWhiteSpace(username))
        {
            return null;
        }

        return await context.Users
            .AsNoTracking()
            .Where(x => x.Username == username)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
