using Microsoft.EntityFrameworkCore;
using WebApplication1.Common;
using WebApplication1.Data;
using WebApplication1.Dtos.Work;
using WebApplication1.Services.Interfaces.IWork;

namespace WebApplication1.Services.Implementations.Work;

public class WorkTaskTypeService : IWorkTaskTypeService
{
    private readonly AppDbContext _context;

    public WorkTaskTypeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PageResult<WorkTaskTypeDto>> GetPageAsync(WorkTaskTypeQueryDto query, CancellationToken cancellationToken = default)
    {
        var q = _context.WorkTaskTypes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            q = q.Where(x => x.TypeName.Contains(query.Keyword) || (x.TypeCode != null && x.TypeCode.Contains(query.Keyword)));
        }

        if (query.Enabled.HasValue)
            q = q.Where(x => x.Enabled == query.Enabled.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.Sort)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new WorkTaskTypeDto
            {
                Id = x.Id,
                TypeName = x.TypeName,
                TypeCode = x.TypeCode,
                Description = x.Description,
                Sort = x.Sort,
                Enabled = x.Enabled,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return PageResult<WorkTaskTypeDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<WorkTaskTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkTaskTypes.FindAsync([id], cancellationToken);
        if (entity == null) return null;
        return new WorkTaskTypeDto
        {
            Id = entity.Id,
            TypeName = entity.TypeName,
            TypeCode = entity.TypeCode,
            Description = entity.Description,
            Sort = entity.Sort,
            Enabled = entity.Enabled,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<WorkTaskTypeDto> CreateAsync(CreateWorkTaskTypeDto input, CancellationToken cancellationToken = default)
    {
        var entity = new Entities.Work.WorkTaskType
        {
            TypeName = input.TypeName,
            TypeCode = input.TypeCode,
            Description = input.Description,
            Sort = input.Sort,
            Enabled = input.Enabled
        };

        _context.WorkTaskTypes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return new WorkTaskTypeDto
        {
            Id = entity.Id,
            TypeName = entity.TypeName,
            TypeCode = entity.TypeCode,
            Description = entity.Description,
            Sort = entity.Sort,
            Enabled = entity.Enabled,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<WorkTaskTypeDto?> UpdateAsync(Guid id, UpdateWorkTaskTypeDto input, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkTaskTypes.FindAsync([id], cancellationToken);
        if (entity == null) return null;

        entity.TypeName = input.TypeName;
        entity.TypeCode = input.TypeCode;
        entity.Description = input.Description;
        entity.Sort = input.Sort;
        entity.Enabled = input.Enabled;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return new WorkTaskTypeDto
        {
            Id = entity.Id,
            TypeName = entity.TypeName,
            TypeCode = entity.TypeCode,
            Description = entity.Description,
            Sort = entity.Sort,
            Enabled = entity.Enabled,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkTaskTypes.FindAsync([id], cancellationToken);
        if (entity == null) return false;

        _context.WorkTaskTypes.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
