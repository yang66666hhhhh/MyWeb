import type { BaseEntity, PageQuery, PageResult } from '../types';

export interface WorkLog extends BaseEntity {
  workDate: string;
  weekDay: string;
  projectId: string;
  projectName: string;
  deviceIds: string[];
  deviceNames: string[];
  taskTypeIds: string[];
  taskTypeNames: string[];
  title: string;
  originalContent: string;
  summary: string;
  totalHours: number;
  status: number;
  sourceType: number;
  importBatchId?: string;
  remark?: string;
}

export interface WorkLogQuery extends PageQuery {
  keyword?: string;
  workDate?: string;
  startDate?: string;
  endDate?: string;
  projectId?: string;
  deviceId?: string;
  taskTypeId?: string;
  sourceType?: number;
  status?: number;
}

export interface CreateWorkLogInput {
  workDate: string;
  weekDay: string;
  projectId: string;
  deviceIds: string[];
  taskTypeIds: string[];
  title: string;
  originalContent?: string;
  summary?: string;
  totalHours?: number;
  status?: number;
  sourceType?: number;
  remark?: string;
}

export interface UpdateWorkLogInput extends Partial<CreateWorkLogInput> {
  id: string;
}

export interface WorkLogItem {
  id: string;
  workLogId: string;
  content: string;
  taskTypeId?: string;
  deviceId?: string;
  progressPercent?: number;
  hours?: number;
  sort?: number;
  remark?: string;
}
