using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Services.Interfaces;

public interface IHabitService
{
    Task<PageResult<HabitDto>> GetPageAsync(HabitQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<HabitDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<HabitDto> CreateAsync(CreateHabitDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<HabitDto?> UpdateAsync(Guid id, UpdateHabitDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<HabitDto?> CheckInAsync(Guid id, CheckInDto input, CancellationToken cancellationToken = default);
    Task<HabitDto?> UpdateStatusAsync(Guid id, int status, CancellationToken cancellationToken = default);
}