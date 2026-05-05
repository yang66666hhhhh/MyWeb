using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Features.Auth.Services;
using WebApplication1.Shared.Data;

namespace WebApplication1.Tests.Services;

public class RoleMenuServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly RoleMenuService _service;
    private readonly List<string> _developerPersonaCodes;
    private readonly List<string> _designerPersonaCodes;
    private readonly List<string> _teacherPersonaCodes;

    public RoleMenuServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        var loggerMock = new Mock<ILogger<RoleMenuService>>();
        _service = new RoleMenuService(_context, loggerMock.Object);

        _developerPersonaCodes = new List<string> { "Developer" };
        _designerPersonaCodes = new List<string> { "Designer" };
        _teacherPersonaCodes = new List<string> { "Teacher" };

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Member 基础菜单
        var memberGrowth = new RoleMenu
        {
            Id = Guid.NewGuid(),
            Name = "个人成长",
            Path = "/growth",
            MinRoleLevel = 1,
            IsBaseMenu = true,
            IsVisible = true,
            IsEnabled = true,
            Sort = 10
        };
        _context.RoleMenus.Add(memberGrowth);

        // Pro 扩展菜单
        var proGrowth = new RoleMenu
        {
            Id = Guid.NewGuid(),
            Name = "知识库",
            Path = "/growth/knowledge-base",
            ParentId = memberGrowth.Id,
            MinRoleLevel = 2,
            IsBaseMenu = false,
            IsVisible = true,
            IsEnabled = true,
            Sort = 1
        };
        _context.RoleMenus.Add(proGrowth);

        // Owner 专属菜单
        var ownerGrowth = new RoleMenu
        {
            Id = Guid.NewGuid(),
            Name = "考研备考",
            Path = "/growth/postgraduate",
            ParentId = memberGrowth.Id,
            MinRoleLevel = 3,
            IsBaseMenu = false,
            IsVisible = true,
            IsEnabled = true,
            Sort = 2
        };
        _context.RoleMenus.Add(ownerGrowth);

        // Developer Persona 专属
        var devCenter = new RoleMenu
        {
            Id = Guid.NewGuid(),
            Name = "开发者中心",
            Path = "/dev",
            MinRoleLevel = 1,
            IsBaseMenu = false,
            PersonaTag = "Developer",
            IsVisible = true,
            IsEnabled = true,
            Sort = 50
        };
        _context.RoleMenus.Add(devCenter);

        var devRepo = new RoleMenu
        {
            Id = Guid.NewGuid(),
            Name = "代码仓库",
            Path = "/dev/code-repository",
            ParentId = devCenter.Id,
            MinRoleLevel = 1,
            IsBaseMenu = false,
            PersonaTag = "Developer",
            IsVisible = true,
            IsEnabled = true,
            Sort = 0
        };
        _context.RoleMenus.Add(devRepo);

        // Designer Persona 专属
        var designerCenter = new RoleMenu
        {
            Id = Guid.NewGuid(),
            Name = "设计师中心",
            Path = "/design",
            MinRoleLevel = 1,
            IsBaseMenu = false,
            PersonaTag = "Designer",
            IsVisible = true,
            IsEnabled = true,
            Sort = 50
        };
        _context.RoleMenus.Add(designerCenter);

        // Teacher Persona 专属
        var teacherCenter = new RoleMenu
        {
            Id = Guid.NewGuid(),
            Name = "教师中心",
            Path = "/teacher",
            MinRoleLevel = 1,
            IsBaseMenu = false,
            PersonaTag = "Teacher",
            IsVisible = true,
            IsEnabled = true,
            Sort = 50
        };
        _context.RoleMenus.Add(teacherCenter);

        // PersonaTypes
        var personaTypes = new List<Features.Admin.Entities.PersonaType>
        {
            new() { Id = Guid.NewGuid(), Code = "Developer", Name = "开发者", IsActive = true },
            new() { Id = Guid.NewGuid(), Code = "Designer", Name = "设计师", IsActive = true },
            new() { Id = Guid.NewGuid(), Code = "Teacher", Name = "教师", IsActive = true }
        };
        _context.PersonaTypes.AddRange(personaTypes);

        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    private static bool FindMenuInTree(List<RoleMenu> menus, string menuName)
    {
        foreach (var menu in menus)
        {
            if (menu.Name == menuName) return true;
            if (menu.Children != null && menu.Children.Count > 0)
            {
                if (FindMenuInTree(menu.Children, menuName)) return true;
            }
        }
        return false;
    }

    private static bool FindMenuInTreeByPath(List<RoleMenu> menus, string path)
    {
        foreach (var menu in menus)
        {
            if (menu.Path == path) return true;
            if (menu.Children != null && menu.Children.Count > 0)
            {
                if (FindMenuInTreeByPath(menu.Children, path)) return true;
            }
        }
        return false;
    }

    [Fact]
    public async Task GetMenusForUserAsync_ShouldReturnBaseMenus_WhenMemberRole()
    {
        var userId = Guid.NewGuid();
        var result = await _service.GetMenusForUserAsync(userId, "member", new List<string>(), new HashSet<string>());

        Assert.NotNull(result);
        Assert.True(FindMenuInTree(result, "个人成长"));
    }

    [Fact]
    public async Task GetMenusForUserAsync_ShouldReturnProMenus_ForProRole()
    {
        var userId = Guid.NewGuid();
        var result = await _service.GetMenusForUserAsync(userId, "pro", new List<string>(), new HashSet<string>());

        Assert.NotNull(result);
        Assert.True(FindMenuInTree(result, "个人成长"), "Should contain 个人成长");
        Assert.True(FindMenuInTree(result, "知识库"), "Should contain 知识库 for pro role");
    }

    [Fact]
    public async Task GetMenusForUserAsync_ShouldReturnOwnerMenus_ForOwnerRole()
    {
        var userId = Guid.NewGuid();
        var result = await _service.GetMenusForUserAsync(userId, "owner", new List<string>(), new HashSet<string>());

        Assert.NotNull(result);
        Assert.True(FindMenuInTree(result, "个人成长"), "Should contain 个人成长");
        Assert.True(FindMenuInTree(result, "知识库"), "Should contain 知识库 for owner");
        Assert.True(FindMenuInTree(result, "考研备考"), "Should contain 考研备考 for owner");
    }

    [Fact]
    public async Task GetMenusForUserAsync_ShouldReturnDeveloperMenus_ForMemberPlusDeveloper()
    {
        var userId = Guid.NewGuid();
        var result = await _service.GetMenusForUserAsync(userId, "member", _developerPersonaCodes, new HashSet<string>());

        Assert.NotNull(result);
        Assert.True(FindMenuInTree(result, "个人成长"), "Should contain 个人成长");
        Assert.True(FindMenuInTree(result, "开发者中心"), "Should contain 开发者中心");
        Assert.True(FindMenuInTree(result, "代码仓库"), "Should contain 代码仓库");
    }

    [Fact]
    public async Task GetMenusForUserAsync_ShouldReturnDesignerMenus_ForProPlusDesigner()
    {
        var userId = Guid.NewGuid();
        var result = await _service.GetMenusForUserAsync(userId, "pro", _designerPersonaCodes, new HashSet<string>());

        Assert.NotNull(result);
        Assert.True(FindMenuInTree(result, "个人成长"), "Should contain 个人成长");
        Assert.True(FindMenuInTree(result, "设计师中心"), "Should contain 设计师中心 for pro + designer");
    }

    [Fact]
    public async Task GetMenusForUserAsync_ShouldReturnTeacherMenus_ForOwnerPlusTeacher()
    {
        var userId = Guid.NewGuid();
        var result = await _service.GetMenusForUserAsync(userId, "owner", _teacherPersonaCodes, new HashSet<string>());

        Assert.NotNull(result);
        Assert.True(FindMenuInTree(result, "个人成长"), "Should contain 个人成长");
        Assert.True(FindMenuInTree(result, "教师中心"), "Should contain 教师中心 for owner + teacher");
    }

    [Fact]
    public async Task GetMenusForUserAsync_ShouldNotReturnDesignerMenus_ForMemberRole()
    {
        var userId = Guid.NewGuid();
        var result = await _service.GetMenusForUserAsync(userId, "member", _designerPersonaCodes, new HashSet<string>());

        Assert.NotNull(result);
        Assert.False(FindMenuInTree(result, "设计师中心"), "Should NOT contain 设计师中心 for member role");
    }

    [Fact]
    public async Task GetMenusForUserAsync_ShouldIncludeParentMenus()
    {
        var userId = Guid.NewGuid();
        var result = await _service.GetMenusForUserAsync(userId, "member", _developerPersonaCodes, new HashSet<string>());

        Assert.NotNull(result);
        Assert.True(FindMenuInTree(result, "个人成长"), "Should contain 个人成长");
        Assert.True(FindMenuInTree(result, "开发者中心"), "Should contain 开发者中心");
    }

    [Fact]
    public async Task GetMenusForUserAsync_ShouldRespectRoleHierarchy()
    {
        var userId = Guid.NewGuid();

        var memberResult = await _service.GetMenusForUserAsync(userId, "member", new List<string>(), new HashSet<string>());
        var proResult = await _service.GetMenusForUserAsync(userId, "pro", new List<string>(), new HashSet<string>());
        var ownerResult = await _service.GetMenusForUserAsync(userId, "owner", new List<string>(), new HashSet<string>());

        Assert.True(memberResult.Count <= proResult.Count);
        Assert.True(proResult.Count <= ownerResult.Count);
    }
}