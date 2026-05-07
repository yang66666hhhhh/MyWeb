using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Entities;

namespace WebApplication1.Features.Work.Services;

public interface IWorkCategoryService
{
    Task<PageResult<WorkCategoryDto>> GetPageAsync(WorkCategoryQueryDto query, CancellationToken cancellationToken = default);
    Task<List<WorkCategoryDto>> GetTreeAsync(bool? isActive = true, CancellationToken cancellationToken = default);
    Task<WorkCategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkCategoryDto> CreateAsync(CreateWorkCategoryDto input, CancellationToken cancellationToken = default);
    Task<WorkCategoryDto?> UpdateAsync(Guid id, UpdateWorkCategoryDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public class WorkCategoryService(AppDbContext context) : IWorkCategoryService
{
    public async Task<PageResult<WorkCategoryDto>> GetPageAsync(WorkCategoryQueryDto query, CancellationToken cancellationToken = default)
    {
        var q = context.WorkCategories.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x => x.Name.Contains(query.Keyword) || x.Code.Contains(query.Keyword));
        if (query.IsActive.HasValue)
            q = q.Where(x => x.IsActive == query.IsActive.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderBy(x => x.Sort)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return PageResult<WorkCategoryDto>.Create(items.Select(MapToDto).ToList(), total, query.Page, query.PageSize);
    }

    public async Task<List<WorkCategoryDto>> GetTreeAsync(bool? isActive = true, CancellationToken cancellationToken = default)
    {
        var q = context.WorkCategories.AsNoTracking().AsQueryable();
        if (isActive.HasValue)
            q = q.Where(x => x.IsActive == isActive.Value);

        var all = await q.OrderBy(x => x.Sort).ToListAsync(cancellationToken);
        return BuildTree(all, null);
    }

    public async Task<WorkCategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.WorkCategories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<WorkCategoryDto> CreateAsync(CreateWorkCategoryDto input, CancellationToken cancellationToken = default)
    {
        var entity = new WorkCategory
        {
            Name = input.Name,
            Code = input.Code,
            Level = input.Level,
            ParentId = input.ParentId,
            Sort = input.Sort,
            IsActive = input.IsActive
        };

        context.WorkCategories.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<WorkCategoryDto?> UpdateAsync(Guid id, UpdateWorkCategoryDto input, CancellationToken cancellationToken = default)
    {
        var entity = await context.WorkCategories.FindAsync([id], cancellationToken);
        if (entity == null) return null;

        if (input.Name != null) entity.Name = input.Name;
        if (input.Code != null) entity.Code = input.Code;
        if (input.Level.HasValue) entity.Level = input.Level.Value;
        if (input.ParentId.HasValue) entity.ParentId = input.ParentId;
        if (input.Sort.HasValue) entity.Sort = input.Sort.Value;
        if (input.IsActive.HasValue) entity.IsActive = input.IsActive.Value;

        await context.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.WorkCategories.FindAsync([id], cancellationToken);
        if (entity == null) return false;

        var hasChildren = await context.WorkCategories.AnyAsync(x => x.ParentId == id, cancellationToken);
        if (hasChildren)
            throw new InvalidOperationException("该分类下存在子分类，无法删除");

        context.WorkCategories.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static WorkCategoryDto MapToDto(WorkCategory entity)
    {
        return new WorkCategoryDto
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            Name = entity.Name,
            Code = entity.Code,
            Level = entity.Level,
            ParentId = entity.ParentId,
            Sort = entity.Sort,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private static List<WorkCategoryDto> BuildTree(List<WorkCategory> all, Guid? parentId)
    {
        return all.Where(x => x.ParentId == parentId).Select(x => new WorkCategoryDto
        {
            Id = x.Id,
            TenantId = x.TenantId,
            Name = x.Name,
            Code = x.Code,
            Level = x.Level,
            ParentId = x.ParentId,
            Sort = x.Sort,
            IsActive = x.IsActive,
            Children = BuildTree(all, x.Id)
        }).ToList();
    }
}
