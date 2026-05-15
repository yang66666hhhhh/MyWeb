using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Features.Work.Services;
using WebApplication1.Shared.Data;

namespace WebApplication1.Tests.Services;

public class ImplLogServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ImplLogService _service;

    public ImplLogServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new ImplLogService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistProjectIdAndResolveProjectName()
    {
        var userId = Guid.NewGuid();
        var project = new WorkProject
        {
            Id = Guid.NewGuid(),
            ProjectName = "实施项目A",
        };
        _context.WorkProjects.Add(project);
        await _context.SaveChangesAsync();

        var input = new CreateImplLogDto
        {
            WorkDate = new DateOnly(2026, 5, 15),
            Title = "项目现场调试",
            ProjectId = project.Id,
            TotalHours = 8,
        };

        var result = await _service.CreateAsync(input, userId);

        Assert.Equal(project.Id, result.ProjectId);
        Assert.Equal("实施项目A", result.ProjectName);

        var entity = await _context.ImplLogs.FirstAsync();
        Assert.Equal(project.Id, entity.ProjectId);
        Assert.Equal("实施项目A", entity.ProjectName);
    }

    [Fact]
    public async Task CreateAsync_ShouldSerializeAndReturnExtraData()
    {
        var userId = Guid.NewGuid();
        using var extraDataDocument = JsonDocument.Parse("""
            {
              "location": "惠州",
              "equipment": ["PLC", "扫码枪"],
              "taskTypes": ["调试", "巡检"]
            }
            """);
        var input = new CreateImplLogDto
        {
            WorkDate = new DateOnly(2026, 5, 15),
            Title = "项目现场调试",
            TotalHours = 8,
            ExtraData = extraDataDocument.RootElement.Clone()
        };

        var result = await _service.CreateAsync(input, userId);

        Assert.NotNull(result.ExtraData);
        Assert.Equal("惠州", result.ExtraData.Value.GetProperty("location").GetString());

        var entity = await _context.ImplLogs.FirstAsync();
        Assert.NotNull(entity.ExtraData);
        using var savedDocument = JsonDocument.Parse(entity.ExtraData);
        Assert.Equal("惠州", savedDocument.RootElement.GetProperty("location").GetString());
        Assert.Equal(2, savedDocument.RootElement.GetProperty("equipment").GetArrayLength());
        Assert.Equal(2, savedDocument.RootElement.GetProperty("taskTypes").GetArrayLength());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNullForOtherUserLog()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var log = new ImplLog
        {
            Id = Guid.NewGuid(),
            UserId = ownerId,
            WorkDate = new DateOnly(2026, 5, 15),
            WeekDay = "Friday",
            Title = "仅本人可见",
            TotalHours = 6,
        };

        _context.ImplLogs.Add(log);
        await _context.SaveChangesAsync();

        var result = await _service.GetByIdAsync(log.Id, otherUserId);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProjectRelationAndProjectName()
    {
        var userId = Guid.NewGuid();
        var oldProject = new WorkProject
        {
            Id = Guid.NewGuid(),
            ProjectName = "旧项目",
        };
        var newProject = new WorkProject
        {
            Id = Guid.NewGuid(),
            ProjectName = "新项目",
        };
        var log = new ImplLog
        {
            UserId = userId,
            WorkDate = new DateOnly(2026, 5, 15),
            WeekDay = "Friday",
            Title = "原始日志",
            ProjectId = oldProject.Id,
            ProjectName = oldProject.ProjectName,
            TotalHours = 6,
        };

        _context.WorkProjects.AddRange(oldProject, newProject);
        _context.ImplLogs.Add(log);
        await _context.SaveChangesAsync();

        var input = new UpdateImplLogDto
        {
            ProjectId = newProject.Id,
            Title = "更新后的日志",
        };

        var result = await _service.UpdateAsync(log.Id, input, userId);

        Assert.NotNull(result);
        Assert.Equal(newProject.Id, result.ProjectId);
        Assert.Equal("新项目", result.ProjectName);
        Assert.Equal("更新后的日志", result.Title);
    }

    [Fact]
    public async Task GetSummaryAsync_ShouldAggregateFilteredResult()
    {
        var userId = Guid.NewGuid();
        using var firstExtraData = JsonDocument.Parse("""
            {
              "location": "惠州",
              "equipment": ["PLC", "扫码枪"],
              "taskTypes": ["调试", "巡检"]
            }
            """);
        using var secondExtraData = JsonDocument.Parse("""
            {
              "location": "惠州",
              "equipment": ["PLC", "工控机"],
              "taskTypes": ["巡检", "交付"]
            }
            """);

        _context.ImplLogs.AddRange(
            new ImplLog
            {
                UserId = userId,
                WorkDate = new DateOnly(2026, 5, 15),
                WeekDay = "Friday",
                Title = "现场调试",
                ProjectName = "实施项目A",
                TotalHours = 8,
                ExtraData = firstExtraData.RootElement.GetRawText(),
            },
            new ImplLog
            {
                UserId = userId,
                WorkDate = new DateOnly(2026, 5, 16),
                WeekDay = "Saturday",
                Title = "交付巡检",
                ProjectName = "实施项目B",
                TotalHours = 6,
                ExtraData = secondExtraData.RootElement.GetRawText(),
            },
            new ImplLog
            {
                UserId = Guid.NewGuid(),
                WorkDate = new DateOnly(2026, 5, 16),
                WeekDay = "Saturday",
                Title = "其他用户日志",
                ProjectName = "外部项目",
                TotalHours = 10,
                ExtraData = firstExtraData.RootElement.GetRawText(),
            });
        await _context.SaveChangesAsync();

        var result = await _service.GetSummaryAsync(new ImplLogQueryDto
        {
            Keyword = "惠州",
            StartDate = new DateOnly(2026, 5, 15),
            EndDate = new DateOnly(2026, 5, 16),
        }, userId);

        Assert.Equal(2, result.TotalCount);
        Assert.Equal(14, result.TotalHours);
        Assert.Equal(3, result.UniqueEquipmentCount);
        Assert.Equal(3, result.UniqueTaskTypeCount);
    }
}
