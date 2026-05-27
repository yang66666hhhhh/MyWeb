using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Assets.Dtos;
using WebApplication1.Features.Assets.Entities;
using WebApplication1.Features.Assets.Services.Interfaces;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Assets.Services;

public class AssetService(AppDbContext dbContext) : IAssetService
{
    public async Task<AssetSummaryDto> GetSummaryAsync(Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var incomes = dbContext.Incomes.AsNoTracking();
        var expenses = dbContext.Expenses.AsNoTracking();
        var investments = dbContext.Investments.AsNoTracking();

        if (userId.HasValue)
        {
            incomes = incomes.Where(x => x.UserId == userId.Value);
            expenses = expenses.Where(x => x.UserId == userId.Value);
            investments = investments.Where(x => x.UserId == userId.Value);
        }

        var totalIncome = await incomes.SumAsync(x => x.Amount, cancellationToken);
        var totalExpense = await expenses.SumAsync(x => x.Amount, cancellationToken);
        var totalInvestment = await investments.SumAsync(x => x.Amount, cancellationToken);
        var incomeCount = await incomes.CountAsync(cancellationToken);
        var expenseCount = await expenses.CountAsync(cancellationToken);
        var investmentCount = await investments.CountAsync(cancellationToken);

        return new AssetSummaryDto
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense,
            TotalInvestment = totalInvestment,
            NetAsset = totalIncome - totalExpense + totalInvestment,
            IncomeCount = incomeCount,
            ExpenseCount = expenseCount,
            InvestmentCount = investmentCount
        };
    }

    public async Task<PageResult<IncomeDto>> GetIncomePageAsync(AssetQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var incomes = dbContext.Incomes.AsNoTracking();

        if (userId.HasValue)
        {
            incomes = incomes.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            incomes = incomes.Where(x => x.Title.Contains(keyword) || x.Category.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            incomes = incomes.Where(x => x.Category == query.Category);
        }

        var total = await incomes.CountAsync(cancellationToken);
        var items = await incomes
            .OrderByDescending(x => x.IncomeDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<IncomeDto>.Create(items.Select(ToIncomeDto).ToList(), total, page, pageSize);
    }

    public async Task<IncomeDto?> GetIncomeByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var income = await dbContext.Incomes.FindAsync(id, cancellationToken);
        return income is null ? null : ToIncomeDto(income);
    }

    public async Task<IncomeDto> CreateIncomeAsync(CreateIncomeDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var income = new Income
        {
            UserId = userId,
            IncomeDate = input.IncomeDate,
            Category = input.Category,
            Title = input.Title,
            Amount = input.Amount,
            Description = input.Description,
            Remark = input.Remark
        };

        dbContext.Incomes.Add(income);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToIncomeDto(income);
    }

    public async Task<IncomeDto?> UpdateIncomeAsync(Guid id, UpdateIncomeDto input, CancellationToken cancellationToken = default)
    {
        var income = await dbContext.Incomes.FindAsync(id, cancellationToken);
        if (income is null) return null;

        if (input.IncomeDate is not null) income.IncomeDate = input.IncomeDate;
        if (input.Category is not null) income.Category = input.Category;
        if (input.Title is not null) income.Title = input.Title;
        if (input.Amount.HasValue) income.Amount = input.Amount.Value;
        if (input.Description is not null) income.Description = input.Description;
        if (input.Remark is not null) income.Remark = input.Remark;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToIncomeDto(income);
    }

    public async Task<bool> DeleteIncomeAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var income = await dbContext.Incomes.FindAsync(id, cancellationToken);
        if (income is null) return false;

        dbContext.Incomes.Remove(income);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PageResult<ExpenseDto>> GetExpensePageAsync(AssetQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var expenses = dbContext.Expenses.AsNoTracking();

        if (userId.HasValue)
        {
            expenses = expenses.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            expenses = expenses.Where(x => x.Title.Contains(keyword) || x.Category.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            expenses = expenses.Where(x => x.Category == query.Category);
        }

        var total = await expenses.CountAsync(cancellationToken);
        var items = await expenses
            .OrderByDescending(x => x.ExpenseDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<ExpenseDto>.Create(items.Select(ToExpenseDto).ToList(), total, page, pageSize);
    }

    public async Task<ExpenseDto?> GetExpenseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var expense = await dbContext.Expenses.FindAsync(id, cancellationToken);
        return expense is null ? null : ToExpenseDto(expense);
    }

    public async Task<ExpenseDto> CreateExpenseAsync(CreateExpenseDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var expense = new Expense
        {
            UserId = userId,
            ExpenseDate = input.ExpenseDate,
            Category = input.Category,
            Title = input.Title,
            Amount = input.Amount,
            Description = input.Description,
            Remark = input.Remark
        };

        dbContext.Expenses.Add(expense);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToExpenseDto(expense);
    }

    public async Task<ExpenseDto?> UpdateExpenseAsync(Guid id, UpdateExpenseDto input, CancellationToken cancellationToken = default)
    {
        var expense = await dbContext.Expenses.FindAsync(id, cancellationToken);
        if (expense is null) return null;

        if (input.ExpenseDate is not null) expense.ExpenseDate = input.ExpenseDate;
        if (input.Category is not null) expense.Category = input.Category;
        if (input.Title is not null) expense.Title = input.Title;
        if (input.Amount.HasValue) expense.Amount = input.Amount.Value;
        if (input.Description is not null) expense.Description = input.Description;
        if (input.Remark is not null) expense.Remark = input.Remark;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToExpenseDto(expense);
    }

    public async Task<bool> DeleteExpenseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var expense = await dbContext.Expenses.FindAsync(id, cancellationToken);
        if (expense is null) return false;

        dbContext.Expenses.Remove(expense);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PageResult<BudgetDto>> GetBudgetPageAsync(AssetQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var budgets = dbContext.Budgets.AsNoTracking();

        if (userId.HasValue)
        {
            budgets = budgets.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            budgets = budgets.Where(x => x.Category.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            budgets = budgets.Where(x => x.Category == query.Category);
        }

        var total = await budgets.CountAsync(cancellationToken);
        var items = await budgets
            .OrderByDescending(x => x.Year).ThenByDescending(x => x.Month)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<BudgetDto>.Create(items.Select(ToBudgetDto).ToList(), total, page, pageSize);
    }

    public async Task<BudgetDto?> GetBudgetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var budget = await dbContext.Budgets.FindAsync(id, cancellationToken);
        return budget is null ? null : ToBudgetDto(budget);
    }

    public async Task<BudgetDto> CreateBudgetAsync(CreateBudgetDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var budget = new Budget
        {
            UserId = userId,
            Year = input.Year,
            Month = input.Month,
            Category = input.Category,
            PlannedAmount = input.PlannedAmount,
            ActualAmount = input.ActualAmount,
            Remark = input.Remark
        };

        dbContext.Budgets.Add(budget);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToBudgetDto(budget);
    }

    public async Task<BudgetDto?> UpdateBudgetAsync(Guid id, UpdateBudgetDto input, CancellationToken cancellationToken = default)
    {
        var budget = await dbContext.Budgets.FindAsync(id, cancellationToken);
        if (budget is null) return null;

        if (input.Year.HasValue) budget.Year = input.Year.Value;
        if (input.Month.HasValue) budget.Month = input.Month.Value;
        if (input.Category is not null) budget.Category = input.Category;
        if (input.PlannedAmount.HasValue) budget.PlannedAmount = input.PlannedAmount.Value;
        if (input.ActualAmount.HasValue) budget.ActualAmount = input.ActualAmount.Value;
        if (input.Remark is not null) budget.Remark = input.Remark;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToBudgetDto(budget);
    }

    public async Task<bool> DeleteBudgetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var budget = await dbContext.Budgets.FindAsync(id, cancellationToken);
        if (budget is null) return false;

        dbContext.Budgets.Remove(budget);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PageResult<InvestmentDto>> GetInvestmentPageAsync(AssetQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var investments = dbContext.Investments.AsNoTracking();

        if (userId.HasValue)
        {
            investments = investments.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            investments = investments.Where(x => x.Name.Contains(keyword) || x.Type.Contains(keyword));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
        {
            investments = investments.Where(x => x.Type == query.Category);
        }

        var total = await investments.CountAsync(cancellationToken);
        var items = await investments
            .OrderByDescending(x => x.InvestmentDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<InvestmentDto>.Create(items.Select(ToInvestmentDto).ToList(), total, page, pageSize);
    }

    public async Task<InvestmentDto?> GetInvestmentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var investment = await dbContext.Investments.FindAsync(id, cancellationToken);
        return investment is null ? null : ToInvestmentDto(investment);
    }

    public async Task<InvestmentDto> CreateInvestmentAsync(CreateInvestmentDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var investment = new Investment
        {
            UserId = userId,
            InvestmentDate = input.InvestmentDate,
            Type = input.Type,
            Name = input.Name,
            Amount = input.Amount,
            CurrentValue = input.CurrentValue,
            ReturnRate = input.ReturnRate,
            Description = input.Description,
            Remark = input.Remark
        };

        dbContext.Investments.Add(investment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToInvestmentDto(investment);
    }

    public async Task<InvestmentDto?> UpdateInvestmentAsync(Guid id, UpdateInvestmentDto input, CancellationToken cancellationToken = default)
    {
        var investment = await dbContext.Investments.FindAsync(id, cancellationToken);
        if (investment is null) return null;

        if (input.InvestmentDate is not null) investment.InvestmentDate = input.InvestmentDate;
        if (input.Type is not null) investment.Type = input.Type;
        if (input.Name is not null) investment.Name = input.Name;
        if (input.Amount.HasValue) investment.Amount = input.Amount.Value;
        if (input.CurrentValue.HasValue) investment.CurrentValue = input.CurrentValue.Value;
        if (input.ReturnRate.HasValue) investment.ReturnRate = input.ReturnRate.Value;
        if (input.Description is not null) investment.Description = input.Description;
        if (input.Remark is not null) investment.Remark = input.Remark;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToInvestmentDto(investment);
    }

    public async Task<bool> DeleteInvestmentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var investment = await dbContext.Investments.FindAsync(id, cancellationToken);
        if (investment is null) return false;

        dbContext.Investments.Remove(investment);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static IncomeDto ToIncomeDto(Income income) => new()
    {
        Id = income.Id.ToString(),
        UserId = income.UserId?.ToString(),
        IncomeDate = income.IncomeDate,
        Category = income.Category,
        Title = income.Title,
        Amount = income.Amount,
        Description = income.Description,
        Remark = income.Remark,
        CreatedAt = income.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
    };

    private static ExpenseDto ToExpenseDto(Expense expense) => new()
    {
        Id = expense.Id.ToString(),
        UserId = expense.UserId?.ToString(),
        ExpenseDate = expense.ExpenseDate,
        Category = expense.Category,
        Title = expense.Title,
        Amount = expense.Amount,
        Description = expense.Description,
        Remark = expense.Remark,
        CreatedAt = expense.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
    };

    private static BudgetDto ToBudgetDto(Budget budget) => new()
    {
        Id = budget.Id.ToString(),
        UserId = budget.UserId?.ToString(),
        Year = budget.Year,
        Month = budget.Month,
        Category = budget.Category,
        PlannedAmount = budget.PlannedAmount,
        ActualAmount = budget.ActualAmount,
        Remark = budget.Remark,
        CreatedAt = budget.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
    };

    private static InvestmentDto ToInvestmentDto(Investment investment) => new()
    {
        Id = investment.Id.ToString(),
        UserId = investment.UserId?.ToString(),
        InvestmentDate = investment.InvestmentDate,
        Type = investment.Type,
        Name = investment.Name,
        Amount = investment.Amount,
        CurrentValue = investment.CurrentValue,
        ReturnRate = investment.ReturnRate,
        Description = investment.Description,
        Remark = investment.Remark,
        CreatedAt = investment.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
    };
}
