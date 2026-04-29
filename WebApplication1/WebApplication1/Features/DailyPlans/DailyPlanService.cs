using Microsoft.EntityFrameworkCore;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;
using WebApplication1.Shared.Enums;

namespace WebApplication1.Features.DailyPlans;

public class DailyPlanService(AppDbContext dbContext) : IDailyPlanService
{
    public async Task<PageResult<DailyPlanDto>> GetPageAsync(DailyPlanQueryDto query, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var plans = dbContext.DailyPlans.AsNoTracking();

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
            plans = plans.Where(x => x.Status == query.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            plans = plans.Where(x =>
                x.Title.Contains(keyword) ||
                (x.Description != null && x.Description.Contains(keyword)) ||
                (x.Remark != null && x.Remark.Contains(keyword)));
        }

        var total = await plans.CountAsync(cancellationToken);
        var items = await plans
            .OrderByDescending(x => x.PlanDate)
            .ThenBy(x => x.Status)
            .ThenBy(x => x.Priority)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<DailyPlanDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<DailyPlanDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var plan = await dbContext.DailyPlans
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return plan is null ? null : ToDto(plan);
    }

    public async Task<DailyPlanDto> CreateAsync(CreateDailyPlanDto input, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var plan = new DailyPlan
        {
            PlanDate = input.PlanDate,
            Title = input.Title.Trim(),
            Description = input.Description?.Trim(),
            Priority = input.Priority,
            Status = input.Status,
            Remark = input.Remark?.Trim(),
            CompletedAt = input.Status == DailyPlanStatus.Completed ? now : null,
            CreatedAt = now
        };

        dbContext.DailyPlans.Add(plan);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(plan);
    }

    public async Task<DailyPlanDto?> UpdateAsync(Guid id, UpdateDailyPlanDto input, CancellationToken cancellationToken = default)
    {
        var plan = await dbContext.DailyPlans.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (plan is null)
        {
            return null;
        }

        plan.PlanDate = input.PlanDate;
        plan.Title = input.Title.Trim();
        plan.Description = input.Description?.Trim();
        plan.Priority = input.Priority;
        plan.Remark = input.Remark?.Trim();
        plan.UpdatedAt = DateTime.UtcNow;

        if (plan.Status != DailyPlanStatus.Completed && input.Status == DailyPlanStatus.Completed)
        {
            plan.CompletedAt = DateTime.UtcNow;
        }
        else if (input.Status != DailyPlanStatus.Completed)
        {
            plan.CompletedAt = null;
        }

        plan.Status = input.Status;

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(plan);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var plan = await dbContext.DailyPlans.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (plan is null)
        {
            return false;
        }

        dbContext.DailyPlans.Remove(plan);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<DailyPlanDto?> CompleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var plan = await dbContext.DailyPlans.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (plan is null)
        {
            return null;
        }

        plan.Status = DailyPlanStatus.Completed;
        plan.CompletedAt ??= DateTime.UtcNow;
        plan.UpdatedAt = DateTime.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(plan);
    }

    private static DailyPlanDto ToDto(DailyPlan plan)
    {
        return new DailyPlanDto
        {
            Id = plan.Id,
            PlanDate = plan.PlanDate,
            Title = plan.Title,
            Description = plan.Description,
            Priority = plan.Priority,
            Status = plan.Status,
            Remark = plan.Remark,
            CompletedAt = plan.CompletedAt,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt
        };
    }
}
