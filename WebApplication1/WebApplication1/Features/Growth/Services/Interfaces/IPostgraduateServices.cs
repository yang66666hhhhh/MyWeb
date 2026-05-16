using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Services.Interfaces;

public interface IPostgraduateTaskService
{
    Task<PageResult<PostgraduateTaskDto>> GetPageAsync(PostgraduateTaskQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<PostgraduateTaskDto?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<PostgraduateTaskDto> CreateAsync(CreatePostgraduateTaskDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<PostgraduateTaskDto?> UpdateAsync(Guid id, UpdatePostgraduateTaskDto input, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default);
}

public interface IExamMistakeService
{
    Task<PageResult<ExamMistakeDto>> GetPageAsync(ExamMistakeQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<ExamMistakeDto?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<ExamMistakeDto> CreateAsync(CreateExamMistakeDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<ExamMistakeDto?> UpdateAsync(Guid id, UpdateExamMistakeDto input, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default);
}

public interface IExamMaterialService
{
    Task<PageResult<ExamMaterialDto>> GetPageAsync(ExamMaterialQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<ExamMaterialDto?> GetByIdAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<ExamMaterialDto> CreateAsync(CreateExamMaterialDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<ExamMaterialDto?> UpdateAsync(Guid id, UpdateExamMaterialDto input, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, Guid? userId = null, CancellationToken cancellationToken = default);
}
