using WebApplication1.Shared;

namespace WebApplication1.Features.Auth.Entities;

public class MenuConfig : EntityBase
{
    public string Path { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? Icon { get; set; }

    public string? Description { get; set; }

    public int Sort { get; set; }

    public bool IsActive { get; set; } = true;
}