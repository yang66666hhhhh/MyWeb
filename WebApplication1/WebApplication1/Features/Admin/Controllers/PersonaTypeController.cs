using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Admin.Dtos;
using WebApplication1.Features.Admin.Services;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Admin.Controllers;

[ApiController]
[Route("api/system/persona-types")]
[Authorize]
[Tags("Admin - Personas")]
public class PersonaTypeController : ControllerBase
{
    private readonly IPersonaService _personaService;

    public PersonaTypeController(IPersonaService personaService)
    {
        _personaService = personaService;
    }

    [HttpGet]
    [Authorize(Roles = "pro,owner")]
    public async Task<ActionResult<ApiResult<PageResult<PersonaTypeDto>>>> GetPage([FromQuery] PersonaTypeQueryDto query, CancellationToken cancellationToken)
    {
        var result = await _personaService.GetPageAsync(query, cancellationToken);
        return Ok(ApiResult<PageResult<PersonaTypeDto>>.Success(result));
    }

    [HttpGet("all")]
    [Authorize(Roles = "pro,owner")]
    public async Task<ActionResult<ApiResult<List<PersonaTypeDto>>>> GetAll([FromQuery] bool? isActive = true, CancellationToken cancellationToken = default)
    {
        var result = await _personaService.GetAllAsync(isActive, cancellationToken);
        return Ok(ApiResult<List<PersonaTypeDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<PersonaTypeDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _personaService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return NotFound(ApiResult<PersonaTypeDto>.Fail("Identity type not found"));
        return Ok(ApiResult<PersonaTypeDto>.Success(result));
    }

    [HttpPost]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult<PersonaTypeDto>>> Create([FromBody] CreatePersonaTypeDto input, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _personaService.CreateAsync(input, cancellationToken);
            return Ok(ApiResult<PersonaTypeDto>.Success(result, "Created successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResult<PersonaTypeDto>.Fail(ex.Message));
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult<PersonaTypeDto>>> Update(Guid id, [FromBody] UpdatePersonaTypeDto input, CancellationToken cancellationToken)
    {
        var result = await _personaService.UpdateAsync(id, input, cancellationToken);
        if (result == null)
            return NotFound(ApiResult<PersonaTypeDto>.Fail("Identity type not found"));
        return Ok(ApiResult<PersonaTypeDto>.Success(result, "Updated successfully"));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _personaService.DeleteAsync(id, cancellationToken);
            if (!success)
                return NotFound(ApiResult.Fail("Identity type not found"));
            return Ok(ApiResult.Success("Deleted successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResult.Fail(ex.Message));
        }
    }

    [HttpGet("{id:guid}/menu-items")]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult<List<PersonaMenuItemDto>>>> GetMenuItems(Guid id, CancellationToken cancellationToken)
    {
        var result = await _personaService.GetMenuItemsAsync(id, cancellationToken);
        return Ok(ApiResult<List<PersonaMenuItemDto>>.Success(result));
    }

    [HttpPut("{id:guid}/menu-items")]
    [Authorize(Roles = "owner")]
    public async Task<ActionResult<ApiResult>> SetMenuItems(Guid id, [FromBody] List<PersonaMenuItemDto> items, CancellationToken cancellationToken)
    {
        await _personaService.SetMenuItemsAsync(id, items, cancellationToken);
        return Ok(ApiResult.Success("Menu items updated successfully"));
    }
}
