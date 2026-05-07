using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Auth.Entities.Subscription;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth.Controllers;

[ApiController]
[Route("api/admin/features")]
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
    public async Task<ActionResult<ApiResult<Feature>>> Create([FromBody] Feature input, CancellationToken ct)
    {
        context.Features.Add(input);
        await context.SaveChangesAsync(ct);
        return Ok(ApiResult<Feature>.Success(input, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<Feature>>> Update(Guid id, [FromBody] Feature input, CancellationToken ct)
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
