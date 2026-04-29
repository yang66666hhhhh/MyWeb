using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Assets.Entities;

public class Budget : EntityBase
{
    public Guid? UserId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal PlannedAmount { get; set; }
    public decimal ActualAmount { get; set; }
    public string? Remark { get; set; }
}
