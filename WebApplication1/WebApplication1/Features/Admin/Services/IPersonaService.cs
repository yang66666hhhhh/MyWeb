using WebApplication1.Features.Admin.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Admin.Services;

public interface IPersonaService
{
    Task<PageResult<PersonaTypeDto>> GetPageAsync(PersonaTypeQueryDto query, CancellationToken cancellationToken = default);
    Task<List<PersonaTypeDto>> GetAllAsync(bool? isActive = true, CancellationToken cancellationToken = default);
    Task<PersonaTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PersonaTypeDto> CreateAsync(CreatePersonaTypeDto input, CancellationToken cancellationToken = default);
    Task<PersonaTypeDto?> UpdateAsync(Guid id, UpdatePersonaTypeDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<PersonaMenuItemDto>> GetMenuItemsAsync(Guid personaTypeId, CancellationToken cancellationToken = default);
    Task<bool> SetMenuItemsAsync(Guid personaTypeId, List<PersonaMenuItemDto> items, CancellationToken cancellationToken = default);
}
