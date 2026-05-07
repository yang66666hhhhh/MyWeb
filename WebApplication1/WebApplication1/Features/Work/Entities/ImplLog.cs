using WebApplication1.Shared;

namespace WebApplication1.Features.Work.Entities;

public class ImplLog : EntityBase
{
    public Guid UserId { get; set; }

    public DateOnly WorkDate { get; set; }

    public string WeekDay { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? ProjectName { get; set; }

    public decimal TotalHours { get; set; }

    public string? PersonaCode { get; set; }

    public Guid? TemplateId { get; set; }

    public string? ExtraData { get; set; }

    public WorkLogTemplate? Template { get; set; }
}
