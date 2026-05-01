using WebApplication1.Shared;

namespace WebApplication1.Features.Auth.Entities;

public class UserType : EntityBase
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Color { get; set; } = "#1890ff";

    public int Sort { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<UserTypeTag> UserTypeTags { get; set; } = new List<UserTypeTag>();

    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
}

public class UserTypeTag
{
    public Guid UserTypeId { get; set; }
    public UserType UserType { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}