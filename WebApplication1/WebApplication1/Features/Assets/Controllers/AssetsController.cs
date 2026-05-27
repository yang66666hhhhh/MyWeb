using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Assets.Dtos;
using WebApplication1.Features.Assets.Services.Interfaces;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Assets.Controllers;

[ApiController]
[Authorize]
[Route("api/assets")]
[Tags("Assets")]
public class AssetsController(IAssetService assetService) : BaseApiController
{
    [HttpGet("summary")]
    public async Task<ActionResult<ApiResult<AssetSummaryDto>>> GetSummary(CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await assetService.GetSummaryAsync(userId, cancellationToken);
        return Ok(ApiResult<AssetSummaryDto>.Success(result));
    }

    [HttpGet("incomes")]
    public async Task<ActionResult<ApiResult<PageResult<IncomeDto>>>> GetIncomePage(
        [FromQuery] AssetQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await assetService.GetIncomePageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<IncomeDto>>.Success(result));
    }

    [HttpGet("incomes/{id:guid}")]
    public async Task<ActionResult<ApiResult<IncomeDto>>> GetIncomeById(Guid id, CancellationToken cancellationToken)
    {
        var result = await assetService.GetIncomeByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<IncomeDto>.Fail("收入记录不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<IncomeDto>.Fail("收入记录不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<IncomeDto>.Success(result));
    }

    [HttpPost("incomes")]
    public async Task<ActionResult<ApiResult<IncomeDto>>> CreateIncome(
        [FromBody] CreateIncomeDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await assetService.CreateIncomeAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetIncomeById), new { id = result.Id }, ApiResult<IncomeDto>.Success(result, "创建成功"));
    }

    [HttpPut("incomes/{id:guid}")]
    public async Task<ActionResult<ApiResult<IncomeDto>>> UpdateIncome(
        Guid id,
        [FromBody] UpdateIncomeDto input,
        CancellationToken cancellationToken)
    {
        var existing = await assetService.GetIncomeByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult<IncomeDto>.Fail("收入记录不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此记录"));

        var result = await assetService.UpdateIncomeAsync(id, input, cancellationToken);
        if (result is null)
            return NotFound(ApiResult.Fail("收入记录不存在"));
        return Ok(ApiResult<IncomeDto>.Success(result, "更新成功"));
    }

    [HttpDelete("incomes/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteIncome(Guid id, CancellationToken cancellationToken)
    {
        var existing = await assetService.GetIncomeByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult.Fail("收入记录不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此记录"));

        var deleted = await assetService.DeleteIncomeAsync(id, cancellationToken);
        return Ok(ApiResult.Success("删除成功"));
    }

    [HttpGet("expenses")]
    public async Task<ActionResult<ApiResult<PageResult<ExpenseDto>>>> GetExpensePage(
        [FromQuery] AssetQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await assetService.GetExpensePageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<ExpenseDto>>.Success(result));
    }

    [HttpGet("expenses/{id:guid}")]
    public async Task<ActionResult<ApiResult<ExpenseDto>>> GetExpenseById(Guid id, CancellationToken cancellationToken)
    {
        var result = await assetService.GetExpenseByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<ExpenseDto>.Fail("支出记录不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<ExpenseDto>.Fail("支出记录不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<ExpenseDto>.Success(result));
    }

    [HttpPost("expenses")]
    public async Task<ActionResult<ApiResult<ExpenseDto>>> CreateExpense(
        [FromBody] CreateExpenseDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await assetService.CreateExpenseAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetExpenseById), new { id = result.Id }, ApiResult<ExpenseDto>.Success(result, "创建成功"));
    }

    [HttpPut("expenses/{id:guid}")]
    public async Task<ActionResult<ApiResult<ExpenseDto>>> UpdateExpense(
        Guid id,
        [FromBody] UpdateExpenseDto input,
        CancellationToken cancellationToken)
    {
        var existing = await assetService.GetExpenseByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult<ExpenseDto>.Fail("支出记录不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此记录"));

        var result = await assetService.UpdateExpenseAsync(id, input, cancellationToken);
        if (result is null)
            return NotFound(ApiResult.Fail("支出记录不存在"));
        return Ok(ApiResult<ExpenseDto>.Success(result, "更新成功"));
    }

    [HttpDelete("expenses/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteExpense(Guid id, CancellationToken cancellationToken)
    {
        var existing = await assetService.GetExpenseByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult.Fail("支出记录不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此记录"));

        var deleted = await assetService.DeleteExpenseAsync(id, cancellationToken);
        return Ok(ApiResult.Success("删除成功"));
    }

    [HttpGet("budgets")]
    public async Task<ActionResult<ApiResult<PageResult<BudgetDto>>>> GetBudgetPage(
        [FromQuery] AssetQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await assetService.GetBudgetPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<BudgetDto>>.Success(result));
    }

    [HttpGet("budgets/{id:guid}")]
    public async Task<ActionResult<ApiResult<BudgetDto>>> GetBudgetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await assetService.GetBudgetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<BudgetDto>.Fail("预算记录不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<BudgetDto>.Fail("预算记录不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<BudgetDto>.Success(result));
    }

    [HttpPost("budgets")]
    public async Task<ActionResult<ApiResult<BudgetDto>>> CreateBudget(
        [FromBody] CreateBudgetDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await assetService.CreateBudgetAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetBudgetById), new { id = result.Id }, ApiResult<BudgetDto>.Success(result, "创建成功"));
    }

    [HttpPut("budgets/{id:guid}")]
    public async Task<ActionResult<ApiResult<BudgetDto>>> UpdateBudget(
        Guid id,
        [FromBody] UpdateBudgetDto input,
        CancellationToken cancellationToken)
    {
        var existing = await assetService.GetBudgetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult<BudgetDto>.Fail("预算记录不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此记录"));

        var result = await assetService.UpdateBudgetAsync(id, input, cancellationToken);
        if (result is null)
            return NotFound(ApiResult.Fail("预算记录不存在"));
        return Ok(ApiResult<BudgetDto>.Success(result, "更新成功"));
    }

    [HttpDelete("budgets/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteBudget(Guid id, CancellationToken cancellationToken)
    {
        var existing = await assetService.GetBudgetByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult.Fail("预算记录不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此记录"));

        var deleted = await assetService.DeleteBudgetAsync(id, cancellationToken);
        return Ok(ApiResult.Success("删除成功"));
    }

    [HttpGet("investments")]
    public async Task<ActionResult<ApiResult<PageResult<InvestmentDto>>>> GetInvestmentPage(
        [FromQuery] AssetQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await assetService.GetInvestmentPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<InvestmentDto>>.Success(result));
    }

    [HttpGet("investments/{id:guid}")]
    public async Task<ActionResult<ApiResult<InvestmentDto>>> GetInvestmentById(Guid id, CancellationToken cancellationToken)
    {
        var result = await assetService.GetInvestmentByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<InvestmentDto>.Fail("投资记录不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<InvestmentDto>.Fail("投资记录不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<InvestmentDto>.Success(result));
    }

    [HttpPost("investments")]
    public async Task<ActionResult<ApiResult<InvestmentDto>>> CreateInvestment(
        [FromBody] CreateInvestmentDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await assetService.CreateInvestmentAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetInvestmentById), new { id = result.Id }, ApiResult<InvestmentDto>.Success(result, "创建成功"));
    }

    [HttpPut("investments/{id:guid}")]
    public async Task<ActionResult<ApiResult<InvestmentDto>>> UpdateInvestment(
        Guid id,
        [FromBody] UpdateInvestmentDto input,
        CancellationToken cancellationToken)
    {
        var existing = await assetService.GetInvestmentByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult<InvestmentDto>.Fail("投资记录不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此记录"));

        var result = await assetService.UpdateInvestmentAsync(id, input, cancellationToken);
        if (result is null)
            return NotFound(ApiResult.Fail("投资记录不存在"));
        return Ok(ApiResult<InvestmentDto>.Success(result, "更新成功"));
    }

    [HttpDelete("investments/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteInvestment(Guid id, CancellationToken cancellationToken)
    {
        var existing = await assetService.GetInvestmentByIdAsync(id, cancellationToken);
        if (existing is null)
            return NotFound(ApiResult.Fail("投资记录不存在"));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此记录"));

        var deleted = await assetService.DeleteInvestmentAsync(id, cancellationToken);
        return Ok(ApiResult.Success("删除成功"));
    }
}
