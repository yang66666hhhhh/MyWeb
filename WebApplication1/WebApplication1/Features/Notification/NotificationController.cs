using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Notification;

[ApiController]
[Authorize]
[Route("api/notifications")]
[Tags("Notifications")]
public class NotificationController(INotificationService notificationService) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<NotificationDto>>>> GetPage(
        [FromQuery] NotificationQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await notificationService.GetPageAsync(query, userId.Value, cancellationToken);
        return Ok(ApiResult<PageResult<NotificationDto>>.Success(result));
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResult<UnreadCountDto>>> GetUnreadCount(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var count = await notificationService.GetUnreadCountAsync(userId.Value, cancellationToken);
        return Ok(ApiResult<UnreadCountDto>.Success(new UnreadCountDto { Count = count }));
    }

    [HttpPut("{id:guid}/read")]
    public async Task<ActionResult<ApiResult>> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var success = await notificationService.MarkAsReadAsync(id, userId.Value, cancellationToken);
        if (!success)
            return NotFound(ApiResult.Fail("通知不存在"));

        return Ok(ApiResult.Success("已标记为已读"));
    }

    [HttpPut("read-all")]
    public async Task<ActionResult<ApiResult>> MarkAllAsRead(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var count = await notificationService.MarkAllAsReadAsync(userId.Value, cancellationToken);
        return Ok(ApiResult.Success($"已将 {count} 条通知标记为已读"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var deleted = await notificationService.DeleteAsync(id, userId.Value, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("通知不存在"));

        return Ok(ApiResult.Success("删除成功"));
    }
}
