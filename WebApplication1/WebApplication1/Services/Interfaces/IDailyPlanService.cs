using WebApplication1.Common;
using WebApplication1.Dtos.DailyPlans;

namespace WebApplication1.Services.Interfaces;

public interface IDailyPlanService
{
    Task<PageResult<DailyPlanDto>> GetPageAsync(DailyPlanQueryDto query, CancellationToken cancellationToken = default);

    Task<DailyPlanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DailyPlanDto> CreateAsync(CreateDailyPlanDto input, CancellationToken cancellationToken = default);

    Task<DailyPlanDto?> UpdateAsync(Guid id, UpdateDailyPlanDto input, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DailyPlanDto?> CompleteAsync(Guid id, CancellationToken cancellationToken = default);
}
