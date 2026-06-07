using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Ai.Services;
using WebApplication1.Features.Auth.Authorization;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Ai;

[ApiController]
[Authorize]
[Route("api/ai")]
[Tags("AI")]
public class AiExtendedController : BaseApiController
{
    private readonly IAiExtendedService _service;

    public AiExtendedController(IAiExtendedService service)
    {
        _service = service;
    }

    // ─── Automation endpoints ────────────────────────────────────

    [HttpGet("automation")]
    [RequireFeature("AI_AUTOMATION")]
    public async Task<ActionResult<ApiResult<PageResult<AutomationWorkflowDto>>>> GetWorkflows(
        [FromQuery] AiExtendedQueryDto query, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await _service.GetWorkflowsAsync(query, userId.Value, ct);
        return Ok(ApiResult<PageResult<AutomationWorkflowDto>>.Success(result));
    }

    [HttpPost("automation")]
    [RequireFeature("AI_AUTOMATION")]
    public async Task<ActionResult<ApiResult<AutomationWorkflowDto>>> CreateWorkflow(
        [FromBody] CreateAutomationWorkflowInput input, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await _service.CreateWorkflowAsync(input, userId.Value, ct);
        return Ok(ApiResult<AutomationWorkflowDto>.Success(result));
    }

    [HttpPut("automation/{id:guid}")]
    [RequireFeature("AI_AUTOMATION")]
    public async Task<ActionResult<ApiResult<AutomationWorkflowDto>>> UpdateWorkflow(
        Guid id, [FromBody] UpdateAutomationWorkflowInput input, CancellationToken ct)
    {
        var result = await _service.UpdateWorkflowAsync(id, input, ct);
        if (result == null)
            return NotFound(ApiResult<AutomationWorkflowDto>.Fail("工作流不存在", StatusCodes.Status404NotFound));
        return Ok(ApiResult<AutomationWorkflowDto>.Success(result));
    }

    [HttpDelete("automation/{id:guid}")]
    [RequireFeature("AI_AUTOMATION")]
    public async Task<ActionResult<ApiResult>> DeleteWorkflow(Guid id, CancellationToken ct)
    {
        var success = await _service.DeleteWorkflowAsync(id, ct);
        return HandleDeleteResult(success, "工作流");
    }

    // ─── Knowledge Chat endpoints ────────────────────────────────

    [HttpGet("knowledge-chat/sessions")]
    [RequireFeature("AI_KNOWLEDGE_CHAT")]
    public async Task<ActionResult<ApiResult<PageResult<KnowledgeChatSessionDto>>>> GetKnowledgeChatSessions(
        [FromQuery] AiExtendedQueryDto query, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await _service.GetSessionsAsync(query, userId.Value, ct);
        return Ok(ApiResult<PageResult<KnowledgeChatSessionDto>>.Success(result));
    }

    [HttpPost("knowledge-chat")]
    [RequireFeature("AI_KNOWLEDGE_CHAT")]
    public async Task<ActionResult<ApiResult<KnowledgeChatResponseDto>>> SendKnowledgeChatMessage(
        [FromBody] KnowledgeChatRequest input, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await _service.SendMessageAsync(input, userId.Value, ct);
        return Ok(ApiResult<KnowledgeChatResponseDto>.Success(result));
    }

    [HttpDelete("knowledge-chat/sessions/{id:guid}")]
    [RequireFeature("AI_KNOWLEDGE_CHAT")]
    public async Task<ActionResult<ApiResult>> DeleteKnowledgeChatSession(Guid id, CancellationToken ct)
    {
        var success = await _service.DeleteSessionAsync(id, ct);
        return HandleDeleteResult(success, "会话");
    }

    // ─── Insights endpoints ──────────────────────────────────────

    [HttpGet("insights")]
    [RequireFeature("AI_INSIGHTS")]
    public async Task<ActionResult<ApiResult<PageResult<AiInsightItemDto>>>> GetInsights(
        [FromQuery] AiExtendedQueryDto query, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await _service.GetInsightsAsync(query, userId.Value, ct);
        return Ok(ApiResult<PageResult<AiInsightItemDto>>.Success(result));
    }

    [HttpPost("insights/generate")]
    [RequireFeature("AI_INSIGHTS")]
    public async Task<ActionResult<ApiResult<AiInsightItemDto>>> GenerateInsight(
        [FromBody] GenerateInsightInput input, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
            return Unauthorized(ApiResult.Fail("无法获取用户信息"));
        var result = await _service.GenerateInsightAsync(input, userId.Value, ct);
        return Ok(ApiResult<AiInsightItemDto>.Success(result));
    }

    [HttpDelete("insights/{id:guid}")]
    [RequireFeature("AI_INSIGHTS")]
    public async Task<ActionResult<ApiResult>> DeleteInsight(Guid id, CancellationToken ct)
    {
        var success = await _service.DeleteInsightAsync(id, ct);
        return HandleDeleteResult(success, "洞察");
    }
}

// ─── DTOs ────────────────────────────────────────────────────────

public record AutomationWorkflowDto
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? TriggerType { get; init; }
    public string? Actions { get; init; }
    public bool IsActive { get; init; } = true;
    public string? LastRunAt { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record CreateAutomationWorkflowInput
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? TriggerType { get; init; }
    public string? Actions { get; init; }
}

public record UpdateAutomationWorkflowInput
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? TriggerType { get; init; }
    public string? Actions { get; init; }
    public bool? IsActive { get; init; }
}

public record KnowledgeChatSessionDto
{
    public string Id { get; init; } = string.Empty;
    public string? Title { get; init; }
    public string? LastMessage { get; init; }
    public int MessageCount { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record KnowledgeChatResponseDto
{
    public string SessionId { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string? Sources { get; init; }
}

public record KnowledgeChatRequest
{
    public string Message { get; init; } = string.Empty;
    public string? SessionId { get; init; }
}

public record AiInsightItemDto
{
    public string Id { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Content { get; init; }
    public string? Category { get; init; }
    public string? Source { get; init; }
    public string CreatedAt { get; init; } = string.Empty;
}

public record GenerateInsightInput
{
    public string? Title { get; init; }
    public string? Category { get; init; }
    public string? Source { get; init; }
}

public class AiExtendedQueryDto : PageQueryDto
{
    public string? Type { get; init; }
    public string? Keyword { get; init; }
}
