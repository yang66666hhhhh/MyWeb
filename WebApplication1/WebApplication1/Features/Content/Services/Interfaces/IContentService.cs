using WebApplication1.Features.Content.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Content.Services.Interfaces;

public interface IContentService
{
    Task<PageResult<ArticleDto>> GetArticlePageAsync(ContentQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<ArticleDto?> GetArticleByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ArticleDto> CreateArticleAsync(CreateArticleDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<ArticleDto?> UpdateArticleAsync(Guid id, UpdateArticleDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteArticleAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PageResult<MediaItemDto>> GetMediaItemPageAsync(ContentQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<MediaItemDto?> GetMediaItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<MediaItemDto> CreateMediaItemAsync(CreateMediaItemDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<MediaItemDto?> UpdateMediaItemAsync(Guid id, UpdateMediaItemDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteMediaItemAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PageResult<PublishingCalendarDto>> GetPublishingCalendarPageAsync(ContentQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<PublishingCalendarDto?> GetPublishingCalendarByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PublishingCalendarDto> CreatePublishingCalendarAsync(CreatePublishingCalendarDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<PublishingCalendarDto?> UpdatePublishingCalendarAsync(Guid id, UpdatePublishingCalendarDto input, CancellationToken cancellationToken = default);
    Task<bool> DeletePublishingCalendarAsync(Guid id, CancellationToken cancellationToken = default);
}
