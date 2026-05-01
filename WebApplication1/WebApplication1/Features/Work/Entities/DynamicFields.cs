using WebApplication1.Shared;

namespace WebApplication1.Features.Work.Entities;

public class IndustryTemplate : EntityBase
{
    public Guid? TenantId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Industry { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    public ICollection<TemplateField> Fields { get; set; } = new List<TemplateField>();

    public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}

public class TemplateField : EntityBase
{
    public Guid TemplateId { get; set; }

    public string FieldName { get; set; } = string.Empty;

    public string FieldLabel { get; set; } = string.Empty;

    public FieldType FieldType { get; set; } = FieldType.Text;

    public string? Options { get; set; }

    public bool IsRequired { get; set; }

    public int Sort { get; set; }

    public string? DefaultValue { get; set; }

    public IndustryTemplate Template { get; set; } = null!;
}

public enum FieldType
{
    Text = 0,
    Number = 1,
    Date = 2,
    Select = 3,
    MultiSelect = 4,
    Textarea = 5,
    File = 6
}

public class WorkLogDynamicValue : EntityBase
{
    public Guid WorkLogId { get; set; }

    public Guid FieldId { get; set; }

    public string FieldName { get; set; } = string.Empty;

    public string? StringValue { get; set; }

    public decimal? NumberValue { get; set; }

    public DateOnly? DateValue { get; set; }

    public WorkLog WorkLog { get; set; } = null!;
}