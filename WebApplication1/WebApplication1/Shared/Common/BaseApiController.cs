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
}
