using WebApplication1.Shared;

namespace WebApplication1.Features.Auth.Entities;

public class RoleMenu : EntityBase
{
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Component { get; set; }
    public int Sort { get; set; }
    public bool IsVisible { get; set; } = true;
    public bool IsEnabled { get; set; } = true;
    public string? Permission { get; set; }
    public string? Redirect { get; set; }
    public bool IsExternal { get; set; }
    public string? Badge { get; set; }
    public string? Tag { get; set; }

    public int MinRoleLevel { get; set; } = 1;
    public string? PersonaTag { get; set; }
    public bool IsBaseMenu { get; set; } = true;
    public string MenuCategory { get; set; } = "General";
    public string? FeatureCode { get; set; }

    public RoleMenu? Parent { get; set; }
    public List<RoleMenu> Children { get; set; } = new();
    public List<MenuAction> Actions { get; set; } = new();
}

public enum MenuBindingType
{
    Role = 0,
    Persona = 1,
    Tag = 2,
}

public static class MenuCategories
{
    public const string Dashboard = "Dashboard";
    public const string Growth = "Growth";
    public const string Work = "Work";
    public const string AI = "AI";
    public const string Assets = "Assets";
    public const string Analytics = "Analytics";
    public const string System = "System";
    public const string Persona = "Persona";
}
