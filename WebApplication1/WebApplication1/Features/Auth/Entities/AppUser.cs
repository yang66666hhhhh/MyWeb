using WebApplication1.Shared;

namespace WebApplication1.Features.Auth.Entities;

public class AppUser : EntityBase
{
    public Guid? TenantId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string RealName { get; set; } = string.Empty;

    public string? Avatar { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public AppUserStatus Status { get; set; } = AppUserStatus.Active;

    public DateTime? LastLoginAt { get; set; }

    public string? LastLoginIp { get; set; }

    public string Roles { get; set; } = "user";

    public Guid? UserTypeId { get; set; }

    public UserType? UserType { get; set; }

    public Tenant? Tenant { get; set; }

    public ICollection<UserTag> UserTags { get; set; } = new List<UserTag>();
}

public enum AppUserStatus
{
    Active = 0,
    Inactive = 1,
    Locked = 2,
    Deleted = 3
}