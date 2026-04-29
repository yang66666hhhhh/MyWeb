using WebApplication1.Shared.Common;

namespace WebApplication1.Features.Work.Dtos;

public class WorkTaskTypeDto
{
    public Guid Id { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string? TypeCode { get; set; }
    public string? Description { get; set; }
    public int Sort { get; set; }
    public bool Enabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateWorkTaskTypeDto
{
    public string TypeName { get; set; } = string.Empty;
    public string? TypeCode { get; set; }
    public string? Description { get; set; }
    public int Sort { get; set; }
    public bool Enabled { get; set; } = true;
}

public class UpdateWorkTaskTypeDto : CreateWorkTaskTypeDto
{
    public Guid Id { get; set; }
}

public class WorkTaskTypeQueryDto : PageQueryDto
{
    public string? Keyword { get; set; }
    public bool? Enabled { get; set; }
}
