export enum WorkLogSourceType {
  Manual = 0,
  ExcelImport = 1,
  PlanConversion = 2,
}

export const WorkLogSourceTypeLabel: Record<WorkLogSourceType, string> = {
  [WorkLogSourceType.Manual]: '手动',
  [WorkLogSourceType.ExcelImport]: 'Excel导入',
  [WorkLogSourceType.PlanConversion]: '计划转换',
};

export enum WorkLogStatus {
  Normal = 0,
  MissingData = 1,
  PendingSupplement = 2,
}

export const WorkLogStatusLabel: Record<WorkLogStatus, string> = {
  [WorkLogStatus.Normal]: '正常',
  [WorkLogStatus.MissingData]: '缺失数据',
  [WorkLogStatus.PendingSupplement]: '待补充',
};

export const WorkLogStatusColor: Record<WorkLogStatus, string> = {
  [WorkLogStatus.Normal]: 'success',
  [WorkLogStatus.MissingData]: 'warning',
  [WorkLogStatus.PendingSupplement]: 'error',
};

export enum WorkProjectStatus {
  Active = 0,
  Completed = 1,
  Suspended = 2,
  Archived = 3,
}

export const WorkProjectStatusLabel: Record<WorkProjectStatus, string> = {
  [WorkProjectStatus.Active]: '进行中',
  [WorkProjectStatus.Completed]: '已完成',
  [WorkProjectStatus.Suspended]: '已暂停',
  [WorkProjectStatus.Archived]: '已归档',
};

export const WorkProjectStatusColor: Record<WorkProjectStatus, string> = {
  [WorkProjectStatus.Active]: 'processing',
  [WorkProjectStatus.Completed]: 'success',
  [WorkProjectStatus.Suspended]: 'warning',
  [WorkProjectStatus.Archived]: 'default',
};

export enum WorkProjectType {
  Internal = 0,
  External = 1,
  RAndD = 2,
  Support = 3,
  Other = 4,
}

export const WorkProjectTypeLabel: Record<WorkProjectType, string> = {
  [WorkProjectType.Internal]: '内部项目',
  [WorkProjectType.External]: '外部项目',
  [WorkProjectType.RAndD]: '研发项目',
  [WorkProjectType.Support]: '支持项目',
  [WorkProjectType.Other]: '其他',
};

export enum WorkDeviceStatus {
  Active = 0,
  Inactive = 1,
  Maintenance = 2,
}

export const WorkDeviceStatusLabel: Record<WorkDeviceStatus, string> = {
  [WorkDeviceStatus.Active]: '正常',
  [WorkDeviceStatus.Inactive]: '停用',
  [WorkDeviceStatus.Maintenance]: '维护中',
};

export const WorkDeviceStatusColor: Record<WorkDeviceStatus, string> = {
  [WorkDeviceStatus.Active]: 'success',
  [WorkDeviceStatus.Inactive]: 'default',
  [WorkDeviceStatus.Maintenance]: 'warning',
};

export enum WorkDeviceType {
  ProductionLine = 0,
  Equipment = 1,
  TestingDevice = 2,
  Other = 3,
}

export const WorkDeviceTypeLabel: Record<WorkDeviceType, string> = {
  [WorkDeviceType.ProductionLine]: '生产线',
  [WorkDeviceType.Equipment]: '设备',
  [WorkDeviceType.TestingDevice]: '测试设备',
  [WorkDeviceType.Other]: '其他',
};

export enum WorkImportStrategy {
  SkipDuplicate = 0,
  OverwriteDuplicate = 1,
  Merge = 2,
}

export const WorkImportStrategyLabel: Record<WorkImportStrategy, string> = {
  [WorkImportStrategy.SkipDuplicate]: '跳过重复',
  [WorkImportStrategy.OverwriteDuplicate]: '覆盖重复',
  [WorkImportStrategy.Merge]: '合并导入',
};

export enum WorkImportStatus {
  Pending = 0,
  Processing = 1,
  Completed = 2,
  Failed = 3,
}

export const WorkImportStatusLabel: Record<WorkImportStatus, string> = {
  [WorkImportStatus.Pending]: '等待处理',
  [WorkImportStatus.Processing]: '处理中',
  [WorkImportStatus.Completed]: '已完成',
  [WorkImportStatus.Failed]: '失败',
};

export enum WorkImportValidationStatus {
  Valid = 0,
  Warning = 1,
  Error = 2,
}

export const WorkImportValidationStatusLabel: Record<WorkImportValidationStatus, string> = {
  [WorkImportValidationStatus.Valid]: '有效',
  [WorkImportValidationStatus.Warning]: '警告',
  [WorkImportValidationStatus.Error]: '错误',
};

export const WorkImportValidationStatusColor: Record<WorkImportValidationStatus, string> = {
  [WorkImportValidationStatus.Valid]: 'success',
  [WorkImportValidationStatus.Warning]: 'warning',
  [WorkImportValidationStatus.Error]: 'error',
};

export const WorkImportStatusColor: Record<WorkImportStatus, string> = {
  [WorkImportStatus.Pending]: 'default',
  [WorkImportStatus.Processing]: 'processing',
  [WorkImportStatus.Completed]: 'success',
  [WorkImportStatus.Failed]: 'error',
};

export enum WorkDailyPlanStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3,
}

export const WorkDailyPlanStatusLabel: Record<WorkDailyPlanStatus, string> = {
  [WorkDailyPlanStatus.Pending]: '待执行',
  [WorkDailyPlanStatus.InProgress]: '进行中',
  [WorkDailyPlanStatus.Completed]: '已完成',
  [WorkDailyPlanStatus.Cancelled]: '已取消',
};

export const WorkDailyPlanStatusColor: Record<WorkDailyPlanStatus, string> = {
  [WorkDailyPlanStatus.Pending]: 'default',
  [WorkDailyPlanStatus.InProgress]: 'processing',
  [WorkDailyPlanStatus.Completed]: 'success',
  [WorkDailyPlanStatus.Cancelled]: 'default',
};

export enum WorkDailyPlanPriority {
  Low = 1,
  Medium = 2,
  High = 3,
  Urgent = 4,
}

export const WorkDailyPlanPriorityLabel: Record<WorkDailyPlanPriority, string> = {
  [WorkDailyPlanPriority.Low]: '低',
  [WorkDailyPlanPriority.Medium]: '中',
  [WorkDailyPlanPriority.High]: '高',
  [WorkDailyPlanPriority.Urgent]: '紧急',
};

export const WorkDailyPlanPriorityColor: Record<WorkDailyPlanPriority, string> = {
  [WorkDailyPlanPriority.Low]: 'default',
  [WorkDailyPlanPriority.Medium]: 'blue',
  [WorkDailyPlanPriority.High]: 'orange',
  [WorkDailyPlanPriority.Urgent]: 'red',
};

export function getWeekDayName(weekDay: number): string {
  const weekDays = ['周日', '周一', '周二', '周三', '周四', '周五', '周六'];
  return weekDays[weekDay] || '';
}

export function getWeekDayFromDate(dateStr: string): number {
  const date = new Date(dateStr);
  return date.getDay();
}
