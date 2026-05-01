using WebApplication1.Shared;

namespace WebApplication1.Features.Auth.Entities;

public class Tag : EntityBase
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Color { get; set; } = "#1890ff";

    public int Sort { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<MenuTag> MenuTags { get; set; } = new List<MenuTag>();

    public ICollection<UserTag> UserTags { get; set; } = new List<UserTag>();
}

public class MenuItem : EntityBase
{
    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;

    public string? Icon { get; set; }

    public Guid? ParentId { get; set; }

    public int Sort { get; set; }

    public bool IsActive { get; set; } = true;

    public string? RequiredPermissions { get; set; }

    public MenuItem? Parent { get; set; }

    public ICollection<MenuItem> Children { get; set; } = new List<MenuItem>();

    public ICollection<MenuTag> MenuTags { get; set; } = new List<MenuTag>();
}

public class MenuTag
{
    public Guid MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}

public class UserTag
{
    public Guid UserId { get; set; }
    public AppUser User { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}