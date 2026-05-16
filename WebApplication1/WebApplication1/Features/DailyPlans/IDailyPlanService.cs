using WebApplication1.Shared.Common;

namespace WebApplication1.Features.DailyPlans;

public interface IDailyPlanService
{
    Task<PageResult<DailyPlanDto>> GetPageAsync(DailyPlanQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);

    Task<DailyPlanDto?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default);

    Task<DailyPlanDto> CreateAsync(CreateDailyPlanDto input, Guid userId, CancellationToken cancellationToken = default);

    Task<DailyPlanDto?> UpdateAsync(Guid id, UpdateDailyPlanDto input, Guid? userId = null, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default);

    Task<DailyPlanDto?> CompleteAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default);
}
