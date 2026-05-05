using WebApplication1.Features.Ai.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Ai.Services;

public interface IAiService
{
    Task<PageResult<AiPlanDto>> GetPlansAsync(AiQueryDto query, Guid? userId, CancellationToken cancellationToken = default);
    Task<AiPlanDto?> GetPlanByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<AiPlanDto> GeneratePlanAsync(GeneratePlanRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeletePlanAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<PageResult<AiReportDto>> GetReportsAsync(AiQueryDto query, Guid? userId, CancellationToken cancellationToken = default);
    Task<AiReportDto?> GetReportByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    Task<AiReportDto> GenerateReportAsync(GenerateReportRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteReportAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<PageResult<AiChatSessionDto>> GetChatSessionsAsync(AiQueryDto query, Guid userId, CancellationToken cancellationToken = default);
    Task<List<AiChatMessageDto>> GetChatMessagesAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default);
    Task<ChatResponse> ChatAsync(ChatRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteChatSessionAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}
