using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Dtos;

public class HabitDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string HabitType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TargetFrequency { get; set; } = string.Empty;
    public int Status { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int TotalCheckIns { get; set; }
    public DateOnly? LastCheckInDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class HabitQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public string? HabitType { get; set; }
    public int? Status { get; set; }
}

public class CreateHabitDto
{
    public string Name { get; set; } = string.Empty;
    public string HabitType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TargetFrequency { get; set; } = "每天";
}

public class UpdateHabitDto
{
    public string? Name { get; set; }
    public string? HabitType { get; set; }
    public string? Description { get; set; }
    public string? TargetFrequency { get; set; }
    public int? Status { get; set; }
}

public class CheckInDto
{
    public string? Remark { get; set; }
}

public class HabitDetailDto : HabitDto
{
    public List<CheckInRecordDto> RecentCheckIns { get; set; } = new();
}

public class CheckInRecordDto
{
    public Guid Id { get; set; }
    public DateOnly CheckInDate { get; set; }
    public string? Remark { get; set; }
}