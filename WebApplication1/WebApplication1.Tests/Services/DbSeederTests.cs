using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Data;

namespace WebApplication1.Tests.Services;

public class DbSeederTests : IDisposable
{
    private readonly AppDbContext _context;

    public DbSeederTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task SeedAsync_ShouldCreatePersonaCentersWithExpectedChildren()
    {
        await DbSeeder.SeedAsync(_context);

        var menus = await _context.RoleMenus
            .AsNoTracking()
            .ToListAsync();

        AssertMenuWithChildren(menus, "/dev", "/dev/code-repository", "/dev/issues", "/dev/pipelines");
        AssertMenuWithChildren(
            menus,
            "/implementation",
            "/implementation/kanban",
            "/implementation/customers",
            "/implementation/tasks",
            "/implementation/impl-log",
            "/implementation/weekly-plan",
            "/implementation/weekly-report");
        AssertMenuWithChildren(menus, "/design", "/design/assets", "/design/prototypes");
        AssertMenuWithChildren(menus, "/teacher", "/teacher/courses", "/teacher/students");
        AssertMenuWithChildren(menus, "/student", "/student/dashboard", "/student/learning", "/student/review", "/student/mistakes", "/student/materials", "/student/records", "/student/subjects");
    }

    [Fact]
    public async Task SeedAsync_ShouldNotLeaveOrphanedRoleMenuParents()
    {
        await DbSeeder.SeedAsync(_context);

        var menus = await _context.RoleMenus
            .AsNoTracking()
            .ToListAsync();

        var ids = menus.Select(x => x.Id).ToHashSet();
        var orphanedParents = menus
            .Where(x => x.ParentId.HasValue && !ids.Contains(x.ParentId.Value))
            .Select(x => x.Path)
            .ToList();

        Assert.Empty(orphanedParents);
    }

    [Fact]
    public async Task SeedAsync_ShouldCreateWorkProjectsWithLocation()
    {
        await DbSeeder.SeedAsync(_context);

        var projects = await _context.WorkProjects
            .AsNoTracking()
            .ToListAsync();

        Assert.NotEmpty(projects);
        Assert.All(projects, project => Assert.False(string.IsNullOrWhiteSpace(project.Location)));
    }

    private static void AssertMenuWithChildren(
        List<WebApplication1.Features.Auth.Entities.RoleMenu> menus,
        string parentPath,
        params string[] childPaths)
    {
        var parent = menus.SingleOrDefault(x => x.Path == parentPath);
        Assert.NotNull(parent);

        foreach (var childPath in childPaths)
        {
            Assert.Contains(menus, x => x.ParentId == parent.Id && x.Path == childPath);
        }
    }
}
