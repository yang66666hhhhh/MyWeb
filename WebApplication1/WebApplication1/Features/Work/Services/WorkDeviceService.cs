using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services.Interfaces;

namespace WebApplication1.Features.Work.Services;

public class WorkDeviceService : IWorkDeviceService
{
    private readonly AppDbContext _context;

    public WorkDeviceService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PageResult<WorkDeviceDto>> GetPageAsync(WorkDeviceQueryDto query, CancellationToken cancellationToken = default)
    {
        var q = _context.WorkDevices.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            q = q.Where(x => x.DeviceName.Contains(query.Keyword) || (x.DeviceCode != null && x.DeviceCode.Contains(query.Keyword)));
        }

        if (query.ProjectId.HasValue)
            q = q.Where(x => x.ProjectId == query.ProjectId.Value);

        if (query.DeviceType.HasValue)
            q = q.Where(x => x.DeviceType == query.DeviceType.Value);

        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new WorkDeviceDto
            {
                Id = x.Id,
                ProjectId = x.ProjectId,
                DeviceName = x.DeviceName,
                DeviceCode = x.DeviceCode,
                DeviceType = x.DeviceType,
                Description = x.Description,
                Status = x.Status,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return PageResult<WorkDeviceDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<WorkDeviceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkDevices.FindAsync([id], cancellationToken);
        if (entity == null) return null;
        return new WorkDeviceDto
        {
            Id = entity.Id,
            ProjectId = entity.ProjectId,
            DeviceName = entity.DeviceName,
            DeviceCode = entity.DeviceCode,
            DeviceType = entity.DeviceType,
            Description = entity.Description,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<WorkDeviceDto> CreateAsync(CreateWorkDeviceDto input, CancellationToken cancellationToken = default)
    {
        var entity = new Work.Entities.WorkDevice
        {
            ProjectId = input.ProjectId,
            DeviceName = input.DeviceName,
            DeviceCode = input.DeviceCode,
            DeviceType = input.DeviceType,
            Description = input.Description,
            Status = input.Status
        };

        _context.WorkDevices.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return new WorkDeviceDto
        {
            Id = entity.Id,
            ProjectId = entity.ProjectId,
            DeviceName = entity.DeviceName,
            DeviceCode = entity.DeviceCode,
            DeviceType = entity.DeviceType,
            Description = entity.Description,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<WorkDeviceDto?> UpdateAsync(Guid id, UpdateWorkDeviceDto input, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkDevices.FindAsync([id], cancellationToken);
        if (entity == null) return null;

        entity.ProjectId = input.ProjectId;
        entity.DeviceName = input.DeviceName;
        entity.DeviceCode = input.DeviceCode;
        entity.DeviceType = input.DeviceType;
        entity.Description = input.Description;
        entity.Status = input.Status;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return new WorkDeviceDto
        {
            Id = entity.Id,
            ProjectId = entity.ProjectId,
            DeviceName = entity.DeviceName,
            DeviceCode = entity.DeviceCode,
            DeviceType = entity.DeviceType,
            Description = entity.Description,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkDevices.FindAsync([id], cancellationToken);
        if (entity == null) return false;

        _context.WorkDevices.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
