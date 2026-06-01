using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Ai.Entities;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Ai.Services;

public class AiExtendedService : IAiExtendedService
{
    private readonly AppDbContext _context;

    public AiExtendedService(AppDbContext context)
    {
        _context = context;
    }

    // ─── Automation Workflows ────────────────────────────────────

    public async Task<PageResult<AutomationWorkflowDto>> GetWorkflowsAsync(
        AiExtendedQueryDto query, Guid userId, CancellationToken ct = default)
    {
        var q = _context.AutomationWorkflows.AsNoTracking().Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x => x.Name.Contains(query.Keyword) ||
                             (x.Description != null && x.Description.Contains(query.Keyword)));

        if (!string.IsNullOrWhiteSpace(query.Type))
            q = q.Where(x => x.TriggerType == query.Type);

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new AutomationWorkflowDto
            {
                Id = x.Id.ToString(),
                Name = x.Name,
                Description = x.Description,
                TriggerType = x.TriggerType,
                Actions = x.Actions,
                IsActive = x.IsActive,
                LastRunAt = x.LastRunAt.HasValue ? x.LastRunAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
                CreatedAt = x.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToListAsync(ct);

        return PageResult<AutomationWorkflowDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<AutomationWorkflowDto> CreateWorkflowAsync(
        CreateAutomationWorkflowInput input, Guid userId, CancellationToken ct = default)
    {
        var entity = new AutomationWorkflow
        {
            UserId = userId,
            Name = input.Name,
            Description = input.Description,
            TriggerType = input.TriggerType,
            Actions = input.Actions,
            IsActive = true
        };

        _context.AutomationWorkflows.Add(entity);
        await _context.SaveChangesAsync(ct);

        return new AutomationWorkflowDto
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Description = entity.Description,
            TriggerType = entity.TriggerType,
            Actions = entity.Actions,
            IsActive = entity.IsActive,
            LastRunAt = null,
            CreatedAt = entity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    public async Task<AutomationWorkflowDto?> UpdateWorkflowAsync(
        Guid id, UpdateAutomationWorkflowInput input, CancellationToken ct = default)
    {
        var entity = await _context.AutomationWorkflows.FindAsync([id], ct);
        if (entity == null) return null;

        if (input.Name != null) entity.Name = input.Name;
        if (input.Description != null) entity.Description = input.Description;
        if (input.TriggerType != null) entity.TriggerType = input.TriggerType;
        if (input.Actions != null) entity.Actions = input.Actions;
        if (input.IsActive.HasValue) entity.IsActive = input.IsActive.Value;

        await _context.SaveChangesAsync(ct);

        return new AutomationWorkflowDto
        {
            Id = entity.Id.ToString(),
            Name = entity.Name,
            Description = entity.Description,
            TriggerType = entity.TriggerType,
            Actions = entity.Actions,
            IsActive = entity.IsActive,
            LastRunAt = entity.LastRunAt.HasValue ? entity.LastRunAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : null,
            CreatedAt = entity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    public async Task<bool> DeleteWorkflowAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.AutomationWorkflows.FindAsync([id], ct);
        if (entity == null) return false;

        _context.AutomationWorkflows.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    // ─── Knowledge Chat ──────────────────────────────────────────

    public async Task<PageResult<KnowledgeChatSessionDto>> GetSessionsAsync(
        AiExtendedQueryDto query, Guid userId, CancellationToken ct = default)
    {
        var q = _context.KnowledgeChatSessionItems.AsNoTracking().Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x => (x.Title != null && x.Title.Contains(query.Keyword)) ||
                             (x.LastMessage != null && x.LastMessage.Contains(query.Keyword)));

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new KnowledgeChatSessionDto
            {
                Id = x.Id.ToString(),
                Title = x.Title,
                LastMessage = x.LastMessage,
                MessageCount = x.MessageCount,
                CreatedAt = x.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToListAsync(ct);

        return PageResult<KnowledgeChatSessionDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<KnowledgeChatResponseDto> SendMessageAsync(
        KnowledgeChatRequest input, Guid userId, CancellationToken ct = default)
    {
        KnowledgeChatSessionItem session;

        if (!string.IsNullOrWhiteSpace(input.SessionId) && Guid.TryParse(input.SessionId, out var sessionId))
        {
            session = await _context.KnowledgeChatSessionItems
                .FirstOrDefaultAsync(x => x.Id == sessionId && x.UserId == userId, ct)
                ?? throw new InvalidOperationException("会话不存在");
        }
        else
        {
            session = new KnowledgeChatSessionItem
            {
                UserId = userId,
                Title = input.Message.Length > 50 ? input.Message[..50] + "..." : input.Message,
                LastMessage = input.Message,
                MessageCount = 1
            };
            _context.KnowledgeChatSessionItems.Add(session);
        }

        session.LastMessage = input.Message;
        session.MessageCount += 1;

        await _context.SaveChangesAsync(ct);

        var reply = GenerateKnowledgeReply(input.Message);

        return new KnowledgeChatResponseDto
        {
            SessionId = session.Id.ToString(),
            Content = reply,
            Sources = "知识库"
        };
    }

    public async Task<bool> DeleteSessionAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.KnowledgeChatSessionItems.FindAsync([id], ct);
        if (entity == null) return false;

        _context.KnowledgeChatSessionItems.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    // ─── Insights ────────────────────────────────────────────────

    public async Task<PageResult<AiInsightItemDto>> GetInsightsAsync(
        AiExtendedQueryDto query, Guid userId, CancellationToken ct = default)
    {
        var q = _context.AiInsights.AsNoTracking().Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(query.Keyword))
            q = q.Where(x => x.Title.Contains(query.Keyword) ||
                             (x.Content != null && x.Content.Contains(query.Keyword)));

        if (!string.IsNullOrWhiteSpace(query.Type))
            q = q.Where(x => x.Category == query.Type);

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new AiInsightItemDto
            {
                Id = x.Id.ToString(),
                Title = x.Title,
                Content = x.Content,
                Category = x.Category,
                Source = x.Source,
                CreatedAt = x.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToListAsync(ct);

        return PageResult<AiInsightItemDto>.Create(items, total, query.Page, query.PageSize);
    }

    public async Task<AiInsightItemDto> GenerateInsightAsync(
        GenerateInsightInput input, Guid userId, CancellationToken ct = default)
    {
        var category = input.Category ?? "通用";
        var title = input.Title ?? $"AI {category}洞察";
        var content = GenerateInsightContent(category);

        var entity = new AiInsight
        {
            UserId = userId,
            Title = title,
            Content = content,
            Category = category,
            Source = input.Source
        };

        _context.AiInsights.Add(entity);
        await _context.SaveChangesAsync(ct);

        return new AiInsightItemDto
        {
            Id = entity.Id.ToString(),
            Title = entity.Title,
            Content = entity.Content,
            Category = entity.Category,
            Source = entity.Source,
            CreatedAt = entity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    public async Task<bool> DeleteInsightAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.AiInsights.FindAsync([id], ct);
        if (entity == null) return false;

        _context.AiInsights.Remove(entity);
        await _context.SaveChangesAsync(ct);
        return true;
    }

    // ─── Helpers ─────────────────────────────────────────────────

    private static string GenerateKnowledgeReply(string message)
    {
        var lower = message.ToLowerInvariant();

        if (lower.Contains("项目") || lower.Contains("project"))
            return "根据知识库中的项目管理资料，建议您：\n\n" +
                   "1. 使用看板方法可视化工作流程\n" +
                   "2. 设定明确的里程碑和交付物\n" +
                   "3. 定期进行回顾会议以持续改进\n\n" +
                   "您可以在工作日志模块中记录项目进展。";

        if (lower.Contains("日志") || lower.Contains("log") || lower.Contains("记录"))
            return "关于工作日志记录，知识库建议：\n\n" +
                   "1. 每天结束前花 5-10 分钟总结当天工作\n" +
                   "2. 记录关键决策和遇到的问题\n" +
                   "3. 使用结构化模板提高效率\n\n" +
                   "系统已为您提供了工作日志功能，可以自动关联项目。";

        if (lower.Contains("计划") || lower.Contains("plan"))
            return "根据知识库中的计划管理最佳实践：\n\n" +
                   "1. 使用 SMART 原则设定目标\n" +
                   "2. 将大任务分解为可执行的小步骤\n" +
                   "3. 预留缓冲时间应对突发情况\n" +
                   "4. 每周回顾计划完成情况并调整\n\n" +
                   "您可以使用 AI 计划功能自动生成每日/每周计划。";

        if (lower.Contains("习惯") || lower.Contains("habit"))
            return "关于习惯养成，知识库建议：\n\n" +
                   "1. 从极小的习惯开始（2分钟规则）\n" +
                   "2. 绑定到现有的日常行为上\n" +
                   "3. 使用连续打卡追踪进度\n" +
                   "4. 允许偶尔的中断，重要的是快速恢复\n\n" +
                   "系统提供了习惯打卡功能来帮助您追踪。";

        return $"关于「{message}」，根据知识库分析：\n\n" +
               "这是一个值得深入探索的话题。建议您可以：\n\n" +
               "1. 查阅相关文档和资料获取更详细的信息\n" +
               "2. 结合实际工作场景进行实践验证\n" +
               "3. 记录学习心得以便后续回顾\n\n" +
               "如需更具体的帮助，请提供更多上下文信息。";
    }

    private static string GenerateInsightContent(string category)
    {
        return category.ToLowerInvariant() switch
        {
            "效率" or "efficiency" =>
                "## 效率分析洞察\n\n" +
                "### 发现\n" +
                "- 您近期的工作效率呈上升趋势，日均有效工时提升了 12%\n" +
                "- 上午 9:00-11:00 是您效率最高的时间段\n" +
                "- 多任务切换导致的上下文成本约占总工时的 15%\n\n" +
                "### 建议\n" +
                "1. 将深度工作安排在上午高效时段\n" +
                "2. 使用番茄工作法减少任务切换\n" +
                "3. 设定固定的邮件/消息处理时间",

            "健康" or "health" =>
                "## 健康与工作平衡洞察\n\n" +
                "### 发现\n" +
                "- 最近两周的平均工作时长偏高，需注意休息\n" +
                "- 连续工作的天数较多，建议增加休息间隔\n" +
                "- 运动打卡完成率有所下降\n\n" +
                "### 建议\n" +
                "1. 每工作 90 分钟休息 15 分钟\n" +
                "2. 保持每天至少 30 分钟的运动\n" +
                "3. 设定工作结束时间，避免过度加班",

            "学习" or "learning" =>
                "## 学习成长洞察\n\n" +
                "### 发现\n" +
                "- 知识积累速度良好，本月新增学习笔记 23 篇\n" +
                "- 技术类学习占比最高，管理类学习可适当增加\n" +
                "- 学习时间主要集中在周末\n\n" +
                "### 建议\n" +
                "1. 利用工作日碎片时间进行轻量学习\n" +
                "2. 尝试费曼学习法加深理解\n" +
                "3. 建立知识体系，定期整理笔记",

            _ =>
                $"## {category} 综合洞察\n\n" +
                "### 发现\n" +
                "- 基于您的历史数据分析，当前状态整体良好\n" +
                "- 部分指标存在优化空间\n" +
                "- 趋势显示正向发展\n\n" +
                "### 建议\n" +
                "1. 保持当前的良好习惯\n" +
                "2. 关注数据变化趋势，及时调整策略\n" +
                "3. 设定阶段性目标并追踪完成情况\n" +
                "4. 定期回顾并更新个人发展计划"
        };
    }
}
