using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Services.Interfaces;

public interface IKnowledgeArticleService
{
    Task<PageResult<KnowledgeArticleDto>> GetPageAsync(KnowledgeArticleQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<KnowledgeArticleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<KnowledgeArticleDto> CreateAsync(CreateKnowledgeArticleDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<KnowledgeArticleDto?> UpdateAsync(Guid id, UpdateKnowledgeArticleDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken = default);
}