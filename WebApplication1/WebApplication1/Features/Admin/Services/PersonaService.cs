using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplication1.Features.Admin.Dtos;
using WebApplication1.Features.Admin.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Admin.Services;

public class PersonaService : IPersonaService
{
    private readonly AppDbContext _context;
    private readonly ILogger<PersonaService> _logger;

    public PersonaService(AppDbContext context, ILogger<PersonaService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PageResult<PersonaTypeDto>> GetPageAsync(PersonaTypeQueryDto query, CancellationToken cancellationToken = default)
    {
        var q = _context.PersonaTypes.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x => x.Name.Contains(query.Keyword) || x.Code.Contains(query.Keyword));

        if (query.IsActive.HasValue)
            q = q.Where(x => x.IsActive == query.IsActive.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderBy(x => x.Sort)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return PageResult<PersonaTypeDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<List<PersonaTypeDto>> GetAllAsync(bool? isActive = true, CancellationToken cancellationToken = default)
    {
        var q = _context.PersonaTypes.AsNoTracking().AsQueryable();

        if (isActive.HasValue)
            q = q.Where(x => x.IsActive == isActive.Value);

        return await q
            .OrderBy(x => x.Sort)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<PersonaTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PersonaTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<PersonaTypeDto> CreateAsync(CreatePersonaTypeDto input, CancellationToken cancellationToken = default)
    {
        if (await _context.PersonaTypes.AnyAsync(x => x.Code == input.Code, cancellationToken))
            throw new InvalidOperationException($"PersonaType with code '{input.Code}' already exists");

        var entity = new PersonaType
        {
            Code = input.Code,
            Name = input.Name,
            Icon = input.Icon,
            Description = input.Description,
            DefaultHomeRoute = input.DefaultHomeRoute,
            Sort = input.Sort,
            IsActive = true
        };

        _context.PersonaTypes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<PersonaTypeDto?> UpdateAsync(Guid id, UpdatePersonaTypeDto input, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PersonaTypes.FindAsync([id], cancellationToken);
        if (entity == null) return null;

        entity.Name = input.Name;
        entity.Icon = input.Icon;
        entity.Description = input.Description;
        entity.DefaultHomeRoute = input.DefaultHomeRoute;
        entity.Sort = input.Sort;
        entity.IsActive = input.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.PersonaTypes.FindAsync([id], cancellationToken);
        if (entity == null) return false;

        var hasUsers = await _context.UserPersonas.AnyAsync(x => x.PersonaTypeId == id, cancellationToken);
        if (hasUsers)
            throw new InvalidOperationException("Cannot delete persona type that is assigned to users");

        var hasMenuItems = await _context.PersonaMenuItems.AnyAsync(x => x.PersonaTypeId == id, cancellationToken);
        if (hasMenuItems)
        {
            var menuItems = await _context.PersonaMenuItems.Where(x => x.PersonaTypeId == id).ToListAsync(cancellationToken);
            _context.PersonaMenuItems.RemoveRange(menuItems);
        }

        _context.PersonaTypes.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<PersonaMenuItemDto>> GetMenuItemsAsync(Guid personaTypeId, CancellationToken cancellationToken = default)
    {
        return await _context.PersonaMenuItems
            .AsNoTracking()
            .Where(x => x.PersonaTypeId == personaTypeId)
            .OrderBy(x => x.Sort)
            .Select(x => new PersonaMenuItemDto
            {
                Id = x.Id,
                PersonaTypeId = x.PersonaTypeId,
                MenuPath = x.MenuPath,
                MenuName = x.MenuName,
                Icon = x.Icon,
                Sort = x.Sort,
                IsVisible = x.IsVisible,
                ParentId = x.ParentId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> SetMenuItemsAsync(Guid personaTypeId, List<PersonaMenuItemDto> items, CancellationToken cancellationToken = default)
    {
        var existing = await _context.PersonaMenuItems
            .Where(x => x.PersonaTypeId == personaTypeId)
            .ToListAsync(cancellationToken);

        _context.PersonaMenuItems.RemoveRange(existing);

        foreach (var item in items)
        {
            _context.PersonaMenuItems.Add(new PersonaMenuItem
            {
                PersonaTypeId = personaTypeId,
                MenuPath = item.MenuPath,
                MenuName = item.MenuName,
                Icon = item.Icon,
                Sort = item.Sort,
                IsVisible = item.IsVisible,
                ParentId = item.ParentId
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static PersonaTypeDto MapToDto(PersonaType entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Name = entity.Name,
        Icon = entity.Icon,
        Description = entity.Description,
        DefaultHomeRoute = entity.DefaultHomeRoute,
        Sort = entity.Sort,
        IsActive = entity.IsActive,
        CreatedAt = entity.CreatedAt
    };
}
