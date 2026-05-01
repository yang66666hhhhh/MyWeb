namespace WebApplication1.Features.Auth.Entities;

public static class Permissions
{
    public const string ViewLogs = "logs.view";
    public const string CreateLogs = "logs.create";
    public const string EditLogs = "logs.edit";
    public const string DeleteLogs = "logs.delete";

    public const string ViewProjects = "projects.view";
    public const string CreateProjects = "projects.create";
    public const string EditProjects = "projects.edit";
    public const string DeleteProjects = "projects.delete";

    public const string ViewTeamLogs = "team.logs.view";
    public const string ViewTeamStats = "team.stats.view";

    public const string ManageTemplates = "templates.manage";
    public const string ManageCategories = "categories.manage";
    public const string ManageUsers = "users.manage";
    public const string ManageTenants = "tenants.manage";

    public const string ViewAdminPages = "admin.pages";
}

public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Permissions { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public AppUser User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}

public static class DefaultRoles
{
    public const string SuperAdmin = "super_admin";
    public const string TenantAdmin = "tenant_admin";
    public const string Manager = "manager";
    public const string Employee = "employee";

    public static readonly Dictionary<string, string[]> RolePermissions = new()
    {
        [SuperAdmin] = new[]
        {
            Permissions.ViewLogs, Permissions.CreateLogs, Permissions.EditLogs, Permissions.DeleteLogs,
            Permissions.ViewProjects, Permissions.CreateProjects, Permissions.EditProjects, Permissions.DeleteProjects,
            Permissions.ViewTeamLogs, Permissions.ViewTeamStats,
            Permissions.ManageTemplates, Permissions.ManageCategories, Permissions.ManageUsers, Permissions.ManageTenants,
            Permissions.ViewAdminPages
        },
        [TenantAdmin] = new[]
        {
            Permissions.ViewLogs, Permissions.CreateLogs, Permissions.EditLogs, Permissions.DeleteLogs,
            Permissions.ViewProjects, Permissions.CreateProjects, Permissions.EditProjects, Permissions.DeleteProjects,
            Permissions.ViewTeamLogs, Permissions.ViewTeamStats,
            Permissions.ManageTemplates, Permissions.ManageCategories, Permissions.ManageUsers,
            Permissions.ViewAdminPages
        },
        [Manager] = new[]
        {
            Permissions.ViewLogs, Permissions.CreateLogs, Permissions.EditLogs,
            Permissions.ViewProjects, Permissions.CreateProjects,
            Permissions.ViewTeamLogs, Permissions.ViewTeamStats,
        },
        [Employee] = new[]
        {
            Permissions.ViewLogs, Permissions.CreateLogs,
            Permissions.ViewProjects,
        }
    };
}