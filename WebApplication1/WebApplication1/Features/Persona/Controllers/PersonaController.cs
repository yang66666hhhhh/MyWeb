using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Persona.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Persona.Controllers;

[ApiController]
[Authorize]
[Route("api/persona")]
[Tags("Persona")]
public class PersonaController(IPersonaFeatureService personaService) : BaseApiController
{
    // Dev - Code Repositories

    [HttpGet("dev/repositories")]
    [RequireFeature("DEV_REPOSITORIES")]
    public async Task<ActionResult<ApiResult<PageResult<CodeRepositoryDto>>>> GetRepositories(
        [FromQuery] PersonaQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await personaService.GetRepositoriesAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<CodeRepositoryDto>>.Success(result));
    }

    [HttpGet("dev/repositories/{id:guid}")]
    [RequireFeature("DEV_REPOSITORIES")]
    public async Task<ActionResult<ApiResult<CodeRepositoryDto>>> GetRepositoryById(Guid id, CancellationToken ct)
    {
        var result = await personaService.GetRepositoryByIdAsync(id, ct);
        if (result is null)
            return NotFound(ApiResult<CodeRepositoryDto>.Fail("仓库不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<CodeRepositoryDto>.Fail("仓库不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<CodeRepositoryDto>.Success(result));
    }

    [HttpPost("dev/repositories")]
    [RequireFeature("DEV_REPOSITORIES")]
    public async Task<ActionResult<ApiResult<CodeRepositoryDto>>> CreateRepository(
        [FromBody] CreateCodeRepositoryInput input, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await personaService.CreateRepositoryAsync(input, userId.Value, ct);
        return CreatedAtAction(nameof(GetRepositoryById), new { id = result.Id }, ApiResult<CodeRepositoryDto>.Success(result, "创建成功"));
    }

    [HttpPut("dev/repositories/{id:guid}")]
    [RequireFeature("DEV_REPOSITORIES")]
    public async Task<ActionResult<ApiResult<CodeRepositoryDto>>> UpdateRepository(
        Guid id, [FromBody] CreateCodeRepositoryInput input, CancellationToken ct)
    {
        var existing = await personaService.GetRepositoryByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult<CodeRepositoryDto>.Fail("仓库不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此仓库"));

        var result = await personaService.UpdateRepositoryAsync(id, input, ct);
        return Ok(ApiResult<CodeRepositoryDto>.Success(result!, "更新成功"));
    }

    [HttpDelete("dev/repositories/{id:guid}")]
    [RequireFeature("DEV_REPOSITORIES")]
    public async Task<ActionResult<ApiResult>> DeleteRepository(Guid id, CancellationToken ct)
    {
        var existing = await personaService.GetRepositoryByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult.Fail("仓库不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此仓库"));

        await personaService.DeleteRepositoryAsync(id, ct);
        return Ok(ApiResult.Success("删除成功"));
    }

    // Dev - Issues

    [HttpGet("dev/issues")]
    [RequireFeature("DEV_ISSUES")]
    public async Task<ActionResult<ApiResult<PageResult<IssueDto>>>> GetIssues(
        [FromQuery] PersonaQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await personaService.GetIssuesAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<IssueDto>>.Success(result));
    }

    [HttpGet("dev/issues/{id:guid}")]
    [RequireFeature("DEV_ISSUES")]
    public async Task<ActionResult<ApiResult<IssueDto>>> GetIssueById(Guid id, CancellationToken ct)
    {
        var result = await personaService.GetIssueByIdAsync(id, ct);
        if (result is null)
            return NotFound(ApiResult<IssueDto>.Fail("Issue不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<IssueDto>.Fail("Issue不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<IssueDto>.Success(result));
    }

    [HttpPost("dev/issues")]
    [RequireFeature("DEV_ISSUES")]
    public async Task<ActionResult<ApiResult<IssueDto>>> CreateIssue(
        [FromBody] CreateIssueInput input, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await personaService.CreateIssueAsync(input, userId.Value, ct);
        return CreatedAtAction(nameof(GetIssueById), new { id = result.Id }, ApiResult<IssueDto>.Success(result, "创建成功"));
    }

    [HttpPut("dev/issues/{id:guid}")]
    [RequireFeature("DEV_ISSUES")]
    public async Task<ActionResult<ApiResult<IssueDto>>> UpdateIssue(
        Guid id, [FromBody] CreateIssueInput input, CancellationToken ct)
    {
        var existing = await personaService.GetIssueByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult<IssueDto>.Fail("Issue不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此Issue"));

        var result = await personaService.UpdateIssueAsync(id, input, ct);
        return Ok(ApiResult<IssueDto>.Success(result!, "更新成功"));
    }

    [HttpDelete("dev/issues/{id:guid}")]
    [RequireFeature("DEV_ISSUES")]
    public async Task<ActionResult<ApiResult>> DeleteIssue(Guid id, CancellationToken ct)
    {
        var existing = await personaService.GetIssueByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult.Fail("Issue不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此Issue"));

        await personaService.DeleteIssueAsync(id, ct);
        return Ok(ApiResult.Success("删除成功"));
    }

    // Dev - Pipelines

    [HttpGet("dev/pipelines")]
    [RequireFeature("DEV_PIPELINES")]
    public async Task<ActionResult<ApiResult<PageResult<PipelineDto>>>> GetPipelines(
        [FromQuery] PersonaQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await personaService.GetPipelinesAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<PipelineDto>>.Success(result));
    }

    [HttpGet("dev/pipelines/{id:guid}")]
    [RequireFeature("DEV_PIPELINES")]
    public async Task<ActionResult<ApiResult<PipelineDto>>> GetPipelineById(Guid id, CancellationToken ct)
    {
        var result = await personaService.GetPipelineByIdAsync(id, ct);
        if (result is null)
            return NotFound(ApiResult<PipelineDto>.Fail("流水线不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<PipelineDto>.Fail("流水线不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<PipelineDto>.Success(result));
    }

    [HttpPost("dev/pipelines")]
    [RequireFeature("DEV_PIPELINES")]
    public async Task<ActionResult<ApiResult<PipelineDto>>> CreatePipeline(
        [FromBody] CreatePipelineInput input, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await personaService.CreatePipelineAsync(input, userId.Value, ct);
        return CreatedAtAction(nameof(GetPipelineById), new { id = result.Id }, ApiResult<PipelineDto>.Success(result, "创建成功"));
    }

    [HttpPut("dev/pipelines/{id:guid}")]
    [RequireFeature("DEV_PIPELINES")]
    public async Task<ActionResult<ApiResult<PipelineDto>>> UpdatePipeline(
        Guid id, [FromBody] CreatePipelineInput input, CancellationToken ct)
    {
        var existing = await personaService.GetPipelineByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult<PipelineDto>.Fail("流水线不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此流水线"));

        var result = await personaService.UpdatePipelineAsync(id, input, ct);
        return Ok(ApiResult<PipelineDto>.Success(result!, "更新成功"));
    }

    [HttpDelete("dev/pipelines/{id:guid}")]
    [RequireFeature("DEV_PIPELINES")]
    public async Task<ActionResult<ApiResult>> DeletePipeline(Guid id, CancellationToken ct)
    {
        var existing = await personaService.GetPipelineByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult.Fail("流水线不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此流水线"));

        await personaService.DeletePipelineAsync(id, ct);
        return Ok(ApiResult.Success("删除成功"));
    }

    // Design - Assets

    [HttpGet("design/assets")]
    [RequireFeature("DESIGN_ASSETS")]
    public async Task<ActionResult<ApiResult<PageResult<DesignAssetDto>>>> GetDesignAssets(
        [FromQuery] PersonaQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await personaService.GetDesignAssetsAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<DesignAssetDto>>.Success(result));
    }

    [HttpGet("design/assets/{id:guid}")]
    [RequireFeature("DESIGN_ASSETS")]
    public async Task<ActionResult<ApiResult<DesignAssetDto>>> GetDesignAssetById(Guid id, CancellationToken ct)
    {
        var result = await personaService.GetDesignAssetByIdAsync(id, ct);
        if (result is null)
            return NotFound(ApiResult<DesignAssetDto>.Fail("设计资产不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<DesignAssetDto>.Fail("设计资产不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<DesignAssetDto>.Success(result));
    }

    [HttpPost("design/assets")]
    [RequireFeature("DESIGN_ASSETS")]
    public async Task<ActionResult<ApiResult<DesignAssetDto>>> CreateDesignAsset(
        [FromBody] CreateDesignAssetInput input, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await personaService.CreateDesignAssetAsync(input, userId.Value, ct);
        return CreatedAtAction(nameof(GetDesignAssetById), new { id = result.Id }, ApiResult<DesignAssetDto>.Success(result, "创建成功"));
    }

    [HttpPut("design/assets/{id:guid}")]
    [RequireFeature("DESIGN_ASSETS")]
    public async Task<ActionResult<ApiResult<DesignAssetDto>>> UpdateDesignAsset(
        Guid id, [FromBody] CreateDesignAssetInput input, CancellationToken ct)
    {
        var existing = await personaService.GetDesignAssetByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult<DesignAssetDto>.Fail("设计资产不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此设计资产"));

        var result = await personaService.UpdateDesignAssetAsync(id, input, ct);
        return Ok(ApiResult<DesignAssetDto>.Success(result!, "更新成功"));
    }

    [HttpDelete("design/assets/{id:guid}")]
    [RequireFeature("DESIGN_ASSETS")]
    public async Task<ActionResult<ApiResult>> DeleteDesignAsset(Guid id, CancellationToken ct)
    {
        var existing = await personaService.GetDesignAssetByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult.Fail("设计资产不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此设计资产"));

        await personaService.DeleteDesignAssetAsync(id, ct);
        return Ok(ApiResult.Success("删除成功"));
    }

    // Design - Prototypes

    [HttpGet("design/prototypes")]
    [RequireFeature("DESIGN_PROTOTYPES")]
    public async Task<ActionResult<ApiResult<PageResult<PrototypeDto>>>> GetPrototypes(
        [FromQuery] PersonaQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await personaService.GetPrototypesAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<PrototypeDto>>.Success(result));
    }

    [HttpGet("design/prototypes/{id:guid}")]
    [RequireFeature("DESIGN_PROTOTYPES")]
    public async Task<ActionResult<ApiResult<PrototypeDto>>> GetPrototypeById(Guid id, CancellationToken ct)
    {
        var result = await personaService.GetPrototypeByIdAsync(id, ct);
        if (result is null)
            return NotFound(ApiResult<PrototypeDto>.Fail("原型不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<PrototypeDto>.Fail("原型不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<PrototypeDto>.Success(result));
    }

    [HttpPost("design/prototypes")]
    [RequireFeature("DESIGN_PROTOTYPES")]
    public async Task<ActionResult<ApiResult<PrototypeDto>>> CreatePrototype(
        [FromBody] CreatePrototypeInput input, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await personaService.CreatePrototypeAsync(input, userId.Value, ct);
        return CreatedAtAction(nameof(GetPrototypeById), new { id = result.Id }, ApiResult<PrototypeDto>.Success(result, "创建成功"));
    }

    [HttpPut("design/prototypes/{id:guid}")]
    [RequireFeature("DESIGN_PROTOTYPES")]
    public async Task<ActionResult<ApiResult<PrototypeDto>>> UpdatePrototype(
        Guid id, [FromBody] CreatePrototypeInput input, CancellationToken ct)
    {
        var existing = await personaService.GetPrototypeByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult<PrototypeDto>.Fail("原型不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此原型"));

        var result = await personaService.UpdatePrototypeAsync(id, input, ct);
        return Ok(ApiResult<PrototypeDto>.Success(result!, "更新成功"));
    }

    [HttpDelete("design/prototypes/{id:guid}")]
    [RequireFeature("DESIGN_PROTOTYPES")]
    public async Task<ActionResult<ApiResult>> DeletePrototype(Guid id, CancellationToken ct)
    {
        var existing = await personaService.GetPrototypeByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult.Fail("原型不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此原型"));

        await personaService.DeletePrototypeAsync(id, ct);
        return Ok(ApiResult.Success("删除成功"));
    }

    // Teacher - Courses

    [HttpGet("teacher/courses")]
    [RequireFeature("TEACHER_COURSES")]
    public async Task<ActionResult<ApiResult<PageResult<TeacherCourseDto>>>> GetTeacherCourses(
        [FromQuery] PersonaQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await personaService.GetTeacherCoursesAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<TeacherCourseDto>>.Success(result));
    }

    [HttpGet("teacher/courses/{id:guid}")]
    [RequireFeature("TEACHER_COURSES")]
    public async Task<ActionResult<ApiResult<TeacherCourseDto>>> GetTeacherCourseById(Guid id, CancellationToken ct)
    {
        var result = await personaService.GetTeacherCourseByIdAsync(id, ct);
        if (result is null)
            return NotFound(ApiResult<TeacherCourseDto>.Fail("课程不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<TeacherCourseDto>.Fail("课程不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<TeacherCourseDto>.Success(result));
    }

    [HttpPost("teacher/courses")]
    [RequireFeature("TEACHER_COURSES")]
    public async Task<ActionResult<ApiResult<TeacherCourseDto>>> CreateTeacherCourse(
        [FromBody] CreateTeacherCourseInput input, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await personaService.CreateTeacherCourseAsync(input, userId.Value, ct);
        return CreatedAtAction(nameof(GetTeacherCourseById), new { id = result.Id }, ApiResult<TeacherCourseDto>.Success(result, "创建成功"));
    }

    [HttpPut("teacher/courses/{id:guid}")]
    [RequireFeature("TEACHER_COURSES")]
    public async Task<ActionResult<ApiResult<TeacherCourseDto>>> UpdateTeacherCourse(
        Guid id, [FromBody] CreateTeacherCourseInput input, CancellationToken ct)
    {
        var existing = await personaService.GetTeacherCourseByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult<TeacherCourseDto>.Fail("课程不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此课程"));

        var result = await personaService.UpdateTeacherCourseAsync(id, input, ct);
        return Ok(ApiResult<TeacherCourseDto>.Success(result!, "更新成功"));
    }

    [HttpDelete("teacher/courses/{id:guid}")]
    [RequireFeature("TEACHER_COURSES")]
    public async Task<ActionResult<ApiResult>> DeleteTeacherCourse(Guid id, CancellationToken ct)
    {
        var existing = await personaService.GetTeacherCourseByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult.Fail("课程不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此课程"));

        await personaService.DeleteTeacherCourseAsync(id, ct);
        return Ok(ApiResult.Success("删除成功"));
    }

    // Teacher - Students

    [HttpGet("teacher/students")]
    [RequireFeature("TEACHER_STUDENTS")]
    public async Task<ActionResult<ApiResult<PageResult<TeacherStudentDto>>>> GetTeacherStudents(
        [FromQuery] PersonaQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        var result = await personaService.GetTeacherStudentsAsync(query, userId, ct);
        return Ok(ApiResult<PageResult<TeacherStudentDto>>.Success(result));
    }

    [HttpGet("teacher/students/{id:guid}")]
    [RequireFeature("TEACHER_STUDENTS")]
    public async Task<ActionResult<ApiResult<TeacherStudentDto>>> GetTeacherStudentById(Guid id, CancellationToken ct)
    {
        var result = await personaService.GetTeacherStudentByIdAsync(id, ct);
        if (result is null)
            return NotFound(ApiResult<TeacherStudentDto>.Fail("学生不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && result.UserId != currentUserId?.ToString())
            return NotFound(ApiResult<TeacherStudentDto>.Fail("学生不存在", StatusCodes.Status404NotFound));

        return Ok(ApiResult<TeacherStudentDto>.Success(result));
    }

    [HttpPost("teacher/students")]
    [RequireFeature("TEACHER_STUDENTS")]
    public async Task<ActionResult<ApiResult<TeacherStudentDto>>> CreateTeacherStudent(
        [FromBody] CreateTeacherStudentInput input, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));

        var result = await personaService.CreateTeacherStudentAsync(input, userId.Value, ct);
        return CreatedAtAction(nameof(GetTeacherStudentById), new { id = result.Id }, ApiResult<TeacherStudentDto>.Success(result, "创建成功"));
    }

    [HttpPut("teacher/students/{id:guid}")]
    [RequireFeature("TEACHER_STUDENTS")]
    public async Task<ActionResult<ApiResult<TeacherStudentDto>>> UpdateTeacherStudent(
        Guid id, [FromBody] CreateTeacherStudentInput input, CancellationToken ct)
    {
        var existing = await personaService.GetTeacherStudentByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult<TeacherStudentDto>.Fail("学生不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限修改此学生"));

        var result = await personaService.UpdateTeacherStudentAsync(id, input, ct);
        return Ok(ApiResult<TeacherStudentDto>.Success(result!, "更新成功"));
    }

    [HttpDelete("teacher/students/{id:guid}")]
    [RequireFeature("TEACHER_STUDENTS")]
    public async Task<ActionResult<ApiResult>> DeleteTeacherStudent(Guid id, CancellationToken ct)
    {
        var existing = await personaService.GetTeacherStudentByIdAsync(id, ct);
        if (existing is null)
            return NotFound(ApiResult.Fail("学生不存在", StatusCodes.Status404NotFound));

        var currentUserId = GetCurrentUserId();
        if (!IsProOrAbove() && existing.UserId != currentUserId?.ToString())
            return StatusCode(StatusCodes.Status403Forbidden, ApiResult.Fail("无权限删除此学生"));

        await personaService.DeleteTeacherStudentAsync(id, ct);
        return Ok(ApiResult.Success("删除成功"));
    }
}
