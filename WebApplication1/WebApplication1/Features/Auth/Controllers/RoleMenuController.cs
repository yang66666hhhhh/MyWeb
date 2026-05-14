using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Features.Auth.Services;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Auth.Controllers;

[ApiController]
[Route("api/role-menus")]
[Authorize]
[Tags("Admin - Role Menus")]
public class RoleMenuController(RoleMenuService roleMenuService, IUserAccessContextService accessContextService) : ControllerBase
{
    [HttpGet("mine")]
    public async Task<ActionResult<ApiResult<List<RoleMenu>>>> GetMyMenus(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Ok(ApiResult<List<RoleMenu>>.Success(new()));

        var access = await accessContextService.GetAsync(userId.Value, cancellationToken);
        if (access is null)
            return Ok(ApiResult<List<RoleMenu>>.Success(new()));

        var menus = await roleMenuService.GetMenusForUserAsync(access, cancellationToken);

        return Ok(ApiResult<List<RoleMenu>>.Success(menus));
    }

    [HttpGet]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult<List<RoleMenu>>>> GetAll(CancellationToken cancellationToken)
    {
        var menus = await roleMenuService.GetAllAsync(cancellationToken);
        return Ok(ApiResult<List<RoleMenu>>.Success(menus));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult<RoleMenu>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var menu = await roleMenuService.GetByIdAsync(id, cancellationToken);
        if (menu is null) return NotFound(ApiResult<RoleMenu>.Fail("菜单不存在"));
        return Ok(ApiResult<RoleMenu>.Success(menu));
    }

    [HttpPost]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult<RoleMenu>>> Create([FromBody] RoleMenu input, CancellationToken cancellationToken)
    {
        var result = await roleMenuService.CreateAsync(input, cancellationToken);
        return Ok(ApiResult<RoleMenu>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult<RoleMenu>>> Update(Guid id, [FromBody] RoleMenu input, CancellationToken cancellationToken)
    {
        var result = await roleMenuService.UpdateAsync(id, input, cancellationToken);
        if (result is null) return NotFound(ApiResult<RoleMenu>.Fail("菜单不存在"));
        return Ok(ApiResult<RoleMenu>.Success(result, "更新成功"));
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
}
