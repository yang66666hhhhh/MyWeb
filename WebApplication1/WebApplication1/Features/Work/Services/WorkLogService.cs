using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Features.Work.Services.Interfaces;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Features.Work.Services;

public class WorkLogService : IWorkLogService
{
    private readonly AppDbContext _context;

    public WorkLogService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PageResult<WorkLogDto>> GetPageAsync(WorkLogQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var q = _context.WorkLogs
            .Include(x => x.Project)
            .Include(x => x.Template)
            .Include(x => x.DynamicValues)
            .AsQueryable();

        if (userId.HasValue)
        {
            q = q.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            q = q.Where(x => x.Title.Contains(keyword) || (x.OriginalContent != null && x.OriginalContent.Contains(keyword)));
        }

        if (query.WorkDate.HasValue)
            q = q.Where(x => x.WorkDate == query.WorkDate.Value);

        if (query.StartDate.HasValue)
            q = q.Where(x => x.WorkDate >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            q = q.Where(x => x.WorkDate <= query.EndDate.Value);

        if (query.ProjectId.HasValue)
            q = q.Where(x => x.ProjectId == query.ProjectId.Value);

        if (query.TemplateId.HasValue)
            q = q.Where(x => x.TemplateId == query.TemplateId.Value);

        if (query.SourceType.HasValue)
            q = q.Where(x => x.SourceType == (WorkLogSourceType)query.SourceType.Value);

        if (query.Status.HasValue)
            q = q.Where(x => x.Status == (WorkLogStatus)query.Status.Value);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.WorkDate)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return PageResult<WorkLogDto>.Create(items.Select(ToDto).ToList(), total, query.Page, query.PageSize);
    }

    public async Task<WorkLogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkLogs
            .Include(x => x.Project)
            .Include(x => x.Template)
            .Include(x => x.DynamicValues)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return entity is null ? null : ToDto(entity);
    }

    public async Task<WorkLogDto> CreateAsync(CreateWorkLogDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var entity = new WorkLog
        {
            UserId = userId,
            WorkDate = input.WorkDate,
            WeekDay = input.WorkDate.DayOfWeek.ToString(),
            ProjectId = input.ProjectId,
            Title = input.Title,
            OriginalContent = input.OriginalContent,
            Summary = input.Summary,
            TotalHours = input.TotalHours,
            Status = WorkLogStatus.Normal,
            SourceType = WorkLogSourceType.Manual,
            Remark = input.Remark,
            TemplateId = input.TemplateId
        };

        _context.WorkLogs.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        if (input.DynamicValues?.Count > 0)
        {
            foreach (var dv in input.DynamicValues)
            {
                entity.DynamicValues.Add(new WorkLogDynamicValue
                {
                    WorkLogId = entity.Id,
                    FieldName = dv.FieldName,
                    StringValue = dv.StringValue,
                    NumberValue = dv.NumberValue,
                    DateValue = dv.DateValue
                });
            }
            await _context.SaveChangesAsync(cancellationToken);
        }

        await _context.Entry(entity).Reference(x => x.Project).LoadAsync(cancellationToken);
        if (entity.TemplateId.HasValue)
        {
            await _context.Entry(entity).Reference(x => x.Template).LoadAsync(cancellationToken);
        }

        return ToDto(entity);
    }

    public async Task<WorkLogDto?> UpdateAsync(Guid id, UpdateWorkLogDto input, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkLogs
            .Include(x => x.DynamicValues)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (entity is null) return null;

        if (input.WorkDate.HasValue) entity.WorkDate = input.WorkDate.Value;
        if (!string.IsNullOrEmpty(input.WorkDate.ToString())) entity.WeekDay = input.WorkDate.Value.DayOfWeek.ToString();
        if (input.ProjectId.HasValue) entity.ProjectId = input.ProjectId.Value;
        if (input.Title is not null) entity.Title = input.Title;
        if (input.OriginalContent is not null) entity.OriginalContent = input.OriginalContent;
        if (input.Summary is not null) entity.Summary = input.Summary;
        if (input.TotalHours.HasValue) entity.TotalHours = input.TotalHours.Value;
        if (input.Status.HasValue) entity.Status = (WorkLogStatus)input.Status.Value;
        if (input.Remark is not null) entity.Remark = input.Remark;
        if (input.TemplateId.HasValue) entity.TemplateId = input.TemplateId;

        if (input.DynamicValues?.Count > 0)
        {
            _context.WorkLogDynamicValues.RemoveRange(entity.DynamicValues);
            entity.DynamicValues.Clear();

            foreach (var dv in input.DynamicValues)
            {
                entity.DynamicValues.Add(new WorkLogDynamicValue
                {
                    WorkLogId = entity.Id,
                    FieldName = dv.FieldName,
                    StringValue = dv.StringValue,
                    NumberValue = dv.NumberValue,
                    DateValue = dv.DateValue
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(entity).Reference(x => x.Project).LoadAsync(cancellationToken);
        if (entity.TemplateId.HasValue)
        {
            await _context.Entry(entity).Reference(x => x.Template).LoadAsync(cancellationToken);
        }

        return ToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.WorkLogs.FindAsync([id], cancellationToken);
        if (entity is null) return false;

        _context.WorkLogs.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static WorkLogDto ToDto(WorkLog entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        WorkDate = entity.WorkDate,
        WeekDay = entity.WeekDay,
        ProjectId = entity.ProjectId,
        ProjectName = entity.Project?.ProjectName,
        Title = entity.Title,
        OriginalContent = entity.OriginalContent,
        Summary = entity.Summary,
        TotalHours = entity.TotalHours,
        Status = (int)entity.Status,
        SourceType = (int)entity.SourceType,
        TemplateId = entity.TemplateId,
        TemplateName = entity.Template?.Name,
        Remark = entity.Remark,
        DynamicValues = entity.DynamicValues?.Select(dv => new WorkLogDynamicValueDto
        {
            Id = dv.Id,
            WorkLogId = dv.WorkLogId,
            FieldName = dv.FieldName,
            StringValue = dv.StringValue,
            NumberValue = dv.NumberValue,
            DateValue = dv.DateValue
        }).ToList() ?? new(),
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}