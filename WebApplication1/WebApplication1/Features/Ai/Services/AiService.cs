using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Ai.Entities;
using WebApplication1.Features.Ai.Dtos;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Enums;
using WebApplication1.Features.Tasks;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Features.Work.Dtos;

namespace WebApplication1.Features.Ai.Services;

public class AiService : IAiService
{
    private sealed class AiReportRemarkMeta
    {
        public Guid? RelatedProjectId { get; set; }

        public string? RelatedProjectName { get; set; }
    }

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
        _openAiApiKey = configuration["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
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

        var recentPlans = await _context.Tasks
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

        if (DateOnly.TryParse(query.StartDate, out var startDate))
            q = q.Where(x => !x.StartDate.HasValue || x.StartDate >= startDate);

        if (DateOnly.TryParse(query.EndDate, out var endDate))
            q = q.Where(x => !x.EndDate.HasValue || x.EndDate <= endDate);

        var reportEntities = await q
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);

        var mappedReports = reportEntities
            .Select(MapToDto)
            .Where(x => !query.RelatedProjectId.HasValue || x.RelatedProjectId == query.RelatedProjectId.Value)
            .ToList();

        var total = mappedReports.Count;
        var items = mappedReports
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

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

        var workLogsQuery = _context.WorkLogs
            .Where(x => x.UserId == userId)
            .Where(x => x.WorkDate >= startDate && x.WorkDate <= endDate);

        if (request.RelatedProjectId.HasValue)
            workLogsQuery = workLogsQuery.Where(x => x.ProjectId == request.RelatedProjectId.Value);

        var workLogs = await workLogsQuery
            .Include(x => x.Project)
            .Include(x => x.Items)
            .ThenInclude(i => i.TaskType)
            .OrderByDescending(x => x.WorkDate)
            .ToListAsync(cancellationToken);

        var implLogsQuery = _context.ImplLogs
            .Where(x => x.UserId == userId)
            .Where(x => x.WorkDate >= startDate && x.WorkDate <= endDate);

        if (request.RelatedProjectId.HasValue)
            implLogsQuery = implLogsQuery.Where(x => x.ProjectId == request.RelatedProjectId.Value);

        var implLogs = await implLogsQuery
            .Include(x => x.Project)
            .OrderByDescending(x => x.WorkDate)
            .ToListAsync(cancellationToken);

        var dailyPlansQuery = _context.Tasks
            .Where(x => x.UserId == userId)
            .Where(x => x.PlanDate >= startDate && x.PlanDate <= endDate);

        if (request.RelatedProjectId.HasValue)
            dailyPlansQuery = dailyPlansQuery.Where(x => x.ProjectId == request.RelatedProjectId.Value);

        var dailyPlans = await dailyPlansQuery
            .ToListAsync(cancellationToken);

        string prompt = BuildReportPrompt(reportType, startDate, endDate, workLogs, implLogs, dailyPlans, request.IncludeStatistics);

        var content = await CallOpenAiAsync(prompt, cancellationToken);

        var entity = new AiReport
        {
            UserId = userId,
            Title = $"{startDate:yyyy-MM-dd} 至 {endDate:yyyy-MM-dd} {GetReportTypeName(reportType)}报告",
            Type = reportType,
            Content = content,
            Remark = BuildReportRemark(request.RelatedProjectId, ResolveRelatedProjectName(workLogs, implLogs)),
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

        var recentImplLogs = await _context.ImplLogs
            .Where(x => x.UserId == userId)
            .Where(x => x.WorkDate >= DateOnly.FromDateTime(DateTime.Today.AddDays(-7)))
            .Include(x => x.Project)
            .OrderByDescending(x => x.WorkDate)
            .AsNoTracking()
            .Take(10)
            .ToListAsync(cancellationToken);

        if (!projects.Any() && !recentLogs.Any() && !recentImplLogs.Any()) return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("当前用户的工作信息：");
        sb.AppendLine($"项目数量: {projects.Count}");
        foreach (var p in projects)
            sb.AppendLine($"- {p.ProjectName} ({p.Status})");

        sb.AppendLine($"\n最近工作日志 ({recentLogs.Count}条):");
        foreach (var log in recentLogs)
            sb.AppendLine($"- {log.WorkDate}: {log.Title} ({log.TotalHours}h)");

        if (recentImplLogs.Any())
        {
            sb.AppendLine($"\n最近实施日志 ({recentImplLogs.Count}条):");
            foreach (var log in recentImplLogs)
                sb.AppendLine($"- {log.WorkDate}: {log.Title} [{GetProjectDisplayName(log)}] ({log.TotalHours}h)");
        }

        return sb.ToString();
    }

    private string BuildPlanPrompt(AiPlanType planType, DateOnly targetDate, string? description, List<TaskItem> recentPlans, List<WorkLog> recentLogs)
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
                sb.AppendLine($"- [{p.PlanDate}] {p.Title}: {p.Description}");
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

    private string BuildReportPrompt(
        AiReportType reportType,
        DateOnly startDate,
        DateOnly endDate,
        List<WorkLog> workLogs,
        List<ImplLog> implLogs,
        List<TaskItem> dailyPlans,
        bool includeStats)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"请生成 {startDate:yyyy年MM月dd日} 至 {endDate:yyyy年MM月dd日} 的");
        sb.AppendLine($"{GetReportTypeName(reportType)}报告。");

        if (includeStats && (workLogs.Any() || implLogs.Any()))
        {
            var totalLogCount = workLogs.Count + implLogs.Count;
            var totalHours = workLogs.Sum(x => x.TotalHours) + implLogs.Sum(x => x.TotalHours);
            var projectCount = workLogs
                .Select(x => (Guid?)x.ProjectId)
                .Concat(implLogs.Select(x => x.ProjectId))
                .Where(x => x.HasValue)
                .Distinct()
                .Count();

            sb.AppendLine($"\n工作统计:");
            sb.AppendLine($"- 日志记录总数: {totalLogCount}");
            sb.AppendLine($"- 工作日志总数: {workLogs.Count}");
            sb.AppendLine($"- 实施日志总数: {implLogs.Count}");
            sb.AppendLine($"- 总工时: {totalHours:F1}h");
            sb.AppendLine($"- 涉及项目: {projectCount}");

            var projectHours = workLogs
                .Select(x => new { ProjectId = (Guid?)x.ProjectId, Project = GetProjectDisplayName(x), Hours = x.TotalHours })
                .Concat(implLogs.Select(x => new { ProjectId = x.ProjectId, Project = GetProjectDisplayName(x), Hours = x.TotalHours }))
                .GroupBy(x => new { x.ProjectId, x.Project })
                .Select(g => new { g.Key.Project, Hours = g.Sum(x => x.Hours) })
                .OrderByDescending(x => x.Hours);

            sb.AppendLine("\n各项目工时:");
            foreach (var p in projectHours.Take(5))
                sb.AppendLine($"- {p.Project}: {p.Hours:F1}h");
        }

        if (dailyPlans.Any())
        {
            var completed = dailyPlans.Count(x => x.Status == TaskItemStatus.Completed);
            var total = dailyPlans.Count;
            sb.AppendLine($"\n计划完成情况: {completed}/{total} ({completed * 100 / Math.Max(1, total)}%)");
        }

        if (workLogs.Any())
        {
            sb.AppendLine("\n工作日志摘要:");
            foreach (var log in workLogs.Take(5))
                sb.AppendLine($"- [{log.WorkDate}] {log.Title}: {log.Summary ?? log.OriginalContent ?? "无摘要"}");
        }

        if (implLogs.Any())
        {
            sb.AppendLine("\n实施日志摘要:");
            foreach (var log in implLogs.Take(5))
                sb.AppendLine($"- [{log.WorkDate}] {log.Title} [{GetProjectDisplayName(log)}] ({log.TotalHours}h)");
        }

        sb.AppendLine("\n请生成分析报告，包括:");
        sb.AppendLine("1. 工作总结");
        sb.AppendLine("2. 成果与亮点");
        sb.AppendLine("3. 问题与改进");
        sb.AppendLine("4. 下一步建议");

        return sb.ToString();
    }

    protected virtual async Task<string> CallOpenAiAsync(string prompt, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_openAiApiKey))
        {
            _logger.LogWarning("OpenAI API key not configured");
            throw new InvalidOperationException("AI 服务未配置 OpenAI API Key，无法生成真实内容");
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

    protected virtual async Task<string> CallOpenAiChatAsync(List<ChatMessage> messages, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_openAiApiKey))
        {
            _logger.LogWarning("OpenAI API key not configured");
            throw new InvalidOperationException("AI 服务未配置 OpenAI API Key，无法生成真实回复");
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
        // remark 当前被复用为报告关联项目元信息，保留原字段便于兼容旧数据读取。
        Id = entity.Id,
        UserId = entity.UserId,
        Title = entity.Title,
        Type = entity.Type.ToString(),
        Content = entity.Content,
        Remark = entity.Remark,
        RelatedProjectId = ParseReportRemark(entity.Remark)?.RelatedProjectId,
        RelatedProjectName = ParseReportRemark(entity.Remark)?.RelatedProjectName,
        StartDate = entity.StartDate,
        EndDate = entity.EndDate,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    private static string GetProjectDisplayName(WorkLog workLog) => workLog.Project?.ProjectName ?? "未知";

    private static string GetProjectDisplayName(ImplLog implLog) => implLog.Project?.ProjectName ?? implLog.ProjectName ?? "未知";

    private static string? ResolveRelatedProjectName(List<WorkLog> workLogs, List<ImplLog> implLogs)
    {
        var workProjectName = workLogs
            .Select(x => x.Project?.ProjectName)
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        if (!string.IsNullOrWhiteSpace(workProjectName))
            return workProjectName;

        return implLogs
            .Select(x => x.Project?.ProjectName ?? x.ProjectName)
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
    }

    private static string? BuildReportRemark(Guid? relatedProjectId, string? relatedProjectName)
    {
        if (!relatedProjectId.HasValue && string.IsNullOrWhiteSpace(relatedProjectName))
            return null;

        return JsonSerializer.Serialize(new AiReportRemarkMeta
        {
            RelatedProjectId = relatedProjectId,
            RelatedProjectName = relatedProjectName,
        });
    }

    private static AiReportRemarkMeta? ParseReportRemark(string? remark)
    {
        if (string.IsNullOrWhiteSpace(remark))
            return null;

        try
        {
            return JsonSerializer.Deserialize<AiReportRemarkMeta>(remark);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
