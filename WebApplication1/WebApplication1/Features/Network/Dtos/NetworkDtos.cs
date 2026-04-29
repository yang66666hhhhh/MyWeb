using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Network.Dtos;

public class ContactDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Tags { get; set; }
    public string? Remark { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class InteractionDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string ContactId { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string InteractionDate { get; set; } = string.Empty;
    public string? Remark { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class TagDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public int UsageCount { get; set; }
}

public class NetworkQueryDto : PageQueryDto
{
    public string? Tag { get; set; }
    public string? Keyword { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}
