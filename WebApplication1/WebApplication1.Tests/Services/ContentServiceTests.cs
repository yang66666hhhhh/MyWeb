using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Content.Services;
using WebApplication1.Features.Content.Dtos;
using WebApplication1.Features.Content.Entities;

namespace WebApplication1.Tests.Services;

public class ContentServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ContentService _service;

    public ContentServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new ContentService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region Article Tests

    [Fact]
    public async Task CreateArticleAsync_ShouldCreateArticle()
    {
        var userId = Guid.NewGuid();
        var input = new CreateArticleDto
        {
            Title = "测试文章",
            Content = "这是测试内容",
            Status = "draft",
            Category = "技术",
            Tags = "C#,ASP.NET"
        };

        var result = await _service.CreateArticleAsync(input, userId);

        Assert.NotNull(result);
        Assert.Equal("测试文章", result.Title);
        Assert.Equal("这是测试内容", result.Content);
        Assert.Equal("draft", result.Status);
        Assert.Equal("技术", result.Category);
        Assert.Equal(userId.ToString(), result.UserId);
    }

    [Fact]
    public async Task GetArticleByIdAsync_ShouldReturnArticle_WhenExists()
    {
        var userId = Guid.NewGuid();
        var article = new Article
        {
            UserId = userId,
            Title = "测试文章",
            Content = "这是测试内容",
            Status = "draft",
            Category = "技术"
        };
        _context.Articles.Add(article);
        await _context.SaveChangesAsync();

        var result = await _service.GetArticleByIdAsync(article.Id);

        Assert.NotNull(result);
        Assert.Equal("测试文章", result.Title);
        Assert.Equal("这是测试内容", result.Content);
    }

    [Fact]
    public async Task GetArticleByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _service.GetArticleByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetArticlePageAsync_ShouldReturnPaginatedResults()
    {
        var userId = Guid.NewGuid();
        for (int i = 1; i <= 15; i++)
        {
            _context.Articles.Add(new Article
            {
                UserId = userId,
                Title = $"文章 {i}",
                Content = $"内容 {i}",
                Status = i % 2 == 0 ? "published" : "draft",
                Category = "技术"
            });
        }
        await _context.SaveChangesAsync();

        var query = new ContentQueryDto { Page = 1, PageSize = 10 };
        var result = await _service.GetArticlePageAsync(query, userId);

        Assert.NotNull(result);
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(15, result.Total);
    }

    [Fact]
    public async Task UpdateArticleAsync_ShouldUpdateArticle_WhenExists()
    {
        var userId = Guid.NewGuid();
        var article = new Article
        {
            UserId = userId,
            Title = "测试文章",
            Content = "这是测试内容",
            Status = "draft",
            Category = "技术"
        };
        _context.Articles.Add(article);
        await _context.SaveChangesAsync();

        var input = new UpdateArticleDto
        {
            Title = "更新后的文章",
            Content = "更新后的内容",
            Status = "published"
        };

        var result = await _service.UpdateArticleAsync(article.Id, input);

        Assert.NotNull(result);
        Assert.Equal("更新后的文章", result.Title);
        Assert.Equal("更新后的内容", result.Content);
        Assert.Equal("published", result.Status);
        Assert.NotNull(result.PublishedAt);
    }

    [Fact]
    public async Task UpdateArticleAsync_ShouldReturnNull_WhenNotExists()
    {
        var input = new UpdateArticleDto { Title = "更新后的文章" };

        var result = await _service.UpdateArticleAsync(Guid.NewGuid(), input);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteArticleAsync_ShouldDeleteArticle_WhenExists()
    {
        var userId = Guid.NewGuid();
        var article = new Article
        {
            UserId = userId,
            Title = "测试文章",
            Content = "这是测试内容",
            Status = "draft"
        };
        _context.Articles.Add(article);
        await _context.SaveChangesAsync();

        var result = await _service.DeleteArticleAsync(article.Id);

        Assert.True(result);
        Assert.Null(await _context.Articles.FindAsync(article.Id));
    }

    [Fact]
    public async Task DeleteArticleAsync_ShouldReturnFalse_WhenNotExists()
    {
        var result = await _service.DeleteArticleAsync(Guid.NewGuid());

        Assert.False(result);
    }

    #endregion

    #region MediaItem Tests

    [Fact]
    public async Task CreateMediaItemAsync_ShouldCreateMediaItem()
    {
        var userId = Guid.NewGuid();
        var input = new CreateMediaItemDto
        {
            FileName = "test.jpg",
            FileUrl = "https://example.com/test.jpg",
            FileType = "image/jpeg",
            FileSize = 1024,
            Tags = "测试,图片"
        };

        var result = await _service.CreateMediaItemAsync(input, userId);

        Assert.NotNull(result);
        Assert.Equal("test.jpg", result.FileName);
        Assert.Equal("https://example.com/test.jpg", result.FileUrl);
        Assert.Equal("image/jpeg", result.FileType);
        Assert.Equal(1024, result.FileSize);
    }

    [Fact]
    public async Task GetMediaItemByIdAsync_ShouldReturnMediaItem_WhenExists()
    {
        var userId = Guid.NewGuid();
        var mediaItem = new MediaItem
        {
            UserId = userId,
            FileName = "test.jpg",
            FileUrl = "https://example.com/test.jpg",
            FileType = "image/jpeg",
            FileSize = 1024
        };
        _context.MediaItems.Add(mediaItem);
        await _context.SaveChangesAsync();

        var result = await _service.GetMediaItemByIdAsync(mediaItem.Id);

        Assert.NotNull(result);
        Assert.Equal("test.jpg", result.FileName);
    }

    [Fact]
    public async Task GetMediaItemByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _service.GetMediaItemByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    #endregion

    #region PublishingCalendar Tests

    [Fact]
    public async Task CreatePublishingCalendarAsync_ShouldCreatePublishingCalendar()
    {
        var userId = Guid.NewGuid();
        var input = new CreatePublishingCalendarDto
        {
            PlannedDate = "2026-02-01",
            Platform = "微信公众号",
            Title = "技术分享文章",
            Status = "pending"
        };

        var result = await _service.CreatePublishingCalendarAsync(input, userId);

        Assert.NotNull(result);
        Assert.Equal("2026-02-01", result.PlannedDate);
        Assert.Equal("微信公众号", result.Platform);
        Assert.Equal("技术分享文章", result.Title);
        Assert.Equal("pending", result.Status);
    }

    [Fact]
    public async Task GetPublishingCalendarByIdAsync_ShouldReturnPublishingCalendar_WhenExists()
    {
        var userId = Guid.NewGuid();
        var calendar = new PublishingCalendar
        {
            UserId = userId,
            PlannedDate = "2026-02-01",
            Platform = "微信公众号",
            Title = "技术分享文章",
            Status = "pending"
        };
        _context.PublishingCalendars.Add(calendar);
        await _context.SaveChangesAsync();

        var result = await _service.GetPublishingCalendarByIdAsync(calendar.Id);

        Assert.NotNull(result);
        Assert.Equal("技术分享文章", result.Title);
    }

    [Fact]
    public async Task GetPublishingCalendarByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _service.GetPublishingCalendarByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    #endregion
}
