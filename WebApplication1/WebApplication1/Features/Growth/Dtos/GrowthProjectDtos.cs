using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Growth.Dtos;

public class GrowthProjectDto
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int Progress { get; set; }
    public int TaskCount { get; set; }
    public int Status { get; set; }
    public int Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class GrowthProjectQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public int? Status { get; set; }
    public int? Type { get; set; }
}

public class CreateGrowthProjectDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int Status { get; set; } = 1;
    public int Type { get; set; } = 0;
}

public class UpdateGrowthProjectDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int? Progress { get; set; }
    public int? TaskCount { get; set; }
    public int? Status { get; set; }
    public int? Type { get; set; }
}