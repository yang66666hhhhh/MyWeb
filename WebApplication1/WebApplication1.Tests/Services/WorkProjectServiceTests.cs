using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Work.Services;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Tests.Services;

public class WorkProjectServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly WorkProjectService _service;

    public WorkProjectServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        var loggerMock = new Mock<ILogger<WorkProjectService>>();
        _service = new WorkProjectService(_context, loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProject()
    {
        var input = new CreateWorkProjectDto
        {
            ProjectName = "Test Project",
            ProjectCode = "TP001",
            Location = "惠州",
            ProjectType = WorkProjectType.Internal,
            Status = WorkProjectStatus.Active
        };

        var result = await _service.CreateAsync(input);

        Assert.NotNull(result);
        Assert.Equal("Test Project", result.ProjectName);
        Assert.Equal("TP001", result.ProjectCode);
        Assert.Equal("惠州", result.Location);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProject_WhenExists()
    {
        var project = new WorkProject
        {
            ProjectName = "Existing Project",
            ProjectCode = "EP001",
            Status = WorkProjectStatus.Active
        };
        _context.WorkProjects.Add(project);
        await _context.SaveChangesAsync();

        var result = await _service.GetByIdAsync(project.Id);

        Assert.NotNull(result);
        Assert.Equal("Existing Project", result.ProjectName);
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
        for (int i = 1; i <= 15; i++)
        {
            _context.WorkProjects.Add(new WorkProject
            {
                ProjectName = $"Project {i}",
                ProjectCode = $"P{i:D3}",
                Status = WorkProjectStatus.Active
            });
        }
        await _context.SaveChangesAsync();

        var query = new WorkProjectQueryDto { Page = 1, PageSize = 10 };
        var result = await _service.GetPageAsync(query);

        Assert.Equal(15, result.Total);
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(1, result.Page);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task GetPageAsync_ShouldFilterByKeyword()
    {
        _context.WorkProjects.Add(new WorkProject { ProjectName = "Alpha Project", Status = WorkProjectStatus.Active });
        _context.WorkProjects.Add(new WorkProject { ProjectName = "Beta Project", Status = WorkProjectStatus.Active });
        _context.WorkProjects.Add(new WorkProject { ProjectName = "Alpha Beta", Status = WorkProjectStatus.Active });
        await _context.SaveChangesAsync();

        var query = new WorkProjectQueryDto { Page = 1, PageSize = 10, Keyword = "Alpha" };
        var result = await _service.GetPageAsync(query);

        Assert.Equal(2, result.Total);
        Assert.All(result.Items, x => Assert.Contains("Alpha", x.ProjectName));
    }

    [Fact]
    public async Task GetPageAsync_ShouldFilterByLocationKeyword()
    {
        _context.WorkProjects.Add(new WorkProject { ProjectName = "Alpha Project", Location = "惠州", Status = WorkProjectStatus.Active });
        _context.WorkProjects.Add(new WorkProject { ProjectName = "Beta Project", Location = "苏州", Status = WorkProjectStatus.Active });
        await _context.SaveChangesAsync();

        var query = new WorkProjectQueryDto { Page = 1, PageSize = 10, Keyword = "惠州" };
        var result = await _service.GetPageAsync(query);

        Assert.Single(result.Items);
        Assert.Equal("惠州", result.Items[0].Location);
    }

    [Fact]
    public async Task GetPageAsync_ShouldFilterByStatus()
    {
        _context.WorkProjects.Add(new WorkProject { ProjectName = "Active Project", Status = WorkProjectStatus.Active });
        _context.WorkProjects.Add(new WorkProject { ProjectName = "Completed Project", Status = WorkProjectStatus.Completed });
        await _context.SaveChangesAsync();

        var query = new WorkProjectQueryDto { Page = 1, PageSize = 10, Status = WorkProjectStatus.Active };
        var result = await _service.GetPageAsync(query);

        Assert.Equal(1, result.Total);
        Assert.Equal("Active Project", result.Items[0].ProjectName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProject()
    {
        var project = new WorkProject
        {
            ProjectName = "Original Name",
            Status = WorkProjectStatus.Active
        };
        _context.WorkProjects.Add(project);
        await _context.SaveChangesAsync();

        var input = new UpdateWorkProjectDto
        {
            ProjectName = "Updated Name",
            Location = "东莞",
            Status = WorkProjectStatus.Completed
        };

        var result = await _service.UpdateAsync(project.Id, input);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.ProjectName);
        Assert.Equal("东莞", result.Location);
        Assert.Equal(WorkProjectStatus.Completed, result.Status);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotExists()
    {
        var input = new UpdateWorkProjectDto
        {
            ProjectName = "Updated Name",
            Status = WorkProjectStatus.Completed
        };

        var result = await _service.UpdateAsync(Guid.NewGuid(), input);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenExists()
    {
        var project = new WorkProject { ProjectName = "To Delete" };
        _context.WorkProjects.Add(project);
        await _context.SaveChangesAsync();

        var result = await _service.DeleteAsync(project.Id);

        Assert.True(result);
        var deleted = await _context.WorkProjects.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == project.Id);
        Assert.NotNull(deleted);
        Assert.True(deleted.IsDeleted);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
    {
        var result = await _service.DeleteAsync(Guid.NewGuid());

        Assert.False(result);
    }
}
