using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Assets.Dtos;
using WebApplication1.Features.Assets.Services.Interfaces;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Assets.Controllers;

[ApiController]
[Authorize]
[RequireFeature("ASSET_DASHBOARD")]
[Route("api/assets")]
[Tags("Assets")]
public class AssetsController(IAssetService assetService, ILogger<AssetsController> logger) : BaseApiController
{
    [HttpGet("summary")]
    public async Task<ActionResult<ApiResult<AssetSummaryDto>>> GetSummary(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await assetService.GetSummaryAsync(userId, cancellationToken);
            return Ok(ApiResult<AssetSummaryDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取资产概览失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("incomes")]
    public async Task<ActionResult<ApiResult<PageResult<IncomeDto>>>> GetIncomePage(
        [FromQuery] AssetQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await assetService.GetIncomePageAsync(query, userId, cancellationToken);
            return Ok(ApiResult<PageResult<IncomeDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取收入列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("incomes/{id:guid}")]
    public async Task<ActionResult<ApiResult<IncomeDto>>> GetIncomeById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await assetService.GetIncomeByIdAsync(id, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<IncomeDto>.Fail("收入记录不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
                return NotFound(ApiResult<IncomeDto>.Fail("收入记录不存在", StatusCodes.Status404NotFound));

            return Ok(ApiResult<IncomeDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取收入详情失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpPost("incomes")]
    public async Task<ActionResult<ApiResult<IncomeDto>>> CreateIncome(
        [FromBody] CreateIncomeDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await assetService.CreateIncomeAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建收入记录成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetIncomeById), new { id = result.Id }, ApiResult<IncomeDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建收入记录失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("incomes/{id:guid}")]
    public async Task<ActionResult<ApiResult<IncomeDto>>> UpdateIncome(
        Guid id,
        [FromBody] UpdateIncomeDto input,
        CancellationToken cancellationToken)
    {
        try
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
            logger.LogInformation("更新收入记录成功: {Id}", id);
            return Ok(ApiResult<IncomeDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新收入记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("incomes/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteIncome(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await assetService.GetIncomeByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult.Fail("收入记录不存在"));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此记录"));

            var deleted = await assetService.DeleteIncomeAsync(id, cancellationToken);
            logger.LogInformation("删除收入记录成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除收入记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("expenses")]
    public async Task<ActionResult<ApiResult<PageResult<ExpenseDto>>>> GetExpensePage(
        [FromQuery] AssetQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await assetService.GetExpensePageAsync(query, userId, cancellationToken);
            return Ok(ApiResult<PageResult<ExpenseDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取支出列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("expenses/{id:guid}")]
    public async Task<ActionResult<ApiResult<ExpenseDto>>> GetExpenseById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await assetService.GetExpenseByIdAsync(id, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<ExpenseDto>.Fail("支出记录不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
                return NotFound(ApiResult<ExpenseDto>.Fail("支出记录不存在", StatusCodes.Status404NotFound));

            return Ok(ApiResult<ExpenseDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取支出详情失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpPost("expenses")]
    public async Task<ActionResult<ApiResult<ExpenseDto>>> CreateExpense(
        [FromBody] CreateExpenseDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await assetService.CreateExpenseAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建支出记录成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetExpenseById), new { id = result.Id }, ApiResult<ExpenseDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建支出记录失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("expenses/{id:guid}")]
    public async Task<ActionResult<ApiResult<ExpenseDto>>> UpdateExpense(
        Guid id,
        [FromBody] UpdateExpenseDto input,
        CancellationToken cancellationToken)
    {
        try
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
            logger.LogInformation("更新支出记录成功: {Id}", id);
            return Ok(ApiResult<ExpenseDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新支出记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("expenses/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteExpense(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await assetService.GetExpenseByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult.Fail("支出记录不存在"));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此记录"));

            var deleted = await assetService.DeleteExpenseAsync(id, cancellationToken);
            logger.LogInformation("删除支出记录成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除支出记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("budgets")]
    public async Task<ActionResult<ApiResult<PageResult<BudgetDto>>>> GetBudgetPage(
        [FromQuery] AssetQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await assetService.GetBudgetPageAsync(query, userId, cancellationToken);
            return Ok(ApiResult<PageResult<BudgetDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取预算列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("budgets/{id:guid}")]
    public async Task<ActionResult<ApiResult<BudgetDto>>> GetBudgetById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await assetService.GetBudgetByIdAsync(id, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<BudgetDto>.Fail("预算记录不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
                return NotFound(ApiResult<BudgetDto>.Fail("预算记录不存在", StatusCodes.Status404NotFound));

            return Ok(ApiResult<BudgetDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取预算详情失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpPost("budgets")]
    public async Task<ActionResult<ApiResult<BudgetDto>>> CreateBudget(
        [FromBody] CreateBudgetDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await assetService.CreateBudgetAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建预算记录成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetBudgetById), new { id = result.Id }, ApiResult<BudgetDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建预算记录失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("budgets/{id:guid}")]
    public async Task<ActionResult<ApiResult<BudgetDto>>> UpdateBudget(
        Guid id,
        [FromBody] UpdateBudgetDto input,
        CancellationToken cancellationToken)
    {
        try
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
            logger.LogInformation("更新预算记录成功: {Id}", id);
            return Ok(ApiResult<BudgetDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新预算记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("budgets/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteBudget(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await assetService.GetBudgetByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult.Fail("预算记录不存在"));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此记录"));

            var deleted = await assetService.DeleteBudgetAsync(id, cancellationToken);
            logger.LogInformation("删除预算记录成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除预算记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("investments")]
    public async Task<ActionResult<ApiResult<PageResult<InvestmentDto>>>> GetInvestmentPage(
        [FromQuery] AssetQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await assetService.GetInvestmentPageAsync(query, userId, cancellationToken);
            return Ok(ApiResult<PageResult<InvestmentDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取投资列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("investments/{id:guid}")]
    public async Task<ActionResult<ApiResult<InvestmentDto>>> GetInvestmentById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await assetService.GetInvestmentByIdAsync(id, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<InvestmentDto>.Fail("投资记录不存在", StatusCodes.Status404NotFound));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
                return NotFound(ApiResult<InvestmentDto>.Fail("投资记录不存在", StatusCodes.Status404NotFound));

            return Ok(ApiResult<InvestmentDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取投资详情失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpPost("investments")]
    public async Task<ActionResult<ApiResult<InvestmentDto>>> CreateInvestment(
        [FromBody] CreateInvestmentDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await assetService.CreateInvestmentAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建投资记录成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetInvestmentById), new { id = result.Id }, ApiResult<InvestmentDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建投资记录失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("investments/{id:guid}")]
    public async Task<ActionResult<ApiResult<InvestmentDto>>> UpdateInvestment(
        Guid id,
        [FromBody] UpdateInvestmentDto input,
        CancellationToken cancellationToken)
    {
        try
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
            logger.LogInformation("更新投资记录成功: {Id}", id);
            return Ok(ApiResult<InvestmentDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新投资记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("investments/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteInvestment(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await assetService.GetInvestmentByIdAsync(id, cancellationToken);
            if (existing is null)
                return NotFound(ApiResult.Fail("投资记录不存在"));

            var currentUserId = GetCurrentUserId();
            if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此记录"));

            var deleted = await assetService.DeleteInvestmentAsync(id, cancellationToken);
            logger.LogInformation("删除投资记录成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除投资记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("charts/income-trend")]
    public async Task<ActionResult<ApiResult<List<MonthlyTrendDto>>>> GetIncomeTrend(
        [FromQuery] int months = 6,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await assetService.GetIncomeTrendAsync(userId, months, cancellationToken);
            return Ok(ApiResult<List<MonthlyTrendDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取收入趋势数据失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("charts/expense-trend")]
    public async Task<ActionResult<ApiResult<List<MonthlyTrendDto>>>> GetExpenseTrend(
        [FromQuery] int months = 6,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await assetService.GetExpenseTrendAsync(userId, months, cancellationToken);
            return Ok(ApiResult<List<MonthlyTrendDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取支出趋势数据失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("charts/expense-category")]
    public async Task<ActionResult<ApiResult<List<CategoryStatDto>>>> GetExpenseCategoryStats(
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await assetService.GetExpenseCategoryStatsAsync(userId, cancellationToken);
            return Ok(ApiResult<List<CategoryStatDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取支出分类统计数据失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("charts/budget-execution")]
    public async Task<ActionResult<ApiResult<List<BudgetExecutionDto>>>> GetBudgetExecution(
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await assetService.GetBudgetExecutionAsync(userId, cancellationToken);
            return Ok(ApiResult<List<BudgetExecutionDto>>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取预算执行数据失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("charts/asset-overview")]
    public async Task<ActionResult<ApiResult<AssetOverviewDto>>> GetAssetOverview(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await assetService.GetAssetOverviewAsync(userId, cancellationToken);
            return Ok(ApiResult<AssetOverviewDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取资产总览数据失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }
}
