using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Work.Services;

public interface IWorkExtendedService
{
    Task<PageResult<OkrDto>> GetOkrsAsync(WorkExtendedQueryDto query, Guid? userId, CancellationToken ct = default);
    Task<OkrDto?> GetOkrByIdAsync(Guid id, CancellationToken ct = default);
    Task<OkrDto> CreateOkrAsync(CreateOkrInput input, Guid userId, CancellationToken ct = default);
    Task<OkrDto?> UpdateOkrAsync(Guid id, UpdateOkrInput input, CancellationToken ct = default);
    Task<bool> DeleteOkrAsync(Guid id, CancellationToken ct = default);

    Task<PageResult<RiskItemDto>> GetRisksAsync(WorkExtendedQueryDto query, Guid? userId, CancellationToken ct = default);
    Task<RiskItemDto?> GetRiskByIdAsync(Guid id, CancellationToken ct = default);
    Task<RiskItemDto> CreateRiskAsync(CreateRiskItemInput input, Guid userId, CancellationToken ct = default);
    Task<RiskItemDto?> UpdateRiskAsync(Guid id, UpdateRiskItemInput input, CancellationToken ct = default);
    Task<bool> DeleteRiskAsync(Guid id, CancellationToken ct = default);

    Task<PageResult<WorkFileDto>> GetFilesAsync(WorkExtendedQueryDto query, Guid? userId, CancellationToken ct = default);
    Task<WorkFileDto?> GetFileByIdAsync(Guid id, CancellationToken ct = default);
    Task<WorkFileDto> CreateFileAsync(CreateWorkFileInput input, Guid userId, CancellationToken ct = default);
    Task<WorkFileDto?> UpdateFileAsync(Guid id, UpdateWorkFileInput input, CancellationToken ct = default);
    Task<bool> DeleteFileAsync(Guid id, CancellationToken ct = default);
}

public class WorkExtendedService(AppDbContext context) : IWorkExtendedService
{
    private static DateTime? ParseDate(string? s) =>
        string.IsNullOrWhiteSpace(s) ? null : DateTime.TryParse(s, out var d) ? d : null;

    private static string FormatDate(DateTime? d) => d?.ToString("yyyy-MM-dd") ?? string.Empty;

    #region OKR

    public async Task<PageResult<OkrDto>> GetOkrsAsync(WorkExtendedQueryDto query, Guid? userId, CancellationToken ct = default)
    {
        var q = context.Okrs.AsNoTracking().AsQueryable();
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x => x.Title.Contains(query.Keyword) || (x.Description != null && x.Description.Contains(query.Keyword)));
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(x => new OkrDto
            {
                Id = x.Id.ToString(), UserId = x.UserId.ToString(), Title = x.Title,
                Description = x.Description, Objective = x.Objective, KeyResults = x.KeyResults,
                Status = x.Status, Progress = x.Progress,
                StartDate = x.StartDate.ToString(), EndDate = x.EndDate.ToString(),
                Tags = x.Tags, CreatedAt = x.CreatedAt.ToString()
            }).ToListAsync(ct);

        return PageResult<OkrDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<OkrDto?> GetOkrByIdAsync(Guid id, CancellationToken ct = default)
    {
        var x = await context.Okrs.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (x == null) return null;
        return new OkrDto
        {
            Id = x.Id.ToString(), UserId = x.UserId.ToString(), Title = x.Title,
            Description = x.Description, Objective = x.Objective, KeyResults = x.KeyResults,
            Status = x.Status, Progress = x.Progress,
            StartDate = x.StartDate.ToString(), EndDate = x.EndDate.ToString(),
            Tags = x.Tags, CreatedAt = x.CreatedAt.ToString()
        };
    }

    public async Task<OkrDto> CreateOkrAsync(CreateOkrInput input, Guid userId, CancellationToken ct = default)
    {
        var entity = new Okr
        {
            UserId = userId, Title = input.Title, Description = input.Description,
            Objective = input.Objective, KeyResults = input.KeyResults,
            StartDate = ParseDate(input.StartDate), EndDate = ParseDate(input.EndDate),
            Tags = input.Tags
        };
        context.Okrs.Add(entity);
        await context.SaveChangesAsync(ct);
        return new OkrDto
        {
            Id = entity.Id.ToString(), UserId = entity.UserId.ToString(), Title = entity.Title,
            Description = entity.Description, Objective = entity.Objective, KeyResults = entity.KeyResults,
            Status = entity.Status, Progress = entity.Progress,
            StartDate = entity.StartDate.ToString(), EndDate = entity.EndDate.ToString(),
            Tags = entity.Tags, CreatedAt = entity.CreatedAt.ToString()
        };
    }

    public async Task<OkrDto?> UpdateOkrAsync(Guid id, UpdateOkrInput input, CancellationToken ct = default)
    {
        var entity = await context.Okrs.FindAsync([id], ct);
        if (entity == null) return null;
        if (input.Title != null) entity.Title = input.Title;
        if (input.Description != null) entity.Description = input.Description;
        if (input.Objective != null) entity.Objective = input.Objective;
        if (input.KeyResults != null) entity.KeyResults = input.KeyResults;
        if (input.Status.HasValue) entity.Status = input.Status.Value;
        if (input.Progress.HasValue) entity.Progress = input.Progress.Value;
        if (input.StartDate != null) entity.StartDate = ParseDate(input.StartDate);
        if (input.EndDate != null) entity.EndDate = ParseDate(input.EndDate);
        if (input.Tags != null) entity.Tags = input.Tags;
        await context.SaveChangesAsync(ct);
        return new OkrDto
        {
            Id = entity.Id.ToString(), UserId = entity.UserId.ToString(), Title = entity.Title,
            Description = entity.Description, Objective = entity.Objective, KeyResults = entity.KeyResults,
            Status = entity.Status, Progress = entity.Progress,
            StartDate = entity.StartDate.ToString(), EndDate = entity.EndDate.ToString(),
            Tags = entity.Tags, CreatedAt = entity.CreatedAt.ToString()
        };
    }

    public async Task<bool> DeleteOkrAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await context.Okrs.FindAsync([id], ct);
        if (entity == null) return false;
        context.Okrs.Remove(entity);
        await context.SaveChangesAsync(ct);
        return true;
    }

    #endregion

    #region Risk

    public async Task<PageResult<RiskItemDto>> GetRisksAsync(WorkExtendedQueryDto query, Guid? userId, CancellationToken ct = default)
    {
        var q = context.WorkRisks.AsNoTracking().AsQueryable();
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x => x.Title.Contains(query.Keyword) || (x.Description != null && x.Description.Contains(query.Keyword)));
        if (query.Status.HasValue) q = q.Where(x => x.Status == query.Status.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(x => new RiskItemDto
            {
                Id = x.Id.ToString(), UserId = x.UserId.ToString(), Title = x.Title,
                Description = x.Description, Impact = x.Impact, Mitigation = x.Mitigation,
                Status = x.Status, Level = x.Level, ProjectId = x.ProjectId.ToString(),
                Tags = x.Tags, CreatedAt = x.CreatedAt.ToString()
            }).ToListAsync(ct);

        return PageResult<RiskItemDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<RiskItemDto?> GetRiskByIdAsync(Guid id, CancellationToken ct = default)
    {
        var x = await context.WorkRisks.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (x == null) return null;
        return new RiskItemDto
        {
            Id = x.Id.ToString(), UserId = x.UserId.ToString(), Title = x.Title,
            Description = x.Description, Impact = x.Impact, Mitigation = x.Mitigation,
            Status = x.Status, Level = x.Level, ProjectId = x.ProjectId.ToString(),
            Tags = x.Tags, CreatedAt = x.CreatedAt.ToString()
        };
    }

    public async Task<RiskItemDto> CreateRiskAsync(CreateRiskItemInput input, Guid userId, CancellationToken ct = default)
    {
        var entity = new WorkRisk
        {
            UserId = userId, Title = input.Title, Description = input.Description,
            Impact = input.Impact, Mitigation = input.Mitigation, Level = input.Level,
            ProjectId = string.IsNullOrWhiteSpace(input.ProjectId) ? null : Guid.Parse(input.ProjectId),
            Tags = input.Tags
        };
        context.WorkRisks.Add(entity);
        await context.SaveChangesAsync(ct);
        return new RiskItemDto
        {
            Id = entity.Id.ToString(), UserId = entity.UserId.ToString(), Title = entity.Title,
            Description = entity.Description, Impact = entity.Impact, Mitigation = entity.Mitigation,
            Status = entity.Status, Level = entity.Level, ProjectId = entity.ProjectId.ToString(),
            Tags = entity.Tags, CreatedAt = entity.CreatedAt.ToString()
        };
    }

    public async Task<RiskItemDto?> UpdateRiskAsync(Guid id, UpdateRiskItemInput input, CancellationToken ct = default)
    {
        var entity = await context.WorkRisks.FindAsync([id], ct);
        if (entity == null) return null;
        if (input.Title != null) entity.Title = input.Title;
        if (input.Description != null) entity.Description = input.Description;
        if (input.Impact != null) entity.Impact = input.Impact;
        if (input.Mitigation != null) entity.Mitigation = input.Mitigation;
        if (input.Status.HasValue) entity.Status = input.Status.Value;
        if (input.Level.HasValue) entity.Level = input.Level.Value;
        if (input.ProjectId != null) entity.ProjectId = string.IsNullOrWhiteSpace(input.ProjectId) ? null : Guid.Parse(input.ProjectId);
        if (input.Tags != null) entity.Tags = input.Tags;
        await context.SaveChangesAsync(ct);
        return new RiskItemDto
        {
            Id = entity.Id.ToString(), UserId = entity.UserId.ToString(), Title = entity.Title,
            Description = entity.Description, Impact = entity.Impact, Mitigation = entity.Mitigation,
            Status = entity.Status, Level = entity.Level, ProjectId = entity.ProjectId.ToString(),
            Tags = entity.Tags, CreatedAt = entity.CreatedAt.ToString()
        };
    }

    public async Task<bool> DeleteRiskAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await context.WorkRisks.FindAsync([id], ct);
        if (entity == null) return false;
        context.WorkRisks.Remove(entity);
        await context.SaveChangesAsync(ct);
        return true;
    }

    #endregion

    #region File

    public async Task<PageResult<WorkFileDto>> GetFilesAsync(WorkExtendedQueryDto query, Guid? userId, CancellationToken ct = default)
    {
        var q = context.WorkFiles.AsNoTracking().AsQueryable();
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x => x.Name.Contains(query.Keyword) || (x.Description != null && x.Description.Contains(query.Keyword)));
        if (!string.IsNullOrWhiteSpace(query.Category))
            q = q.Where(x => x.FileType.Contains(query.Category));

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .Select(x => new WorkFileDto
            {
                Id = x.Id.ToString(), UserId = x.UserId.ToString(), Name = x.Name,
                Description = x.Description, FilePath = x.FilePath, FileSize = x.FileSize,
                FileType = x.FileType, ProjectId = x.ProjectId.ToString(),
                Tags = x.Tags, CreatedAt = x.CreatedAt.ToString()
            }).ToListAsync(ct);

        return PageResult<WorkFileDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<WorkFileDto?> GetFileByIdAsync(Guid id, CancellationToken ct = default)
    {
        var x = await context.WorkFiles.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (x == null) return null;
        return new WorkFileDto
        {
            Id = x.Id.ToString(), UserId = x.UserId.ToString(), Name = x.Name,
            Description = x.Description, FilePath = x.FilePath, FileSize = x.FileSize,
            FileType = x.FileType, ProjectId = x.ProjectId.ToString(),
            Tags = x.Tags, CreatedAt = x.CreatedAt.ToString()
        };
    }

    public async Task<WorkFileDto> CreateFileAsync(CreateWorkFileInput input, Guid userId, CancellationToken ct = default)
    {
        var entity = new WorkFile
        {
            UserId = userId, Name = input.Name, Description = input.Description,
            FilePath = input.FilePath, FileSize = input.FileSize, FileType = input.FileType,
            ProjectId = string.IsNullOrWhiteSpace(input.ProjectId) ? null : Guid.Parse(input.ProjectId),
            Tags = input.Tags
        };
        context.WorkFiles.Add(entity);
        await context.SaveChangesAsync(ct);
        return new WorkFileDto
        {
            Id = entity.Id.ToString(), UserId = entity.UserId.ToString(), Name = entity.Name,
            Description = entity.Description, FilePath = entity.FilePath, FileSize = entity.FileSize,
            FileType = entity.FileType, ProjectId = entity.ProjectId.ToString(),
            Tags = entity.Tags, CreatedAt = entity.CreatedAt.ToString()
        };
    }

    public async Task<WorkFileDto?> UpdateFileAsync(Guid id, UpdateWorkFileInput input, CancellationToken ct = default)
    {
        var entity = await context.WorkFiles.FindAsync([id], ct);
        if (entity == null) return null;
        if (input.Name != null) entity.Name = input.Name;
        if (input.Description != null) entity.Description = input.Description;
        if (input.FilePath != null) entity.FilePath = input.FilePath;
        if (input.FileSize.HasValue) entity.FileSize = input.FileSize.Value;
        if (input.FileType != null) entity.FileType = input.FileType;
        if (input.ProjectId != null) entity.ProjectId = string.IsNullOrWhiteSpace(input.ProjectId) ? null : Guid.Parse(input.ProjectId);
        if (input.Tags != null) entity.Tags = input.Tags;
        await context.SaveChangesAsync(ct);
        return new WorkFileDto
        {
            Id = entity.Id.ToString(), UserId = entity.UserId.ToString(), Name = entity.Name,
            Description = entity.Description, FilePath = entity.FilePath, FileSize = entity.FileSize,
            FileType = entity.FileType, ProjectId = entity.ProjectId.ToString(),
            Tags = entity.Tags, CreatedAt = entity.CreatedAt.ToString()
        };
    }

    public async Task<bool> DeleteFileAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await context.WorkFiles.FindAsync([id], ct);
        if (entity == null) return false;
        context.WorkFiles.Remove(entity);
        await context.SaveChangesAsync(ct);
        return true;
    }

    #endregion
}
