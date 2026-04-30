using WebApplication1.Shared;

namespace WebApplication1.Features.Auth.Entities;

public class AppUser : EntityBase
{
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
}

public enum AppUserStatus
{
    Active = 0,
    Inactive = 1,
    Locked = 2,
    Deleted = 3
}