using Microsoft.EntityFrameworkCore;
using WebApplication1.Features.Growth.Dtos;
using WebApplication1.Features.Growth.Entities;
using WebApplication1.Features.Growth.Services.Interfaces;
using WebApplication1.Shared.Common;
using WebApplication1.Shared.Data;

namespace WebApplication1.Features.Growth.Services;

public class HabitService(AppDbContext dbContext) : IHabitService
{
    public async Task<PageResult<HabitDto>> GetPageAsync(HabitQueryDto query, Guid? userId = null, CancellationToken cancellationToken = default)
    {
        var page = Math.Max(query.Page, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var habits = dbContext.Habits.AsNoTracking();

        if (userId.HasValue)
        {
            habits = habits.Where(x => x.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            var keyword = query.Keyword.Trim();
            habits = habits.Where(x => x.Name.Contains(keyword) || (x.Description != null && x.Description.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.HabitType))
        {
            habits = habits.Where(x => x.HabitType == query.HabitType);
        }

        if (query.Status.HasValue)
        {
            habits = habits.Where(x => x.Status == query.Status.Value);
        }

        var total = await habits.CountAsync(cancellationToken);
        var items = await habits
            .OrderByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return PageResult<HabitDto>.Create(items.Select(ToDto).ToList(), total, page, pageSize);
    }

    public async Task<HabitDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var habit = await dbContext.Habits
            .AsNoTracking()
            .Include(x => x.CheckIns.OrderByDescending(c => c.CheckInDate).Take(30))
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (habit is null) return null;

        return ToDetailDto(habit);
    }

    public async Task<HabitDto> CreateAsync(CreateHabitDto input, Guid userId, CancellationToken cancellationToken = default)
    {
        var habit = new Habit
        {
            UserId = userId,
            Name = input.Name,
            HabitType = input.HabitType,
            Description = input.Description,
            TargetFrequency = input.TargetFrequency,
            Status = 1
        };

        dbContext.Habits.Add(habit);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ToDto(habit);
    }

    public async Task<HabitDto?> UpdateAsync(Guid id, UpdateHabitDto input, CancellationToken cancellationToken = default)
    {
        var habit = await dbContext.Habits.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (habit is null) return null;

        if (input.Name is not null) habit.Name = input.Name;
        if (input.HabitType is not null) habit.HabitType = input.HabitType;
        if (input.Description is not null) habit.Description = input.Description;
        if (input.TargetFrequency is not null) habit.TargetFrequency = input.TargetFrequency;
        if (input.Status.HasValue) habit.Status = input.Status.Value;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(habit);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var habit = await dbContext.Habits.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (habit is null) return false;

        dbContext.Habits.Remove(habit);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<HabitDto?> CheckInAsync(Guid id, CheckInDto input, CancellationToken cancellationToken = default)
    {
        var habit = await dbContext.Habits
            .Include(x => x.CheckIns)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (habit is null) return null;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var existingCheckIn = habit.CheckIns.FirstOrDefault(c => c.CheckInDate == today);
        if (existingCheckIn != null)
        {
            if (!string.IsNullOrWhiteSpace(input.Remark))
            {
                existingCheckIn.Remark = input.Remark;
            }
        }
        else
        {
            var checkIn = new HabitCheckIn
            {
                HabitId = habit.Id,
                CheckInDate = today,
                Remark = input.Remark
            };
            dbContext.HabitCheckIns.Add(checkIn);
            habit.TotalCheckIns++;
            habit.LastCheckInDate = today;

            UpdateStreak(habit);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(habit);
    }

    public async Task<HabitDto?> UpdateStatusAsync(Guid id, int status, CancellationToken cancellationToken = default)
    {
        var habit = await dbContext.Habits.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (habit is null) return null;

        habit.Status = status;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(habit);
    }

    private static void UpdateStreak(Habit habit)
    {
        if (!habit.LastCheckInDate.HasValue)
        {
            habit.CurrentStreak = 1;
            habit.LongestStreak = 1;
            return;
        }

        var yesterday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1));
        if (habit.LastCheckInDate.Value == yesterday)
        {
            habit.CurrentStreak++;
            if (habit.CurrentStreak > habit.LongestStreak)
            {
                habit.LongestStreak = habit.CurrentStreak;
            }
        }
        else if (habit.LastCheckInDate.Value != DateOnly.FromDateTime(DateTime.UtcNow))
        {
            habit.CurrentStreak = 1;
        }
    }

    private static HabitDto ToDto(Habit habit) => new()
    {
        Id = habit.Id,
        UserId = habit.UserId,
        Name = habit.Name,
        HabitType = habit.HabitType,
        Description = habit.Description,
        TargetFrequency = habit.TargetFrequency,
        Status = habit.Status,
        CurrentStreak = habit.CurrentStreak,
        LongestStreak = habit.LongestStreak,
        TotalCheckIns = habit.TotalCheckIns,
        LastCheckInDate = habit.LastCheckInDate,
        CreatedAt = habit.CreatedAt,
        UpdatedAt = habit.UpdatedAt
    };

    private static HabitDetailDto ToDetailDto(Habit habit) => new()
    {
        Id = habit.Id,
        UserId = habit.UserId,
        Name = habit.Name,
        HabitType = habit.HabitType,
        Description = habit.Description,
        TargetFrequency = habit.TargetFrequency,
        Status = habit.Status,
        CurrentStreak = habit.CurrentStreak,
        LongestStreak = habit.LongestStreak,
        TotalCheckIns = habit.TotalCheckIns,
        LastCheckInDate = habit.LastCheckInDate,
        CreatedAt = habit.CreatedAt,
        UpdatedAt = habit.UpdatedAt,
        RecentCheckIns = habit.CheckIns.OrderByDescending(c => c.CheckInDate).Take(30).Select(c => new CheckInRecordDto
        {
            Id = c.Id,
            CheckInDate = c.CheckInDate,
            Remark = c.Remark
        }).ToList()
    };
}