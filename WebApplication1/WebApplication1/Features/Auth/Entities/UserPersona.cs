using WebApplication1.Features.Admin.Entities;

namespace WebApplication1.Features.Auth.Entities;

public class UserPersona
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid PersonaTypeId { get; set; }
    public bool IsPrimary { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AppUser User { get; set; } = null!;
    public PersonaType PersonaType { get; set; } = null!;
}
