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
public class PersonaController : BaseApiController
{
    // Dev - Code Repositories
    [HttpGet("dev/repositories")]
    [RequireFeature("WORK_PROJECT")]
    public async Task<ActionResult<ApiResult<PageResult<CodeRepositoryDto>>>> GetRepositories(
        [FromQuery] PersonaQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        // TODO: Implement repository service
        return Ok(ApiResult<PageResult<CodeRepositoryDto>>.Success(
            PageResult<CodeRepositoryDto>.Create(new List<CodeRepositoryDto>(), 0, query.Page, query.PageSize)));
    }

    [HttpPost("dev/repositories")]
    [RequireFeature("WORK_PROJECT")]
    public async Task<ActionResult<ApiResult<CodeRepositoryDto>>> CreateRepository(
        [FromBody] CreateCodeRepositoryInput input, CancellationToken ct)
    {
        // TODO: Implement repository service
        return Ok(ApiResult<CodeRepositoryDto>.Success(new CodeRepositoryDto { Id = Guid.NewGuid().ToString(), Name = input.Name }));
    }

    // Dev - Issues
    [HttpGet("dev/issues")]
    [RequireFeature("WORK_TASK")]
    public async Task<ActionResult<ApiResult<PageResult<IssueDto>>>> GetIssues(
        [FromQuery] PersonaQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        // TODO: Implement issue service
        return Ok(ApiResult<PageResult<IssueDto>>.Success(
            PageResult<IssueDto>.Create(new List<IssueDto>(), 0, query.Page, query.PageSize)));
    }

    [HttpPost("dev/issues")]
    [RequireFeature("WORK_TASK")]
    public async Task<ActionResult<ApiResult<IssueDto>>> CreateIssue(
        [FromBody] CreateIssueInput input, CancellationToken ct)
    {
        // TODO: Implement issue service
        return Ok(ApiResult<IssueDto>.Success(new IssueDto { Id = Guid.NewGuid().ToString(), Title = input.Title }));
    }

    // Dev - Pipelines
    [HttpGet("dev/pipelines")]
    [RequireFeature("WORK_PROJECT")]
    public async Task<ActionResult<ApiResult<PageResult<PipelineDto>>>> GetPipelines(
        [FromQuery] PersonaQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        // TODO: Implement pipeline service
        return Ok(ApiResult<PageResult<PipelineDto>>.Success(
            PageResult<PipelineDto>.Create(new List<PipelineDto>(), 0, query.Page, query.PageSize)));
    }

    [HttpPost("dev/pipelines")]
    [RequireFeature("WORK_PROJECT")]
    public async Task<ActionResult<ApiResult<PipelineDto>>> CreatePipeline(
        [FromBody] CreatePipelineInput input, CancellationToken ct)
    {
        // TODO: Implement pipeline service
        return Ok(ApiResult<PipelineDto>.Success(new PipelineDto { Id = Guid.NewGuid().ToString(), Name = input.Name }));
    }

    // Design - Assets
    [HttpGet("design/assets")]
    public async Task<ActionResult<ApiResult<PageResult<DesignAssetDto>>>> GetDesignAssets(
        [FromQuery] PersonaQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        // TODO: Implement design asset service
        return Ok(ApiResult<PageResult<DesignAssetDto>>.Success(
            PageResult<DesignAssetDto>.Create(new List<DesignAssetDto>(), 0, query.Page, query.PageSize)));
    }

    [HttpPost("design/assets")]
    public async Task<ActionResult<ApiResult<DesignAssetDto>>> CreateDesignAsset(
        [FromBody] CreateDesignAssetInput input, CancellationToken ct)
    {
        // TODO: Implement design asset service
        return Ok(ApiResult<DesignAssetDto>.Success(new DesignAssetDto { Id = Guid.NewGuid().ToString(), Name = input.Name }));
    }

    // Design - Prototypes
    [HttpGet("design/prototypes")]
    public async Task<ActionResult<ApiResult<PageResult<PrototypeDto>>>> GetPrototypes(
        [FromQuery] PersonaQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        // TODO: Implement prototype service
        return Ok(ApiResult<PageResult<PrototypeDto>>.Success(
            PageResult<PrototypeDto>.Create(new List<PrototypeDto>(), 0, query.Page, query.PageSize)));
    }

    [HttpPost("design/prototypes")]
    public async Task<ActionResult<ApiResult<PrototypeDto>>> CreatePrototype(
        [FromBody] CreatePrototypeInput input, CancellationToken ct)
    {
        // TODO: Implement prototype service
        return Ok(ApiResult<PrototypeDto>.Success(new PrototypeDto { Id = Guid.NewGuid().ToString(), Title = input.Title }));
    }

    // Teacher - Courses
    [HttpGet("teacher/courses")]
    public async Task<ActionResult<ApiResult<PageResult<TeacherCourseDto>>>> GetTeacherCourses(
        [FromQuery] PersonaQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        // TODO: Implement teacher course service
        return Ok(ApiResult<PageResult<TeacherCourseDto>>.Success(
            PageResult<TeacherCourseDto>.Create(new List<TeacherCourseDto>(), 0, query.Page, query.PageSize)));
    }

    [HttpPost("teacher/courses")]
    public async Task<ActionResult<ApiResult<TeacherCourseDto>>> CreateTeacherCourse(
        [FromBody] CreateTeacherCourseInput input, CancellationToken ct)
    {
        // TODO: Implement teacher course service
        return Ok(ApiResult<TeacherCourseDto>.Success(new TeacherCourseDto { Id = Guid.NewGuid().ToString(), Name = input.Name }));
    }

    // Teacher - Students
    [HttpGet("teacher/students")]
    public async Task<ActionResult<ApiResult<PageResult<TeacherStudentDto>>>> GetTeacherStudents(
        [FromQuery] PersonaQueryDto query, CancellationToken ct)
    {
        var userId = GetUserIdForQuery();
        // TODO: Implement teacher student service
        return Ok(ApiResult<PageResult<TeacherStudentDto>>.Success(
            PageResult<TeacherStudentDto>.Create(new List<TeacherStudentDto>(), 0, query.Page, query.PageSize)));
    }

    [HttpPost("teacher/students")]
    public async Task<ActionResult<ApiResult<TeacherStudentDto>>> CreateTeacherStudent(
        [FromBody] CreateTeacherStudentInput input, CancellationToken ct)
    {
        // TODO: Implement teacher student service
        return Ok(ApiResult<TeacherStudentDto>.Success(new TeacherStudentDto { Id = Guid.NewGuid().ToString(), Name = input.Name }));
    }
}
