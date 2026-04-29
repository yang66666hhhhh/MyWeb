using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services.Interfaces;

namespace WebApplication1.Features.Work.Services;

public class WorkProjectService : IWorkProjectService
{
    private readonly AppDbContext _context;

    public WorkProjectService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PageResult<WorkProjectDto>> GetPageAsync(WorkProjectQueryDto query, CancellationToken cancellationToken = default)
    {
        var q = _context.WorkProjects.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            q = q.Where(x => x.ProjectName.Contains(query.Keyword) || (x.ProjectCode != null && x.ProjectCode.Contains(query.Keyword)));
        }

        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        if (query.ProjectType.HasValue)
            q = q.Where(x => x.ProjectType == query.ProjectType.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new WorkProjectDto
            {
                Id = x.Id,
                ProjectName = x.ProjectName,
                ProjectCode = x.ProjectCode,
                ProjectType = x.ProjectType,
                CustomerName = x.CustomerName,
                Description = x.Description,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Status = x.Status,
                Sort = x.Sort,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return PageResult<WorkProjectDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<WorkProjectDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkProjects.FindAsync([id], cancellationToken);
        if (entity == null) return null;
        return new WorkProjectDto
        {
            Id = entity.Id,
            ProjectName = entity.ProjectName,
            ProjectCode = entity.ProjectCode,
            ProjectType = entity.ProjectType,
            CustomerName = entity.CustomerName,
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Status = entity.Status,
            Sort = entity.Sort,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<WorkProjectDto> CreateAsync(CreateWorkProjectDto input, CancellationToken cancellationToken = default)
    {
        var entity = new Work.Entities.WorkProject
        {
            ProjectName = input.ProjectName,
            ProjectCode = input.ProjectCode,
            ProjectType = input.ProjectType,
            CustomerName = input.CustomerName,
            Description = input.Description,
            StartDate = input.StartDate,
            EndDate = input.EndDate,
            Status = input.Status,
            Sort = input.Sort
        };

        _context.WorkProjects.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return new WorkProjectDto
        {
            Id = entity.Id,
            ProjectName = entity.ProjectName,
            ProjectCode = entity.ProjectCode,
            ProjectType = entity.ProjectType,
            CustomerName = entity.CustomerName,
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Status = entity.Status,
            Sort = entity.Sort,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<WorkProjectDto?> UpdateAsync(Guid id, UpdateWorkProjectDto input, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkProjects.FindAsync([id], cancellationToken);
        if (entity == null) return null;

        entity.ProjectName = input.ProjectName;
        entity.ProjectCode = input.ProjectCode;
        entity.ProjectType = input.ProjectType;
        entity.CustomerName = input.CustomerName;
        entity.Description = input.Description;
        entity.StartDate = input.StartDate;
        entity.EndDate = input.EndDate;
        entity.Status = input.Status;
        entity.Sort = input.Sort;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return new WorkProjectDto
        {
            Id = entity.Id,
            ProjectName = entity.ProjectName,
            ProjectCode = entity.ProjectCode,
            ProjectType = entity.ProjectType,
            CustomerName = entity.CustomerName,
            Description = entity.Description,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Status = entity.Status,
            Sort = entity.Sort,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkProjects.FindAsync([id], cancellationToken);
        if (entity == null) return false;

        _context.WorkProjects.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
