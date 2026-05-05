using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Admin.Dtos;
using WebApplication1.Features.Admin.Entities;
using WebApplication1.Features.Admin.Services;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Admin.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "owner")]
public class UserPersonaAdminController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPersonaService _personaService;

    public UserPersonaAdminController(AppDbContext context, IPersonaService personaService)
    {
        _context = context;
        _personaService = personaService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<UserPersonaDto>>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? keyword = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
            query = query.Where(x => x.Username.Contains(keyword) || x.RealName.Contains(keyword));

        var total = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var userIds = users.Select(x => x.Id).ToList();

        var userPersonas = await _context.UserPersonas
            .Where(x => userIds.Contains(x.UserId))
            .Include(x => x.PersonaType)
            .ToListAsync(cancellationToken);

        var userPersonaDict = userPersonas.GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => g.Select(x => new PersonaTypeDto
            {
                Id = x.PersonaType.Id,
                Code = x.PersonaType.Code,
                Name = x.PersonaType.Name,
                Icon = x.PersonaType.Icon,
                IsPrimary = x.IsPrimary
            }).ToList());

        var items = users.Select(x => new UserPersonaDto
        {
            Id = x.Id,
            UserId = x.Id,
            Username = x.Username,
            RealName = x.RealName,
            Personas = userPersonaDict.GetValueOrDefault(x.Id, new()),
            Role = x.Roles,
            CreatedAt = x.CreatedAt
        }).ToList();

        var result = PageResult<UserPersonaDto>.Create(items, total, page, pageSize);
        return Ok(ApiResult<PageResult<UserPersonaDto>>.Success(result));
    }

    [HttpPost("{id:guid}/personas")]
    public async Task<ActionResult<ApiResult>> AssignPersona(Guid id, [FromBody] AssignPersonaRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync([id], cancellationToken);
        if (user == null)
            return NotFound(ApiResult.Fail("User not found"));

        var personaType = await _context.PersonaTypes.FindAsync([request.PersonaTypeId], cancellationToken);
        if (personaType == null || !personaType.IsActive)
            return BadRequest(ApiResult.Fail("Invalid persona type"));

        var existing = await _context.UserPersonas
            .FirstOrDefaultAsync(x => x.UserId == id && x.PersonaTypeId == request.PersonaTypeId, cancellationToken);

        if (existing != null)
            return Ok(ApiResult.Success("Persona already assigned"));

        var userPersona = new UserPersona
        {
            UserId = id,
            PersonaTypeId = request.PersonaTypeId,
            IsPrimary = request.IsPrimary
        };

        if (request.IsPrimary)
        {
            var currentPrimary = await _context.UserPersonas
                .Where(x => x.UserId == id && x.IsPrimary)
                .ToListAsync(cancellationToken);
            foreach (var p in currentPrimary)
                p.IsPrimary = false;
        }

        _context.UserPersonas.Add(userPersona);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResult.Success("Persona assigned successfully"));
    }

    [HttpDelete("{id:guid}/personas/{personaTypeId:guid}")]
    public async Task<ActionResult<ApiResult>> RemovePersona(Guid id, Guid personaTypeId, CancellationToken cancellationToken)
    {
        var userPersona = await _context.UserPersonas
            .FirstOrDefaultAsync(x => x.UserId == id && x.PersonaTypeId == personaTypeId, cancellationToken);

        if (userPersona == null)
            return NotFound(ApiResult.Fail("Persona not found"));

        _context.UserPersonas.Remove(userPersona);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok(ApiResult.Success("Persona removed successfully"));
    }
}
