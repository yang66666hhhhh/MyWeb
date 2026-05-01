using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth.Controllers;

[ApiController]
[Authorize]
[Route("api/admin/user-types")]
public class UserTypeController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<List<UserTypeDto>>>> GetAll()
    {
        var types = await db.UserTypes
            .Include(t => t.UserTypeTags)
            .ThenInclude(utt => utt.Tag)
            .Where(t => t.IsActive)
            .OrderBy(t => t.Sort)
            .ToListAsync();

        return Ok(ApiResult<List<UserTypeDto>>.Success(types.Select(t => new UserTypeDto
        {
            Id = t.Id,
            Name = t.Name,
            Code = t.Code,
            Description = t.Description,
            Color = t.Color,
            Sort = t.Sort,
            TagIds = t.UserTypeTags.Select(utt => utt.TagId).ToList(),
            TagNames = t.UserTypeTags.Select(utt => utt.Tag.Name).ToList()
        }).ToList()));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<UserTypeDto>>> Create([FromBody] CreateUserTypeDto input)
    {
        if (await db.UserTypes.AnyAsync(t => t.Code == input.Code))
            return BadRequest(ApiResult<UserTypeDto>.Fail("代码已存在"));

        var userType = new UserType
        {
            Name = input.Name,
            Code = input.Code,
            Description = input.Description,
            Color = input.Color ?? "#1890ff",
            Sort = input.Sort
        };
        db.UserTypes.Add(userType);
        await db.SaveChangesAsync();

        if (input.TagIds?.Count > 0)
        {
            foreach (var tagId in input.TagIds)
            {
                db.UserTypeTags.Add(new UserTypeTag { UserTypeId = userType.Id, TagId = tagId });
            }
            await db.SaveChangesAsync();
        }

        var tags = await db.Tags.Where(t => input.TagIds.Contains(t.Id)).Select(t => t.Name).ToListAsync();
        return Ok(ApiResult<UserTypeDto>.Success(new UserTypeDto
        {
            Id = userType.Id,
            Name = userType.Name,
            Code = userType.Code,
            Description = userType.Description,
            Color = userType.Color,
            Sort = userType.Sort,
            TagIds = input.TagIds ?? new List<Guid>(),
            TagNames = tags
        }, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<UserTypeDto>>> Update(Guid id, [FromBody] CreateUserTypeDto input)
    {
        var userType = await db.UserTypes
            .Include(t => t.UserTypeTags)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (userType == null)
            return NotFound(ApiResult<UserTypeDto>.Fail("用户类型不存在"));

        if (await db.UserTypes.AnyAsync(t => t.Code == input.Code && t.Id != id))
            return BadRequest(ApiResult<UserTypeDto>.Fail("代码已存在"));

        userType.Name = input.Name;
        userType.Code = input.Code;
        userType.Description = input.Description;
        userType.Color = input.Color ?? userType.Color;
        userType.Sort = input.Sort;

        db.UserTypeTags.RemoveRange(userType.UserTypeTags);
        if (input.TagIds?.Count > 0)
        {
            foreach (var tagId in input.TagIds)
            {
                db.UserTypeTags.Add(new UserTypeTag { UserTypeId = userType.Id, TagId = tagId });
            }
        }
        await db.SaveChangesAsync();

        var tags = await db.Tags.Where(t => input.TagIds.Contains(t.Id)).Select(t => t.Name).ToListAsync();
        return Ok(ApiResult<UserTypeDto>.Success(new UserTypeDto
        {
            Id = userType.Id,
            Name = userType.Name,
            Code = userType.Code,
            Description = userType.Description,
            Color = userType.Color,
            Sort = userType.Sort,
            TagIds = input.TagIds ?? new List<Guid>(),
            TagNames = tags
        }, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id)
    {
        var userType = await db.UserTypes.FindAsync([id]);
        if (userType == null)
            return NotFound(ApiResult.Fail("用户类型不存在"));

        db.UserTypes.Remove(userType);
        await db.SaveChangesAsync();
        return Ok(ApiResult.Success("删除成功"));
    }

    [HttpPut("{userId:guid}/assign")]
    public async Task<ActionResult<ApiResult>> AssignUserType(Guid userId, [FromBody] Guid? userTypeId)
    {
        var user = await db.Users.FindAsync([userId]);
        if (user == null)
            return NotFound(ApiResult.Fail("用户不存在"));

        user.UserTypeId = userTypeId;
        await db.SaveChangesAsync();
        return Ok(ApiResult.Success("分配成功"));
    }
}

public class UserTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = "#1890ff";
    public int Sort { get; set; }
    public List<Guid> TagIds { get; set; } = new();
    public List<string> TagNames { get; set; } = new();
}

public class CreateUserTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public int Sort { get; set; }
    public List<Guid>? TagIds { get; set; }
}