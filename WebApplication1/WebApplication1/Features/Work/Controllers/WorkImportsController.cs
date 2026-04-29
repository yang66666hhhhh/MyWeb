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

    [HttpPost("preview")]
    public async Task<ActionResult<ApiResult<WorkImportPreviewResultDto>>> Preview(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResult.Fail("请上传文件"));

        using var stream = file.OpenReadStream();
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        var result = await importService.PreviewAsync(memoryStream, file.FileName, cancellationToken);
        return Ok(ApiResult<WorkImportPreviewResultDto>.Success(result));
    }

    [HttpPost("execute")]
    public async Task<ActionResult<ApiResult<WorkImportConfirmResultDto>>> Execute(
        [FromBody] WorkImportConfirmDto input,
        CancellationToken cancellationToken)
    {
        var result = await importService.ExecuteAsync(input, cancellationToken);
        return Ok(ApiResult<WorkImportConfirmResultDto>.Success(result, "导入完成"));
    }

    [HttpGet("template")]
    public ActionResult<FileStreamResult> GetTemplate()
    {
        var template = importService.GenerateTemplate();
        var stream = new MemoryStream(template);
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "工作日志导入模板.xlsx");
    }
}