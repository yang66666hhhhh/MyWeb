using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Features.Growth.Services.Interfaces;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Controllers;

[ApiController]
[Authorize]
[Route("api/postgraduate")]
public class PostgraduateController(
    IPostgraduateTaskService taskService,
    IExamMistakeService mistakeService,
    IExamMaterialService materialService) : ControllerBase
{
    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private bool IsAdmin()
    {
        return User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Any(c => c.Value.Equals("admin", StringComparison.OrdinalIgnoreCase) ||
                       c.Value.Equals("super", StringComparison.OrdinalIgnoreCase));
    }

    [HttpGet("tasks")]
    public async Task<ActionResult<ApiResult<PageResult<PostgraduateTaskDto>>>> GetTasks(
        [FromQuery] PostgraduateTaskQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = IsAdmin() ? null : GetUserId();
        var result = await taskService.GetPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<PostgraduateTaskDto>>.Success(result));
    }

    [HttpGet("tasks/{id:guid}")]
    public async Task<ActionResult<ApiResult<PostgraduateTaskDto>>> GetTaskById(Guid id, CancellationToken cancellationToken)
    {
        var result = await taskService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<PostgraduateTaskDto>.Fail("任务不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<PostgraduateTaskDto>.Success(result));
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResult<ExamDashboardDto>>> GetDashboard(CancellationToken cancellationToken)
    {
        var userId = IsAdmin() ? null : GetUserId();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekStart = today.AddDays(-(int)today.DayOfWeek);

        var taskQuery = new PostgraduateTaskQueryDto { Page = 1, PageSize = 100 };
        var tasksResult = await taskService.GetPageAsync(taskQuery, userId, cancellationToken);

        var mistakeQuery = new ExamMistakeQueryDto { Page = 1, PageSize = 1 };
        var mistakesResult = await mistakeService.GetPageAsync(mistakeQuery, userId, cancellationToken);

        var materialQuery = new ExamMaterialQueryDto { Page = 1, PageSize = 1 };
        var materialsResult = await materialService.GetPageAsync(materialQuery, userId, cancellationToken);

        var todayTasks = tasksResult.Items
            .Where(t => t.DueDate.HasValue && t.DueDate.Value <= today && t.Status != 2)
            .Take(5)
            .ToList();

        var reviewTaskCount = tasksResult.Items.Count(t => t.Status == 1);

        var dashboard = new ExamDashboardDto
        {
            TodayTasks = todayTasks,
            WeeklyHours = 0,
            MistakeCount = mistakesResult.Total,
            MaterialCount = materialsResult.Total,
            ReviewTaskCount = reviewTaskCount,
            Subjects = new List<SubjectProgressDto>
            {
                new() { Id = "1", Name = "数据结构", Progress = 24, WeeklyHours = 5, TargetHours = 120, Color = "blue" },
                new() { Id = "2", Name = "数学", Progress = 12, WeeklyHours = 4, TargetHours = 180, Color = "cyan" },
                new() { Id = "3", Name = "英语", Progress = 18, WeeklyHours = 3, TargetHours = 90, Color = "green" },
                new() { Id = "4", Name = "政治", Progress = 5, WeeklyHours = 1.5, TargetHours = 80, Color = "purple" }
            },
            RecentRecords = new List<StudyRecordDto>()
        };

        return Ok(ApiResult<ExamDashboardDto>.Success(dashboard));
    }

    [HttpPost("tasks")]
    public async Task<ActionResult<ApiResult<PostgraduateTaskDto>>> CreateTask(
        [FromBody] CreatePostgraduateTaskDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await taskService.CreateAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetTaskById), new { id = result.Id }, ApiResult<PostgraduateTaskDto>.Success(result, "创建成功"));
    }

    [HttpPut("tasks/{id:guid}")]
    public async Task<ActionResult<ApiResult<PostgraduateTaskDto>>> UpdateTask(
        Guid id,
        [FromBody] UpdatePostgraduateTaskDto input,
        CancellationToken cancellationToken)
    {
        var result = await taskService.UpdateAsync(id, input, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<PostgraduateTaskDto>.Fail("任务不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<PostgraduateTaskDto>.Success(result, "更新成功"));
    }

    [HttpDelete("tasks/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteTask(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await taskService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("任务不存在"));
        return Ok(ApiResult.Success("删除成功"));
    }

    [HttpGet("mistakes")]
    public async Task<ActionResult<ApiResult<PageResult<ExamMistakeDto>>>> GetMistakes(
        [FromQuery] ExamMistakeQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = IsAdmin() ? null : GetUserId();
        var result = await mistakeService.GetPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<ExamMistakeDto>>.Success(result));
    }

    [HttpGet("mistakes/{id:guid}")]
    public async Task<ActionResult<ApiResult<ExamMistakeDto>>> GetMistakeById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mistakeService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<ExamMistakeDto>.Fail("错题不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<ExamMistakeDto>.Success(result));
    }

    [HttpPost("mistakes")]
    public async Task<ActionResult<ApiResult<ExamMistakeDto>>> CreateMistake(
        [FromBody] CreateExamMistakeDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await mistakeService.CreateAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetMistakeById), new { id = result.Id }, ApiResult<ExamMistakeDto>.Success(result, "创建成功"));
    }

    [HttpPut("mistakes/{id:guid}")]
    public async Task<ActionResult<ApiResult<ExamMistakeDto>>> UpdateMistake(
        Guid id,
        [FromBody] UpdateExamMistakeDto input,
        CancellationToken cancellationToken)
    {
        var result = await mistakeService.UpdateAsync(id, input, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<ExamMistakeDto>.Fail("错题不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<ExamMistakeDto>.Success(result, "更新成功"));
    }

    [HttpDelete("mistakes/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteMistake(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await mistakeService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("错题不存在"));
        return Ok(ApiResult.Success("删除成功"));
    }

    [HttpGet("materials")]
    public async Task<ActionResult<ApiResult<PageResult<ExamMaterialDto>>>> GetMaterials(
        [FromQuery] ExamMaterialQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = IsAdmin() ? null : GetUserId();
        var result = await materialService.GetPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<ExamMaterialDto>>.Success(result));
    }

    [HttpGet("materials/{id:guid}")]
    public async Task<ActionResult<ApiResult<ExamMaterialDto>>> GetMaterialById(Guid id, CancellationToken cancellationToken)
    {
        var result = await materialService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<ExamMaterialDto>.Fail("资料不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<ExamMaterialDto>.Success(result));
    }

    [HttpPost("materials")]
    public async Task<ActionResult<ApiResult<ExamMaterialDto>>> CreateMaterial(
        [FromBody] CreateExamMaterialDto input,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await materialService.CreateAsync(input, userId.Value, cancellationToken);
        return CreatedAtAction(nameof(GetMaterialById), new { id = result.Id }, ApiResult<ExamMaterialDto>.Success(result, "创建成功"));
    }

    [HttpPut("materials/{id:guid}")]
    public async Task<ActionResult<ApiResult<ExamMaterialDto>>> UpdateMaterial(
        Guid id,
        [FromBody] UpdateExamMaterialDto input,
        CancellationToken cancellationToken)
    {
        var result = await materialService.UpdateAsync(id, input, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<ExamMaterialDto>.Fail("资料不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<ExamMaterialDto>.Success(result, "更新成功"));
    }

    [HttpDelete("materials/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteMaterial(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await materialService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound(ApiResult.Fail("资料不存在"));
        return Ok(ApiResult.Success("删除成功"));
    }
}