using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Entities;

namespace WebApplication1.Features.Work.Services;

public interface IImplLogService
{
    Task<PageResult<ImplLogDto>> GetPageAsync(ImplLogQueryDto query, Guid userId, CancellationToken cancellationToken = default);
    Task<ImplLogSummaryDto> GetSummaryAsync(ImplLogQueryDto query, Guid userId, CancellationToken cancellationToken = default);
    Task<ImplLogDto?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<ImplLogDto> CreateAsync(CreateImplLogDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<ImplLogDto?> UpdateAsync(Guid id, UpdateImplLogDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}

public class ImplLogService(AppDbContext context) : IImplLogService
{
    public async Task<PageResult<ImplLogDto>> GetPageAsync(ImplLogQueryDto query, Guid userId, CancellationToken cancellationToken = default)
    {
        var q = BuildQuery(query, userId);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.WorkDate)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return PageResult<ImplLogDto>.Create(items.Select(MapToDto).ToList(), total, query.Page, query.PageSize);
    }

    public async Task<ImplLogSummaryDto> GetSummaryAsync(ImplLogQueryDto query, Guid userId, CancellationToken cancellationToken = default)
    {
        var q = BuildQuery(query, userId);
        var items = await q.ToListAsync(cancellationToken);

        var equipment = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var taskTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in items)
        {
            var extraData = ParseExtraData(item.ExtraData);
            AppendUniqueValues(extraData, "equipment", equipment);
            AppendUniqueValues(extraData, "taskTypes", taskTypes);
        }

        return new ImplLogSummaryDto
        {
            TotalCount = items.Count,
            TotalHours = items.Sum(x => x.TotalHours),
            UniqueEquipmentCount = equipment.Count,
            UniqueTaskTypeCount = taskTypes.Count,
        };
    }

    public async Task<ImplLogDto?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var entity = await context.ImplLogs
            .AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.Template)
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
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
            ProjectId = input.ProjectId,
            ProjectName = input.ProjectName,
            TotalHours = input.TotalHours,
            TemplateId = input.TemplateId,
            ExtraData = SerializeExtraData(input.ExtraData)
        };

        context.ImplLogs.Add(entity);
        await context.SaveChangesAsync(cancellationToken);

        if (input.TemplateId.HasValue)
        {
            var template = await context.WorkLogTemplates.FindAsync([input.TemplateId.Value], cancellationToken);
            entity.Template = template;
        }

        if (input.ProjectId.HasValue)
        {
            var project = await context.WorkProjects.FindAsync([input.ProjectId.Value], cancellationToken);
            entity.Project = project;
            entity.ProjectName = project?.ProjectName ?? entity.ProjectName;
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
        if (input.ProjectId.HasValue || input.ProjectId == null)
        {
            entity.ProjectId = input.ProjectId;
        }
        if (input.ProjectName != null) entity.ProjectName = input.ProjectName;
        if (input.TotalHours.HasValue) entity.TotalHours = input.TotalHours.Value;
        if (input.TemplateId.HasValue) entity.TemplateId = input.TemplateId;
        if (input.ExtraData.HasValue) entity.ExtraData = SerializeExtraData(input.ExtraData);

        await context.SaveChangesAsync(cancellationToken);

        if (entity.ProjectId.HasValue)
        {
            var project = await context.WorkProjects.FindAsync([entity.ProjectId.Value], cancellationToken);
            entity.Project = project;
            entity.ProjectName = project?.ProjectName ?? entity.ProjectName;
        }

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
            ProjectId = entity.ProjectId,
            ProjectName = entity.Project?.ProjectName ?? entity.ProjectName,
            TotalHours = entity.TotalHours,
            PersonaCode = entity.PersonaCode,
            TemplateId = entity.TemplateId,
            TemplateName = entity.Template?.Name,
            ExtraData = ParseExtraData(entity.ExtraData),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private IQueryable<ImplLog> BuildQuery(ImplLogQueryDto query, Guid userId)
    {
        var q = context.ImplLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Include(x => x.Project)
            .Include(x => x.Template)
            .AsQueryable();

        if (query.WorkDate.HasValue)
            q = q.Where(x => x.WorkDate == query.WorkDate.Value);
        if (query.StartDate.HasValue)
            q = q.Where(x => x.WorkDate >= query.StartDate.Value);
        if (query.EndDate.HasValue)
            q = q.Where(x => x.WorkDate <= query.EndDate.Value);
        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x =>
                x.Title.Contains(query.Keyword) ||
                (x.ProjectName != null && x.ProjectName.Contains(query.Keyword)) ||
                (x.ExtraData != null && x.ExtraData.Contains(query.Keyword)));
        if (query.TemplateId.HasValue)
            q = q.Where(x => x.TemplateId == query.TemplateId.Value);

        return q;
    }

    private static string? SerializeExtraData(JsonElement? extraData)
    {
        if (!extraData.HasValue)
        {
            return null;
        }

        var value = extraData.Value;
        return value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined
            ? null
            : value.GetRawText();
    }

    private static JsonElement? ParseExtraData(string? extraData)
    {
        if (string.IsNullOrWhiteSpace(extraData))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(extraData);
            return document.RootElement.Clone();
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static void AppendUniqueValues(JsonElement? extraData, string propertyName, HashSet<string> values)
    {
        if (!extraData.HasValue || extraData.Value.ValueKind != JsonValueKind.Object)
        {
            return;
        }

        if (!extraData.Value.TryGetProperty(propertyName, out var property) || property.ValueKind != JsonValueKind.Array)
        {
            return;
        }

        foreach (var item in property.EnumerateArray())
        {
            var value = item.GetString();
            if (!string.IsNullOrWhiteSpace(value))
            {
                values.Add(value);
            }
        }
    }
}
