using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth.Controllers;

[ApiController]
[Authorize(Roles = "owner")]
[Route("api/system/tags")]
[Tags("Tags")]
public class TagsController(AppDbContext db, ILogger<TagsController> logger) : ControllerBase
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
        try
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
            logger.LogInformation("创建标签成功: {Id}", tag.Id);
            return Ok(ApiResult<TagDto>.Success(new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Description = tag.Description,
                Color = tag.Color,
                Sort = tag.Sort
            }, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建标签失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<TagDto>>> Update(Guid id, [FromBody] CreateTagDto input)
    {
        try
        {
            var tag = await db.Tags.FindAsync([id]);
            if (tag == null) return NotFound(ApiResult<TagDto>.Fail("标签不存在"));
            tag.Name = input.Name;
            tag.Description = input.Description ?? tag.Description;
            tag.Color = input.Color ?? tag.Color;
            tag.Sort = input.Sort;
            await db.SaveChangesAsync();
            logger.LogInformation("更新标签成功: {Id}", id);
            return Ok(ApiResult<TagDto>.Success(new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Description = tag.Description,
                Color = tag.Color,
                Sort = tag.Sort
            }, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新标签失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id)
    {
        try
        {
            var tag = await db.Tags.FindAsync([id]);
            if (tag == null) return NotFound(ApiResult.Fail("标签不存在"));
            db.Tags.Remove(tag);
            await db.SaveChangesAsync();
            logger.LogInformation("删除标签成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除标签失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
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
    [Required(ErrorMessage = "标签名称不能为空")]
    [StringLength(50, ErrorMessage = "标签名称不能超过50个字符")]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public int Sort { get; set; }
}
