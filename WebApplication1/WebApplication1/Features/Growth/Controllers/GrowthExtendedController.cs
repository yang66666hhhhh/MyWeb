using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Features.Growth.Services;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Controllers;

[ApiController]
[Authorize]
[Route("api/growth")]
[Tags("Growth")]
public class GrowthExtendedController(IGrowthExtendedService service, ILogger<GrowthExtendedController> logger) : BaseApiController
{
    [HttpGet("skills")]
    [RequireFeature("GROWTH_SKILL")]
    public async Task<ActionResult<ApiResult<PageResult<SkillDto>>>> GetSkills(
        [FromQuery] GrowthQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await service.GetSkillsAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<SkillDto>>.Success(result));
    }

    [HttpPost("skills")]
    [RequireFeature("GROWTH_SKILL")]
    public async Task<ActionResult<ApiResult<SkillDto>>> CreateSkill(
        [FromBody] CreateSkillInput input, CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized(ApiResult.Fail("无法获取用户信息"));
            var result = await service.CreateSkillAsync(input, userId.Value, ct);
            logger.LogInformation("创建技能成功: {Id}", result.Id);
            return Ok(ApiResult<SkillDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建技能失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("skills/{id:guid}")]
    [RequireFeature("GROWTH_SKILL")]
    public async Task<ActionResult<ApiResult<SkillDto>>> UpdateSkill(
        Guid id, [FromBody] UpdateSkillInput input, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetSkillByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult<SkillDto>.Fail("技能不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此技能"));

            var result = await service.UpdateSkillAsync(id, input, ct);
            if (result is null) return NotFound(ApiResult<SkillDto>.Fail("技能不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新技能成功: {Id}", id);
            return Ok(ApiResult<SkillDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新技能失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("skills/{id:guid}")]
    [RequireFeature("GROWTH_SKILL")]
    public async Task<ActionResult<ApiResult>> DeleteSkill(Guid id, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetSkillByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult.Fail("技能不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此技能"));

            var deleted = await service.DeleteSkillAsync(id, ct);
            if (!deleted) return NotFound(ApiResult.Fail("技能不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除技能成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除技能失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("goals")]
    public async Task<ActionResult<ApiResult<PageResult<GoalDto>>>> GetGoals(
        [FromQuery] GrowthQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await service.GetGoalsAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<GoalDto>>.Success(result));
    }

    [HttpPost("goals")]
    public async Task<ActionResult<ApiResult<GoalDto>>> CreateGoal(
        [FromBody] CreateGoalInput input, CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized(ApiResult.Fail("无法获取用户信息"));
            var result = await service.CreateGoalAsync(input, userId.Value, ct);
            logger.LogInformation("创建目标成功: {Id}", result.Id);
            return Ok(ApiResult<GoalDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建目标失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("goals/{id:guid}")]
    public async Task<ActionResult<ApiResult<GoalDto>>> UpdateGoal(
        Guid id, [FromBody] UpdateGoalInput input, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetGoalByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult<GoalDto>.Fail("目标不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此目标"));

            var result = await service.UpdateGoalAsync(id, input, ct);
            if (result is null) return NotFound(ApiResult<GoalDto>.Fail("目标不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新目标成功: {Id}", id);
            return Ok(ApiResult<GoalDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新目标失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("goals/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteGoal(Guid id, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetGoalByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult.Fail("目标不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此目标"));

            var deleted = await service.DeleteGoalAsync(id, ct);
            if (!deleted) return NotFound(ApiResult.Fail("目标不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除目标成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除目标失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("year-plans")]
    public async Task<ActionResult<ApiResult<PageResult<YearPlanDto>>>> GetYearPlans(
        [FromQuery] GrowthQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await service.GetYearPlansAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<YearPlanDto>>.Success(result));
    }

    [HttpPost("year-plans")]
    public async Task<ActionResult<ApiResult<YearPlanDto>>> CreateYearPlan(
        [FromBody] CreateYearPlanInput input, CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized(ApiResult.Fail("无法获取用户信息"));
            var result = await service.CreateYearPlanAsync(input, userId.Value, ct);
            logger.LogInformation("创建年度计划成功: {Id}", result.Id);
            return Ok(ApiResult<YearPlanDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建年度计划失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("year-plans/{id:guid}")]
    public async Task<ActionResult<ApiResult<YearPlanDto>>> UpdateYearPlan(
        Guid id, [FromBody] UpdateYearPlanInput input, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetYearPlanByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult<YearPlanDto>.Fail("年度计划不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此年度计划"));

            var result = await service.UpdateYearPlanAsync(id, input, ct);
            if (result is null) return NotFound(ApiResult<YearPlanDto>.Fail("年度计划不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新年度计划成功: {Id}", id);
            return Ok(ApiResult<YearPlanDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新年度计划失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("year-plans/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteYearPlan(Guid id, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetYearPlanByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult.Fail("年度计划不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此年度计划"));

            var deleted = await service.DeleteYearPlanAsync(id, ct);
            if (!deleted) return NotFound(ApiResult.Fail("年度计划不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除年度计划成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除年度计划失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("monthly-reviews")]
    public async Task<ActionResult<ApiResult<PageResult<MonthlyReviewDto>>>> GetMonthlyReviews(
        [FromQuery] GrowthQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await service.GetMonthlyReviewsAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<MonthlyReviewDto>>.Success(result));
    }

    [HttpPost("monthly-reviews")]
    public async Task<ActionResult<ApiResult<MonthlyReviewDto>>> CreateMonthlyReview(
        [FromBody] CreateMonthlyReviewInput input, CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized(ApiResult.Fail("无法获取用户信息"));
            var result = await service.CreateMonthlyReviewAsync(input, userId.Value, ct);
            logger.LogInformation("创建月度回顾成功: {Id}", result.Id);
            return Ok(ApiResult<MonthlyReviewDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建月度回顾失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("monthly-reviews/{id:guid}")]
    public async Task<ActionResult<ApiResult<MonthlyReviewDto>>> UpdateMonthlyReview(
        Guid id, [FromBody] UpdateMonthlyReviewInput input, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetMonthlyReviewByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult<MonthlyReviewDto>.Fail("月度回顾不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此月度回顾"));

            var result = await service.UpdateMonthlyReviewAsync(id, input, ct);
            if (result is null) return NotFound(ApiResult<MonthlyReviewDto>.Fail("月度回顾不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新月度回顾成功: {Id}", id);
            return Ok(ApiResult<MonthlyReviewDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新月度回顾失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("monthly-reviews/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteMonthlyReview(Guid id, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetMonthlyReviewByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult.Fail("月度回顾不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此月度回顾"));

            var deleted = await service.DeleteMonthlyReviewAsync(id, ct);
            if (!deleted) return NotFound(ApiResult.Fail("月度回顾不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除月度回顾成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除月度回顾失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("learning-paths")]
    public async Task<ActionResult<ApiResult<PageResult<LearningPathDto>>>> GetLearningPaths(
        [FromQuery] GrowthQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await service.GetLearningPathsAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<LearningPathDto>>.Success(result));
    }

    [HttpPost("learning-paths")]
    public async Task<ActionResult<ApiResult<LearningPathDto>>> CreateLearningPath(
        [FromBody] CreateLearningPathInput input, CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized(ApiResult.Fail("无法获取用户信息"));
            var result = await service.CreateLearningPathAsync(input, userId.Value, ct);
            logger.LogInformation("创建学习路径成功: {Id}", result.Id);
            return Ok(ApiResult<LearningPathDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建学习路径失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("learning-paths/{id:guid}")]
    public async Task<ActionResult<ApiResult<LearningPathDto>>> UpdateLearningPath(
        Guid id, [FromBody] UpdateLearningPathInput input, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetLearningPathByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult<LearningPathDto>.Fail("学习路径不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此学习路径"));

            var result = await service.UpdateLearningPathAsync(id, input, ct);
            if (result is null) return NotFound(ApiResult<LearningPathDto>.Fail("学习路径不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新学习路径成功: {Id}", id);
            return Ok(ApiResult<LearningPathDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新学习路径失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("learning-paths/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteLearningPath(Guid id, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetLearningPathByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult.Fail("学习路径不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此学习路径"));

            var deleted = await service.DeleteLearningPathAsync(id, ct);
            if (!deleted) return NotFound(ApiResult.Fail("学习路径不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除学习路径成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除学习路径失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("courses")]
    public async Task<ActionResult<ApiResult<PageResult<CourseDto>>>> GetCourses(
        [FromQuery] GrowthQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await service.GetCoursesAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<CourseDto>>.Success(result));
    }

    [HttpPost("courses")]
    public async Task<ActionResult<ApiResult<CourseDto>>> CreateCourse(
        [FromBody] CreateCourseInput input, CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized(ApiResult.Fail("无法获取用户信息"));
            var result = await service.CreateCourseAsync(input, userId.Value, ct);
            logger.LogInformation("创建课程成功: {Id}", result.Id);
            return Ok(ApiResult<CourseDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建课程失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("courses/{id:guid}")]
    public async Task<ActionResult<ApiResult<CourseDto>>> UpdateCourse(
        Guid id, [FromBody] UpdateCourseInput input, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetCourseByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult<CourseDto>.Fail("课程不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此课程"));

            var result = await service.UpdateCourseAsync(id, input, ct);
            if (result is null) return NotFound(ApiResult<CourseDto>.Fail("课程不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新课程成功: {Id}", id);
            return Ok(ApiResult<CourseDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新课程失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("courses/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteCourse(Guid id, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetCourseByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult.Fail("课程不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此课程"));

            var deleted = await service.DeleteCourseAsync(id, ct);
            if (!deleted) return NotFound(ApiResult.Fail("课程不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除课程成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除课程失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("fitness")]
    public async Task<ActionResult<ApiResult<PageResult<FitnessRecordDto>>>> GetFitnessRecords(
        [FromQuery] GrowthQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await service.GetFitnessRecordsAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<FitnessRecordDto>>.Success(result));
    }

    [HttpPost("fitness")]
    public async Task<ActionResult<ApiResult<FitnessRecordDto>>> CreateFitnessRecord(
        [FromBody] CreateFitnessRecordInput input, CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized(ApiResult.Fail("无法获取用户信息"));
            var result = await service.CreateFitnessRecordAsync(input, userId.Value, ct);
            logger.LogInformation("创建健身记录成功: {Id}", result.Id);
            return Ok(ApiResult<FitnessRecordDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建健身记录失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("fitness/{id:guid}")]
    public async Task<ActionResult<ApiResult<FitnessRecordDto>>> UpdateFitnessRecord(
        Guid id, [FromBody] UpdateFitnessRecordInput input, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetFitnessRecordByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult<FitnessRecordDto>.Fail("健身记录不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此健身记录"));

            var result = await service.UpdateFitnessRecordAsync(id, input, ct);
            if (result is null) return NotFound(ApiResult<FitnessRecordDto>.Fail("健身记录不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新健身记录成功: {Id}", id);
            return Ok(ApiResult<FitnessRecordDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新健身记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("fitness/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteFitnessRecord(Guid id, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetFitnessRecordByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult.Fail("健身记录不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此健身记录"));

            var deleted = await service.DeleteFitnessRecordAsync(id, ct);
            if (!deleted) return NotFound(ApiResult.Fail("健身记录不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除健身记录成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除健身记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("sleep")]
    public async Task<ActionResult<ApiResult<PageResult<SleepRecordDto>>>> GetSleepRecords(
        [FromQuery] GrowthQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await service.GetSleepRecordsAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<SleepRecordDto>>.Success(result));
    }

    [HttpPost("sleep")]
    public async Task<ActionResult<ApiResult<SleepRecordDto>>> CreateSleepRecord(
        [FromBody] CreateSleepRecordInput input, CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized(ApiResult.Fail("无法获取用户信息"));
            var result = await service.CreateSleepRecordAsync(input, userId.Value, ct);
            logger.LogInformation("创建睡眠记录成功: {Id}", result.Id);
            return Ok(ApiResult<SleepRecordDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建睡眠记录失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("sleep/{id:guid}")]
    public async Task<ActionResult<ApiResult<SleepRecordDto>>> UpdateSleepRecord(
        Guid id, [FromBody] UpdateSleepRecordInput input, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetSleepRecordByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult<SleepRecordDto>.Fail("睡眠记录不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此睡眠记录"));

            var result = await service.UpdateSleepRecordAsync(id, input, ct);
            if (result is null) return NotFound(ApiResult<SleepRecordDto>.Fail("睡眠记录不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新睡眠记录成功: {Id}", id);
            return Ok(ApiResult<SleepRecordDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新睡眠记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("sleep/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteSleepRecord(Guid id, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetSleepRecordByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult.Fail("睡眠记录不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此睡眠记录"));

            var deleted = await service.DeleteSleepRecordAsync(id, ct);
            if (!deleted) return NotFound(ApiResult.Fail("睡眠记录不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除睡眠记录成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除睡眠记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("mood")]
    public async Task<ActionResult<ApiResult<PageResult<MoodRecordDto>>>> GetMoodRecords(
        [FromQuery] GrowthQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await service.GetMoodRecordsAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<MoodRecordDto>>.Success(result));
    }

    [HttpPost("mood")]
    public async Task<ActionResult<ApiResult<MoodRecordDto>>> CreateMoodRecord(
        [FromBody] CreateMoodRecordInput input, CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized(ApiResult.Fail("无法获取用户信息"));
            var result = await service.CreateMoodRecordAsync(input, userId.Value, ct);
            logger.LogInformation("创建心情记录成功: {Id}", result.Id);
            return Ok(ApiResult<MoodRecordDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建心情记录失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("mood/{id:guid}")]
    public async Task<ActionResult<ApiResult<MoodRecordDto>>> UpdateMoodRecord(
        Guid id, [FromBody] UpdateMoodRecordInput input, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetMoodRecordByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult<MoodRecordDto>.Fail("心情记录不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此心情记录"));

            var result = await service.UpdateMoodRecordAsync(id, input, ct);
            if (result is null) return NotFound(ApiResult<MoodRecordDto>.Fail("心情记录不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新心情记录成功: {Id}", id);
            return Ok(ApiResult<MoodRecordDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新心情记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("mood/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteMoodRecord(Guid id, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetMoodRecordByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult.Fail("心情记录不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此心情记录"));

            var deleted = await service.DeleteMoodRecordAsync(id, ct);
            if (!deleted) return NotFound(ApiResult.Fail("心情记录不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除心情记录成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除心情记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("reading")]
    public async Task<ActionResult<ApiResult<PageResult<ReadingBookDto>>>> GetReadingBooks(
        [FromQuery] GrowthQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await service.GetReadingBooksAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<ReadingBookDto>>.Success(result));
    }

    [HttpPost("reading")]
    public async Task<ActionResult<ApiResult<ReadingBookDto>>> CreateReadingBook(
        [FromBody] CreateReadingBookInput input, CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized(ApiResult.Fail("无法获取用户信息"));
            var result = await service.CreateReadingBookAsync(input, userId.Value, ct);
            logger.LogInformation("创建阅读记录成功: {Id}", result.Id);
            return Ok(ApiResult<ReadingBookDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建阅读记录失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("reading/{id:guid}")]
    public async Task<ActionResult<ApiResult<ReadingBookDto>>> UpdateReadingBook(
        Guid id, [FromBody] UpdateReadingBookInput input, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetReadingBookByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult<ReadingBookDto>.Fail("阅读记录不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此阅读记录"));

            var result = await service.UpdateReadingBookAsync(id, input, ct);
            if (result is null) return NotFound(ApiResult<ReadingBookDto>.Fail("阅读记录不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新阅读记录成功: {Id}", id);
            return Ok(ApiResult<ReadingBookDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新阅读记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("reading/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteReadingBook(Guid id, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetReadingBookByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult.Fail("阅读记录不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此阅读记录"));

            var deleted = await service.DeleteReadingBookAsync(id, ct);
            if (!deleted) return NotFound(ApiResult.Fail("阅读记录不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除阅读记录成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除阅读记录失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("focus")]
    public async Task<ActionResult<ApiResult<PageResult<FocusSessionDto>>>> GetFocusSessions(
        [FromQuery] GrowthQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await service.GetFocusSessionsAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<FocusSessionDto>>.Success(result));
    }

    [HttpPost("focus")]
    public async Task<ActionResult<ApiResult<FocusSessionDto>>> CreateFocusSession(
        [FromBody] CreateFocusSessionInput input, CancellationToken ct)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue) return Unauthorized(ApiResult.Fail("无法获取用户信息"));
            var result = await service.CreateFocusSessionAsync(input, userId.Value, ct);
            logger.LogInformation("创建专注会话成功: {Id}", result.Id);
            return Ok(ApiResult<FocusSessionDto>.Success(result, "创建成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "创建专注会话失败");
            return StatusCode(500, ApiResult.Fail("创建失败，请稍后重试"));
        }
    }

    [HttpPut("focus/{id:guid}")]
    public async Task<ActionResult<ApiResult<FocusSessionDto>>> UpdateFocusSession(
        Guid id, [FromBody] UpdateFocusSessionInput input, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetFocusSessionByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult<FocusSessionDto>.Fail("专注会话不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此专注会话"));

            var result = await service.UpdateFocusSessionAsync(id, input, ct);
            if (result is null) return NotFound(ApiResult<FocusSessionDto>.Fail("专注会话不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("更新专注会话成功: {Id}", id);
            return Ok(ApiResult<FocusSessionDto>.Success(result, "更新成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "更新专注会话失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("更新失败，请稍后重试"));
        }
    }

    [HttpDelete("focus/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteFocusSession(Guid id, CancellationToken ct)
    {
        try
        {
            var existing = await service.GetFocusSessionByIdAsync(id, ct);
            if (existing is null) return NotFound(ApiResult.Fail("专注会话不存在", StatusCodes.Status404NotFound));
            if (IsUnauthorizedForResource(existing.UserId))
                return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此专注会话"));

            var deleted = await service.DeleteFocusSessionAsync(id, ct);
            if (!deleted) return NotFound(ApiResult.Fail("专注会话不存在", StatusCodes.Status404NotFound));
            logger.LogInformation("删除专注会话成功: {Id}", id);
            return Ok(ApiResult.Success("删除成功"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "删除专注会话失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }
}
