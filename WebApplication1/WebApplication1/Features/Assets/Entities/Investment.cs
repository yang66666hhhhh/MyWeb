using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Assets.Entities;

public class Investment : EntityBase
{
    public Guid? UserId { get; set; }
    public string InvestmentDate { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal? CurrentValue { get; set; }
    public decimal? ReturnRate { get; set; }
    public string? Description { get; set; }
    public string? Remark { get; set; }
}
