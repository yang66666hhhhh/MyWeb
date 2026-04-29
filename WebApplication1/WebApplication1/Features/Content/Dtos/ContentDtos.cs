using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Content.Dtos;

public class ArticleDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string Status { get; set; } = "draft";
    public string? Tags { get; set; }
    public string? Category { get; set; }
    public string? PublishedAt { get; set; }
    public string? Remark { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class MediaItemDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string? Tags { get; set; }
    public string? Remark { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class PublishingCalendarDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string PlannedDate { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public string? Remark { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class ContentQueryDto : PageQueryDto
{
    public string? Status { get; set; }
    public string? Category { get; set; }
    public string? Keyword { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}
