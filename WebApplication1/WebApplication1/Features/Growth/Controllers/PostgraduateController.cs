using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Features.Growth.Services.Interfaces;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Controllers;

[ApiController]
[Authorize]
[Route("api/student")]
[Tags("Student")]
public class PostgraduateController(
    IPostgraduateTaskService taskService,
    IExamMistakeService mistakeService,
    IExamMaterialService materialService,
    IStudentSubjectService subjectService,
    IStudentStudyRecordService recordService,
    ILogger<PostgraduateController> logger) : BaseApiController
{
    [RequireFeature("STUDENT_LEARNING")]
    [HttpGet("tasks")]
    public async Task<ActionResult<ApiResult<PageResult<PostgraduateTaskDto>>>> GetTasks(
        [FromQuery] PostgraduateTaskQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await taskService.GetPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<PostgraduateTaskDto>>.Success(result));
    }

    [RequireFeature("STUDENT_LEARNING")]
    [HttpGet("tasks/{id:guid}")]
    public async Task<ActionResult<ApiResult<PostgraduateTaskDto>>> GetTaskById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await taskService.GetByIdAsync(id, userId, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<PostgraduateTaskDto>.Fail("任务不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<PostgraduateTaskDto>.Success(result));
    }

    [RequireFeature("STUDENT_EXAM")]
    [HttpGet("dashboard")]
    public async Task<ActionResult<ApiResult<ExamDashboardDto>>> GetDashboard(CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekStart = today.AddDays(-(((int)today.DayOfWeek + 6) % 7));

        var taskQuery = new PostgraduateTaskQueryDto { Page = 1, PageSize = 100 };
        var tasksResult = await taskService.GetPageAsync(taskQuery, userId, cancellationToken);

        var mistakeQuery = new ExamMistakeQueryDto { Page = 1, PageSize = 100 };
        var mistakesResult = await mistakeService.GetPageAsync(mistakeQuery, userId, cancellationToken);

        var materialQuery = new ExamMaterialQueryDto { Page = 1, PageSize = 100 };
        var materialsResult = await materialService.GetPageAsync(materialQuery, userId, cancellationToken);

        var subjectQuery = new StudentSubjectQueryDto { Page = 1, PageSize = 100, IsActive = true };
        var subjectsResult = await subjectService.GetPageAsync(subjectQuery, userId, cancellationToken);

        var recordQuery = new StudentStudyRecordQueryDto { Page = 1, PageSize = 100, StartDate = weekStart, EndDate = today };
        var recordsResult = await recordService.GetPageAsync(recordQuery, userId, cancellationToken);

        var todayTasks = tasksResult.Items
            .Where(t => (!t.DueDate.HasValue || t.DueDate.Value <= today) && t.Status != 2)
            .Take(8)
            .ToList();

        var dueReviews = mistakesResult.Items
            .Where(x => x.Status != 2 && (!x.NextReviewDate.HasValue || x.NextReviewDate.Value <= today))
            .ToList();

        var subjectNames = subjectsResult.Items.Select(x => x.Name)
            .Concat(mistakesResult.Items.Select(x => x.Subject))
            .Concat(materialsResult.Items.Select(x => x.Subject))
            .Concat(recordsResult.Items.Select(x => x.Subject))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var subjects = subjectNames.Select(name =>
        {
            var subject = subjectsResult.Items.FirstOrDefault(x => SubjectEquals(x.Name, name));
            var weeklyMinutes = recordsResult.Items
                .Where(x => SubjectEquals(x.Subject, name))
                .Sum(x => x.DurationMinutes);
            var targetHours = Math.Max(subject?.TargetHours ?? 0, 1);
            var progress = Math.Min(100, (int)Math.Round(weeklyMinutes / 60.0 / targetHours * 100));

            return new SubjectProgressDto
            {
                Id = (subject?.Id.ToString()) ?? name,
                Name = name,
                Color = subject?.Color ?? "blue",
                TargetHours = subject?.TargetHours ?? 0,
                WeeklyHours = Math.Round(weeklyMinutes / 60.0, 1),
                Progress = progress,
                MistakeCount = mistakesResult.Items.Count(x => SubjectEquals(x.Subject, name) && x.Status != 2),
                MaterialCount = materialsResult.Items.Count(x => SubjectEquals(x.Subject, name))
            };
        }).OrderByDescending(x => x.MistakeCount).ThenBy(x => x.Name).ToList();

        var dashboard = new ExamDashboardDto
        {
            TodayTasks = todayTasks,
            WeeklyHours = Math.Round(recordsResult.Items.Sum(x => x.DurationMinutes) / 60.0, 1),
            MistakeCount = mistakesResult.Total,
            MaterialCount = materialsResult.Total,
            ReviewTaskCount = tasksResult.Items.Count(t => t.Status == 1),
            PendingTaskCount = tasksResult.Items.Count(t => t.Status != 2),
            TodayReviewCount = dueReviews.Count,
            OverdueTaskCount = tasksResult.Items.Count(t => t.DueDate.HasValue && t.DueDate.Value < today && t.Status != 2),
            SubjectCount = subjects.Count,
            Subjects = subjects,
            RecentRecords = recordsResult.Items
                .OrderByDescending(x => x.RecordDate)
                .ThenByDescending(x => x.CreatedAt)
                .Take(10)
                .Select(x => new StudyRecordDto
                {
                    Id = x.Id.ToString(),
                    Subject = NormalizeSubject(x.Subject),
                    Summary = x.Summary,
                    DurationMinutes = x.DurationMinutes,
                    RecordDate = x.RecordDate.ToString("yyyy-MM-dd"),
                    TaskTitle = x.TaskTitle,
                    Remark = x.Remark
                })
                .ToList()
        };

        return Ok(ApiResult<ExamDashboardDto>.Success(dashboard));
    }

    [RequireFeature("STUDENT_LEARNING")]
    [HttpPost("tasks")]
    public async Task<ActionResult<ApiResult<PostgraduateTaskDto>>> CreateTask(
        [FromBody] CreatePostgraduateTaskDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await taskService.CreateAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建考研任务成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetTaskById), new { id = result.Id }, ApiResult<PostgraduateTaskDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建考研任务失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [RequireFeature("STUDENT_LEARNING")]
    [HttpPut("tasks/{id:guid}")]
    public async Task<ActionResult<ApiResult<PostgraduateTaskDto>>> UpdateTask(
        Guid id,
        [FromBody] UpdatePostgraduateTaskDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await taskService.UpdateAsync(id, input, userId, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<PostgraduateTaskDto>.Fail("任务不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新考研任务成功: {Id}", id);
            return Ok(ApiResult<PostgraduateTaskDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新考研任务失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [RequireFeature("STUDENT_LEARNING")]
    [HttpDelete("tasks/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteTask(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var deleted = await taskService.DeleteAsync(id, userId, cancellationToken);
            if (!deleted)
                return NotFound(ApiResult.Fail("任务不存在"));
            logger.LogInformation("删除考研任务成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除考研任务失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [RequireFeature("STUDENT_MISTAKES")]
    [HttpGet("mistakes")]
    public async Task<ActionResult<ApiResult<PageResult<ExamMistakeDto>>>> GetMistakes(
        [FromQuery] ExamMistakeQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await mistakeService.GetPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<ExamMistakeDto>>.Success(result));
    }

    [RequireFeature("STUDENT_MISTAKES")]
    [HttpGet("mistakes/{id:guid}")]
    public async Task<ActionResult<ApiResult<ExamMistakeDto>>> GetMistakeById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await mistakeService.GetByIdAsync(id, userId, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<ExamMistakeDto>.Fail("错题不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<ExamMistakeDto>.Success(result));
    }

    [RequireFeature("STUDENT_MISTAKES")]
    [HttpPost("mistakes")]
    public async Task<ActionResult<ApiResult<ExamMistakeDto>>> CreateMistake(
        [FromBody] CreateExamMistakeDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await mistakeService.CreateAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建错题成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetMistakeById), new { id = result.Id }, ApiResult<ExamMistakeDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建错题失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [RequireFeature("STUDENT_MISTAKES")]
    [HttpPut("mistakes/{id:guid}")]
    public async Task<ActionResult<ApiResult<ExamMistakeDto>>> UpdateMistake(
        Guid id,
        [FromBody] UpdateExamMistakeDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await mistakeService.UpdateAsync(id, input, userId, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<ExamMistakeDto>.Fail("错题不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新错题成功: {Id}", id);
            return Ok(ApiResult<ExamMistakeDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新错题失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [RequireFeature("STUDENT_MISTAKES")]
    [HttpDelete("mistakes/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteMistake(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var deleted = await mistakeService.DeleteAsync(id, userId, cancellationToken);
            if (!deleted)
                return NotFound(ApiResult.Fail("错题不存在"));
            logger.LogInformation("删除错题成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除错题失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [RequireFeature("STUDENT_MATERIALS")]
    [HttpGet("materials")]
    public async Task<ActionResult<ApiResult<PageResult<ExamMaterialDto>>>> GetMaterials(
        [FromQuery] ExamMaterialQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await materialService.GetPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<ExamMaterialDto>>.Success(result));
    }

    [RequireFeature("STUDENT_MATERIALS")]
    [HttpGet("materials/{id:guid}")]
    public async Task<ActionResult<ApiResult<ExamMaterialDto>>> GetMaterialById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await materialService.GetByIdAsync(id, userId, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<ExamMaterialDto>.Fail("资料不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<ExamMaterialDto>.Success(result));
    }

    [RequireFeature("STUDENT_MATERIALS")]
    [HttpPost("materials")]
    public async Task<ActionResult<ApiResult<ExamMaterialDto>>> CreateMaterial(
        [FromBody] CreateExamMaterialDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await materialService.CreateAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建学习资料成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetMaterialById), new { id = result.Id }, ApiResult<ExamMaterialDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建学习资料失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [RequireFeature("STUDENT_MATERIALS")]
    [HttpPut("materials/{id:guid}")]
    public async Task<ActionResult<ApiResult<ExamMaterialDto>>> UpdateMaterial(
        Guid id,
        [FromBody] UpdateExamMaterialDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await materialService.UpdateAsync(id, input, userId, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<ExamMaterialDto>.Fail("资料不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新学习资料成功: {Id}", id);
            return Ok(ApiResult<ExamMaterialDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新学习资料失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [RequireFeature("STUDENT_MATERIALS")]
    [HttpDelete("materials/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteMaterial(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var deleted = await materialService.DeleteAsync(id, userId, cancellationToken);
            if (!deleted)
                return NotFound(ApiResult.Fail("资料不存在"));
            logger.LogInformation("删除学习资料成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除学习资料失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [RequireFeature("STUDENT_SUBJECTS")]
    [HttpGet("subjects")]
    public async Task<ActionResult<ApiResult<PageResult<StudentSubjectDto>>>> GetSubjects(
        [FromQuery] StudentSubjectQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await subjectService.GetPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<StudentSubjectDto>>.Success(result));
    }

    [RequireFeature("STUDENT_SUBJECTS")]
    [HttpGet("subjects/{id:guid}")]
    public async Task<ActionResult<ApiResult<StudentSubjectDto>>> GetSubjectById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await subjectService.GetByIdAsync(id, userId, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<StudentSubjectDto>.Fail("科目不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<StudentSubjectDto>.Success(result));
    }

    [RequireFeature("STUDENT_SUBJECTS")]
    [HttpPost("subjects")]
    public async Task<ActionResult<ApiResult<StudentSubjectDto>>> CreateSubject(
        [FromBody] CreateStudentSubjectDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await subjectService.CreateAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建科目成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetSubjectById), new { id = result.Id }, ApiResult<StudentSubjectDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建科目失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [RequireFeature("STUDENT_SUBJECTS")]
    [HttpPut("subjects/{id:guid}")]
    public async Task<ActionResult<ApiResult<StudentSubjectDto>>> UpdateSubject(
        Guid id,
        [FromBody] UpdateStudentSubjectDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await subjectService.UpdateAsync(id, input, userId, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<StudentSubjectDto>.Fail("科目不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新科目成功: {Id}", id);
            return Ok(ApiResult<StudentSubjectDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新科目失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [RequireFeature("STUDENT_SUBJECTS")]
    [HttpDelete("subjects/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteSubject(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var deleted = await subjectService.DeleteAsync(id, userId, cancellationToken);
            if (!deleted)
                return NotFound(ApiResult.Fail("科目不存在"));
            logger.LogInformation("删除科目成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除科目失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [RequireFeature("STUDENT_RECORDS")]
    [HttpGet("records")]
    public async Task<ActionResult<ApiResult<PageResult<StudentStudyRecordDto>>>> GetRecords(
        [FromQuery] StudentStudyRecordQueryDto query,
        CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await recordService.GetPageAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<StudentStudyRecordDto>>.Success(result));
    }

    [RequireFeature("STUDENT_RECORDS")]
    [HttpGet("records/{id:guid}")]
    public async Task<ActionResult<ApiResult<StudentStudyRecordDto>>> GetRecordById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserIdForQuery();
        var result = await recordService.GetByIdAsync(id, userId, cancellationToken);
        if (result is null)
            return NotFound(ApiResult<StudentStudyRecordDto>.Fail("学习记录不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<StudentStudyRecordDto>.Success(result));
    }

    [RequireFeature("STUDENT_RECORDS")]
    [HttpPost("records")]
    public async Task<ActionResult<ApiResult<StudentStudyRecordDto>>> CreateRecord(
        [FromBody] CreateStudentStudyRecordDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("无法获取用户信息"));

            var result = await recordService.CreateAsync(input, userId.Value, cancellationToken);
            logger.LogInformation("创建学习记录成功: {Id}", result.Id);
            return CreatedAtAction(nameof(GetRecordById), new { id = result.Id }, ApiResult<StudentStudyRecordDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建学习记录失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [RequireFeature("STUDENT_RECORDS")]
    [HttpPut("records/{id:guid}")]
    public async Task<ActionResult<ApiResult<StudentStudyRecordDto>>> UpdateRecord(
        Guid id,
        [FromBody] UpdateStudentStudyRecordDto input,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var result = await recordService.UpdateAsync(id, input, userId, cancellationToken);
            if (result is null)
                return NotFound(ApiResult<StudentStudyRecordDto>.Fail("学习记录不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新学习记录成功: {Id}", id);
            return Ok(ApiResult<StudentStudyRecordDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新学习记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [RequireFeature("STUDENT_RECORDS")]
    [HttpDelete("records/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteRecord(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserIdForQuery();
            var deleted = await recordService.DeleteAsync(id, userId, cancellationToken);
            if (!deleted)
                return NotFound(ApiResult.Fail("学习记录不存在"));
            logger.LogInformation("删除学习记录成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除学习记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    private static bool SubjectEquals(string? subject, string target)
    {
        return string.Equals(NormalizeSubject(subject), target, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeSubject(string? subject)
    {
        return string.IsNullOrWhiteSpace(subject) ? "未归类" : subject.Trim();
    }
}
