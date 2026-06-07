using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Notification;

public interface INotificationService
{
    Task<PageResult<NotificationDto>> GetPageAsync(NotificationQueryDto query, Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> MarkAsReadAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<int> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<NotificationDto> CreateAsync(CreateNotificationDto input, CancellationToken cancellationToken = default);
    Task<List<NotificationDto>> CreateBatchAsync(List<CreateNotificationDto> inputs, CancellationToken cancellationToken = default);
}

public class NotificationService(AppDbContext dbContext) : INotificationService
{
    public async Task<PageResult<NotificationDto>> GetPageAsync(NotificationQueryDto query, Guid userId, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var notifications = dbContext.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(query.Type) && Enum.TryParse<NotificationType>(query.Type, true, out var type))
        {
            notifications = notifications.Where(x => x.Type == type);
        }

        if (query.IsRead.HasValue)
        {
            notifications = notifications.Where(x => x.IsRead == query.IsRead.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            notifications = notifications.Where(x =>
                x.Title.Contains(keyword) ||
                (x.Content != null && x.Content.Contains(keyword)));
        }

        var total = await notifications.CountAsync(cancellationToken);
        var items = await notifications
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<NotificationDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Notifications
            .AsNoTracking()
            .CountAsync(x => x.UserId == userId && !x.IsRead, cancellationToken);
    }

    public async Task<bool> MarkAsReadAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var notification = await dbContext.Notifications
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (notification is null)
            return false;

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    public async Task<int> MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var unread = await dbContext.Notifications
            .Where(x => x.UserId == userId && !x.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in unread)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return unread.Count;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var notification = await dbContext.Notifications
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);

        if (notification is null)
            return false;

        dbContext.Notifications.Remove(notification);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<NotificationDto> CreateAsync(CreateNotificationDto input, CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            UserId = input.UserId,
            Title = input.Title.Trim(),
            Content = input.Content?.Trim(),
            Type = Enum.TryParse<NotificationType>(input.Type, true, out var t) ? t : NotificationType.System,
            Link = input.Link?.Trim()
        };

        dbContext.Notifications.Add(notification);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(notification);
    }

    public async Task<List<NotificationDto>> CreateBatchAsync(List<CreateNotificationDto> inputs, CancellationToken cancellationToken = default)
    {
        var notifications = inputs.Select(input => new Notification
        {
            UserId = input.UserId,
            Title = input.Title.Trim(),
            Content = input.Content?.Trim(),
            Type = Enum.TryParse<NotificationType>(input.Type, true, out var t) ? t : NotificationType.System,
            Link = input.Link?.Trim()
        }).ToList();

        dbContext.Notifications.AddRange(notifications);
        await dbContext.SaveChangesAsync(cancellationToken);

        return notifications.Select(ToDto).ToList();
    }

    private static NotificationDto ToDto(Notification notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Title = notification.Title,
            Content = notification.Content,
            Type = notification.Type.ToString(),
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt,
            Link = notification.Link,
            CreatedAt = notification.CreatedAt
        };
    }
}
