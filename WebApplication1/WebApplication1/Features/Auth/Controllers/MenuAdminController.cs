using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth.Controllers;

[ApiController]
[Authorize(Roles = "owner")]
[Route("api/system/menus")]
[Tags("Admin - Menus")]
public class MenuAdminController(AppDbContext db, ILogger<MenuAdminController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<List<MenuTreeAdminDto>>>> GetAll()
    {
        var menuItems = await db.MenuItems
            .Include(m => m.MenuTags)
            .ThenInclude(mt => mt.Tag)
            .OrderBy(m => m.Sort)
            .ToListAsync();

        var result = BuildTree(menuItems, null);
        return Ok(ApiResult<List<MenuTreeAdminDto>>.Success(result));
    }

    [HttpGet("paths")]
    public async Task<ActionResult<ApiResult<List<MenuPathDto>>>> GetAllPaths()
    {
        var existingPaths = await db.MenuItems.Select(m => m.Path).ToHashSetAsync();

        var configs = await db.MenuConfigs
            .Where(c => c.IsActive)
            .OrderBy(c => c.Sort)
            .ToListAsync();

        var result = configs.Select(c => new MenuPathDto
        {
            Path = c.Path,
            Name = c.Name,
            Icon = c.Icon,
            Description = c.Description,
            Exists = existingPaths.Contains(c.Path)
        }).ToList();

        return Ok(ApiResult<List<MenuPathDto>>.Success(result));
    }

    private static List<MenuTreeAdminDto> BuildTree(List<MenuItem> all, Guid? parentId)
    {
        return all.Where(m => m.ParentId == parentId).Select(m => new MenuTreeAdminDto
        {
            Id = m.Id,
            Name = m.Name,
            Path = m.Path,
            Icon = m.Icon,
            Sort = m.Sort,
            IsActive = m.IsActive,
            ParentId = m.ParentId,
            TagIds = m.MenuTags.Select(mt => mt.TagId).ToList(),
            TagNames = m.MenuTags.Select(mt => mt.Tag.Name).ToList(),
            Children = BuildTree(all, m.Id)
        }).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<MenuItemDto>>> Create([FromBody] CreateMenuItemDto input)
    {
        try
        {
            if (await db.MenuItems.AnyAsync(m => m.Path == input.Path))
                return BadRequest(ApiResult<MenuItemDto>.Fail("路径已存在"));

            var menuItem = new MenuItem
            {
                Name = input.Name,
                Path = input.Path,
                Icon = input.Icon,
                Sort = input.Sort,
                ParentId = input.ParentId,
                IsActive = true
            };
            db.MenuItems.Add(menuItem);
            await db.SaveChangesAsync();

            if (input.TagIds?.Count > 0)
            {
                foreach (var tagId in input.TagIds)
                {
                    db.MenuTags.Add(new MenuTag { MenuItemId = menuItem.Id, TagId = tagId });
                }
                await db.SaveChangesAsync();
            }

            var tags = input.TagIds != null 
                ? await db.Tags.Where(t => input.TagIds.Contains(t.Id)).Select(t => t.Name).ToListAsync()
                : new List<string>();
            logger.LogInformation("创建菜单成功: {Id}", menuItem.Id);
            return Ok(ApiResult<MenuItemDto>.Success(new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Path = menuItem.Path,
                Icon = menuItem.Icon,
                Sort = menuItem.Sort,
                ParentId = menuItem.ParentId,
                IsActive = menuItem.IsActive,
                TagIds = input.TagIds ?? new List<Guid>(),
                TagNames = tags
            }, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建菜单失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<MenuItemDto>>> Update(Guid id, [FromBody] CreateMenuItemDto input)
    {
        try
        {
            var menuItem = await db.MenuItems
                .Include(m => m.MenuTags)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
                return NotFound(ApiResult<MenuItemDto>.Fail("菜单不存在"));

            if (await db.MenuItems.AnyAsync(m => m.Path == input.Path && m.Id != id))
                return BadRequest(ApiResult<MenuItemDto>.Fail("路径已存在"));

            menuItem.Name = input.Name;
            menuItem.Path = input.Path;
            menuItem.Icon = input.Icon;
            menuItem.Sort = input.Sort;
            menuItem.ParentId = input.ParentId;

            db.MenuTags.RemoveRange(menuItem.MenuTags);
            if (input.TagIds?.Count > 0)
            {
                foreach (var tagId in input.TagIds)
                {
                    db.MenuTags.Add(new MenuTag { MenuItemId = menuItem.Id, TagId = tagId });
                }
            }
            await db.SaveChangesAsync();

            var tags = input.TagIds != null 
                ? await db.Tags.Where(t => input.TagIds.Contains(t.Id)).Select(t => t.Name).ToListAsync()
                : new List<string>();
            logger.LogInformation("更新菜单成功: {Id}", id);
            return Ok(ApiResult<MenuItemDto>.Success(new MenuItemDto
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Path = menuItem.Path,
                Icon = menuItem.Icon,
                Sort = menuItem.Sort,
                ParentId = menuItem.ParentId,
                IsActive = menuItem.IsActive,
                TagIds = input.TagIds ?? new List<Guid>(),
                TagNames = tags
            }, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新菜单失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id)
    {
        try
        {
            var menuItem = await db.MenuItems.FindAsync([id]);
            if (menuItem == null)
                return NotFound(ApiResult.Fail("菜单不存在"));

            db.MenuItems.Remove(menuItem);
            await db.SaveChangesAsync();
            logger.LogInformation("删除菜单成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除菜单失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }
}

public class MenuTreeAdminDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int Sort { get; set; }
    public bool IsActive { get; set; }
    public Guid? ParentId { get; set; }
    public List<Guid> TagIds { get; set; } = new();
    public List<string> TagNames { get; set; } = new();
    public List<MenuTreeAdminDto> Children { get; set; } = new();
}

public class MenuItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int Sort { get; set; }
    public Guid? ParentId { get; set; }
    public bool IsActive { get; set; }
    public List<Guid> TagIds { get; set; } = new();
    public List<string> TagNames { get; set; } = new();
}

public class CreateMenuItemDto
{
    [Required(ErrorMessage = "菜单名称不能为空")]
    [StringLength(100, ErrorMessage = "菜单名称不能超过100个字符")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "路由路径不能为空")]
    [StringLength(200, ErrorMessage = "路由路径不能超过200个字符")]
    public string Path { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "图标不能超过50个字符")]
    public string? Icon { get; set; }

    public int Sort { get; set; }
    public Guid? ParentId { get; set; }
    public List<Guid>? TagIds { get; set; }
}

public class MenuPathDto
{
    public string Path { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public bool Exists { get; set; }
}
