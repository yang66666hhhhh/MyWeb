using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Services.Interfaces;

public interface IGrowthProjectService
{
    Task<PageResult<GrowthProjectDto>> GetPageAsync(GrowthProjectQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<GrowthProjectDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GrowthProjectDto> CreateAsync(CreateGrowthProjectDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<GrowthProjectDto?> UpdateAsync(Guid id, UpdateGrowthProjectDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}