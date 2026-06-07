using WebApplication1.Shared;

namespace WebApplication1.Features.Notification;

public class Notification : EntityBase
{
    public Guid UserId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }

    public NotificationType Type { get; set; } = NotificationType.System;

    public bool IsRead { get; set; }

    public DateTime? ReadAt { get; set; }

    public string? Link { get; set; }
}

public enum NotificationType
{
    System = 0,
    Task = 1,
    Habit = 2,
    Ai = 3,
    Finance = 4
}
