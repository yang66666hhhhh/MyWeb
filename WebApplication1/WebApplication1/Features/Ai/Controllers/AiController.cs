using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Features.Ai.Dtos;
using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Ai.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public class AiController : ControllerBase
{
    [HttpPost("generate-plan")]
    public ActionResult<ApiResult<AiPlanDto>> GeneratePlan([FromBody] GeneratePlanRequest request)
    {
        return Ok(ApiResult<AiPlanDto>.Success(new AiPlanDto
        {
            Title = "AI 生成的计划",
            Description = request.Description,
            Type = request.Type,
            Status = "completed",
            GeneratedContent = "这是 AI 生成的计划内容..."
        }));
    }

    [HttpPost("generate-report")]
    public ActionResult<ApiResult<AiReportDto>> GenerateReport([FromBody] GenerateReportRequest request)
    {
        return Ok(ApiResult<AiReportDto>.Success(new AiReportDto
        {
            Title = "AI 生成的报告",
            Type = request.Type,
            Content = "这是 AI 生成的报告内容..."
        }));
    }

    [HttpPost("chat")]
    public ActionResult<ApiResult<AiChatMessageDto>> Chat([FromBody] ChatRequest request)
    {
        return Ok(ApiResult<AiChatMessageDto>.Success(new AiChatMessageDto
        {
            Role = "assistant",
            Content = "这是 AI 的回复..."
        }));
    }

    [HttpGet("plans")]
    public ActionResult<ApiResult<PageResult<AiPlanDto>>> GetPlans([FromQuery] AiQueryDto query)
    {
        return Ok(ApiResult<PageResult<AiPlanDto>>.Success(new PageResult<AiPlanDto>
        {
            Items = new List<AiPlanDto>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }

    [HttpGet("reports")]
    public ActionResult<ApiResult<PageResult<AiReportDto>>> GetReports([FromQuery] AiQueryDto query)
    {
        return Ok(ApiResult<PageResult<AiReportDto>>.Success(new PageResult<AiReportDto>
        {
            Items = new List<AiReportDto>(),
            Total = 0,
            Page = query.Page,
            PageSize = query.PageSize
        }));
    }
}
