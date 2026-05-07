using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Route("api/work/log-templates")]
[Authorize]
[Tags("Work - Log Templates")]
public class WorkLogTemplateController(AppDbContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<WorkLogTemplateDto>>>> GetPage(
        [FromQuery] WorkLogTemplateQueryDto query,
        CancellationToken ct)
    {
        var q = context.WorkLogTemplates.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.PersonaCode))
            q = q.Where(x => x.PersonaCode == query.PersonaCode);
        if (query.IsActive.HasValue)
            q = q.Where(x => x.IsActive == query.IsActive.Value);

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderBy(x => x.Sort)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        return Ok(ApiResult<PageResult<WorkLogTemplateDto>>.Success(new PageResult<WorkLogTemplateDto>
        {
            Items = items.Select(MapToDto).ToList(),
            Total = total,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }

    [HttpGet("all")]
    public async Task<ActionResult<ApiResult<List<WorkLogTemplateDto>>>> GetAll(CancellationToken ct)
    {
        var templates = await context.WorkLogTemplates
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Sort)
            .ToListAsync(ct);
        return Ok(ApiResult<List<WorkLogTemplateDto>>.Success(templates.Select(MapToDto).ToList()));
    }

    [HttpGet("{personaCode}")]
    public async Task<ActionResult<ApiResult<WorkLogTemplateDto>>> GetByPersona(string personaCode, CancellationToken ct)
    {
        var template = await context.WorkLogTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PersonaCode == personaCode && x.IsActive, ct);
        return Ok(ApiResult<WorkLogTemplateDto>.Success(template == null ? null! : MapToDto(template)));
    }

    [HttpGet("my")]
    public async Task<ActionResult<ApiResult<WorkLogTemplateDto>>> GetMyTemplate(CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var template = await context.WorkLogTemplates
            .AsNoTracking()
            .Where(x => x.IsActive &&
                context.UserPersonas.Any(up => up.UserId == userId.Value && up.PersonaType!.Code == x.PersonaCode))
            .OrderByDescending(x => context.UserPersonas
                .Any(up => up.UserId == userId.Value && up.PersonaType!.Code == x.PersonaCode && up.IsPrimary))
            .ThenBy(x => x.Sort)
            .FirstOrDefaultAsync(ct);

        return Ok(ApiResult<WorkLogTemplateDto>.Success(template == null ? null! : MapToDto(template)));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkLogTemplateDto>>> GetById(Guid id, CancellationToken ct)
    {
        var template = await context.WorkLogTemplates.FindAsync([id], ct);
        if (template == null)
            return NotFound(ApiResult<WorkLogTemplateDto>.Fail("模板不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WorkLogTemplateDto>.Success(MapToDto(template)));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<WorkLogTemplateDto>>> Create(
        [FromBody] CreateWorkLogTemplateDto input,
        CancellationToken ct)
    {
        var exists = await context.WorkLogTemplates
            .AnyAsync(x => x.PersonaCode == input.PersonaCode && x.Name == input.Name, ct);
        if (exists)
            return BadRequest(ApiResult<WorkLogTemplateDto>.Fail("该Persona下已存在同名模板"));

        var entity = new WorkLogTemplate
        {
            PersonaCode = input.PersonaCode,
            Name = input.Name,
            Description = input.Description,
            FieldDefinitions = JsonSerializer.Serialize(input.FieldDefinitions),
            IsActive = input.IsActive,
            Sort = input.Sort
        };

        context.WorkLogTemplates.Add(entity);
        await context.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ApiResult<WorkLogTemplateDto>.Success(MapToDto(entity), "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkLogTemplateDto>>> Update(
        Guid id,
        [FromBody] UpdateWorkLogTemplateDto input,
        CancellationToken ct)
    {
        var entity = await context.WorkLogTemplates.FindAsync([id], ct);
        if (entity == null)
            return NotFound(ApiResult<WorkLogTemplateDto>.Fail("模板不存在", StatusCodes.Status404NotFound));

        if (input.Name != null) entity.Name = input.Name;
        if (input.Description != null) entity.Description = input.Description;
        if (input.FieldDefinitions != null) entity.FieldDefinitions = JsonSerializer.Serialize(input.FieldDefinitions);
        if (input.IsActive.HasValue) entity.IsActive = input.IsActive.Value;
        if (input.Sort.HasValue) entity.Sort = input.Sort.Value;

        await context.SaveChangesAsync(ct);
        return Ok(ApiResult<WorkLogTemplateDto>.Success(MapToDto(entity), "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken ct)
    {
        var entity = await context.WorkLogTemplates.FindAsync([id], ct);
        if (entity == null)
            return NotFound(ApiResult.Fail("模板不存在", StatusCodes.Status404NotFound));

        context.WorkLogTemplates.Remove(entity);
        await context.SaveChangesAsync(ct);
        return Ok(ApiResult.Success("删除成功"));
    }

    private static WorkLogTemplateDto MapToDto(WorkLogTemplate entity)
    {
        var fieldDefs = new FieldDefinitionsDto();
        try
        {
            fieldDefs = JsonSerializer.Deserialize<FieldDefinitionsDto>(entity.FieldDefinitions) ?? new FieldDefinitionsDto();
        }
        catch { }

        return new WorkLogTemplateDto
        {
            Id = entity.Id,
            PersonaCode = entity.PersonaCode,
            Name = entity.Name,
            Description = entity.Description,
            FieldDefinitions = fieldDefs,
            IsActive = entity.IsActive,
            Sort = entity.Sort,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
