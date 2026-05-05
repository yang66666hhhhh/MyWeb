using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Admin.Dtos;
using WebApplication1.Features.Admin.Entities;
using WebApplication1.Features.Admin.Services;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.User.Controllers;

[ApiController]
[Route("api/user/persona")]
[Authorize]
public class UserPersonaController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPersonaService _personaService;

    public UserPersonaController(AppDbContext context, IPersonaService personaService)
    {
        _context = context;
        _personaService = personaService;
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }

    [HttpGet("current")]
    public async Task<ActionResult<ApiResult<PersonaTypeDto?>>> GetCurrentPersona(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("User not authenticated"));

        var userPersonas = await _context.UserPersonas
            .Where(x => x.UserId == userId.Value)
            .Include(x => x.PersonaType)
            .ToListAsync(cancellationToken);

        if (!userPersonas.Any())
        {
            var generalPersona = await _personaService.GetAllAsync(isActive: true, cancellationToken: cancellationToken);
            var general = generalPersona.FirstOrDefault(x => x.Code == "General");
            if (general != null)
                return Ok(ApiResult<PersonaTypeDto?>.Success(general));
            return Ok(ApiResult<PersonaTypeDto?>.Success(null));
        }

        var primary = userPersonas.FirstOrDefault(x => x.IsPrimary) ?? userPersonas.First();
        var persona = new PersonaTypeDto
        {
            Id = primary.PersonaType.Id,
            Code = primary.PersonaType.Code,
            Name = primary.PersonaType.Name,
            Icon = primary.PersonaType.Icon,
            Description = primary.PersonaType.Description,
            IsPrimary = primary.IsPrimary
        };

        return Ok(ApiResult<PersonaTypeDto?>.Success(persona));
    }

    [HttpGet("available")]
    public async Task<ActionResult<ApiResult<List<PersonaTypeDto>>>> GetAvailablePersonas(CancellationToken cancellationToken)
    {
        var personas = await _personaService.GetAllAsync(isActive: true, cancellationToken: cancellationToken);
        return Ok(ApiResult<List<PersonaTypeDto>>.Success(personas));
    }

    [HttpPost("assign")]
    public async Task<ActionResult<ApiResult>> AssignPersona([FromBody] AssignPersonaRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("User not authenticated"));

        var personaType = await _context.PersonaTypes.FindAsync([request.PersonaTypeId], cancellationToken);
        if (personaType == null || !personaType.IsActive)
            return BadRequest(ApiResult.Fail("Invalid persona type"));

        var existing = await _context.UserPersonas
            .FirstOrDefaultAsync(x => x.UserId == userId.Value && x.PersonaTypeId == request.PersonaTypeId, cancellationToken);

        if (existing != null)
            return Ok(ApiResult.Success("Persona already assigned"));

        var userPersona = new UserPersona
        {
            UserId = userId.Value,
            PersonaTypeId = request.PersonaTypeId,
            IsPrimary = request.IsPrimary
        };

        if (request.IsPrimary)
        {
            var currentPrimary = await _context.UserPersonas
                .Where(x => x.UserId == userId.Value && x.IsPrimary)
                .ToListAsync(cancellationToken);
            foreach (var p in currentPrimary)
                p.IsPrimary = false;
        }

        _context.UserPersonas.Add(userPersona);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResult.Success("Persona assigned successfully"));
    }

    [HttpDelete("remove/{personaTypeId:guid}")]
    public async Task<ActionResult<ApiResult>> RemovePersona(Guid personaTypeId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("User not authenticated"));

        var userPersona = await _context.UserPersonas
            .FirstOrDefaultAsync(x => x.UserId == userId.Value && x.PersonaTypeId == personaTypeId, cancellationToken);

        if (userPersona == null)
            return NotFound(ApiResult.Fail("Persona not found"));

        _context.UserPersonas.Remove(userPersona);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResult.Success("Persona removed successfully"));
    }

    [HttpPut("set-primary/{personaTypeId:guid}")]
    public async Task<ActionResult<ApiResult>> SetPrimaryPersona(Guid personaTypeId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("User not authenticated"));

        var userPersona = await _context.UserPersonas
            .FirstOrDefaultAsync(x => x.UserId == userId.Value && x.PersonaTypeId == personaTypeId, cancellationToken);

        if (userPersona == null)
            return NotFound(ApiResult.Fail("Persona not found"));

        var currentPrimary = await _context.UserPersonas
            .Where(x => x.UserId == userId.Value && x.IsPrimary)
            .ToListAsync(cancellationToken);
        foreach (var p in currentPrimary)
            p.IsPrimary = false;

        userPersona.IsPrimary = true;
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResult.Success("Primary persona set successfully"));
    }

    [HttpGet("history")]
    public async Task<ActionResult<ApiResult<List<UserPersonaHistoryDto>>>> GetHistory(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("User not authenticated"));

        var records = await _context.UserPersonaRecords
            .Where(x => x.UserId == userId.Value)
            .OrderByDescending(x => x.SwitchedAt)
            .Take(20)
            .ToListAsync(cancellationToken);

        var personaTypeIds = records.Select(r => r.PersonaTypeId).Distinct().ToList();
        var personaTypes = await _context.PersonaTypes
            .Where(p => personaTypeIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, cancellationToken);

        var result = records.Select(r => new UserPersonaHistoryDto
        {
            Id = r.Id,
            PersonaTypeId = r.PersonaTypeId,
            PersonaName = personaTypes.TryGetValue(r.PersonaTypeId, out var p) ? p.Name : "Unknown",
            PersonaIcon = personaTypes.TryGetValue(r.PersonaTypeId, out var p2) ? p2.Icon : "❓",
            SwitchedAt = r.SwitchedAt,
            Remark = r.Remark
        }).ToList();

        return Ok(ApiResult<List<UserPersonaHistoryDto>>.Success(result));
    }
}

public class UserPersonaHistoryDto
{
    public Guid Id { get; set; }
    public Guid PersonaTypeId { get; set; }
    public string PersonaName { get; set; } = string.Empty;
    public string PersonaIcon { get; set; } = string.Empty;
    public DateTime SwitchedAt { get; set; }
    public string? Remark { get; set; }
}
