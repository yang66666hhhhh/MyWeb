export enum WorkLogSourceType {
  Manual = 0,
  ExcelImport = 1,
  PlanConversion = 2,
}

export enum WorkLogSourceTypeLabel {
  '手动' = WorkLogSourceType.Manual,
  'Excel导入' = WorkLogSourceType.ExcelImport,
  '计划转换' = WorkLogSourceType.PlanConversion,
}

export enum WorkLogStatus {
  Normal = 0,
  MissingData = 1,
  PendingSupplement = 2,
}

export enum WorkLogStatusLabel {
  '正常' = WorkLogStatus.Normal,
  '缺失数据' = WorkLogStatus.MissingData,
  '待补充' = WorkLogStatus.PendingSupplement,
}

export enum WorkLogStatusColor {
  Normal = 'success',
  MissingData = 'warning',
  PendingSupplement = 'error',
}

export enum WorkProjectStatus {
  Active = 0,
  Completed = 1,
  Suspended = 2,
  Archived = 3,
}

export enum WorkProjectStatusLabel {
  '进行中' = WorkProjectStatus.Active,
  '已完成' = WorkProjectStatus.Completed,
  '已暂停' = WorkProjectStatus.Suspended,
  '已归档' = WorkProjectStatus.Archived,
}

export enum WorkProjectStatusColor {
  Active = 'processing',
  Completed = 'success',
  Suspended = 'warning',
  Archived = 'default',
}

export enum WorkProjectType {
  Internal = 0,
  External = 1,
  RAndD = 2,
  Support = 3,
  Other = 4,
}

export enum WorkProjectTypeLabel {
  '内部项目' = WorkProjectType.Internal,
  '外部项目' = WorkProjectType.External,
  '研发项目' = WorkProjectType.RAndD,
  '支持项目' = WorkProjectType.Support,
  '其他' = WorkProjectType.Other,
}

export enum WorkDeviceStatus {
  Active = 0,
  Inactive = 1,
  Maintenance = 2,
}

export enum WorkDeviceStatusLabel {
  '正常' = WorkDeviceStatus.Active,
  '停用' = WorkDeviceStatus.Inactive,
  '维护中' = WorkDeviceStatus.Maintenance,
}

export enum WorkDeviceStatusColor {
  Active = 'success',
  Inactive = 'default',
  Maintenance = 'warning',
}

export enum WorkDeviceType {
  ProductionLine = 0,
  Equipment = 1,
  TestingDevice = 2,
  Other = 3,
}

export enum WorkDeviceTypeLabel {
  '生产线' = WorkDeviceType.ProductionLine,
  '设备' = WorkDeviceType.Equipment,
  '测试设备' = WorkDeviceType.TestingDevice,
  '其他' = WorkDeviceType.Other,
}

export enum WorkImportStrategy {
  SkipDuplicate = 0,
  OverwriteDuplicate = 1,
  Merge = 2,
}

export enum WorkImportStrategyLabel {
  '跳过重复' = WorkImportStrategy.SkipDuplicate,
  '覆盖重复' = WorkImportStrategy.OverwriteDuplicate,
  '合并导入' = WorkImportStrategy.Merge,
}

export enum WorkImportStatus {
  Pending = 0,
  Processing = 1,
  Completed = 2,
  Failed = 3,
}

export enum WorkImportStatusLabel {
  '等待处理' = WorkImportStatus.Pending,
  '处理中' = WorkImportStatus.Processing,
  '已完成' = WorkImportStatus.Completed,
  '失败' = WorkImportStatus.Failed,
}

export enum WorkImportValidationStatus {
  Valid = 0,
  Warning = 1,
  Error = 2,
}

export enum WorkImportValidationStatusLabel {
  '有效' = WorkImportValidationStatus.Valid,
  '警告' = WorkImportValidationStatus.Warning,
  '错误' = WorkImportValidationStatus.Error,
}

export enum WorkImportValidationStatusColor {
  Valid = 'success',
  Warning = 'warning',
  Error = 'error',
}

export enum WorkDailyPlanStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3,
}

export enum WorkDailyPlanStatusLabel {
  '待执行' = WorkDailyPlanStatus.Pending,
  '进行中' = WorkDailyPlanStatus.InProgress,
  '已完成' = WorkDailyPlanStatus.Completed,
  '已取消' = WorkDailyPlanStatus.Cancelled,
}

export enum WorkDailyPlanStatusColor {
  Pending = 'default',
  InProgress = 'processing',
  Completed = 'success',
  Cancelled = 'default',
}

export enum WorkDailyPlanPriority {
  Low = 1,
  Medium = 2,
  High = 3,
  Urgent = 4,
}

export enum WorkDailyPlanPriorityLabel {
  '低' = WorkDailyPlanPriority.Low,
  '中' = WorkDailyPlanPriority.Medium,
  '高' = WorkDailyPlanPriority.High,
  '紧急' = WorkDailyPlanPriority.Urgent,
}

export enum WorkDailyPlanPriorityColor {
  Low = 'default',
  Medium = 'blue',
  High = 'orange',
  Urgent = 'red',
}

export function getWeekDayName(weekDay: number): string {
  const weekDays = ['周日', '周一', '周二', '周三', '周四', '周五', '周六'];
  return weekDays[weekDay] || '';
}

export function getWeekDayFromDate(dateStr: string): number {
  const date = new Date(dateStr);
  return date.getDay();
}
