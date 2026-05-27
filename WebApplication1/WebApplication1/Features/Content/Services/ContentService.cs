using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Content.Dtos;
using WebApplication1.Features.Content.Entities;
using WebApplication1.Features.Content.Services.Interfaces;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Content.Services;

public class ContentService(AppDbContext dbContext) : IContentService
{
    public async Task<PageResult<ArticleDto>> GetArticlePageAsync(ContentQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var articles = dbContext.Articles.AsNoTracking();

        if (userId.HasValue)
        {
            articles = articles.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            articles = articles.Where(x => x.Title.Contains(keyword) || (x.Content != null && x.Content.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            articles = articles.Where(x => x.Status == query.Status);
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            articles = articles.Where(x => x.Category == query.Category);
        }

        var total = await articles.CountAsync(cancellationToken);
        var items = await articles
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<ArticleDto>.Create(items.Select(ToArticleDto).ToList(), total, page, pageSize);
    }

    public async Task<ArticleDto?> GetArticleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await dbContext.Articles.FindAsync(id, cancellationToken);
        return article is null ? null : ToArticleDto(article);
    }

    public async Task<ArticleDto> CreateArticleAsync(CreateArticleDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var article = new Article
        {
            UserId = userId,
            Title = input.Title,
            Content = input.Content,
            Status = input.Status,
            Tags = input.Tags,
            Category = input.Category,
            Remark = input.Remark
        };

        dbContext.Articles.Add(article);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToArticleDto(article);
    }

    public async Task<ArticleDto?> UpdateArticleAsync(Guid id, UpdateArticleDto input, CancellationToken cancellationToken = default)
    {
        var article = await dbContext.Articles.FindAsync(id, cancellationToken);
        if (article is null) return null;

        if (input.Title is not null) article.Title = input.Title;
        if (input.Content is not null) article.Content = input.Content;
        if (input.Status is not null) article.Status = input.Status;
        if (input.Tags is not null) article.Tags = input.Tags;
        if (input.Category is not null) article.Category = input.Category;
        if (input.Remark is not null) article.Remark = input.Remark;

        if (input.Status == "published" && article.PublishedAt is null)
        {
            article.PublishedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToArticleDto(article);
    }

    public async Task<bool> DeleteArticleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await dbContext.Articles.FindAsync(id, cancellationToken);
        if (article is null) return false;

        dbContext.Articles.Remove(article);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PageResult<MediaItemDto>> GetMediaItemPageAsync(ContentQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var mediaItems = dbContext.MediaItems.AsNoTracking();

        if (userId.HasValue)
        {
            mediaItems = mediaItems.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            mediaItems = mediaItems.Where(x => x.FileName.Contains(keyword) || (x.Tags != null && x.Tags.Contains(keyword)));
        }

        var total = await mediaItems.CountAsync(cancellationToken);
        var items = await mediaItems
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<MediaItemDto>.Create(items.Select(ToMediaItemDto).ToList(), total, page, pageSize);
    }

    public async Task<MediaItemDto?> GetMediaItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var mediaItem = await dbContext.MediaItems.FindAsync(id, cancellationToken);
        return mediaItem is null ? null : ToMediaItemDto(mediaItem);
    }

    public async Task<MediaItemDto> CreateMediaItemAsync(CreateMediaItemDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var mediaItem = new MediaItem
        {
            UserId = userId,
            FileName = input.FileName,
            FileUrl = input.FileUrl,
            FileType = input.FileType,
            FileSize = input.FileSize,
            Tags = input.Tags,
            Remark = input.Remark
        };

        dbContext.MediaItems.Add(mediaItem);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToMediaItemDto(mediaItem);
    }

    public async Task<MediaItemDto?> UpdateMediaItemAsync(Guid id, UpdateMediaItemDto input, CancellationToken cancellationToken = default)
    {
        var mediaItem = await dbContext.MediaItems.FindAsync(id, cancellationToken);
        if (mediaItem is null) return null;

        if (input.FileName is not null) mediaItem.FileName = input.FileName;
        if (input.FileUrl is not null) mediaItem.FileUrl = input.FileUrl;
        if (input.FileType is not null) mediaItem.FileType = input.FileType;
        if (input.FileSize.HasValue) mediaItem.FileSize = input.FileSize.Value;
        if (input.Tags is not null) mediaItem.Tags = input.Tags;
        if (input.Remark is not null) mediaItem.Remark = input.Remark;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToMediaItemDto(mediaItem);
    }

    public async Task<bool> DeleteMediaItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var mediaItem = await dbContext.MediaItems.FindAsync(id, cancellationToken);
        if (mediaItem is null) return false;

        dbContext.MediaItems.Remove(mediaItem);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PageResult<PublishingCalendarDto>> GetPublishingCalendarPageAsync(ContentQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var calendars = dbContext.PublishingCalendars.AsNoTracking();

        if (userId.HasValue)
        {
            calendars = calendars.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            calendars = calendars.Where(x => x.Title.Contains(keyword) || x.Platform.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            calendars = calendars.Where(x => x.Status == query.Status);
        }

        var total = await calendars.CountAsync(cancellationToken);
        var items = await calendars
            .OrderByDescending(x => x.PlannedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<PublishingCalendarDto>.Create(items.Select(ToPublishingCalendarDto).ToList(), total, page, pageSize);
    }

    public async Task<PublishingCalendarDto?> GetPublishingCalendarByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var calendar = await dbContext.PublishingCalendars.FindAsync(id, cancellationToken);
        return calendar is null ? null : ToPublishingCalendarDto(calendar);
    }

    public async Task<PublishingCalendarDto> CreatePublishingCalendarAsync(CreatePublishingCalendarDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var calendar = new PublishingCalendar
        {
            UserId = userId,
            PlannedDate = input.PlannedDate,
            Platform = input.Platform,
            Title = input.Title,
            Status = input.Status,
            Remark = input.Remark
        };

        dbContext.PublishingCalendars.Add(calendar);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToPublishingCalendarDto(calendar);
    }

    public async Task<PublishingCalendarDto?> UpdatePublishingCalendarAsync(Guid id, UpdatePublishingCalendarDto input, CancellationToken cancellationToken = default)
    {
        var calendar = await dbContext.PublishingCalendars.FindAsync(id, cancellationToken);
        if (calendar is null) return null;

        if (input.PlannedDate is not null) calendar.PlannedDate = input.PlannedDate;
        if (input.Platform is not null) calendar.Platform = input.Platform;
        if (input.Title is not null) calendar.Title = input.Title;
        if (input.Status is not null) calendar.Status = input.Status;
        if (input.Remark is not null) calendar.Remark = input.Remark;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToPublishingCalendarDto(calendar);
    }

    public async Task<bool> DeletePublishingCalendarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var calendar = await dbContext.PublishingCalendars.FindAsync(id, cancellationToken);
        if (calendar is null) return false;

        dbContext.PublishingCalendars.Remove(calendar);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ArticleDto ToArticleDto(Article article) => new()
    {
        Id = article.Id.ToString(),
        UserId = article.UserId?.ToString(),
        Title = article.Title,
        Content = article.Content,
        Status = article.Status,
        Tags = article.Tags,
        Category = article.Category,
        PublishedAt = article.PublishedAt,
        Remark = article.Remark,
        CreatedAt = article.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
    };

    private static MediaItemDto ToMediaItemDto(MediaItem mediaItem) => new()
    {
        Id = mediaItem.Id.ToString(),
        UserId = mediaItem.UserId?.ToString(),
        FileName = mediaItem.FileName,
        FileUrl = mediaItem.FileUrl,
        FileType = mediaItem.FileType,
        FileSize = mediaItem.FileSize,
        Tags = mediaItem.Tags,
        Remark = mediaItem.Remark,
        CreatedAt = mediaItem.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
    };

    private static PublishingCalendarDto ToPublishingCalendarDto(PublishingCalendar calendar) => new()
    {
        Id = calendar.Id.ToString(),
        UserId = calendar.UserId?.ToString(),
        PlannedDate = calendar.PlannedDate,
        Platform = calendar.Platform,
        Title = calendar.Title,
        Status = calendar.Status,
        Remark = calendar.Remark,
        CreatedAt = calendar.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
    };
}
