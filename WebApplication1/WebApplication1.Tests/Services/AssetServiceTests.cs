using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Data;
using WebApplication1.Features.Assets.Services;
using WebApplication1.Features.Assets.Dtos;
using WebApplication1.Features.Assets.Entities;

namespace WebApplication1.Tests.Services;

public class AssetServiceTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AssetService _service;

    public AssetServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _service = new AssetService(_context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    #region Income Tests

    [Fact]
    public async Task CreateIncomeAsync_ShouldCreateIncome()
    {
        var userId = Guid.NewGuid();
        var input = new CreateIncomeDto
        {
            IncomeDate = "2026-01-15",
            Category = "工资",
            Title = "月工资",
            Amount = 10000,
            Description = "1月份工资"
        };

        var result = await _service.CreateIncomeAsync(input, userId);

        Assert.NotNull(result);
        Assert.Equal("月工资", result.Title);
        Assert.Equal(10000, result.Amount);
        Assert.Equal(userId.ToString(), result.UserId);
    }

    [Fact]
    public async Task GetIncomeByIdAsync_ShouldReturnIncome_WhenExists()
    {
        var userId = Guid.NewGuid();
        var income = new Income
        {
            UserId = userId,
            IncomeDate = "2026-01-15",
            Category = "工资",
            Title = "月工资",
            Amount = 10000
        };
        _context.Incomes.Add(income);
        await _context.SaveChangesAsync();

        var result = await _service.GetIncomeByIdAsync(income.Id);

        Assert.NotNull(result);
        Assert.Equal("月工资", result.Title);
    }

    [Fact]
    public async Task GetIncomeByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        var result = await _service.GetIncomeByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetIncomePageAsync_ShouldReturnPaginatedResults()
    {
        var userId = Guid.NewGuid();
        for (int i = 1; i <= 15; i++)
        {
            _context.Incomes.Add(new Income
            {
                UserId = userId,
                IncomeDate = $"2026-01-{i:D2}",
                Category = "工资",
                Title = $"收入 {i}",
                Amount = 1000 * i
            });
        }
        await _context.SaveChangesAsync();

        var query = new AssetQueryDto { Page = 1, PageSize = 10 };
        var result = await _service.GetIncomePageAsync(query, userId);

        Assert.NotNull(result);
        Assert.Equal(10, result.Items.Count);
        Assert.Equal(15, result.Total);
    }

    [Fact]
    public async Task UpdateIncomeAsync_ShouldUpdateIncome_WhenExists()
    {
        var userId = Guid.NewGuid();
        var income = new Income
        {
            UserId = userId,
            IncomeDate = "2026-01-15",
            Category = "工资",
            Title = "月工资",
            Amount = 10000
        };
        _context.Incomes.Add(income);
        await _context.SaveChangesAsync();

        var input = new UpdateIncomeDto
        {
            Title = "更新后的工资",
            Amount = 12000
        };

        var result = await _service.UpdateIncomeAsync(income.Id, input);

        Assert.NotNull(result);
        Assert.Equal("更新后的工资", result.Title);
        Assert.Equal(12000, result.Amount);
    }

    [Fact]
    public async Task UpdateIncomeAsync_ShouldReturnNull_WhenNotExists()
    {
        var input = new UpdateIncomeDto { Title = "更新后的工资" };

        var result = await _service.UpdateIncomeAsync(Guid.NewGuid(), input);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteIncomeAsync_ShouldDeleteIncome_WhenExists()
    {
        var userId = Guid.NewGuid();
        var income = new Income
        {
            UserId = userId,
            IncomeDate = "2026-01-15",
            Category = "工资",
            Title = "月工资",
            Amount = 10000
        };
        _context.Incomes.Add(income);
        await _context.SaveChangesAsync();

        var result = await _service.DeleteIncomeAsync(income.Id);

        Assert.True(result);
        Assert.Null(await _context.Incomes.FindAsync(income.Id));
    }

    [Fact]
    public async Task DeleteIncomeAsync_ShouldReturnFalse_WhenNotExists()
    {
        var result = await _service.DeleteIncomeAsync(Guid.NewGuid());

        Assert.False(result);
    }

    #endregion

    #region Expense Tests

    [Fact]
    public async Task CreateExpenseAsync_ShouldCreateExpense()
    {
        var userId = Guid.NewGuid();
        var input = new CreateExpenseDto
        {
            ExpenseDate = "2026-01-15",
            Category = "餐饮",
            Title = "午餐",
            Amount = 50,
            Description = "工作日午餐"
        };

        var result = await _service.CreateExpenseAsync(input, userId);

        Assert.NotNull(result);
        Assert.Equal("午餐", result.Title);
        Assert.Equal(50, result.Amount);
    }

    [Fact]
    public async Task GetExpenseByIdAsync_ShouldReturnExpense_WhenExists()
    {
        var userId = Guid.NewGuid();
        var expense = new Expense
        {
            UserId = userId,
            ExpenseDate = "2026-01-15",
            Category = "餐饮",
            Title = "午餐",
            Amount = 50
        };
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        var result = await _service.GetExpenseByIdAsync(expense.Id);

        Assert.NotNull(result);
        Assert.Equal("午餐", result.Title);
    }

    #endregion

    #region Budget Tests

    [Fact]
    public async Task CreateBudgetAsync_ShouldCreateBudget()
    {
        var userId = Guid.NewGuid();
        var input = new CreateBudgetDto
        {
            Year = 2026,
            Month = 1,
            Category = "餐饮",
            PlannedAmount = 3000,
            ActualAmount = 2500
        };

        var result = await _service.CreateBudgetAsync(input, userId);

        Assert.NotNull(result);
        Assert.Equal(2026, result.Year);
        Assert.Equal(1, result.Month);
        Assert.Equal("餐饮", result.Category);
        Assert.Equal(3000, result.PlannedAmount);
        Assert.Equal(2500, result.ActualAmount);
        Assert.Equal(500, result.RemainingAmount);
    }

    [Fact]
    public async Task GetBudgetByIdAsync_ShouldReturnBudget_WhenExists()
    {
        var userId = Guid.NewGuid();
        var budget = new Budget
        {
            UserId = userId,
            Year = 2026,
            Month = 1,
            Category = "餐饮",
            PlannedAmount = 3000,
            ActualAmount = 2500
        };
        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        var result = await _service.GetBudgetByIdAsync(budget.Id);

        Assert.NotNull(result);
        Assert.Equal("餐饮", result.Category);
    }

    #endregion

    #region Investment Tests

    [Fact]
    public async Task CreateInvestmentAsync_ShouldCreateInvestment()
    {
        var userId = Guid.NewGuid();
        var input = new CreateInvestmentDto
        {
            InvestmentDate = "2026-01-15",
            Type = "股票",
            Name = "腾讯控股",
            Amount = 50000,
            CurrentValue = 55000,
            ReturnRate = 10
        };

        var result = await _service.CreateInvestmentAsync(input, userId);

        Assert.NotNull(result);
        Assert.Equal("腾讯控股", result.Name);
        Assert.Equal(50000, result.Amount);
        Assert.Equal(55000, result.CurrentValue);
        Assert.Equal(10, result.ReturnRate);
    }

    [Fact]
    public async Task GetInvestmentByIdAsync_ShouldReturnInvestment_WhenExists()
    {
        var userId = Guid.NewGuid();
        var investment = new Investment
        {
            UserId = userId,
            InvestmentDate = "2026-01-15",
            Type = "股票",
            Name = "腾讯控股",
            Amount = 50000,
            CurrentValue = 55000,
            ReturnRate = 10
        };
        _context.Investments.Add(investment);
        await _context.SaveChangesAsync();

        var result = await _service.GetInvestmentByIdAsync(investment.Id);

        Assert.NotNull(result);
        Assert.Equal("腾讯控股", result.Name);
    }

    #endregion

    #region Summary Tests

    [Fact]
    public async Task GetSummaryAsync_ShouldReturnCorrectSummary()
    {
        var userId = Guid.NewGuid();

        // Add test data
        _context.Incomes.Add(new Income
        {
            UserId = userId,
            IncomeDate = "2026-01-15",
            Category = "工资",
            Title = "月工资",
            Amount = 10000
        });

        _context.Expenses.Add(new Expense
        {
            UserId = userId,
            ExpenseDate = "2026-01-15",
            Category = "餐饮",
            Title = "午餐",
            Amount = 3000
        });

        _context.Investments.Add(new Investment
        {
            UserId = userId,
            InvestmentDate = "2026-01-15",
            Type = "股票",
            Name = "腾讯控股",
            Amount = 50000
        });

        await _context.SaveChangesAsync();

        var result = await _service.GetSummaryAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(10000, result.TotalIncome);
        Assert.Equal(3000, result.TotalExpense);
        Assert.Equal(50000, result.TotalInvestment);
        Assert.Equal(57000, result.NetAsset);
        Assert.Equal(1, result.IncomeCount);
        Assert.Equal(1, result.ExpenseCount);
        Assert.Equal(1, result.InvestmentCount);
    }

    #endregion
}
