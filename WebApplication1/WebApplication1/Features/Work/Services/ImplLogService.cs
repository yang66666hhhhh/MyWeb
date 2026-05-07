using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Entities;

namespace WebApplication1.Features.Work.Services;

public interface IImplLogService
{
    Task<PageResult<ImplLogDto>> GetPageAsync(ImplLogQueryDto query, Guid userId, CancellationToken cancellationToken = default);
    Task<ImplLogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ImplLogDto> CreateAsync(CreateImplLogDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<ImplLogDto?> UpdateAsync(Guid id, UpdateImplLogDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}

public class ImplLogService(AppDbContext context) : IImplLogService
{
    public async Task<PageResult<ImplLogDto>> GetPageAsync(ImplLogQueryDto query, Guid userId, CancellationToken cancellationToken = default)
    {
        var q = context.ImplLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Include(x => x.Template)
            .AsQueryable();

        if (query.WorkDate.HasValue)
            q = q.Where(x => x.WorkDate == query.WorkDate.Value);
        if (query.StartDate.HasValue)
            q = q.Where(x => x.WorkDate >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            q = q.Where(x => x.WorkDate <= query.EndDate.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x => x.Title.Contains(query.Keyword) || (x.ProjectName != null && x.ProjectName.Contains(query.Keyword)));
        if (query.TemplateId.HasValue)
            q = q.Where(x => x.TemplateId == query.TemplateId.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.WorkDate)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return PageResult<ImplLogDto>.Create(items.Select(MapToDto).ToList(), total, query.Page, query.PageSize);
    }

    public async Task<ImplLogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.ImplLogs
            .AsNoTracking()
            .Include(x => x.Template)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<ImplLogDto> CreateAsync(CreateImplLogDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var entity = new ImplLog
        {
            UserId = userId,
            WorkDate = input.WorkDate,
            WeekDay = input.WorkDate.DayOfWeek.ToString(),
            Title = input.Title,
            ProjectName = input.ProjectName,
            TotalHours = input.TotalHours,
            TemplateId = input.TemplateId,
            ExtraData = input.ExtraData
        };

        context.ImplLogs.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        if (input.TemplateId.HasValue)
        {
            var template = await context.WorkLogTemplates.FindAsync([input.TemplateId.Value], cancellationToken);
            entity.Template = template;
        }

        return MapToDto(entity);
    }

    public async Task<ImplLogDto?> UpdateAsync(Guid id, UpdateImplLogDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var entity = await context.ImplLogs.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
        if (entity == null) return null;

        if (input.WorkDate.HasValue)
        {
            entity.WorkDate = input.WorkDate.Value;
            entity.WeekDay = input.WorkDate.Value.DayOfWeek.ToString();
        }
        if (input.Title != null) entity.Title = input.Title;
        if (input.ProjectName != null) entity.ProjectName = input.ProjectName;
        if (input.TotalHours.HasValue) entity.TotalHours = input.TotalHours.Value;
        if (input.TemplateId.HasValue) entity.TemplateId = input.TemplateId;
        if (input.ExtraData != null) entity.ExtraData = input.ExtraData;

        await context.SaveChangesAsync(cancellationToken);

        if (entity.TemplateId.HasValue)
        {
            var template = await context.WorkLogTemplates.FindAsync([entity.TemplateId.Value], cancellationToken);
            entity.Template = template;
        }

        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var entity = await context.ImplLogs.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
        if (entity == null) return false;

        context.ImplLogs.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ImplLogDto MapToDto(ImplLog entity)
    {
        return new ImplLogDto
        {
            Id = entity.Id,
            UserId = entity.UserId,
            WorkDate = entity.WorkDate,
            WeekDay = entity.WeekDay,
            Title = entity.Title,
            ProjectName = entity.ProjectName,
            TotalHours = entity.TotalHours,
            PersonaCode = entity.PersonaCode,
            TemplateId = entity.TemplateId,
            TemplateName = entity.Template?.Name,
            ExtraData = entity.ExtraData,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
