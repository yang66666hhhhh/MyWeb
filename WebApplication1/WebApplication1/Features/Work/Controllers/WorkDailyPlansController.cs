using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Shared.Common;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services.Interfaces;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Authorize]
[Route("api/work/daily-plans")]
public class WorkDailyPlansController(IWorkDailyPlanService dailyPlanService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<WorkDailyPlanDto>>>> GetPage(
        [FromQuery] WorkDailyPlanQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await dailyPlanService.GetPageAsync(query, cancellationToken);
        return Ok(ApiResult<PageResult<WorkDailyPlanDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkDailyPlanDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await dailyPlanService.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound(ApiResult<WorkDailyPlanDto>.Fail("工作计划不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult<WorkDailyPlanDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<WorkDailyPlanDto>>> Create(
        [FromBody] CreateWorkDailyPlanDto input,
        CancellationToken cancellationToken)
    {
        var result = await dailyPlanService.CreateAsync(input, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<WorkDailyPlanDto>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkDailyPlanDto>>> Update(
        Guid id,
        [FromBody] UpdateWorkDailyPlanDto input,
        CancellationToken cancellationToken)
    {
        var result = await dailyPlanService.UpdateAsync(id, input, cancellationToken);
        if (result is null)
        {
            return NotFound(ApiResult<WorkDailyPlanDto>.Fail("工作计划不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult<WorkDailyPlanDto>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await dailyPlanService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(ApiResult.Fail("工作计划不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult.Success("删除成功"));
    }

    [HttpPost("{id:guid}/complete")]
    public async Task<ActionResult<ApiResult<WorkDailyPlanDto>>> Complete(Guid id, CancellationToken cancellationToken)
    {
        var result = await dailyPlanService.CompleteAsync(id, cancellationToken);
        if (result is null)
        {
            return NotFound(ApiResult<WorkDailyPlanDto>.Fail("工作计划不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult<WorkDailyPlanDto>.Success(result, "已完成"));
    }

    [HttpPost("convert-to-log")]
    public async Task<ActionResult<ApiResult<object>>> ConvertToLog(
        [FromBody] ConvertToWorkLogDto input,
        CancellationToken cancellationToken)
    {
        var workLogId = await dailyPlanService.ConvertToWorkLogAsync(input, cancellationToken);
        if (!workLogId.HasValue)
        {
            return NotFound(ApiResult.Fail("工作计划不存在", StatusCodes.Status404NotFound));
        }

        return Ok(ApiResult<object>.Success(new { workLogId }, "已转换为工作日志"));
    }
}