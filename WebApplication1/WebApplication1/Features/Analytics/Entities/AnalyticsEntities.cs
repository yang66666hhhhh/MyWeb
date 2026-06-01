using WebApplication1.Shared;

namespace WebApplication1.Features.Analytics.Entities;

public class CustomReport : EntityBase
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Config { get; set; }
    public string? Tags { get; set; }
}
