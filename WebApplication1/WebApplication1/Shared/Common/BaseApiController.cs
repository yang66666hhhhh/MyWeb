using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Shared.Common;

public abstract class BaseApiController : ControllerBase
{
    protected Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    protected string GetCurrentUserRole()
    {
        return User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .FirstOrDefault() ?? "member";
    }

    protected bool IsProOrAbove()
    {
        return User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Any(c => c.Value.Equals("pro", StringComparison.OrdinalIgnoreCase) ||
                       c.Value.Equals("owner", StringComparison.OrdinalIgnoreCase));
    }

    protected bool IsOwner()
    {
        return User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Any(c => c.Value.Equals("owner", StringComparison.OrdinalIgnoreCase));
    }

    protected Guid? GetUserIdForQuery()
    {
        return IsProOrAbove() ? null : GetCurrentUserId();
    }

    protected bool IsUnauthorizedForResource(string? resourceUserId)
    {
        var currentUserId = GetCurrentUserId();
        return !IsProOrAbove() && resourceUserId != currentUserId?.ToString();
    }

    protected ActionResult<ApiResult<T>> HandleResourceNotFound<T>(T? result, string resourceName)
    {
        if (result is null)
            return NotFound(ApiResult<T>.Fail($"{resourceName}不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<T>.Success(result));
    }

    protected ActionResult<ApiResult> HandleDeleteResult(bool success, string resourceName)
    {
        if (!success)
            return NotFound(ApiResult.Fail($"{resourceName}不存在"));
        return Ok(ApiResult.Success("删除成功"));
    }
}
