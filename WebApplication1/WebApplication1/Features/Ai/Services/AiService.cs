using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Ai.Entities;
using WebApplication1.Features.Ai.Dtos;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Enums;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Features.Work.Dtos;

namespace WebApplication1.Features.Ai.Services;

public class AiService : IAiService
{
    private readonly AppDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AiService> _logger;
    private readonly string? _openAiApiKey;
    private readonly string? _openAiModel;

    public AiService(
        AppDbContext context,
        IHttpClientFactory httpClientFactory,
        ILogger<AiService> logger,
        IConfiguration configuration)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _openAiApiKey = configuration["OpenAI:ApiKey"];
        _openAiModel = configuration["OpenAI:Model"] ?? "gpt-3.5-turbo";
    }

    public async Task<PageResult<AiPlanDto>> GetPlansAsync(AiQueryDto query, Guid? userId, CancellationToken cancellationToken = default)
    {
        var q = _context.AiPlans.AsNoTracking().AsQueryable();

        if (userId.HasValue)
            q = q.Where(x => x.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x => x.Title.Contains(query.Keyword) || (x.Description != null && x.Description.Contains(query.Keyword)));

        if (!string.IsNullOrWhiteSpace(query.Type) && Enum.TryParse<AiPlanType>(query.Type, true, out var planType))
            q = q.Where(x => x.Type == planType);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return PageResult<AiPlanDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<AiPlanDto?> GetPlanByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.AiPlans.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<AiPlanDto> GeneratePlanAsync(GeneratePlanRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        var planType = Enum.TryParse<AiPlanType>(request.Type, true, out var pt) ? pt : AiPlanType.Daily;
        var targetDate = DateOnly.TryParse(request.TargetDate, out var td) ? td : DateOnly.FromDateTime(DateTime.Today);

        var recentPlans = await _context.WorkDailyPlans
            .Where(x => x.UserId == userId)
            .Where(x => x.PlanDate >= targetDate.AddDays(-7) && x.PlanDate <= targetDate.AddDays(7))
            .OrderByDescending(x => x.PlanDate)
            .Take(10)
            .ToListAsync(cancellationToken);

        var recentLogs = await _context.WorkLogs
            .Where(x => x.UserId == userId)
            .Where(x => x.WorkDate >= targetDate.AddDays(-7))
            .Include(x => x.Project)
            .OrderByDescending(x => x.WorkDate)
            .Take(10)
            .ToListAsync(cancellationToken);

        string prompt = BuildPlanPrompt(planType, targetDate, request.Description, recentPlans, recentLogs);

        var generatedContent = await CallOpenAiAsync(prompt, cancellationToken);

        var entity = new AiPlan
        {
            UserId = userId,
            Title = $"{targetDate:yyyy-MM-dd]} {GetPlanTypeName(planType)}计划",
            Description = request.Description,
            Type = planType,
            Status = string.IsNullOrEmpty(generatedContent) ? AiPlanStatus.Failed : AiPlanStatus.Completed,
            GeneratedContent = generatedContent
        };

        _context.AiPlans.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<bool> DeletePlanAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.AiPlans.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
        if (entity == null) return false;

        _context.AiPlans.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PageResult<AiReportDto>> GetReportsAsync(AiQueryDto query, Guid? userId, CancellationToken cancellationToken = default)
    {
        var q = _context.AiReports.AsNoTracking().AsQueryable();

        if (userId.HasValue)
            q = q.Where(x => x.UserId == userId.Value);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x => x.Title.Contains(query.Keyword) || (x.Content != null && x.Content.Contains(query.Keyword)));

        if (!string.IsNullOrWhiteSpace(query.Type) && Enum.TryParse<AiReportType>(query.Type, true, out var reportType))
            q = q.Where(x => x.Type == reportType);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return PageResult<AiReportDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<AiReportDto?> GetReportByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.AiReports.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<AiReportDto> GenerateReportAsync(GenerateReportRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        var reportType = Enum.TryParse<AiReportType>(request.Type, true, out var rt) ? rt : AiReportType.Daily;
        var startDate = DateOnly.TryParse(request.StartDate, out var sd) ? sd : DateOnly.FromDateTime(DateTime.Today.AddDays(-7));
        var endDate = DateOnly.TryParse(request.EndDate, out var ed) ? ed : DateOnly.FromDateTime(DateTime.Today);

        var workLogs = await _context.WorkLogs
            .Where(x => x.UserId == userId)
            .Where(x => x.WorkDate >= startDate && x.WorkDate <= endDate)
            .Include(x => x.Project)
            .Include(x => x.Items)
            .ThenInclude(i => i.TaskType)
            .OrderByDescending(x => x.WorkDate)
            .ToListAsync(cancellationToken);

        var dailyPlans = await _context.WorkDailyPlans
            .Where(x => x.UserId == userId)
            .Where(x => x.PlanDate >= startDate && x.PlanDate <= endDate)
            .ToListAsync(cancellationToken);

        string prompt = BuildReportPrompt(reportType, startDate, endDate, workLogs, dailyPlans, request.IncludeStatistics);

        var content = await CallOpenAiAsync(prompt, cancellationToken);

        var entity = new AiReport
        {
            UserId = userId,
            Title = $"{startDate:yyyy-MM-dd} 至 {endDate:yyyy-MM-dd} {GetReportTypeName(reportType)}报告",
            Type = reportType,
            Content = content,
            StartDate = startDate,
            EndDate = endDate
        };

        _context.AiReports.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(entity);
    }

    public async Task<bool> DeleteReportAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var entity = await _context.AiReports.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
        if (entity == null) return false;

        _context.AiReports.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PageResult<AiChatSessionDto>> GetChatSessionsAsync(AiQueryDto query, Guid userId, CancellationToken cancellationToken = default)
    {
        var q = _context.AiChatSessions.AsNoTracking().Where(x => x.UserId == userId);

        var total = await q.CountAsync(cancellationToken);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new AiChatSessionDto
            {
                Id = x.Id,
                UserId = x.UserId,
                Title = x.Title,
                LastMessage = x.LastMessage,
                MessageCount = x.MessageCount,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return PageResult<AiChatSessionDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<List<AiChatMessageDto>> GetChatMessagesAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default)
    {
        var session = await _context.AiChatSessions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == sessionId && x.UserId == userId, cancellationToken);
        if (session == null) return [];

        return await _context.AiChatMessages
            .AsNoTracking()
            .Where(x => x.SessionId == sessionId)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new AiChatMessageDto
            {
                Id = x.Id,
                SessionId = x.SessionId,
                Role = x.Role.ToString(),
                Content = x.Content,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ChatResponse> ChatAsync(ChatRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        var session = request.SessionId.HasValue
            ? await _context.AiChatSessions.FindAsync([request.SessionId.Value], cancellationToken)
            : null;

        if (session == null)
        {
            session = new AiChatSession
            {
                UserId = userId,
                Title = request.Message.Length > 50 ? request.Message[..50] + "..." : request.Message,
                LastMessage = request.Message
            };
            _context.AiChatSessions.Add(session);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var userMessage = new AiChatMessage
        {
            SessionId = session.Id,
            Role = AiMessageRole.User,
            Content = request.Message
        };
        _context.AiChatMessages.Add(userMessage);

        var contextMessages = request.History?
            .Select(h => new ChatMessage { Role = h.Role, Content = h.Content })
            .ToList() ?? new List<ChatMessage>();

        contextMessages.Add(new ChatMessage { Role = "user", Content = request.Message });

        var workContext = await BuildWorkContextAsync(userId, cancellationToken);
        if (!string.IsNullOrEmpty(workContext))
        {
            contextMessages.Insert(0, new ChatMessage { Role = "system", Content = workContext });
        }

        var assistantContent = await CallOpenAiChatAsync(contextMessages, cancellationToken);

        var assistantMessage = new AiChatMessage
        {
            SessionId = session.Id,
            Role = AiMessageRole.Assistant,
            Content = assistantContent
        };
        _context.AiChatMessages.Add(assistantMessage);

        session.LastMessage = request.Message;
        session.MessageCount += 2;

        await _context.SaveChangesAsync(cancellationToken);

        return new ChatResponse
        {
            SessionId = session.Id,
            MessageId = assistantMessage.Id,
            Content = assistantContent,
            Success = true
        };
    }

    public async Task<bool> DeleteChatSessionAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var session = await _context.AiChatSessions.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
        if (session == null) return false;

        var messages = await _context.AiChatMessages.Where(x => x.SessionId == id).ToListAsync(cancellationToken);
        _context.AiChatMessages.RemoveRange(messages);
        _context.AiChatSessions.Remove(session);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<string> BuildWorkContextAsync(Guid userId, CancellationToken cancellationToken)
    {
        var projects = await _context.WorkProjects.AsNoTracking().Where(x => x.UserId == userId).Take(5).ToListAsync(cancellationToken);
        var recentLogs = await _context.WorkLogs
            .Where(x => x.UserId == userId)
            .Where(x => x.WorkDate >= DateOnly.FromDateTime(DateTime.Today.AddDays(-7)))
            .Include(x => x.Project)
            .AsNoTracking()
            .Take(10)
            .ToListAsync(cancellationToken);

        if (!projects.Any() && !recentLogs.Any()) return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("当前用户的工作信息：");
        sb.AppendLine($"项目数量: {projects.Count}");
        foreach (var p in projects)
            sb.AppendLine($"- {p.ProjectName} ({p.Status})");

        sb.AppendLine($"\n最近工作日志 ({recentLogs.Count}条):");
        foreach (var log in recentLogs)
            sb.AppendLine($"- {log.WorkDate}: {log.Title} ({log.TotalHours}h)");

        return sb.ToString();
    }

    private string BuildPlanPrompt(AiPlanType planType, DateOnly targetDate, string? description, List<WorkDailyPlan> recentPlans, List<WorkLog> recentLogs)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"请为 {targetDate:yyyy年MM月dd日} 生成一个");
        sb.AppendLine($"{GetPlanTypeName(planType)}计划。");
        sb.AppendLine($"计划类型: {GetPlanTypeName(planType)}");

        if (!string.IsNullOrWhiteSpace(description))
            sb.AppendLine($"\n用户需求: {description}");

        if (recentPlans.Any())
        {
            sb.AppendLine("\n用户最近的工作计划:");
            foreach (var p in recentPlans.Take(5))
                sb.AppendLine($"- [{p.PlanDate}] {p.Title}: {p.Content}");
        }

        if (recentLogs.Any())
        {
            sb.AppendLine("\n用户最近的工作日志:");
            foreach (var log in recentLogs.Take(5))
                sb.AppendLine($"- [{log.WorkDate}] {log.Title} ({log.TotalHours}h)");
        }

        sb.AppendLine("\n请生成结构化的计划，包括:");
        sb.AppendLine("1. 目标设定");
        sb.AppendLine("2. 具体任务列表 (包含优先级)");
        sb.AppendLine("3. 时间安排建议");
        sb.AppendLine("4. 预期成果");

        return sb.ToString();
    }

    private string BuildReportPrompt(AiReportType reportType, DateOnly startDate, DateOnly endDate, List<WorkLog> workLogs, List<WorkDailyPlan> dailyPlans, bool includeStats)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"请生成 {startDate:yyyy年MM月dd日} 至 {endDate:yyyy年MM月dd日} 的");
        sb.AppendLine($"{GetReportTypeName(reportType)}报告。");

        if (includeStats && workLogs.Any())
        {
            sb.AppendLine($"\n工作统计:");
            sb.AppendLine($"- 工作日志总数: {workLogs.Count}");
            sb.AppendLine($"- 总工时: {workLogs.Sum(x => x.TotalHours):F1}h");
            sb.AppendLine($"- 涉及项目: {workLogs.Select(x => x.ProjectId).Distinct().Count()}");

            var projectHours = workLogs
                .GroupBy(x => x.Project?.ProjectName ?? "未知")
                .Select(g => new { Project = g.Key, Hours = g.Sum(x => x.TotalHours) })
                .OrderByDescending(x => x.Hours);

            sb.AppendLine("\n各项目工时:");
            foreach (var p in projectHours.Take(5))
                sb.AppendLine($"- {p.Project}: {p.Hours:F1}h");
        }

        if (dailyPlans.Any())
        {
            var completed = dailyPlans.Count(x => x.Status == WorkDailyPlanStatus.Completed);
            var total = dailyPlans.Count;
            sb.AppendLine($"\n计划完成情况: {completed}/{total} ({completed * 100 / Math.Max(1, total)}%)");
        }

        if (workLogs.Any())
        {
            sb.AppendLine("\n工作日志摘要:");
            foreach (var log in workLogs.Take(5))
                sb.AppendLine($"- [{log.WorkDate}] {log.Title}: {log.Summary ?? log.OriginalContent}");
        }

        sb.AppendLine("\n请生成分析报告，包括:");
        sb.AppendLine("1. 工作总结");
        sb.AppendLine("2. 成果与亮点");
        sb.AppendLine("3. 问题与改进");
        sb.AppendLine("4. 下一步建议");

        return sb.ToString();
    }

    private async Task<string> CallOpenAiAsync(string prompt, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_openAiApiKey))
        {
            _logger.LogWarning("OpenAI API key not configured, returning mock response");
            return GetMockPlanResponse(prompt);
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiApiKey}");

            var requestBody = new
            {
                model = _openAiModel,
                messages = new[] { new { role = "user", content = prompt } },
                max_tokens = 2000,
                temperature = 0.7
            };

            var response = await client.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("OpenAI API returned {StatusCode}", response.StatusCode);
                return $"API调用失败 ({(int)response.StatusCode})，请检查API配置";
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return content ?? "AI返回为空";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call OpenAI API");
            return "AI服务调用失败，请稍后重试";
        }
    }

    private async Task<string> CallOpenAiChatAsync(List<ChatMessage> messages, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_openAiApiKey))
        {
            _logger.LogWarning("OpenAI API key not configured, returning mock response");
            return GetMockChatResponse();
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiApiKey}");

            var requestBody = new
            {
                model = _openAiModel,
                messages = messages,
                max_tokens = 1500,
                temperature = 0.8
            };

            var response = await client.PostAsync(
                "https://api.openai.com/v1/chat/completions",
                new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json"),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("OpenAI API returned {StatusCode}", response.StatusCode);
                return $"抱歉，AI服务暂时不可用 ({(int)response.StatusCode})";
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);
            var content = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return content ?? "AI返回为空";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call OpenAI Chat API");
            return "抱歉，AI服务暂时不可用，请稍后重试";
        }
    }

    private string GetMockPlanResponse(string prompt)
    {
        return @"## AI 智能计划

### 目标设定
1. 完成主要工作任务
2. 跟进项目进度
3. 提升专业技能

### 具体任务
| 优先级 | 任务 | 预计时长 |
|--------|------|----------|
| 高 | 重要会议准备 | 2h |
| 高 | 核心功能开发 | 4h |
| 中 | 文档整理 | 1h |
| 低 | 学习新技术 | 1h |

### 时间安排
- 上午: 重要会议准备、核心功能开发
- 下午: 核心功能收尾、文档整理
- 晚间: 学习与反思

### 预期成果
- 完成今日主要工作目标
- 产出一份完整的会议材料
- 掌握一项新技术要点

---
*此为模拟响应，配置OpenAI API Key后可获得真实AI生成内容*";
    }

    private string GetMockChatResponse()
    {
        var responses = new[]
        {
            "您好！我是您的AI助手。根据您的工作数据，我可以帮您分析工作进度、生成计划或报告。有什么可以帮您的吗？",
            "根据您最近的工作日志，您在项目开发上投入了较多时间。建议适当平衡各项目的时间分配。",
            "您的习惯打卡记录显示，连续坚持很重要。建议每天固定时间进行习惯培养。",
            "我可以帮您生成每日/周/月度计划，或者根据您的工作数据生成分析报告。请问需要什么服务？"
        };
        return responses[Random.Shared.Next(responses.Length)];
    }

    private static string GetPlanTypeName(AiPlanType type) => type switch
    {
        AiPlanType.Daily => "每日",
        AiPlanType.Weekly => "每周",
        AiPlanType.Monthly => "每月",
        AiPlanType.Project => "项目",
        AiPlanType.Custom => "自定义",
        _ => "每日"
    };

    private static string GetReportTypeName(AiReportType type) => type switch
    {
        AiReportType.Daily => "每日",
        AiReportType.Weekly => "每周",
        AiReportType.Monthly => "每月",
        AiReportType.Project => "项目",
        AiReportType.Custom => "自定义",
        _ => "每日"
    };

    private static AiPlanDto MapToDto(AiPlan entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        Title = entity.Title,
        Description = entity.Description,
        Type = entity.Type.ToString(),
        Status = entity.Status.ToString(),
        GeneratedContent = entity.GeneratedContent,
        Remark = entity.Remark,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static AiReportDto MapToDto(AiReport entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        Title = entity.Title,
        Type = entity.Type.ToString(),
        Content = entity.Content,
        Remark = entity.Remark,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };
}
