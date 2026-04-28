import type { BaseEntity, PageQuery, PageResult } from '../../types';

import { requestClient } from '#/api/request';

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

export async function getWorkLogPageApi(params: WorkLogQuery) {
  return requestClient.get<PageResult<WorkLog>>('/work/logs', { params });
}

export async function getWorkLogApi(id: string) {
  return requestClient.get<WorkLog>(`/work/logs/${id}`);
}

export async function createWorkLogApi(data: CreateWorkLogInput) {
  return requestClient.post<WorkLog>('/work/logs', data);
}

export async function updateWorkLogApi(id: string, data: Partial<CreateWorkLogInput>) {
  return requestClient.put<WorkLog>(`/work/logs/${id}`, data);
}

export async function deleteWorkLogApi(id: string) {
  return requestClient.delete(`/work/logs/${id}`);
}
