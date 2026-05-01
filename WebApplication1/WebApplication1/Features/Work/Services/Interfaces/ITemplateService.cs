using WebApplication1.Features.Work.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Services.Interfaces;

public interface ITemplateService
{
    Task<PageResult<IndustryTemplateDto>> GetPageAsync(PageQueryDto query, CancellationToken cancellationToken = default);
    Task<IndustryTemplateDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TemplateFieldDto>> GetFieldsAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<IndustryTemplateDto> CreateAsync(CreateTemplateDto input, CancellationToken cancellationToken = default);
    Task<IndustryTemplateDto?> UpdateAsync(Guid id, CreateTemplateDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> SetDefaultAsync(Guid id, CancellationToken cancellationToken = default);
}