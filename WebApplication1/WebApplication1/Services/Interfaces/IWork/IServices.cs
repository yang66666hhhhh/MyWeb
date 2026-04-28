using WebApplication1.Common;
using WebApplication1.Dtos.Work;

namespace WebApplication1.Services.Interfaces.IWork;

public interface IWorkProjectService
{
    Task<PageResult<WorkProjectDto>> GetPageAsync(WorkProjectQueryDto query, CancellationToken cancellationToken = default);
    Task<WorkProjectDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkProjectDto> CreateAsync(CreateWorkProjectDto input, CancellationToken cancellationToken = default);
    Task<WorkProjectDto?> UpdateAsync(Guid id, UpdateWorkProjectDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IWorkDeviceService
{
    Task<PageResult<WorkDeviceDto>> GetPageAsync(WorkDeviceQueryDto query, CancellationToken cancellationToken = default);
    Task<WorkDeviceDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkDeviceDto> CreateAsync(CreateWorkDeviceDto input, CancellationToken cancellationToken = default);
    Task<WorkDeviceDto?> UpdateAsync(Guid id, UpdateWorkDeviceDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IWorkTaskTypeService
{
    Task<PageResult<WorkTaskTypeDto>> GetPageAsync(WorkTaskTypeQueryDto query, CancellationToken cancellationToken = default);
    Task<WorkTaskTypeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkTaskTypeDto> CreateAsync(CreateWorkTaskTypeDto input, CancellationToken cancellationToken = default);
    Task<WorkTaskTypeDto?> UpdateAsync(Guid id, UpdateWorkTaskTypeDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IWorkLogService
{
    Task<PageResult<WorkLogDto>> GetPageAsync(WorkLogQueryDto query, CancellationToken cancellationToken = default);
    Task<WorkLogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkLogDto> CreateAsync(CreateWorkLogDto input, CancellationToken cancellationToken = default);
    Task<WorkLogDto?> UpdateAsync(Guid id, UpdateWorkLogDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IWorkStatisticsService
{
    Task<WorkStatisticsOverviewDto> GetOverviewAsync(WorkStatisticsQueryDto query, CancellationToken cancellationToken = default);
    Task<List<WorkStatisticsDailyHoursDto>> GetDailyHoursAsync(WorkStatisticsQueryDto query, CancellationToken cancellationToken = default);
    Task<List<WorkStatisticsProjectHoursDto>> GetProjectHoursAsync(WorkStatisticsQueryDto query, CancellationToken cancellationToken = default);
    Task<List<WorkStatisticsTaskTypeDistributionDto>> GetTaskTypeDistributionAsync(WorkStatisticsQueryDto query, CancellationToken cancellationToken = default);
    Task<List<WorkStatisticsDeviceRankingDto>> GetDeviceRankingAsync(WorkStatisticsQueryDto query, CancellationToken cancellationToken = default);
}
