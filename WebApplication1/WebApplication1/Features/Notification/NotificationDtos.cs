using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Notification;

public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string Type { get; set; } = "System";
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? Link { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationQueryDto : PageQueryDto
{
    public string? Type { get; set; }
    public bool? IsRead { get; set; }
    public string? Keyword { get; set; }
}

public class CreateNotificationDto
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string Type { get; set; } = "System";
    public string? Link { get; set; }
}

public class UnreadCountDto
{
    public int Count { get; set; }
}
