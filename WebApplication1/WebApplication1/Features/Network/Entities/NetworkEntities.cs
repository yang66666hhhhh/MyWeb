using WebApplication1.Shared;

namespace WebApplication1.Features.Network.Entities;

public class Contact : EntityBase
{
    public Guid? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? WeChat { get; set; }
    public string? Tags { get; set; }
    public string? Remark { get; set; }
    public int InteractionCount { get; set; }
    public DateTime? LastInteractionAt { get; set; }
}

public class Interaction : EntityBase
{
    public Guid? UserId { get; set; }
    public Guid ContactId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string InteractionDate { get; set; } = string.Empty;
    public string? NextFollowUpDate { get; set; }
    public string? Remark { get; set; }
    public Contact? Contact { get; set; }
}
