import type { BaseEntity, PageQuery, PageResult } from './types';

import { requestClient } from '#/api/request';

export interface WorkLog extends BaseEntity {
  category: string;
  content: string;
  durationMinutes: number;
  issue?: string;
  logDate: string;
  nextPlan?: string;
  projectId?: string;
  projectName?: string;
  summary: string;
  title: string;
  tags?: string[];
}

export interface WorkLogQuery extends PageQuery {
  endDate?: string;
  keyword?: string;
  projectId?: string;
  startDate?: string;
}

export type SaveWorkLogInput = Omit<WorkLog, 'createdAt' | 'id' | 'updatedAt'>;

export async function getWorkLogPageApi(params: WorkLogQuery) {
  return requestClient.get<PageResult<WorkLog>>('/work/logs', { params });
}

export async function getWorkLogApi(id: string) {
  return requestClient.get<WorkLog>(`/work/logs/${id}`);
}

export async function createWorkLogApi(data: SaveWorkLogInput) {
  return requestClient.post<WorkLog>('/work/logs', data);
}

export async function updateWorkLogApi(id: string, data: SaveWorkLogInput) {
  return requestClient.put<WorkLog>(`/work/logs/${id}`, data);
}

export async function deleteWorkLogApi(id: string) {
  return requestClient.delete(`/work/logs/${id}`);
}

export interface ConvertToWorkLogInput {
  originalContent?: null | string;
  planId: string;
  workDate: string;
}

export async function convertToWorkLogApi(data: ConvertToWorkLogInput) {
  return requestClient.post<{ workLogId: string }>('/work/daily-plans/convert-to-log', data);
}