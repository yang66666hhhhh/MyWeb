using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Tasks;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Tests.Services;

public class TaskItemServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly TaskItemService _service;

    public TaskItemServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new TaskItemService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTaskItem()
    {
        var userId = Guid.NewGuid();
        var input = new CreateTaskItemDto
        {
            PlanDate = DateOnly.FromDateTime(DateTime.Today),
            Title = "Test Task",
            Description = "Test Description",
            Priority = 2,
            TaskType = "Personal",
            Source = "Growth"
        };

        var result = await _service.CreateAsync(input, userId);

        Assert.NotNull(result);
        Assert.Equal("Test Task", result.Title);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("Personal", result.Type);
        Assert.Equal("Growth", result.Source);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask_WhenExists()
    {
        var userId = Guid.NewGuid();
        var task = new TaskItem
        {
            UserId = userId,
            Title = "Existing Task",
            PlanDate = DateOnly.FromDateTime(DateTime.Today),
            Type = TaskType.Work,
            Source = TaskSource.Work
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var result = await _service.GetByIdAsync(task.Id);

        Assert.NotNull(result);
        Assert.Equal("Existing Task", result.Title);
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
            _context.Tasks.Add(new TaskItem
            {
                UserId = userId,
                Title = $"Task {i}",
                PlanDate = DateOnly.FromDateTime(DateTime.Today.AddDays(i)),
                Type = TaskType.Personal,
                Source = TaskSource.Growth
            });
        }
        await _context.SaveChangesAsync();

        var query = new TaskItemQueryDto { Page = 1, PageSize = 10 };
        var result = await _service.GetPageAsync(query, userId);

        Assert.Equal(15, result.Total);
        Assert.Equal(10, result.Items.Count);
    }

    [Fact]
    public async Task GetPageAsync_ShouldFilterByTaskType()
    {
        var userId = Guid.NewGuid();
        _context.Tasks.Add(new TaskItem { UserId = userId, Title = "Personal Task", PlanDate = DateOnly.FromDateTime(DateTime.Today), Type = TaskType.Personal, Source = TaskSource.Growth });
        _context.Tasks.Add(new TaskItem { UserId = userId, Title = "Work Task", PlanDate = DateOnly.FromDateTime(DateTime.Today), Type = TaskType.Work, Source = TaskSource.Work });
        await _context.SaveChangesAsync();

        var query = new TaskItemQueryDto { TaskType = "Personal" };
        var result = await _service.GetPageAsync(query, userId);

        Assert.Equal(1, result.Total);
        Assert.Equal("Personal Task", result.Items[0].Title);
    }

    [Fact]
    public async Task GetPageAsync_ShouldFilterBySource()
    {
        var userId = Guid.NewGuid();
        _context.Tasks.Add(new TaskItem { UserId = userId, Title = "Growth Task", PlanDate = DateOnly.FromDateTime(DateTime.Today), Type = TaskType.Personal, Source = TaskSource.Growth });
        _context.Tasks.Add(new TaskItem { UserId = userId, Title = "Work Task", PlanDate = DateOnly.FromDateTime(DateTime.Today), Type = TaskType.Work, Source = TaskSource.Work });
        await _context.SaveChangesAsync();

        var query = new TaskItemQueryDto { Source = "Work" };
        var result = await _service.GetPageAsync(query, userId);

        Assert.Equal(1, result.Total);
        Assert.Equal("Work Task", result.Items[0].Title);
    }

    [Fact]
    public async Task GetPageAsync_ShouldFilterByDateRange()
    {
        var userId = Guid.NewGuid();
        var today = DateOnly.FromDateTime(DateTime.Today);
        _context.Tasks.Add(new TaskItem { UserId = userId, Title = "Today Task", PlanDate = today, Type = TaskType.Personal, Source = TaskSource.Growth });
        _context.Tasks.Add(new TaskItem { UserId = userId, Title = "Tomorrow Task", PlanDate = today.AddDays(1), Type = TaskType.Personal, Source = TaskSource.Growth });
        _context.Tasks.Add(new TaskItem { UserId = userId, Title = "Yesterday Task", PlanDate = today.AddDays(-1), Type = TaskType.Personal, Source = TaskSource.Growth });
        await _context.SaveChangesAsync();

        var query = new TaskItemQueryDto { StartDate = today, EndDate = today.AddDays(1) };
        var result = await _service.GetPageAsync(query, userId);

        Assert.Equal(2, result.Total);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTask()
    {
        var userId = Guid.NewGuid();
        var task = new TaskItem
        {
            UserId = userId,
            Title = "Original Title",
            PlanDate = DateOnly.FromDateTime(DateTime.Today),
            Type = TaskType.Personal,
            Source = TaskSource.Growth
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var input = new UpdateTaskItemDto
        {
            Title = "Updated Title",
            Description = "Updated Description",
            TaskType = "Work"
        };

        var result = await _service.UpdateAsync(task.Id, input);

        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated Description", result.Description);
        Assert.Equal("Work", result.Type);
    }

    [Fact]
    public async Task CompleteAsync_ShouldMarkTaskAsCompleted()
    {
        var userId = Guid.NewGuid();
        var task = new TaskItem
        {
            UserId = userId,
            Title = "Task to Complete",
            PlanDate = DateOnly.FromDateTime(DateTime.Today),
            Status = TaskItemStatus.Pending,
            Type = TaskType.Personal,
            Source = TaskSource.Growth
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var result = await _service.CompleteAsync(task.Id);

        Assert.NotNull(result);
        Assert.Equal("Completed", result.Status);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTask()
    {
        var userId = Guid.NewGuid();
        var task = new TaskItem
        {
            UserId = userId,
            Title = "Task to Delete",
            PlanDate = DateOnly.FromDateTime(DateTime.Today),
            Type = TaskType.Personal,
            Source = TaskSource.Growth
        };
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        var success = await _service.DeleteAsync(task.Id);

        Assert.True(success);
        var deleted = await _context.Tasks.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == task.Id);
        Assert.NotNull(deleted);
        Assert.True(deleted.IsDeleted);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
    {
        var success = await _service.DeleteAsync(Guid.NewGuid());

        Assert.False(success);
    }
}