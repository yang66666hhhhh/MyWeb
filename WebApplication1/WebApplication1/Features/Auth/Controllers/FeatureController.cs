using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Auth.Entities.Subscription;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth.Controllers;

[ApiController]
[Route("api/system/features")]
[Authorize(Roles = "owner")]
[Tags("Admin - Features")]
public class FeatureController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<List<Feature>>>> GetAll(CancellationToken ct)
    {
        var features = await context.Features
            .AsNoTracking()
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Name)
            .ToListAsync(ct);
        return Ok(ApiResult<List<Feature>>.Success(features));
    }

    [HttpGet("by-category")]
    public async Task<ActionResult<ApiResult<object>>> GetByCategory(CancellationToken ct)
    {
        var features = await context.Features
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(ct);

        var grouped = features.GroupBy(x => x.Category)
            .ToDictionary(g => g.Key, g => g.ToList());

        return Ok(ApiResult<object>.Success(grouped));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<Feature>>> Create([FromBody] CreateFeatureDto input, CancellationToken ct)
    {
        var feature = new Feature
        {
            Code = input.Code,
            Name = input.Name,
            Category = input.Category,
            Description = input.Description,
            IsEnabled = input.IsEnabled
        };
        context.Features.Add(feature);
        await context.SaveChangesAsync(ct);
        return Ok(ApiResult<Feature>.Success(feature, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<Feature>>> Update(Guid id, [FromBody] CreateFeatureDto input, CancellationToken ct)
    {
        var entity = await context.Features.FindAsync([id], ct);
        if (entity == null) return NotFound();

        entity.Code = input.Code;
        entity.Name = input.Name;
        entity.Description = input.Description;
        entity.Category = input.Category;
        entity.IsEnabled = input.IsEnabled;

        await context.SaveChangesAsync(ct);
        return Ok(ApiResult<Feature>.Success(entity, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken ct)
    {
        var entity = await context.Features.FindAsync([id], ct);
        if (entity == null) return NotFound();

        context.Features.Remove(entity);
        await context.SaveChangesAsync(ct);
        return Ok(ApiResult.Success("删除成功"));
    }
}

public record CreateFeatureDto
{
    [Required(ErrorMessage = "功能编码不能为空")]
    [StringLength(100, ErrorMessage = "编码不能超过100个字符")]
    public string Code { get; init; } = string.Empty;

    [Required(ErrorMessage = "功能名称不能为空")]
    [StringLength(100, ErrorMessage = "名称不能超过100个字符")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "分类不能为空")]
    [StringLength(50, ErrorMessage = "分类不能超过50个字符")]
    public string Category { get; init; } = string.Empty;

    [StringLength(500, ErrorMessage = "描述不能超过500个字符")]
    public string? Description { get; init; }

    public bool IsEnabled { get; init; } = true;
}
