using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Shared.Common;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services.Interfaces;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Authorize]
[Route("api/work/import")]
public class WorkImportsController(IWorkImportService importService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<WorkImportBatchDto>>>> GetPage(
        [FromQuery] WorkImportBatchQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await importService.GetBatchPageAsync(query, cancellationToken);
        return Ok(ApiResult<PageResult<WorkImportBatchDto>>.Success(result));
    }

    [HttpPost("worklog/preview")]
    public async Task<ActionResult<ApiResult<WorkImportPreviewResultDto>>> PreviewWorkLog(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResult.Fail("请上传文件"));

        using var stream = file.OpenReadStream();
        var result = await importService.PreviewWorkLogAsync(stream, file.FileName, cancellationToken);
        return Ok(ApiResult<WorkImportPreviewResultDto>.Success(result));
    }

    [HttpPost("worklog/execute")]
    public async Task<ActionResult<ApiResult<WorkImportConfirmResultDto>>> ExecuteWorkLog(
        [FromBody] WorkImportConfirmDto input,
        CancellationToken cancellationToken)
    {
        var result = await importService.ExecuteWorkLogImportAsync(input, cancellationToken);
        return Ok(ApiResult<WorkImportConfirmResultDto>.Success(result, "导入完成"));
    }

    [HttpGet("worklog/template")]
    public ActionResult<FileStreamResult> GetWorkLogTemplate()
    {
        var template = importService.GenerateWorkLogTemplate();
        var stream = new MemoryStream(template);
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "工作日志导入模板.xlsx");
    }

    [HttpPost("project/preview")]
    public async Task<ActionResult<ApiResult<WorkImportPreviewResultDto>>> PreviewProject(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResult.Fail("请上传文件"));

        using var stream = file.OpenReadStream();
        var result = await importService.PreviewProjectAsync(stream, file.FileName, cancellationToken);
        return Ok(ApiResult<WorkImportPreviewResultDto>.Success(result));
    }

    [HttpPost("project/execute")]
    public async Task<ActionResult<ApiResult<WorkImportConfirmResultDto>>> ExecuteProject(
        [FromBody] WorkImportConfirmDto input,
        CancellationToken cancellationToken)
    {
        var result = await importService.ExecuteProjectImportAsync(input, cancellationToken);
        return Ok(ApiResult<WorkImportConfirmResultDto>.Success(result, "导入完成"));
    }

    [HttpGet("project/template")]
    public ActionResult<FileStreamResult> GetProjectTemplate()
    {
        var template = importService.GenerateProjectTemplate();
        var stream = new MemoryStream(template);
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "项目导入模板.xlsx");
    }

    [HttpPost("device/preview")]
    public async Task<ActionResult<ApiResult<WorkImportPreviewResultDto>>> PreviewDevice(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResult.Fail("请上传文件"));

        using var stream = file.OpenReadStream();
        var result = await importService.PreviewDeviceAsync(stream, file.FileName, cancellationToken);
        return Ok(ApiResult<WorkImportPreviewResultDto>.Success(result));
    }

    [HttpPost("device/execute")]
    public async Task<ActionResult<ApiResult<WorkImportConfirmResultDto>>> ExecuteDevice(
        [FromBody] WorkImportConfirmDto input,
        CancellationToken cancellationToken)
    {
        var result = await importService.ExecuteDeviceImportAsync(input, cancellationToken);
        return Ok(ApiResult<WorkImportConfirmResultDto>.Success(result, "导入完成"));
    }

    [HttpGet("device/template")]
    public ActionResult<FileStreamResult> GetDeviceTemplate()
    {
        var template = importService.GenerateDeviceTemplate();
        var stream = new MemoryStream(template);
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "设备导入模板.xlsx");
    }

    [HttpPost("tasktype/preview")]
    public async Task<ActionResult<ApiResult<WorkImportPreviewResultDto>>> PreviewTaskType(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResult.Fail("请上传文件"));

        using var stream = file.OpenReadStream();
        var result = await importService.PreviewTaskTypeAsync(stream, file.FileName, cancellationToken);
        return Ok(ApiResult<WorkImportPreviewResultDto>.Success(result));
    }

    [HttpPost("tasktype/execute")]
    public async Task<ActionResult<ApiResult<WorkImportConfirmResultDto>>> ExecuteTaskType(
        [FromBody] WorkImportConfirmDto input,
        CancellationToken cancellationToken)
    {
        var result = await importService.ExecuteTaskTypeImportAsync(input, cancellationToken);
        return Ok(ApiResult<WorkImportConfirmResultDto>.Success(result, "导入完成"));
    }

    [HttpGet("tasktype/template")]
    public ActionResult<FileStreamResult> GetTaskTypeTemplate()
    {
        var template = importService.GenerateTaskTypeTemplate();
        var stream = new MemoryStream(template);
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "任务类型导入模板.xlsx");
    }
}