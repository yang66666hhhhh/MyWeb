using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services.Interfaces;

namespace WebApplication1.Features.Work.Services;

public class WorkLogService : IWorkLogService
{
    private readonly AppDbContext _context;

    public WorkLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PageResult<WorkLogDto>> GetPageAsync(WorkLogQueryDto query, CancellationToken cancellationToken = default)
    {
        var q = _context.WorkLogs
            .Include(x => x.Project)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            q = q.Where(x => x.Title.Contains(query.Keyword) || (x.OriginalContent != null && x.OriginalContent.Contains(query.Keyword)));
        }

        if (query.WorkDate.HasValue)
            q = q.Where(x => x.WorkDate == query.WorkDate.Value);

        if (query.StartDate.HasValue)
            q = q.Where(x => x.WorkDate >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            q = q.Where(x => x.WorkDate <= query.EndDate.Value);

        if (query.ProjectId.HasValue)
            q = q.Where(x => x.ProjectId == query.ProjectId.Value);

        if (query.SourceType.HasValue)
            q = q.Where(x => x.SourceType == query.SourceType.Value);

        if (query.Status.HasValue)
            q = q.Where(x => x.Status == query.Status.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.WorkDate)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new WorkLogDto
            {
                Id = x.Id,
                WorkDate = x.WorkDate,
                WeekDay = x.WeekDay,
                ProjectId = x.ProjectId,
                ProjectName = x.Project != null ? x.Project.ProjectName : "",
                Title = x.Title,
                OriginalContent = x.OriginalContent,
                Summary = x.Summary,
                TotalHours = x.TotalHours,
                Status = x.Status,
                SourceType = x.SourceType,
                Remark = x.Remark,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        return PageResult<WorkLogDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<WorkLogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkLogs
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity == null) return null;
        return new WorkLogDto
        {
            Id = entity.Id,
            WorkDate = entity.WorkDate,
            WeekDay = entity.WeekDay,
            ProjectId = entity.ProjectId,
            ProjectName = entity.Project?.ProjectName ?? "",
            Title = entity.Title,
            OriginalContent = entity.OriginalContent,
            Summary = entity.Summary,
            TotalHours = entity.TotalHours,
            Status = entity.Status,
            SourceType = entity.SourceType,
            Remark = entity.Remark,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<WorkLogDto> CreateAsync(CreateWorkLogDto input, CancellationToken cancellationToken = default)
    {
        var entity = new Work.Entities.WorkLog
        {
            WorkDate = input.WorkDate,
            WeekDay = input.WeekDay,
            ProjectId = input.ProjectId,
            Title = input.Title,
            OriginalContent = input.OriginalContent,
            Summary = input.Summary,
            TotalHours = input.TotalHours ?? 0,
            Status = input.Status,
            SourceType = input.SourceType,
            Remark = input.Remark
        };

        _context.WorkLogs.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return new WorkLogDto
        {
            Id = entity.Id,
            WorkDate = entity.WorkDate,
            WeekDay = entity.WeekDay,
            ProjectId = entity.ProjectId,
            Title = entity.Title,
            OriginalContent = entity.OriginalContent,
            Summary = entity.Summary,
            TotalHours = entity.TotalHours,
            Status = entity.Status,
            SourceType = entity.SourceType,
            Remark = entity.Remark,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<WorkLogDto?> UpdateAsync(Guid id, UpdateWorkLogDto input, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkLogs.FindAsync([id], cancellationToken);
        if (entity == null) return null;

        entity.WorkDate = input.WorkDate;
        entity.WeekDay = input.WeekDay;
        entity.ProjectId = input.ProjectId;
        entity.Title = input.Title;
        entity.OriginalContent = input.OriginalContent;
        entity.Summary = input.Summary;
        entity.TotalHours = input.TotalHours ?? 0;
        entity.Status = input.Status;
        entity.SourceType = input.SourceType;
        entity.Remark = input.Remark;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return new WorkLogDto
        {
            Id = entity.Id,
            WorkDate = entity.WorkDate,
            WeekDay = entity.WeekDay,
            ProjectId = entity.ProjectId,
            Title = entity.Title,
            OriginalContent = entity.OriginalContent,
            Summary = entity.Summary,
            TotalHours = entity.TotalHours,
            Status = entity.Status,
            SourceType = entity.SourceType,
            Remark = entity.Remark,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkLogs.FindAsync([id], cancellationToken);
        if (entity == null) return false;

        _context.WorkLogs.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
