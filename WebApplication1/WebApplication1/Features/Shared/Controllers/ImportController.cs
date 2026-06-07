using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Shared.Dtos;
using WebApplication1.Features.Shared.Services;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Shared.Controllers;

[ApiController]
[Authorize]
[Route("api/import")]
[RequestSizeLimit(10 * 1024 * 1024)]
[Tags("Import")]
public class ImportController(IImportService importService, ILogger<ImportController> logger) : BaseApiController
{
    [HttpPost("tasks/preview")]
    public async Task<ActionResult<ApiResult<ImportPreviewDto>>> PreviewTasks(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResult.Fail("请上传文件"));

        try
        {
            using var stream = file.OpenReadStream();
            var result = await importService.PreviewTasksAsync(stream, file.FileName, cancellationToken);
            return Ok(ApiResult<ImportPreviewDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "预览任务导入失败");
            return StatusCode(500, ApiResult.Fail("预览失败，请检查文件格式"));
        }
    }

    [HttpPost("tasks/execute")]
    public async Task<ActionResult<ApiResult<ImportResultDto>>> ImportTasks(
        [FromBody] List<Dictionary<string, object?>> rows,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await importService.ImportTasksAsync(rows, userId.Value, cancellationToken);
            return Ok(ApiResult<ImportResultDto>.Success(result, "导入完成"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "导入任务失败");
            return StatusCode(500, ApiResult.Fail("导入失败，请稍后重试"));
        }
    }

    [HttpPost("worklogs/preview")]
    public async Task<ActionResult<ApiResult<ImportPreviewDto>>> PreviewWorkLogs(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResult.Fail("请上传文件"));

        try
        {
            using var stream = file.OpenReadStream();
            var result = await importService.PreviewWorkLogsAsync(stream, file.FileName, cancellationToken);
            return Ok(ApiResult<ImportPreviewDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "预览工作日志导入失败");
            return StatusCode(500, ApiResult.Fail("预览失败，请检查文件格式"));
        }
    }

    [HttpPost("worklogs/execute")]
    public async Task<ActionResult<ApiResult<ImportResultDto>>> ImportWorkLogs(
        [FromBody] List<Dictionary<string, object?>> rows,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await importService.ImportWorkLogsAsync(rows, userId.Value, cancellationToken);
            return Ok(ApiResult<ImportResultDto>.Success(result, "导入完成"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "导入工作日志失败");
            return StatusCode(500, ApiResult.Fail("导入失败，请稍后重试"));
        }
    }

    [HttpPost("habits/preview")]
    public async Task<ActionResult<ApiResult<ImportPreviewDto>>> PreviewHabits(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResult.Fail("请上传文件"));

        try
        {
            using var stream = file.OpenReadStream();
            var result = await importService.PreviewHabitsAsync(stream, file.FileName, cancellationToken);
            return Ok(ApiResult<ImportPreviewDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "预览习惯导入失败");
            return StatusCode(500, ApiResult.Fail("预览失败，请检查文件格式"));
        }
    }

    [HttpPost("habits/execute")]
    public async Task<ActionResult<ApiResult<ImportResultDto>>> ImportHabits(
        [FromBody] List<Dictionary<string, object?>> rows,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await importService.ImportHabitsAsync(rows, userId.Value, cancellationToken);
            return Ok(ApiResult<ImportResultDto>.Success(result, "导入完成"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "导入习惯失败");
            return StatusCode(500, ApiResult.Fail("导入失败，请稍后重试"));
        }
    }

    [HttpPost("income/preview")]
    public async Task<ActionResult<ApiResult<ImportPreviewDto>>> PreviewIncome(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResult.Fail("请上传文件"));

        try
        {
            using var stream = file.OpenReadStream();
            var result = await importService.PreviewIncomeAsync(stream, file.FileName, cancellationToken);
            return Ok(ApiResult<ImportPreviewDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "预览收入导入失败");
            return StatusCode(500, ApiResult.Fail("预览失败，请检查文件格式"));
        }
    }

    [HttpPost("income/execute")]
    public async Task<ActionResult<ApiResult<ImportResultDto>>> ImportIncome(
        [FromBody] List<Dictionary<string, object?>> rows,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await importService.ImportIncomeAsync(rows, userId.Value, cancellationToken);
            return Ok(ApiResult<ImportResultDto>.Success(result, "导入完成"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "导入收入失败");
            return StatusCode(500, ApiResult.Fail("导入失败，请稍后重试"));
        }
    }

    [HttpPost("expense/preview")]
    public async Task<ActionResult<ApiResult<ImportPreviewDto>>> PreviewExpense(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResult.Fail("请上传文件"));

        try
        {
            using var stream = file.OpenReadStream();
            var result = await importService.PreviewExpenseAsync(stream, file.FileName, cancellationToken);
            return Ok(ApiResult<ImportPreviewDto>.Success(result));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "预览支出导入失败");
            return StatusCode(500, ApiResult.Fail("预览失败，请检查文件格式"));
        }
    }

    [HttpPost("expense/execute")]
    public async Task<ActionResult<ApiResult<ImportResultDto>>> ImportExpense(
        [FromBody] List<Dictionary<string, object?>> rows,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await importService.ImportExpenseAsync(rows, userId.Value, cancellationToken);
            return Ok(ApiResult<ImportResultDto>.Success(result, "导入完成"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "导入支出失败");
            return StatusCode(500, ApiResult.Fail("导入失败，请稍后重试"));
        }
    }
}
