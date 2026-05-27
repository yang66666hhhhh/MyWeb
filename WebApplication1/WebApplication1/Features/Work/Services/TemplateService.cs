using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Features.Work.Services.Interfaces;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Work.Services;

public class TemplateService(AppDbContext dbContext) : ITemplateService
{
    public async Task<PageResult<IndustryTemplateDto>> GetPageAsync(PageQueryDto query, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var templates = dbContext.IndustryTemplates
            .AsNoTracking()
            .Include(x => x.Fields.OrderBy(f => f.Sort))
            .AsQueryable();

        var total = await templates.CountAsync(cancellationToken);
        var items = await templates
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<IndustryTemplateDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<IndustryTemplateDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var template = await dbContext.IndustryTemplates
            .AsNoTracking()
            .Include(x => x.Fields.OrderBy(f => f.Sort))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return template is null ? null : ToDto(template);
    }

    public async Task<List<TemplateFieldDto>> GetFieldsAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        var fields = await dbContext.TemplateFields
            .AsNoTracking()
            .Where(x => x.TemplateId == templateId)
            .OrderBy(x => x.Sort)
            .ToListAsync(cancellationToken);

        return fields.Select(ToFieldDto).ToList();
    }

    public async Task<IndustryTemplateDto> CreateAsync(CreateTemplateDto input, CancellationToken cancellationToken = default)
    {
        if (input.IsDefault)
        {
            await ClearDefaultFlags(cancellationToken);
        }

        var template = new IndustryTemplate
        {
            Name = input.Name,
            Description = input.Description,
            Industry = input.Industry,
            IsDefault = input.IsDefault
        };

        dbContext.IndustryTemplates.Add(template);
        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var field in input.Fields)
        {
            template.Fields.Add(new TemplateField
            {
                TemplateId = template.Id,
                FieldName = field.FieldName,
                FieldLabel = field.FieldLabel,
                FieldType = (FieldType)field.FieldType,
                Options = field.Options,
                IsRequired = field.IsRequired,
                Sort = field.Sort,
                DefaultValue = field.DefaultValue
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(template);
    }

    public async Task<IndustryTemplateDto?> UpdateAsync(Guid id, CreateTemplateDto input, CancellationToken cancellationToken = default)
    {
        var template = await dbContext.IndustryTemplates
            .Include(x => x.Fields)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (template is null) return null;

        if (input.IsDefault && !template.IsDefault)
        {
            await ClearDefaultFlags(cancellationToken);
        }

        template.Name = input.Name;
        template.Description = input.Description;
        template.Industry = input.Industry;
        template.IsDefault = input.IsDefault;

        dbContext.TemplateFields.RemoveRange(template.Fields);
        template.Fields.Clear();

        foreach (var field in input.Fields)
        {
            template.Fields.Add(new TemplateField
            {
                TemplateId = template.Id,
                FieldName = field.FieldName,
                FieldLabel = field.FieldLabel,
                FieldType = (FieldType)field.FieldType,
                Options = field.Options,
                IsRequired = field.IsRequired,
                Sort = field.Sort,
                DefaultValue = field.DefaultValue
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(template);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var template = await dbContext.IndustryTemplates.FindAsync([id], cancellationToken);
        if (template is null) return false;

        dbContext.IndustryTemplates.Remove(template);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var template = await dbContext.IndustryTemplates.FindAsync([id], cancellationToken);
        if (template is null) return false;

        await ClearDefaultFlags(cancellationToken);

        template.IsDefault = true;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task ClearDefaultFlags(CancellationToken cancellationToken)
    {
        var defaults = await dbContext.IndustryTemplates.Where(x => x.IsDefault).ToListAsync(cancellationToken);
        foreach (var t in defaults)
        {
            t.IsDefault = false;
        }
    }

    private static IndustryTemplateDto ToDto(IndustryTemplate template) => new()
    {
        Id = template.Id,
        Name = template.Name,
        Description = template.Description,
        Industry = template.Industry,
        IsDefault = template.IsDefault,
        Fields = template.Fields.OrderBy(x => x.Sort).Select(ToFieldDto).ToList(),
        CreatedAt = template.CreatedAt
    };

    private static TemplateFieldDto ToFieldDto(TemplateField field) => new()
    {
        Id = field.Id,
        TemplateId = field.TemplateId,
        FieldName = field.FieldName,
        FieldLabel = field.FieldLabel,
        FieldType = (int)field.FieldType,
        Options = field.Options,
        IsRequired = field.IsRequired,
        Sort = field.Sort,
        DefaultValue = field.DefaultValue
    };
}