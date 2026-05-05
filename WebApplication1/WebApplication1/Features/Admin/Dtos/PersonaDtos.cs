using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Admin.Dtos;

public class PersonaTypeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DefaultHomeRoute { get; set; }
    public int Sort { get; set; }
    public bool IsActive { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PersonaTypeQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public bool? IsActive { get; set; }
}

public class CreatePersonaTypeDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DefaultHomeRoute { get; set; }
    public int Sort { get; set; }
}

public class UpdatePersonaTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DefaultHomeRoute { get; set; }
    public int Sort { get; set; }
    public bool IsActive { get; set; }
}

public class UserPersonaDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;
    public List<PersonaTypeDto> Personas { get; set; } = new();
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PersonaMenuItemDto
{
    public Guid Id { get; set; }
    public Guid PersonaTypeId { get; set; }
    public string MenuPath { get; set; } = string.Empty;
    public string MenuName { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public int Sort { get; set; }
    public bool IsVisible { get; set; }
    public Guid? ParentId { get; set; }
    public List<PersonaMenuItemDto> Children { get; set; } = new();
}
