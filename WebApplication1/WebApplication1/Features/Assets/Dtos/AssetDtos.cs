using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Assets.Dtos;

public class AssetSummaryDto
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal TotalInvestment { get; set; }
    public decimal NetAsset { get; set; }
    public int IncomeCount { get; set; }
    public int ExpenseCount { get; set; }
    public int InvestmentCount { get; set; }
}

public class IncomeDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string IncomeDate { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? Remark { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class ExpenseDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string ExpenseDate { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? Remark { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class BudgetDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal PlannedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public decimal RemainingAmount => PlannedAmount - ActualAmount;
    public string? Remark { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class InvestmentDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string InvestmentDate { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? CurrentValue { get; set; }
    public decimal? ReturnRate { get; set; }
    public string? Description { get; set; }
    public string? Remark { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}

public class AssetQueryDto : PageQueryDto
{
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? Category { get; set; }
    public string? Keyword { get; set; }
}
