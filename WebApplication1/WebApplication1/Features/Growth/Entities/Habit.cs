using WebApplication1.Shared;

namespace WebApplication1.Features.Growth.Entities;

public class Habit : EntityBase
{
    public Guid? UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string HabitType { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string TargetFrequency { get; set; } = "每天";

    public int Status { get; set; } = 1;

    public int CurrentStreak { get; set; }

    public int LongestStreak { get; set; }

    public int TotalCheckIns { get; set; }

    public DateOnly? LastCheckInDate { get; set; }

    public ICollection<HabitCheckIn> CheckIns { get; set; } = new List<HabitCheckIn>();
}

public class HabitCheckIn : EntityBase
{
    public Guid HabitId { get; set; }

    public DateOnly CheckInDate { get; set; }

    public string? Remark { get; set; }

    public Habit Habit { get; set; } = null!;
}