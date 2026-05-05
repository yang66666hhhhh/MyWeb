using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Ai.Dtos;
using WebApplication1.Features.Ai.Services;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Ai.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IAiService _aiService;

    public AiController(IAiService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("generate-plan")]
    public async Task<ActionResult<ApiResult<AiPlanDto>>> GeneratePlan([FromBody] GeneratePlanRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("用户未认证"));

        var result = await _aiService.GeneratePlanAsync(request, userId.Value, cancellationToken);
        return Ok(ApiResult<AiPlanDto>.Success(result));
    }

    [HttpPost("generate-report")]
    public async Task<ActionResult<ApiResult<AiReportDto>>> GenerateReport([FromBody] GenerateReportRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("用户未认证"));

        var result = await _aiService.GenerateReportAsync(request, userId.Value, cancellationToken);
        return Ok(ApiResult<AiReportDto>.Success(result));
    }

    [HttpPost("chat")]
    public async Task<ActionResult<ApiResult<ChatResponse>>> Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("用户未认证"));

        var result = await _aiService.ChatAsync(request, userId.Value, cancellationToken);
        return Ok(ApiResult<ChatResponse>.Success(result));
    }

    [HttpGet("plans")]
    public async Task<ActionResult<ApiResult<PageResult<AiPlanDto>>>> GetPlans([FromQuery] AiQueryDto query, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _aiService.GetPlansAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<AiPlanDto>>.Success(result));
    }

    [HttpGet("plans/{id:guid}")]
    public async Task<ActionResult<ApiResult<AiPlanDto>>> GetPlanById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("用户未认证"));

        var result = await _aiService.GetPlanByIdAsync(id, userId.Value, cancellationToken);
        if (result == null)
            return NotFound(ApiResult<AiPlanDto>.Fail("计划不存在"));
        return Ok(ApiResult<AiPlanDto>.Success(result));
    }

    [HttpDelete("plans/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeletePlan(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("用户未认证"));

        var success = await _aiService.DeletePlanAsync(id, userId.Value, cancellationToken);
        return success
            ? Ok(ApiResult.Success("删除成功"))
            : NotFound(ApiResult.Fail("计划不存在"));
    }

    [HttpGet("reports")]
    public async Task<ActionResult<ApiResult<PageResult<AiReportDto>>>> GetReports([FromQuery] AiQueryDto query, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var result = await _aiService.GetReportsAsync(query, userId, cancellationToken);
        return Ok(ApiResult<PageResult<AiReportDto>>.Success(result));
    }

    [HttpGet("reports/{id:guid}")]
    public async Task<ActionResult<ApiResult<AiReportDto>>> GetReportById(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("用户未认证"));

        var result = await _aiService.GetReportByIdAsync(id, userId.Value, cancellationToken);
        if (result == null)
            return NotFound(ApiResult<AiReportDto>.Fail("报告不存在"));
        return Ok(ApiResult<AiReportDto>.Success(result));
    }

    [HttpDelete("reports/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteReport(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("用户未认证"));

        var success = await _aiService.DeleteReportAsync(id, userId.Value, cancellationToken);
        return success
            ? Ok(ApiResult.Success("删除成功"))
            : NotFound(ApiResult.Fail("报告不存在"));
    }

    [HttpGet("chat/sessions")]
    public async Task<ActionResult<ApiResult<PageResult<AiChatSessionDto>>>> GetChatSessions([FromQuery] AiQueryDto query, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("用户未认证"));

        var result = await _aiService.GetChatSessionsAsync(query, userId.Value, cancellationToken);
        return Ok(ApiResult<PageResult<AiChatSessionDto>>.Success(result));
    }

    [HttpGet("chat/sessions/{sessionId:guid}/messages")]
    public async Task<ActionResult<ApiResult<List<AiChatMessageDto>>>> GetChatMessages(Guid sessionId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("用户未认证"));

        var result = await _aiService.GetChatMessagesAsync(sessionId, userId.Value, cancellationToken);
        return Ok(ApiResult<List<AiChatMessageDto>>.Success(result));
    }

    [HttpDelete("chat/sessions/{id:guid}")]
    public async Task<ActionResult<ApiResult>> DeleteChatSession(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("用户未认证"));

        var success = await _aiService.DeleteChatSessionAsync(id, userId.Value, cancellationToken);
        return success
            ? Ok(ApiResult.Success("删除成功"))
            : NotFound(ApiResult.Fail("会话不存在"));
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }
}
