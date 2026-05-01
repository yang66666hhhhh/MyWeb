using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth.Controllers;

[ApiController]
[Authorize]
[Route("api/menus")]
public class MenuController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<List<MenuRouteDto>>>> GetMenus()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Ok(ApiResult<List<MenuRouteDto>>.Success(new List<MenuRouteDto>()));

        var user = await db.Users
            .Include(u => u.UserTags).ThenInclude(ut => ut.Tag)
            .Include(u => u.UserType).ThenInclude(ut => ut!.UserTypeTags).ThenInclude(utt => utt.Tag)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            return Ok(ApiResult<List<MenuRouteDto>>.Success(new List<MenuRouteDto>()));

        var allMenuItems = await db.MenuItems
            .Where(m => m.IsActive)
            .Include(m => m.MenuTags).ThenInclude(mt => mt.Tag)
            .OrderBy(m => m.Sort)
            .ToListAsync();

        var role = (user.Roles ?? "").ToLower();
        List<MenuItem> topMenus;

        if (role == "super")
        {
            topMenus = allMenuItems.Where(m => m.ParentId == null).ToList();
        }
        else if (role == "admin")
        {
            var adminTag = await db.Tags.FirstOrDefaultAsync(t => t.Name == "系统配置");
            var adminTagIds = adminTag != null ? new HashSet<Guid> { adminTag.Id } : new HashSet<Guid>();
            
            var userTagIds = GetUserTagIds(user);
            
            topMenus = allMenuItems.Where(m =>
            {
                if (m.ParentId != null) return false;
                if (m.MenuTags.Count == 0) return false;
                var menuTagIds = m.MenuTags.Select(mt => mt.TagId).ToHashSet();
                return menuTagIds.Overlaps(adminTagIds) || menuTagIds.Overlaps(userTagIds);
            }).ToList();
        }
        else
        {
            var userTagIds = GetUserTagIds(user);
            topMenus = allMenuItems.Where(m =>
            {
                if (m.ParentId != null) return false;
                if (m.MenuTags.Count == 0) return false;
                var menuTagIds = m.MenuTags.Select(mt => mt.TagId).ToHashSet();
                return menuTagIds.Overlaps(userTagIds);
            }).ToList();
        }

        var result = BuildTree(topMenus, allMenuItems);
        return Ok(ApiResult<List<MenuRouteDto>>.Success(result));
    }

    private static HashSet<Guid> GetUserTagIds(AppUser user)
    {
        var tagIds = new HashSet<Guid>();
        if (user.UserType?.UserTypeTags != null)
        {
            foreach (var utt in user.UserType.UserTypeTags)
                tagIds.Add(utt.TagId);
        }
        foreach (var ut in user.UserTags)
            tagIds.Add(ut.TagId);
        return tagIds;
    }

    private static List<MenuRouteDto> BuildTree(List<MenuItem> topMenus, List<MenuItem> allItems)
    {
        return topMenus.OrderBy(x => x.Sort).Select(x => new MenuRouteDto
        {
            Id = x.Id.ToString(),
            Path = x.Path,
            Name = x.Name,
            Component = x.Path,
            Meta = new MenuMetaDto { Title = x.Name, Icon = x.Icon, Order = x.Sort },
            Children = BuildTree(allItems.Where(m => m.ParentId == x.Id).ToList(), allItems)
        }).ToList();
    }
}

public class MenuRouteDto
{
    public string Id { get; set; } = "";
    public string Path { get; set; } = "";
    public string Name { get; set; } = "";
    public string Component { get; set; } = "";
    public MenuMetaDto Meta { get; set; } = new();
    public List<MenuRouteDto> Children { get; set; } = new();
}

public class MenuMetaDto
{
    public string Title { get; set; } = "";
    public string? Icon { get; set; }
    public int Order { get; set; }
}