namespace WebApplication1.Features.Auth.Entities;

public class RolePermission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoleId { get; set; }
    public Guid MenuId { get; set; }
    public string ActionCode { get; set; } = string.Empty;
    public bool IsAllowed { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Role Role { get; set; } = null!;
    public RoleMenu Menu { get; set; } = null!;
}
