using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Content.Entities;

public class Article : EntityBase
{
    public Guid? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string Status { get; set; } = "draft";
    public string? Tags { get; set; }
    public string? Category { get; set; }
    public string? PublishedAt { get; set; }
    public string? Remark { get; set; }
}

public class MediaItem : EntityBase
{
    public Guid? UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Tags { get; set; }
    public string? Remark { get; set; }
}

public class PublishingCalendar : EntityBase
{
    public Guid? UserId { get; set; }
    public string PlannedDate { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public string? Remark { get; set; }
}
