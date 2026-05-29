using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Shared.Common;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Services;

namespace WebApplication1.Features.Work.Controllers;

[ApiController]
[Authorize]
[RequireFeature("WORK_LOG")]
[Route("api/work/categories")]
[Tags("Work - Categories")]
public class WorkCategoryController : BaseApiController
{
    private readonly IWorkCategoryService _categoryService;

    public WorkCategoryController(IWorkCategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<PageResult<WorkCategoryDto>>>> GetPage(
        [FromQuery] WorkCategoryQueryDto query,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetPageAsync(query, cancellationToken);
        return Ok(ApiResult<PageResult<WorkCategoryDto>>.Success(result));
    }

    [HttpGet("tree")]
    public async Task<ActionResult<ApiResult<List<WorkCategoryDto>>>> GetTree(
        [FromQuery] bool? isActive = true,
        CancellationToken cancellationToken = default)
    {
        var result = await _categoryService.GetTreeAsync(isActive, cancellationToken);
        return Ok(ApiResult<List<WorkCategoryDto>>.Success(result));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkCategoryDto>>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return NotFound(ApiResult<WorkCategoryDto>.Fail("分类不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WorkCategoryDto>.Success(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResult<WorkCategoryDto>>> Create(
        [FromBody] CreateWorkCategoryDto input,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.CreateAsync(input, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResult<WorkCategoryDto>.Success(result, "创建成功"));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<WorkCategoryDto>>> Update(
        Guid id,
        [FromBody] UpdateWorkCategoryDto input,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.UpdateAsync(id, input, cancellationToken);
        if (result == null)
            return NotFound(ApiResult<WorkCategoryDto>.Fail("分类不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<WorkCategoryDto>.Success(result, "更新成功"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult>> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _categoryService.DeleteAsync(id, cancellationToken);
            if (!deleted)
                return NotFound(ApiResult.Fail("分类不存在", StatusCodes.Status404NotFound));
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResult.Fail(ex.Message));
        }
    }
}
