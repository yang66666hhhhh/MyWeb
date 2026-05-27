using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Growth.Services;
using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Features.Growth.Entities;

namespace WebApplication1.Tests.Services;

public class KnowledgeArticleServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly KnowledgeArticleService _service;

    public KnowledgeArticleServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new KnowledgeArticleService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateArticle()
    {
        var userId = Guid.NewGuid();
        var input = new CreateKnowledgeArticleDto
        {
            Title = "Test Article",
            Content = "Test Content",
            Category = "Test/Category",
            Tags = "tag1,tag2",
            IsPublished = true
        };

        var result = await _service.CreateAsync(input, userId);

        Assert.NotNull(result);
        Assert.Equal("Test Article", result.Title);
        Assert.Equal(userId, result.UserId);
        Assert.True(result.IsPublished);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnArticle_WhenExists()
    {
        var userId = Guid.NewGuid();
        var article = new KnowledgeArticle
        {
            UserId = userId,
            Title = "Existing Article",
            Content = "Content",
            Category = "Test"
        };
        _context.KnowledgeArticles.Add(article);
        await _context.SaveChangesAsync();

        var result = await _service.GetByIdAsync(article.Id);

        Assert.NotNull(result);
        Assert.Equal("Existing Article", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetPageAsync_ShouldReturnPaginatedResults()
    {
        var userId = Guid.NewGuid();
        for (int i = 1; i <= 15; i++)
        {
            _context.KnowledgeArticles.Add(new KnowledgeArticle
            {
                UserId = userId,
                Title = $"Article {i}",
                Category = "Test"
            });
        }
        await _context.SaveChangesAsync();

        var query = new KnowledgeArticleQueryDto { Page = 1, PageSize = 10 };
        var result = await _service.GetPageAsync(query, userId);

        Assert.Equal(15, result.Total);
        Assert.Equal(10, result.Items.Count);
    }

    [Fact]
    public async Task GetPageAsync_ShouldFilterByCategory()
    {
        var userId = Guid.NewGuid();
        _context.KnowledgeArticles.Add(new KnowledgeArticle { UserId = userId, Title = "Work Article", Category = "Work" });
        _context.KnowledgeArticles.Add(new KnowledgeArticle { UserId = userId, Title = "Study Article", Category = "Study" });
        await _context.SaveChangesAsync();

        var query = new KnowledgeArticleQueryDto { Category = "Work" };
        var result = await _service.GetPageAsync(query, userId);

        Assert.Equal(1, result.Total);
        Assert.Equal("Work Article", result.Items[0].Title);
    }

    [Fact]
    public async Task GetPageAsync_ShouldFilterByKeyword()
    {
        var userId = Guid.NewGuid();
        _context.KnowledgeArticles.Add(new KnowledgeArticle { UserId = userId, Title = "ASP.NET Core Guide", Content = "Web development", Category = "Tech" });
        _context.KnowledgeArticles.Add(new KnowledgeArticle { UserId = userId, Title = "Vue.js Guide", Content = "Frontend development", Category = "Tech" });
        await _context.SaveChangesAsync();

        var query = new KnowledgeArticleQueryDto { Keyword = "ASP.NET" };
        var result = await _service.GetPageAsync(query, userId);

        Assert.Equal(1, result.Total);
        Assert.Equal("ASP.NET Core Guide", result.Items[0].Title);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateArticle()
    {
        var userId = Guid.NewGuid();
        var article = new KnowledgeArticle
        {
            UserId = userId,
            Title = "Original Title",
            Category = "Original"
        };
        _context.KnowledgeArticles.Add(article);
        await _context.SaveChangesAsync();

        var input = new UpdateKnowledgeArticleDto
        {
            Title = "Updated Title",
            Category = "Updated"
        };

        var result = await _service.UpdateAsync(article.Id, input);

        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated", result.Category);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveArticle()
    {
        var userId = Guid.NewGuid();
        var article = new KnowledgeArticle
        {
            UserId = userId,
            Title = "Article to Delete",
            Category = "Test"
        };
        _context.KnowledgeArticles.Add(article);
        await _context.SaveChangesAsync();

        var success = await _service.DeleteAsync(article.Id);

        Assert.True(success);
        var deleted = await _context.KnowledgeArticles.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == article.Id);
        Assert.NotNull(deleted);
        Assert.True(deleted.IsDeleted);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
    {
        var success = await _service.DeleteAsync(Guid.NewGuid());

        Assert.False(success);
    }

    [Fact]
    public async Task IncrementViewCountAsync_ShouldIncreaseViewCount()
    {
        var userId = Guid.NewGuid();
        var article = new KnowledgeArticle
        {
            UserId = userId,
            Title = "Test Article",
            Category = "Test",
            ViewCount = 5
        };
        _context.KnowledgeArticles.Add(article);
        await _context.SaveChangesAsync();

        await _service.IncrementViewCountAsync(article.Id);

        var updated = await _context.KnowledgeArticles.FindAsync(article.Id);
        Assert.Equal(6, updated!.ViewCount);
    }
}