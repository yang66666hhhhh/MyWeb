namespace WebApplication1.Shared.Enums;

public enum WorkLogSourceType
{
    Manual = 0,
    ExcelImport = 1,
    PlanConversion = 2
}

public enum WorkLogStatus
{
    Normal = 0,
    MissingData = 1,
    PendingSupplement = 2
}

public enum WorkProjectStatus
{
    Active = 0,
    Completed = 1,
    Suspended = 2,
    Archived = 3
}

public enum WorkProjectType
{
    Internal = 0,
    External = 1,
    RAndD = 2,
    Support = 3,
    Other = 4
}

public enum WorkDeviceStatus
{
    Active = 0,
    Inactive = 1,
    Maintenance = 2
}

public enum WorkDeviceType
{
    ProductionLine = 0,
    Equipment = 1,
    TestingDevice = 2,
    Other = 3
}

public enum WorkImportStrategy
{
    SkipDuplicate = 0,
    OverwriteDuplicate = 1,
    Merge = 2
}

public enum WorkImportStatus
{
    Pending = 0,
    Processing = 1,
    Completed = 2,
    Failed = 3
}

public enum WorkImportValidationStatus
{
    Valid = 0,
    Warning = 1,
    Error = 2
}

public enum WorkDailyPlanStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Cancelled = 3
}

public enum WorkDailyPlanPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}
