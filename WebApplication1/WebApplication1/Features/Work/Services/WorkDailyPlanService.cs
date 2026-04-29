using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Enums;
using WebApplication1.Features.Work.Dtos;
using WebApplication1.Features.Work.Entities;
using WebApplication1.Features.Work.Services.Interfaces;

namespace WebApplication1.Features.Work.Services;

public class WorkDailyPlanService(AppDbContext dbContext) : IWorkDailyPlanService
{
    public async Task<PageResult<WorkDailyPlanDto>> GetPageAsync(WorkDailyPlanQueryDto query, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var plans = dbContext.WorkDailyPlans.AsNoTracking();

        if (query.StartDate.HasValue)
        {
            plans = plans.Where(x => x.PlanDate >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            plans = plans.Where(x => x.PlanDate <= query.EndDate.Value);
        }

        if (query.Status.HasValue)
        {
            plans = plans.Where(x => x.Status == (WorkDailyPlanStatus)query.Status.Value);
        }

        if (query.Priority.HasValue)
        {
            plans = plans.Where(x => x.Priority == (WorkDailyPlanPriority)query.Priority.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            plans = plans.Where(x =>
                x.Title.Contains(keyword) ||
                (x.Content != null && x.Content.Contains(keyword)) ||
                (x.Remark != null && x.Remark.Contains(keyword)));
        }

        var total = await plans.CountAsync(cancellationToken);
        var items = await plans
            .OrderByDescending(x => x.PlanDate)
            .ThenBy(x => x.Status)
            .ThenBy(x => x.Priority)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(x => x.Project)
            .ToListAsync(cancellationToken);

        return PageResult<WorkDailyPlanDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<WorkDailyPlanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var plan = await dbContext.WorkDailyPlans
            .AsNoTracking()
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return plan is null ? null : ToDto(plan);
    }

    public async Task<WorkDailyPlanDto> CreateAsync(CreateWorkDailyPlanDto input, CancellationToken cancellationToken = default)
    {
        Guid? projectId = null;
        if (!string.IsNullOrWhiteSpace(input.ProjectId) && Guid.TryParse(input.ProjectId, out var parsedProjectId))
        {
            projectId = parsedProjectId;
        }

        var plan = new WorkDailyPlan
        {
            PlanDate = input.PlanDate,
            Title = input.Title,
            Content = input.Content,
            ProjectId = projectId,
            Priority = (WorkDailyPlanPriority)input.Priority,
            StartTime = input.StartTime,
            EndTime = input.EndTime,
            EstimatedHours = input.EstimatedHours,
            Remark = input.Remark,
            Status = WorkDailyPlanStatus.Pending
        };

        dbContext.WorkDailyPlans.Add(plan);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (plan.ProjectId.HasValue)
        {
            await dbContext.Entry(plan).Reference(x => x.Project).LoadAsync(cancellationToken);
        }

        return ToDto(plan);
    }

    public async Task<WorkDailyPlanDto?> UpdateAsync(Guid id, UpdateWorkDailyPlanDto input, CancellationToken cancellationToken = default)
    {
        var plan = await dbContext.WorkDailyPlans.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (plan is null)
        {
            return null;
        }

        if (input.PlanDate.HasValue) plan.PlanDate = input.PlanDate.Value;
        if (input.Title is not null) plan.Title = input.Title;
        if (input.Content is not null) plan.Content = input.Content;
        if (input.ProjectId is not null && Guid.TryParse(input.ProjectId, out var updatedProjectId))
        {
            plan.ProjectId = updatedProjectId;
        }
        if (input.Priority.HasValue) plan.Priority = (WorkDailyPlanPriority)input.Priority.Value;
        if (input.Status.HasValue) plan.Status = (WorkDailyPlanStatus)input.Status.Value;
        if (input.StartTime is not null) plan.StartTime = input.StartTime;
        if (input.EndTime is not null) plan.EndTime = input.EndTime;
        if (input.EstimatedHours.HasValue) plan.EstimatedHours = input.EstimatedHours;
        if (input.ActualHours.HasValue) plan.ActualHours = input.ActualHours;
        if (input.Remark is not null) plan.Remark = input.Remark;

        await dbContext.SaveChangesAsync(cancellationToken);

        if (plan.ProjectId.HasValue)
        {
            await dbContext.Entry(plan).Reference(x => x.Project).LoadAsync(cancellationToken);
        }

        return ToDto(plan);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var plan = await dbContext.WorkDailyPlans.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (plan is null)
        {
            return false;
        }

        dbContext.WorkDailyPlans.Remove(plan);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<WorkDailyPlanDto?> CompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var plan = await dbContext.WorkDailyPlans
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (plan is null)
        {
            return null;
        }

        plan.Status = WorkDailyPlanStatus.Completed;
        plan.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(plan);
    }

    public async Task<Guid?> ConvertToWorkLogAsync(ConvertToWorkLogDto input, CancellationToken cancellationToken = default)
    {
        if (!Guid.TryParse(input.PlanId, out var planId))
        {
            return null;
        }

        var plan = await dbContext.WorkDailyPlans
            .Include(x => x.Project)
            .FirstOrDefaultAsync(x => x.Id == planId, cancellationToken);
        if (plan is null)
        {
            return null;
        }

        var workLog = new WorkLog
        {
            WorkDate = input.WorkDate,
            Title = plan.Title,
            OriginalContent = input.OriginalContent ?? plan.Content,
            TotalHours = input.TotalHours ?? plan.EstimatedHours ?? 0,
            ProjectId = plan.ProjectId ?? Guid.Empty,
            WeekDay = input.WorkDate.DayOfWeek.ToString(),
            Status = WorkLogStatus.Normal,
            SourceType = WorkLogSourceType.PlanConversion
        };

        dbContext.WorkLogs.Add(workLog);
        plan.ConvertedWorkLogId = workLog.Id;
        plan.Status = WorkDailyPlanStatus.Completed;

        await dbContext.SaveChangesAsync(cancellationToken);

        return workLog.Id;
    }

    private static WorkDailyPlanDto ToDto(WorkDailyPlan plan)
    {
        return new WorkDailyPlanDto
        {
            Id = plan.Id,
            UserId = plan.UserId,
            PlanDate = plan.PlanDate,
            Title = plan.Title,
            Content = plan.Content,
            ProjectId = plan.ProjectId,
            ProjectName = plan.Project?.ProjectName,
            Priority = (int)plan.Priority,
            Status = (int)plan.Status,
            StartTime = plan.StartTime,
            EndTime = plan.EndTime,
            EstimatedHours = plan.EstimatedHours,
            ActualHours = plan.ActualHours,
            ConvertedWorkLogId = plan.ConvertedWorkLogId,
            Remark = plan.Remark,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt
        };
    }
}