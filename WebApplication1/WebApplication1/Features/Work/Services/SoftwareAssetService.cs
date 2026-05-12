using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Entities;

namespace WebApplication1.Features.Work.Services;

public interface ISoftwareAssetService
{
    Task<PageResult<SoftwareAssetDto>> GetPageAsync(SoftwareAssetQueryDto query, CancellationToken cancellationToken = default);
    Task<SoftwareAssetDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SoftwareAssetDto> CreateAsync(CreateSoftwareAssetDto input, CancellationToken cancellationToken = default);
    Task<SoftwareAssetDto?> UpdateAsync(Guid id, UpdateSoftwareAssetDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public class SoftwareAssetService(AppDbContext context) : ISoftwareAssetService
{
    public async Task<PageResult<SoftwareAssetDto>> GetPageAsync(SoftwareAssetQueryDto query, CancellationToken cancellationToken = default)
    {
        var q = context.SoftwareAssets.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x => x.Name.Contains(query.Keyword) || (x.Vendor != null && x.Vendor.Contains(query.Keyword)));
        if (query.Type.HasValue)
            q = q.Where(x => x.Type == (SoftwareAssetType)query.Type.Value);
        if (query.LicenseType.HasValue)
            q = q.Where(x => x.LicenseType == (SoftwareLicenseType)query.LicenseType.Value);
        if (query.Status.HasValue)
            q = q.Where(x => x.Status == (SoftwareAssetStatus)query.Status.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return PageResult<SoftwareAssetDto>.Create(items.Select(MapToDto).ToList(), total, query.Page, query.PageSize);
    }

    public async Task<SoftwareAssetDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.SoftwareAssets.FindAsync([id], cancellationToken);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<SoftwareAssetDto> CreateAsync(CreateSoftwareAssetDto input, CancellationToken cancellationToken = default)
    {
        var entity = new SoftwareAsset
        {
            Name = input.Name,
            Version = input.Version,
            Type = (SoftwareAssetType)input.Type,
            LicenseType = (SoftwareLicenseType)input.LicenseType,
            Status = (SoftwareAssetStatus)input.Status,
            Vendor = input.Vendor,
            PurchaseDate = input.PurchaseDate,
            ExpireDate = input.ExpireDate,
            Cost = input.Cost,
            Description = input.Description,
            AssignedTo = input.AssignedTo
        };

        context.SoftwareAssets.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<SoftwareAssetDto?> UpdateAsync(Guid id, UpdateSoftwareAssetDto input, CancellationToken cancellationToken = default)
    {
        var entity = await context.SoftwareAssets.FindAsync([id], cancellationToken);
        if (entity == null) return null;

        if (input.Name != null) entity.Name = input.Name;
        if (input.Version != null) entity.Version = input.Version;
        if (input.Type.HasValue) entity.Type = (SoftwareAssetType)input.Type.Value;
        if (input.LicenseType.HasValue) entity.LicenseType = (SoftwareLicenseType)input.LicenseType.Value;
        if (input.Status.HasValue) entity.Status = (SoftwareAssetStatus)input.Status.Value;
        if (input.Vendor != null) entity.Vendor = input.Vendor;
        if (input.PurchaseDate.HasValue) entity.PurchaseDate = input.PurchaseDate;
        if (input.ExpireDate.HasValue) entity.ExpireDate = input.ExpireDate;
        if (input.Cost.HasValue) entity.Cost = input.Cost;
        if (input.Description != null) entity.Description = input.Description;
        if (input.AssignedTo != null) entity.AssignedTo = input.AssignedTo;

        await context.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.SoftwareAssets.FindAsync([id], cancellationToken);
        if (entity == null) return false;

        context.SoftwareAssets.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static SoftwareAssetDto MapToDto(SoftwareAsset entity)
    {
        return new SoftwareAssetDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = entity.Name,
            Version = entity.Version,
            Type = (int)entity.Type,
            LicenseType = (int)entity.LicenseType,
            Status = (int)entity.Status,
            Vendor = entity.Vendor,
            PurchaseDate = entity.PurchaseDate,
            ExpireDate = entity.ExpireDate,
            Cost = entity.Cost,
            Description = entity.Description,
            AssignedTo = entity.AssignedTo,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
