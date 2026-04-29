using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Assets.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Assets.Controllers;

[ApiController]
[Route("api/assets")]
[Authorize]
public class AssetsController : ControllerBase
{
    [HttpGet("summary")]
    public ActionResult<ApiResult<AssetSummaryDto>> GetSummary()
    {
        var summary = new AssetSummaryDto
        {
            TotalIncome = 0,
            TotalExpense = 0,
            TotalInvestment = 0,
            NetAsset = 0,
            IncomeCount = 0,
            ExpenseCount = 0,
            InvestmentCount = 0
        };
        return Ok(ApiResult<AssetSummaryDto>.Success(summary));
    }

    [HttpGet("incomes")]
    public ActionResult<ApiResult<PageResult<IncomeDto>>> GetIncomes([FromQuery] AssetQueryDto query)
    {
        return Ok(ApiResult<PageResult<IncomeDto>>.Success(new PageResult<IncomeDto>
        {
            Items = new List<IncomeDto>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }

    [HttpGet("expenses")]
    public ActionResult<ApiResult<PageResult<ExpenseDto>>> GetExpenses([FromQuery] AssetQueryDto query)
    {
        return Ok(ApiResult<PageResult<ExpenseDto>>.Success(new PageResult<ExpenseDto>
        {
            Items = new List<ExpenseDto>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }

    [HttpGet("budgets")]
    public ActionResult<ApiResult<PageResult<BudgetDto>>> GetBudgets([FromQuery] AssetQueryDto query)
    {
        return Ok(ApiResult<PageResult<BudgetDto>>.Success(new PageResult<BudgetDto>
        {
            Items = new List<BudgetDto>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }

    [HttpGet("investments")]
    public ActionResult<ApiResult<PageResult<InvestmentDto>>> GetInvestments([FromQuery] AssetQueryDto query)
    {
        return Ok(ApiResult<PageResult<InvestmentDto>>.Success(new PageResult<InvestmentDto>
        {
            Items = new List<InvestmentDto>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }
}
