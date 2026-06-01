using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Features.Ai.Dtos;
using WebApplication1.Features.Ai.Services;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Ai.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
[Tags("AI")]
public class AiController : ControllerBase
{
    private readonly IAiService _aiService;
    private readonly ILogger<AiController> _logger;

    public AiController(IAiService aiService, ILogger<AiController> logger)
    {
        _aiService = aiService;
        _logger = logger;
    }

    [HttpPost("generate-plan")]
    [RequireFeature("AI_PLANNER")]
    public async Task<ActionResult<ApiResult<AiPlanDto>>> GeneratePlan([FromBody] GeneratePlanRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("用户未认证"));

        try
        {
            var result = await _aiService.GeneratePlanAsync(request, userId.Value, cancellationToken);
            return Ok(ApiResult<AiPlanDto>.Success(result));
        }
        catch (InvalidOperationException ex) when (IsAiUnavailable(ex))
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ApiResult<AiPlanDto>.Fail(ex.Message, StatusCodes.Status503ServiceUnavailable));
        }
    }

    [HttpPost("generate-report")]
    [RequireFeature("AI_REPORT")]
    public async Task<ActionResult<ApiResult<AiReportDto>>> GenerateReport([FromBody] GenerateReportRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("用户未认证"));

        try
        {
            var result = await _aiService.GenerateReportAsync(request, userId.Value, cancellationToken);
            return Ok(ApiResult<AiReportDto>.Success(result));
        }
        catch (InvalidOperationException ex) when (IsAiUnavailable(ex))
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ApiResult<AiReportDto>.Fail(ex.Message, StatusCodes.Status503ServiceUnavailable));
        }
    }

    [HttpPost("chat")]
    [RequireFeature("AI_ASSISTANT")]
    public async Task<ActionResult<ApiResult<ChatResponse>>> Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("用户未认证"));

        try
        {
            var result = await _aiService.ChatAsync(request, userId.Value, cancellationToken);
            return Ok(ApiResult<ChatResponse>.Success(result));
        }
        catch (InvalidOperationException ex) when (IsAiUnavailable(ex))
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, ApiResult<ChatResponse>.Fail(ex.Message, StatusCodes.Status503ServiceUnavailable));
        }
    }

    [HttpGet("plans")]
    [RequireFeature("AI_PLANNER")]
    public async Task<ActionResult<ApiResult<PageResult<AiPlanDto>>>> GetPlans([FromQuery] AiQueryDto query, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _aiService.GetPlansAsync(query, userId, cancellationToken);
            return Ok(ApiResult<PageResult<AiPlanDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取AI计划列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("plans/{id:guid}")]
    [RequireFeature("AI_PLANNER")]
    public async Task<ActionResult<ApiResult<AiPlanDto>>> GetPlanById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("用户未认证"));

            var result = await _aiService.GetPlanByIdAsync(id, userId.Value, cancellationToken);
            if (result == null)
                return NotFound(ApiResult<AiPlanDto>.Fail("计划不存在"));
            return Ok(ApiResult<AiPlanDto>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取AI计划详情失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpDelete("plans/{id:guid}")]
    [RequireFeature("AI_PLANNER")]
    public async Task<ActionResult<ApiResult>> DeletePlan(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("用户未认证"));

            var success = await _aiService.DeletePlanAsync(id, userId.Value, cancellationToken);
            if (success)
                _logger.LogInformation("删除AI计划成功: {Id}", id);
            return success
                ? Ok(ApiResult.Success("删除成功"))
                : NotFound(ApiResult.Fail("计划不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除AI计划失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("reports")]
    [RequireFeature("AI_REPORT")]
    public async Task<ActionResult<ApiResult<PageResult<AiReportDto>>>> GetReports([FromQuery] AiQueryDto query, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _aiService.GetReportsAsync(query, userId, cancellationToken);
            return Ok(ApiResult<PageResult<AiReportDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取AI报告列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("reports/{id:guid}")]
    [RequireFeature("AI_REPORT")]
    public async Task<ActionResult<ApiResult<AiReportDto>>> GetReportById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("用户未认证"));

            var result = await _aiService.GetReportByIdAsync(id, userId.Value, cancellationToken);
            if (result == null)
                return NotFound(ApiResult<AiReportDto>.Fail("报告不存在"));
            return Ok(ApiResult<AiReportDto>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取AI报告详情失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpDelete("reports/{id:guid}")]
    [RequireFeature("AI_REPORT")]
    public async Task<ActionResult<ApiResult>> DeleteReport(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("用户未认证"));

            var success = await _aiService.DeleteReportAsync(id, userId.Value, cancellationToken);
            if (success)
                _logger.LogInformation("删除AI报告成功: {Id}", id);
            return success
                ? Ok(ApiResult.Success("删除成功"))
                : NotFound(ApiResult.Fail("报告不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除AI报告失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    [HttpGet("chat/sessions")]
    [RequireFeature("AI_ASSISTANT")]
    public async Task<ActionResult<ApiResult<PageResult<AiChatSessionDto>>>> GetChatSessions([FromQuery] AiQueryDto query, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("用户未认证"));

            var result = await _aiService.GetChatSessionsAsync(query, userId.Value, cancellationToken);
            return Ok(ApiResult<PageResult<AiChatSessionDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取AI聊天会话列表失败");
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpGet("chat/sessions/{sessionId:guid}/messages")]
    [RequireFeature("AI_ASSISTANT")]
    public async Task<ActionResult<ApiResult<List<AiChatMessageDto>>>> GetChatMessages(Guid sessionId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("用户未认证"));

            var result = await _aiService.GetChatMessagesAsync(sessionId, userId.Value, cancellationToken);
            return Ok(ApiResult<List<AiChatMessageDto>>.Success(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取AI聊天消息失败: {SessionId}", sessionId);
            return StatusCode(500, ApiResult.Fail("获取数据失败，请稍后重试"));
        }
    }

    [HttpDelete("chat/sessions/{id:guid}")]
    [RequireFeature("AI_ASSISTANT")]
    public async Task<ActionResult<ApiResult>> DeleteChatSession(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return Unauthorized(ApiResult.Fail("用户未认证"));

            var success = await _aiService.DeleteChatSessionAsync(id, userId.Value, cancellationToken);
            if (success)
                _logger.LogInformation("删除AI聊天会话成功: {Id}", id);
            return success
                ? Ok(ApiResult.Success("删除成功"))
                : NotFound(ApiResult.Fail("会话不存在"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除AI聊天会话失败: {Id}", id);
            return StatusCode(500, ApiResult.Fail("删除失败，请稍后重试"));
        }
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }

    private static bool IsAiUnavailable(InvalidOperationException ex)
    {
        return ex.Message.StartsWith("AI 服务未配置", StringComparison.Ordinal);
    }
}
