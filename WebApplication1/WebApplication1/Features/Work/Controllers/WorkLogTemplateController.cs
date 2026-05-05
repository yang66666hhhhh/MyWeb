using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Route("api/work/log-templates")]
[Authorize]
public class WorkLogTemplateController(AppDbContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<List<WorkLogTemplate>>>> GetAll(CancellationToken ct)
    {
        var templates = await context.WorkLogTemplates
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Sort)
            .ToListAsync(ct);
        return Ok(ApiResult<List<WorkLogTemplate>>.Success(templates));
    }

    [HttpGet("{personaCode}")]
    public async Task<ActionResult<ApiResult<WorkLogTemplate?>>> GetByPersona(string personaCode, CancellationToken ct)
    {
        var template = await context.WorkLogTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PersonaCode == personaCode && x.IsActive, ct);
        return Ok(ApiResult<WorkLogTemplate?>.Success(template));
    }

    [HttpGet("my")]
    public async Task<ActionResult<ApiResult<WorkLogTemplate?>>> GetMyTemplate(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var personaCodes = await context.UserPersonas
            .Where(x => x.UserId == userId.Value)
            .Select(x => x.PersonaType!.Code)
            .ToListAsync(ct);

        var template = await context.WorkLogTemplates
            .AsNoTracking()
            .Where(x => x.IsActive && personaCodes.Contains(x.PersonaCode))
            .OrderByDescending(x => x.PersonaCode == personaCodes.FirstOrDefault())
            .ThenBy(x => x.Sort)
            .FirstOrDefaultAsync(ct);

        return Ok(ApiResult<WorkLogTemplate?>.Success(template));
    }
}
