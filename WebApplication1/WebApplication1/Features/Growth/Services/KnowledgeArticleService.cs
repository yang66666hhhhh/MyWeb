using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Features.Growth.Entities;
using WebApplication1.Features.Growth.Services.Interfaces;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Growth.Services;

public class KnowledgeArticleService(AppDbContext dbContext) : IKnowledgeArticleService
{
    public async Task<PageResult<KnowledgeArticleDto>> GetPageAsync(KnowledgeArticleQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var articles = dbContext.KnowledgeArticles.AsNoTracking();

        if (userId.HasValue)
        {
            articles = articles.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            articles = articles.Where(x => x.Title.Contains(keyword) || (x.Content != null && x.Content.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            articles = articles.Where(x => x.Category == query.Category);
        }

        if (query.IsPublished.HasValue)
        {
            articles = articles.Where(x => x.IsPublished == query.IsPublished.Value);
        }

        var total = await articles.CountAsync(cancellationToken);
        var items = await articles
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<KnowledgeArticleDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<KnowledgeArticleDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await dbContext.KnowledgeArticles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return article is null ? null : ToDto(article);
    }

    public async Task<KnowledgeArticleDto> CreateAsync(CreateKnowledgeArticleDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var article = new KnowledgeArticle
        {
            UserId = userId,
            Title = input.Title,
            Content = input.Content,
            Category = input.Category,
            Tags = input.Tags,
            IsPublished = input.IsPublished
        };

        dbContext.KnowledgeArticles.Add(article);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(article);
    }

    public async Task<KnowledgeArticleDto?> UpdateAsync(Guid id, UpdateKnowledgeArticleDto input, CancellationToken cancellationToken = default)
    {
        var article = await dbContext.KnowledgeArticles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (article is null) return null;

        if (input.Title is not null) article.Title = input.Title;
        if (input.Content is not null) article.Content = input.Content;
        if (input.Category is not null) article.Category = input.Category;
        if (input.Tags is not null) article.Tags = input.Tags;
        if (input.IsPublished.HasValue) article.IsPublished = input.IsPublished.Value;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(article);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await dbContext.KnowledgeArticles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (article is null) return false;

        dbContext.KnowledgeArticles.Remove(article);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task IncrementViewCountAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await dbContext.KnowledgeArticles.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (article is not null)
        {
            article.ViewCount++;
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static KnowledgeArticleDto ToDto(KnowledgeArticle article) => new()
    {
        Id = article.Id,
        UserId = article.UserId,
        Title = article.Title,
        Content = article.Content,
        Category = article.Category,
        Tags = article.Tags,
        ViewCount = article.ViewCount,
        IsPublished = article.IsPublished,
        CreatedAt = article.CreatedAt,
        UpdatedAt = article.UpdatedAt
    };
}