using WebApplication1.Features.Assets.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Assets.Services.Interfaces;

public interface IAssetService
{
    Task<AssetSummaryDto> GetSummaryAsync(Guid? userId = null, CancellationToken cancellationToken = default);
    Task<PageResult<IncomeDto>> GetIncomePageAsync(AssetQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<IncomeDto?> GetIncomeByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IncomeDto> CreateIncomeAsync(CreateIncomeDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<IncomeDto?> UpdateIncomeAsync(Guid id, UpdateIncomeDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteIncomeAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PageResult<ExpenseDto>> GetExpensePageAsync(AssetQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<ExpenseDto?> GetExpenseByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ExpenseDto> CreateExpenseAsync(CreateExpenseDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<ExpenseDto?> UpdateExpenseAsync(Guid id, UpdateExpenseDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteExpenseAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PageResult<BudgetDto>> GetBudgetPageAsync(AssetQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<BudgetDto?> GetBudgetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BudgetDto> CreateBudgetAsync(CreateBudgetDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<BudgetDto?> UpdateBudgetAsync(Guid id, UpdateBudgetDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteBudgetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PageResult<InvestmentDto>> GetInvestmentPageAsync(AssetQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default);
    Task<InvestmentDto?> GetInvestmentByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InvestmentDto> CreateInvestmentAsync(CreateInvestmentDto input, Guid userId, CancellationToken cancellationToken = default);
    Task<InvestmentDto?> UpdateInvestmentAsync(Guid id, UpdateInvestmentDto input, CancellationToken cancellationToken = default);
    Task<bool> DeleteInvestmentAsync(Guid id, CancellationToken cancellationToken = default);
}
