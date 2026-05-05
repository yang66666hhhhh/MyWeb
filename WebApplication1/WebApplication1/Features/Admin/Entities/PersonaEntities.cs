using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared;

namespace WebApplication1.Features.Admin.Entities;

public class PersonaType : EntityBase
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? DefaultHomeRoute { get; set; }

    public int Sort { get; set; }

    public bool IsActive { get; set; } = true;

    public ICollection<UserPersonaRecord> UserPersonaRecords { get; set; } = new List<UserPersonaRecord>();

    public ICollection<PersonaMenuItem> MenuItems { get; set; } = new List<PersonaMenuItem>();
}

public class UserPersonaRecord : EntityBase
{
    public Guid UserId { get; set; }

    public AppUser? User { get; set; }

    public Guid PersonaTypeId { get; set; }

    public PersonaType? PersonaType { get; set; }

    public DateTime SwitchedAt { get; set; } = DateTime.UtcNow;

    public string? Remark { get; set; }
}

public class PersonaMenuItem : EntityBase
{
    public Guid PersonaTypeId { get; set; }

    public PersonaType? PersonaType { get; set; }

    public string MenuPath { get; set; } = string.Empty;

    public string MenuName { get; set; } = string.Empty;

    public string? Icon { get; set; }

    public int Sort { get; set; }

    public bool IsVisible { get; set; } = true;

    public Guid? ParentId { get; set; }

    public PersonaMenuItem? Parent { get; set; }

    public ICollection<PersonaMenuItem> Children { get; set; } = new List<PersonaMenuItem>();
}
