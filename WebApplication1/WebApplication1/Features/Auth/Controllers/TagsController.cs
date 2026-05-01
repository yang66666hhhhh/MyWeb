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
[Route("api/tags")]
public class TagsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<List<TagDto>>>> GetAll()
    {
        var tags = await db.Tags.Where(t => t.IsActive).OrderBy(t => t.Sort).ToListAsync();
        return Ok(ApiResult<List<TagDto>>.Success(tags.Select(t => new TagDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            Color = t.Color,
            Sort = t.Sort,
        }).ToList()));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<TagDto>>> Create([FromBody] CreateTagDto input)
    {
        var tag = new Tag
        {
            Name = input.Name,
            Description = input.Description,
            Color = input.Color ?? "#1890ff",
            Sort = input.Sort
        };
        db.Tags.Add(tag);
        await db.SaveChangesAsync();
        return Ok(ApiResult<TagDto>.Success(new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Description = tag.Description,
            Color = tag.Color,
            Sort = tag.Sort
        }, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<TagDto>>> Update(Guid id, [FromBody] CreateTagDto input)
    {
        var tag = await db.Tags.FindAsync([id]);
        if (tag == null) return NotFound(ApiResult<TagDto>.Fail("标签不存在"));
        tag.Name = input.Name;
        tag.Description = input.Description ?? tag.Description;
        tag.Color = input.Color ?? tag.Color;
        tag.Sort = input.Sort;
        await db.SaveChangesAsync();
        return Ok(ApiResult<TagDto>.Success(new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
            Description = tag.Description,
            Color = tag.Color,
            Sort = tag.Sort
        }, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id)
    {
        var tag = await db.Tags.FindAsync([id]);
        if (tag == null) return NotFound(ApiResult.Fail("标签不存在"));
        db.Tags.Remove(tag);
        await db.SaveChangesAsync();
        return Ok(ApiResult.Success("删除成功"));
    }
}

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = "#1890ff";
    public int Sort { get; set; }
}

public class CreateTagDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public int Sort { get; set; }
}