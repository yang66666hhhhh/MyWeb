using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Auth.Entities.Subscription;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Auth.Controllers;

[ApiController]
[Route("api/admin/persona-features")]
[Authorize(Roles = "owner")]
[Tags("Admin - Features")]
public class PersonaFeatureController(AppDbContext context) : ControllerBase
{
    [HttpGet("{personaCode}")]
    public async Task<ActionResult<ApiResult<List<Feature>>>> GetPersonaFeatures(string personaCode, CancellationToken ct)
    {
        var features = await context.PersonaFeatures
            .AsNoTracking()
            .Where(x => x.PersonaCode == personaCode)
            .Select(x => x.Feature!)
            .ToListAsync(ct);
        return Ok(ApiResult<List<Feature>>.Success(features));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<List<PersonaFeature>>>> GetAll(CancellationToken ct)
    {
        var all = await context.PersonaFeatures
            .AsNoTracking()
            .Include(x => x.Feature)
            .ToListAsync(ct);
        return Ok(ApiResult<List<PersonaFeature>>.Success(all));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult>> AddFeature([FromBody] AddPersonaFeatureRequest request, CancellationToken ct)
    {
        var exists = await context.PersonaFeatures
            .AnyAsync(x => x.PersonaCode == request.PersonaCode && x.FeatureId == request.FeatureId, ct);

        if (exists)
            return BadRequest(ApiResult.Fail("该功能已分配给此 Persona"));

        context.PersonaFeatures.Add(new PersonaFeature
        {
            PersonaCode = request.PersonaCode,
            FeatureId = request.FeatureId
        });
        await context.SaveChangesAsync(ct);
        return Ok(ApiResult.Success("功能已添加"));
    }

    [HttpPost("batch")]
    public async Task<ActionResult<ApiResult>> BatchAddFeatures([FromBody] BatchPersonaFeatureRequest request, CancellationToken ct)
    {
        var featureIds = request.FeatureIds.ToHashSet();
        var existing = await context.PersonaFeatures
            .Where(x => x.PersonaCode == request.PersonaCode && featureIds.Contains(x.FeatureId))
            .Select(x => x.FeatureId)
            .ToListAsync(ct);

        var newFeatures = request.FeatureIds
            .Where(id => !existing.Contains(id))
            .Select(id => new PersonaFeature { PersonaCode = request.PersonaCode, FeatureId = id })
            .ToList();

        context.PersonaFeatures.AddRange(newFeatures);
        await context.SaveChangesAsync(ct);
        return Ok(ApiResult.Success($"已添加 {newFeatures.Count} 个功能"));
    }

    [HttpDelete("{personaCode}/{featureId:guid}")]
    public async Task<ActionResult<ApiResult>> RemoveFeature(string personaCode, Guid featureId, CancellationToken ct)
    {
        var entity = await context.PersonaFeatures
            .FirstOrDefaultAsync(x => x.PersonaCode == personaCode && x.FeatureId == featureId, ct);

        if (entity == null) return NotFound();

        context.PersonaFeatures.Remove(entity);
        await context.SaveChangesAsync(ct);
        return Ok(ApiResult.Success("功能已移除"));
    }

    [HttpDelete("{personaCode}")]
    public async Task<ActionResult<ApiResult>> ClearPersonaFeatures(string personaCode, CancellationToken ct)
    {
        var entities = await context.PersonaFeatures
            .Where(x => x.PersonaCode == personaCode)
            .ToListAsync(ct);

        context.PersonaFeatures.RemoveRange(entities);
        await context.SaveChangesAsync(ct);
        return Ok(ApiResult.Success("已清空该 Persona 的所有功能"));
    }
}

public class AddPersonaFeatureRequest
{
    public string PersonaCode { get; set; } = string.Empty;
    public Guid FeatureId { get; set; }
}

public class BatchPersonaFeatureRequest
{
    public string PersonaCode { get; set; } = string.Empty;
    public List<Guid> FeatureIds { get; set; } = new();
}
