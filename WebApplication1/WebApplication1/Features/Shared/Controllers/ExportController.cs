using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Shared.Dtos;
using WebApplication1.Features.Shared.Services;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Shared.Controllers;

[ApiController]
[Authorize]
[Route("api/export")]
[Tags("Export")]
public class ExportController(IExportService exportService, ILogger<ExportController> logger) : BaseApiController
{
    [HttpGet("tasks")]
    public async Task<IActionResult> ExportTasks(
        [FromQuery] ExportQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var fileBytes = await exportService.ExportTasksAsync(query, userId, cancellationToken);
            var (contentType, fileName) = GetFileInfo(query.Format, "任务列表");
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "导出任务失败");
            return StatusCode(500, ApiResult.Fail("导出失败，请稍后重试"));
        }
    }

    [HttpGet("worklogs")]
    public async Task<IActionResult> ExportWorkLogs(
        [FromQuery] ExportQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var fileBytes = await exportService.ExportWorkLogsAsync(query, userId, cancellationToken);
            var (contentType, fileName) = GetFileInfo(query.Format, "工作日志");
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "导出工作日志失败");
            return StatusCode(500, ApiResult.Fail("导出失败，请稍后重试"));
        }
    }

    [HttpGet("habits")]
    public async Task<IActionResult> ExportHabits(
        [FromQuery] ExportQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var fileBytes = await exportService.ExportHabitsAsync(query, userId, cancellationToken);
            var (contentType, fileName) = GetFileInfo(query.Format, "习惯记录");
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "导出习惯失败");
            return StatusCode(500, ApiResult.Fail("导出失败，请稍后重试"));
        }
    }

    [HttpGet("income")]
    public async Task<IActionResult> ExportIncome(
        [FromQuery] ExportQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var fileBytes = await exportService.ExportIncomeAsync(query, userId, cancellationToken);
            var (contentType, fileName) = GetFileInfo(query.Format, "收入记录");
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "导出收入失败");
            return StatusCode(500, ApiResult.Fail("导出失败，请稍后重试"));
        }
    }

    [HttpGet("expense")]
    public async Task<IActionResult> ExportExpense(
        [FromQuery] ExportQueryDto query,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var fileBytes = await exportService.ExportExpenseAsync(query, userId, cancellationToken);
            var (contentType, fileName) = GetFileInfo(query.Format, "支出记录");
            return File(fileBytes, contentType, fileName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "导出支出失败");
            return StatusCode(500, ApiResult.Fail("导出失败，请稍后重试"));
        }
    }

    private static (string ContentType, string FileName) GetFileInfo(ExportFormat format, string baseName)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return format switch
        {
            ExportFormat.Excel => ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{baseName}_{timestamp}.xlsx"),
            ExportFormat.Csv => ("text/csv", $"{baseName}_{timestamp}.csv"),
            ExportFormat.Json => ("application/json", $"{baseName}_{timestamp}.json"),
            _ => ("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{baseName}_{timestamp}.xlsx")
        };
    }
}
