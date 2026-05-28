using WebApplication1.Features.Network.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Network.Services.Interfaces;

public interface INetworkService
{
    Task<PageResult<ContactDto>> GetContactPageAsync(NetworkQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<ContactDto?> GetContactByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ContactDto> CreateContactAsync(CreateContactDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<ContactDto?> UpdateContactAsync(Guid id, UpdateContactDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteContactAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PageResult<InteractionDto>> GetInteractionPageAsync(Guid contactId, NetworkQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<InteractionDto?> GetInteractionByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InteractionDto> CreateInteractionAsync(CreateInteractionDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<InteractionDto?> UpdateInteractionAsync(Guid id, UpdateInteractionDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteInteractionAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<TagDto>> GetContactTagsAsync(Guid? userId = null, CancellationToken cancellationToken = default);
}
