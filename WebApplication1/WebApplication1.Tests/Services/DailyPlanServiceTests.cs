using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using WebApplication1.Shared.Data;
using WebApplication1.Features.DailyPlans;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Tests.Services;

public class DailyPlanServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly DailyPlanService _service;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _otherUserId = Guid.NewGuid();

    public DailyPlanServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        var loggerMock = new Mock<ILogger<DailyPlanService>>();
        _service = new DailyPlanService(_context, loggerMock.Object);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateDailyPlan()
    {
        var input = new CreateDailyPlanDto
        {
            PlanDate = DateOnly.FromDateTime(DateTime.Today),
            Title = "Test Plan",
            Description = "Test Description",
            Priority = 2
        };

        var result = await _service.CreateAsync(input, _userId);

        Assert.NotNull(result);
        Assert.Equal("Test Plan", result.Title);
        Assert.Equal(_userId, result.UserId);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPlan_WhenExists()
    {
        var plan = new DailyPlan
        {
            UserId = _userId,
            Title = "Existing Plan",
            PlanDate = DateOnly.FromDateTime(DateTime.Today)
        };
        _context.DailyPlans.Add(plan);
        await _context.SaveChangesAsync();

        var result = await _service.GetByIdAsync(plan.Id, _userId);

        Assert.NotNull(result);
        Assert.Equal("Existing Plan", result.Title);
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
            _context.DailyPlans.Add(new DailyPlan
            {
                UserId = _userId,
                Title = $"Plan {i}",
                PlanDate = DateOnly.FromDateTime(DateTime.Today.AddDays(i))
            });
        }
        _context.DailyPlans.Add(new DailyPlan
        {
            UserId = _otherUserId,
            Title = "Other User Plan",
            PlanDate = DateOnly.FromDateTime(DateTime.Today)
        });
        await _context.SaveChangesAsync();

        var query = new DailyPlanQueryDto { Page = 1, PageSize = 10 };
        var result = await _service.GetPageAsync(query, _userId);

        Assert.Equal(15, result.Total);
        Assert.Equal(10, result.Items.Count);
    }

    [Fact]
    public async Task GetPageAsync_ShouldFilterByDateRange()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        _context.DailyPlans.Add(new DailyPlan { UserId = _userId, Title = "Today Plan", PlanDate = today });
        _context.DailyPlans.Add(new DailyPlan { UserId = _userId, Title = "Tomorrow Plan", PlanDate = today.AddDays(1) });
        _context.DailyPlans.Add(new DailyPlan { UserId = _userId, Title = "Yesterday Plan", PlanDate = today.AddDays(-1) });
        _context.DailyPlans.Add(new DailyPlan { UserId = _otherUserId, Title = "Other Tomorrow Plan", PlanDate = today.AddDays(1) });
        await _context.SaveChangesAsync();

        var query = new DailyPlanQueryDto
        {
            Page = 1,
            PageSize = 10,
            StartDate = today,
            EndDate = today.AddDays(1)
        };
        var result = await _service.GetPageAsync(query, _userId);

        Assert.Equal(2, result.Total);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePlan()
    {
        var plan = new DailyPlan
        {
            UserId = _userId,
            Title = "Original Title",
            PlanDate = DateOnly.FromDateTime(DateTime.Today)
        };
        _context.DailyPlans.Add(plan);
        await _context.SaveChangesAsync();

        var input = new UpdateDailyPlanDto
        {
            Title = "Updated Title",
            Description = "Updated Description"
        };

        var result = await _service.UpdateAsync(plan.Id, input, _userId);

        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated Description", result.Description);
    }

    [Fact]
    public async Task CompleteAsync_ShouldMarkPlanAsCompleted()
    {
        var plan = new DailyPlan
        {
            UserId = _userId,
            Title = "Plan to Complete",
            PlanDate = DateOnly.FromDateTime(DateTime.Today),
            Status = DailyPlanStatus.Pending
        };
        _context.DailyPlans.Add(plan);
        await _context.SaveChangesAsync();

        var result = await _service.CompleteAsync(plan.Id, _userId);

        Assert.NotNull(result);
        Assert.Equal(DailyPlanStatus.Completed, result.Status);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemovePlan()
    {
        var plan = new DailyPlan
        {
            UserId = _userId,
            Title = "Plan to Delete",
            PlanDate = DateOnly.FromDateTime(DateTime.Today)
        };
        _context.DailyPlans.Add(plan);
        await _context.SaveChangesAsync();

        var success = await _service.DeleteAsync(plan.Id, _userId);

        Assert.True(success);
        var deleted = await _context.DailyPlans.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == plan.Id);
        Assert.NotNull(deleted);
        Assert.True(deleted.IsDeleted);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
    {
        var success = await _service.DeleteAsync(Guid.NewGuid(), _userId);

        Assert.False(success);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotOwnPlan()
    {
        var plan = new DailyPlan
        {
            UserId = _otherUserId,
            Title = "Other User Plan",
            PlanDate = DateOnly.FromDateTime(DateTime.Today)
        };
        _context.DailyPlans.Add(plan);
        await _context.SaveChangesAsync();

        var result = await _service.GetByIdAsync(plan.Id, _userId);

        Assert.Null(result);
    }
}
