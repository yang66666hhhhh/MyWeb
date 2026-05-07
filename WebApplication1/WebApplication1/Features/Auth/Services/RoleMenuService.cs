using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth.Services;

public class RoleMenuService
{
    private readonly AppDbContext _context;
    private readonly ILogger<RoleMenuService> _logger;

    public RoleMenuService(AppDbContext context, ILogger<RoleMenuService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<RoleMenu>> GetMenusForUserAsync(
        Guid userId, string roleCode,
        HashSet<string> availableFeatureCodes, CancellationToken ct = default)
    {
        var userRoleLevel = roleCode.ToLower() switch
        {
            "owner" => 3,
            "pro" => 2,
            _ => 1
        };

        var userPersonaCodes = await _context.UserPersonas
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.PersonaType!.Code)
            .ToListAsync(ct);

        var allMenus = await _context.RoleMenus
            .AsNoTracking()
            .Where(x => x.IsEnabled)
            .ToListAsync(ct);

        var matched = allMenus
            .Where(m => UserCanViewMenu(m, userRoleLevel, userPersonaCodes, availableFeatureCodes))
            .ToList();

        var matchedIds = matched.Select(x => x.Id).ToHashSet();

        var allParents = new HashSet<Guid>();
        foreach (var menu in matched)
        {
            var pid = menu.ParentId;
            while (pid.HasValue && allParents.Add(pid.Value))
            {
                pid = allMenus.FirstOrDefault(x => x.Id == pid.Value)?.ParentId;
            }
        }

        var parentMenus = allMenus.Where(x => allParents.Contains(x.Id) && !matchedIds.Contains(x.Id));
        matched.AddRange(parentMenus);

        var tree = BuildTree(matched, null);
        return MergeByPath(tree);
    }

    private static bool UserCanViewMenu(RoleMenu menu, int userRoleLevel, List<string> userPersonaCodes, HashSet<string> availableFeatureCodes)
    {
        if (!menu.IsVisible || !menu.IsEnabled)
            return false;

        if (menu.MinRoleLevel > userRoleLevel)
            return false;

        if (menu.PersonaTag != null && !userPersonaCodes.Contains(menu.PersonaTag))
            return false;

        if (!string.IsNullOrEmpty(menu.FeatureCode) && !availableFeatureCodes.Contains(menu.FeatureCode))
            return false;

        return true;
    }

    private static List<RoleMenu> MergeByPath(List<RoleMenu> treeNodes)
    {
        return treeNodes
            .GroupBy(x => x.Path)
            .Select(g =>
            {
                var representative = g.First();
                var allChildren = g.Where(x => x.Children != null && x.Children.Count > 0)
                                   .SelectMany(x => x.Children)
                                   .ToList();
                if (allChildren.Count > 0)
                {
                    representative.Children = MergeByPath(allChildren);
                }
                else if (representative.Children == null || representative.Children.Count == 0)
                {
                    representative.Children = new List<RoleMenu>();
                }
                return representative;
            })
            .OrderBy(x => x.Sort)
            .ToList();
    }

    public async Task<List<RoleMenu>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.RoleMenus
            .AsNoTracking()
            .OrderBy(x => x.Sort)
            .ToListAsync(ct);
    }

    public async Task<RoleMenu?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.RoleMenus.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task<RoleMenu> CreateAsync(RoleMenu input, CancellationToken ct = default)
    {
        _context.RoleMenus.Add(input);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Created menu {Name} path={Path}", input.Name, input.Path);
        return input;
    }

    public async Task<RoleMenu?> UpdateAsync(Guid id, RoleMenu input, CancellationToken ct = default)
    {
        var entity = await _context.RoleMenus.FindAsync([id], ct);
        if (entity is null) return null;

        entity.ParentId = input.ParentId;
        entity.Name = input.Name;
        entity.Path = input.Path;
        entity.Icon = input.Icon;
        entity.Component = input.Component;
        entity.Sort = input.Sort;
        entity.IsVisible = input.IsVisible;
        entity.IsEnabled = input.IsEnabled;
        entity.Permission = input.Permission;
        entity.Redirect = input.Redirect;
        entity.IsExternal = input.IsExternal;
        entity.Badge = input.Badge;
        entity.Tag = input.Tag;
        entity.MinRoleLevel = input.MinRoleLevel;
        entity.PersonaTag = input.PersonaTag;
        entity.IsBaseMenu = input.IsBaseMenu;
        entity.MenuCategory = input.MenuCategory;
        entity.FeatureCode = input.FeatureCode;

        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.RoleMenus.FindAsync([id], ct);
        if (entity is null) return false;

        _context.RoleMenus.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    private static List<RoleMenu> BuildTree(List<RoleMenu> all, Guid? parentId)
    {
        return all
            .Where(x => x.ParentId == parentId)
            .OrderBy(x => x.Sort)
            .Select(x =>
            {
                x.Children = BuildTree(all, x.Id);
                return x;
            })
            .ToList();
    }
}
