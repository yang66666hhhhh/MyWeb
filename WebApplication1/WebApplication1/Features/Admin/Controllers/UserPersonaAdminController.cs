using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Admin.Dtos;
using WebApplication1.Features.Admin.Entities;
using WebApplication1.Features.Auth.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Admin.Controllers;

[ApiController]
[Route("api/system/users")]
[Authorize(Roles = "owner")]
[Tags("Admin - Users")]
public class UserPersonaAdminController(AppDbContext context, ILogger<UserPersonaAdminController> logger) : BaseApiController
{
    [HttpGet("personas")]
    public async Task<ActionResult<ApiResult<PageResult<UserPersonaDto>>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? keyword = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x => x.Username.Contains(keyword) || x.RealName.Contains(keyword));

            var total = await query.CountAsync(cancellationToken);

            var users = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var userIds = users.Select(x => x.Id).ToList();

            var userPersonas = await context.UserPersonas
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
        catch (Exception ex)
        {
            logger.LogError(ex, "获取用户身份列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpPost("{id:guid}/personas")]
    public async Task<ActionResult<ApiResult>> AssignPersona(Guid id, [FromBody] AssignPersonaRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await context.Users.FindAsync([id], cancellationToken);
            if (user == null)
                return NotFound(ApiResult.Fail("用户不存在"));

            var personaType = await context.PersonaTypes.FindAsync([request.PersonaTypeId], cancellationToken);
            if (personaType == null || !personaType.IsActive)
                return BadRequest(ApiResult.Fail("身份类型无效"));

            var existing = await context.UserPersonas
                .FirstOrDefaultAsync(x => x.UserId == id && x.PersonaTypeId == request.PersonaTypeId, cancellationToken);

            if (existing != null)
                return Ok(ApiResult.Success("身份已分配"));

            var userPersona = new UserPersona
            {
                UserId = id,
                PersonaTypeId = request.PersonaTypeId,
                IsPrimary = request.IsPrimary
            };

            if (request.IsPrimary)
            {
                var currentPrimary = await context.UserPersonas
                    .Where(x => x.UserId == id && x.IsPrimary)
                    .ToListAsync(cancellationToken);
                foreach (var p in currentPrimary)
                    p.IsPrimary = false;
            }

            context.UserPersonas.Add(userPersona);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("为用户 {UserId} 分配身份 {PersonaType}", id, personaType.Code);
            return Ok(ApiResult.Success("身份分配成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "分配身份失败: {UserId}", id);
            return StatusCode(500, ApiResult.Fail("分配失败，请稍后重试"));
        }
    }

    [HttpDelete("{id:guid}/personas/{personaTypeId:guid}")]
    public async Task<ActionResult<ApiResult>> RemovePersona(Guid id, Guid personaTypeId, CancellationToken cancellationToken)
    {
        try
        {
            var userPersona = await context.UserPersonas
                .FirstOrDefaultAsync(x => x.UserId == id && x.PersonaTypeId == personaTypeId, cancellationToken);

            if (userPersona == null)
                return NotFound(ApiResult.Fail("身份不存在"));

            var wasPrimary = userPersona.IsPrimary;
            context.UserPersonas.Remove(userPersona);
            if (wasPrimary)
            {
                var nextPrimary = await context.UserPersonas
                    .Where(x => x.UserId == id && x.PersonaTypeId != personaTypeId)
                    .OrderBy(x => x.CreatedAt)
                    .FirstOrDefaultAsync(cancellationToken);
                if (nextPrimary != null)
                {
                    nextPrimary.IsPrimary = true;
                }
            }

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("移除用户 {UserId} 的身份 {PersonaType}", id, personaTypeId);
            return Ok(ApiResult.Success("身份移除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "移除身份失败: {UserId}, {PersonaType}", id, personaTypeId);
            return StatusCode(500, ApiResult.Fail("移除失败，请稍后重试"));
        }
    }
}
