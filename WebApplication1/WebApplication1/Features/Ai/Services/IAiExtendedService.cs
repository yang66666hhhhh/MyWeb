using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Ai.Services;

public interface IAiExtendedService
{
    Task<PageResult<AutomationWorkflowDto>> GetWorkflowsAsync(AiExtendedQueryDto query, Guid userId, CancellationToken ct = default);
    Task<AutomationWorkflowDto> CreateWorkflowAsync(CreateAutomationWorkflowInput input, Guid userId, CancellationToken ct = default);
    Task<AutomationWorkflowDto?> UpdateWorkflowAsync(Guid id, UpdateAutomationWorkflowInput input, CancellationToken ct = default);
    Task<bool> DeleteWorkflowAsync(Guid id, CancellationToken ct = default);

    Task<PageResult<KnowledgeChatSessionDto>> GetSessionsAsync(AiExtendedQueryDto query, Guid userId, CancellationToken ct = default);
    Task<KnowledgeChatResponseDto> SendMessageAsync(KnowledgeChatRequest input, Guid userId, CancellationToken ct = default);
    Task<bool> DeleteSessionAsync(Guid id, CancellationToken ct = default);

    Task<PageResult<AiInsightItemDto>> GetInsightsAsync(AiExtendedQueryDto query, Guid userId, CancellationToken ct = default);
    Task<AiInsightItemDto> GenerateInsightAsync(GenerateInsightInput input, Guid userId, CancellationToken ct = default);
    Task<bool> DeleteInsightAsync(Guid id, CancellationToken ct = default);
}
