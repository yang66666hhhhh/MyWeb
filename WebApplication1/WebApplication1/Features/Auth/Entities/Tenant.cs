using WebApplication1.Shared;

namespace WebApplication1.Features.Auth.Entities;

public class Tenant : EntityBase
{
    public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    public int MaxUsers { get; set; } = 100;

    public DateTime? ExpiresAt { get; set; }

    public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
}