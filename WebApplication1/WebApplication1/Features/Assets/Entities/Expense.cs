using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Assets.Entities;

public class Expense : EntityBase
{
    public Guid? UserId { get; set; }
    public string ExpenseDate { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? Remark { get; set; }
}
