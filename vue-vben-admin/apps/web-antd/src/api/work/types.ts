// Work module types
export interface WorkProject {
  id: string;
  projectName: string;
  projectCode?: string;
  projectType: number;
  customerName?: string;
  description?: string;
  startDate?: string;
  endDate?: string;
  status: number;
  sort: number;
  createdAt: string;
  updatedAt?: string;
}

export interface WorkDevice {
  id: string;
  projectId?: string;
  deviceName: string;
  deviceCode?: string;
  deviceType: number;
  description?: string;
  status: number;
  createdAt: string;
  updatedAt?: string;
}

export interface WorkTaskType {
  id: string;
  typeName: string;
  typeCode?: string;
  description?: string;
  sort: number;
  enabled: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface WorkLog {
  id: string;
  workDate: string;
  weekDay: string;
  projectId: string;
  projectName?: string;
  title: string;
  originalContent?: string;
  summary?: string;
  totalHours: number;
  status: number;
  sourceType: number;
  remark?: string;
  createdAt: string;
  updatedAt?: string;
  sort?: number;
}

export interface WorkDailyPlan {
  id: string;
  planDate: string;
  title: string;
  content?: string;
  projectId?: string;
  projectName?: string;
  priority: number;
  status: number;
  startTime?: string;
  endTime?: string;
  estimatedHours?: number;
  actualHours?: number;
  remark?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface WorkImportBatch {
  id: string;
  fileName: string;
  status: number;
  totalRows: number;
  successCount: number;
  failCount: number;
  errorMessage?: string;
  createdAt: string;
}

export interface WorkImportPreviewItem {
  rowNumber: number;
  workDate: string;
  projectName: string;
  deviceName: string;
  taskTypeName: string;
  content: string;
  hours: number;
  isValid: boolean;
  errorMessage?: string;
}

export const WorkProjectStatus = {
  Active: 0,
  Completed: 1,
  Suspended: 2,
  Archived: 3,
} as const;

export const WorkDeviceStatus = {
  Active: 0,
  Inactive: 1,
  Maintenance: 2,
} as const;

export const WorkLogStatus = {
  Normal: 0,
  MissingData: 1,
  PendingSupplement: 2,
} as const;

export const WorkDailyPlanStatus = {
  Pending: 0,
  InProgress: 1,
  Completed: 2,
  Cancelled: 3,
} as const;
