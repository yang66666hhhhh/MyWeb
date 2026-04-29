using WebApplication1.Shared.Common;

namespace WebApplication1.Features.DailyPlans;

public interface IDailyPlanService
{
    Task<PageResult<DailyPlanDto>> GetPageAsync(DailyPlanQueryDto query, CancellationToken cancellationToken = default);

    Task<DailyPlanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DailyPlanDto> CreateAsync(CreateDailyPlanDto input, CancellationToken cancellationToken = default);

    Task<DailyPlanDto?> UpdateAsync(Guid id, UpdateDailyPlanDto input, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DailyPlanDto?> CompleteAsync(Guid id, CancellationToken cancellationToken = default);
}
