namespace WebApplication1.Features.Auth.Entities;

public class MenuAction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MenuId { get; set; }
    public string ActionCode { get; set; } = string.Empty;
    public string ActionName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Sort { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public RoleMenu Menu { get; set; } = null!;
}
