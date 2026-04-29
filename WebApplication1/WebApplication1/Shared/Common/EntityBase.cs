namespace WebApplication1.Shared.Common;

public interface IAuditable
{
    string? CreatedBy { get; set; }
    string? ModifiedBy { get; set; }
}

public abstract class EntityBase : IAuditable
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? ModifiedBy { get; set; }
}
