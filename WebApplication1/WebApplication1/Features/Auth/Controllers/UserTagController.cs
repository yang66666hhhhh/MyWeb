using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth.Controllers;

[ApiController]
[Authorize]
[Route("api/admin/user-tags")]
public class UserTagController(AppDbContext db) : ControllerBase
{
    [HttpGet("{userId:guid}")]
    public async Task<ActionResult<ApiResult<List<Guid>>>> GetUserTags(Guid userId)
    {
        var tagIds = await db.UserTags.Where(ut => ut.UserId == userId).Select(ut => ut.TagId).ToListAsync();
        return Ok(ApiResult<List<Guid>>.Success(tagIds));
    }

    [HttpPut("{userId:guid}")]
    public async Task<ActionResult<ApiResult>> UpdateUserTags(Guid userId, [FromBody] List<Guid> tagIds)
    {
        var user = await db.Users.FindAsync([userId]);
        if (user == null)
            return NotFound(ApiResult.Fail("用户不存在"));

        var existing = await db.UserTags.Where(ut => ut.UserId == userId).ToListAsync();
        db.UserTags.RemoveRange(existing);

        foreach (var tagId in tagIds)
        {
            db.UserTags.Add(new UserTag { UserId = userId, TagId = tagId });
        }
        await db.SaveChangesAsync();

        return Ok(ApiResult.Success("更新成功"));
    }

    [HttpGet("users")]
    public async Task<ActionResult<ApiResult<List<UserTagDto>>>> GetUsersWithTags()
    {
        var users = await db.Users
            .Include(u => u.UserTags)
            .Include(u => u.UserType)
            .Select(u => new UserTagDto
            {
                UserId = u.Id,
                Username = u.Username,
                UserTypeId = u.UserTypeId,
                UserTypeName = u.UserType != null ? u.UserType.Name : null,
                TagIds = u.UserTags.Select(ut => ut.TagId).ToList(),
                TagNames = u.UserTags.Select(ut => ut.Tag.Name).ToList()
            })
            .ToListAsync();

        return Ok(ApiResult<List<UserTagDto>>.Success(users));
    }
}

public class UserTagDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public Guid? UserTypeId { get; set; }
    public string? UserTypeName { get; set; }
    public List<Guid> TagIds { get; set; } = new();
    public List<string> TagNames { get; set; } = new();
}