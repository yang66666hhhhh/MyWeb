import type { BaseEntity, PageQuery, PageResult } from '../../types';

import { requestClient } from '#/api/request';

export interface WorkDailyPlan extends BaseEntity {
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
  convertedWorkLogId?: string;
  remark?: string;
}

export interface WorkDailyPlanQuery extends PageQuery {
  keyword?: string;
  planDate?: string;
  startDate?: string;
  endDate?: string;
  projectId?: string;
  status?: number;
  priority?: number;
}

export interface CreateWorkDailyPlanInput {
  planDate: string;
  title: string;
  content?: string;
  projectId?: string;
  priority?: number;
  status?: number;
  startTime?: string;
  endTime?: string;
  estimatedHours?: number;
  remark?: string;
}

export interface ConvertToWorkLogInput {
  planId: string;
  workDate: string;
  originalContent?: string;
  totalHours?: number;
}

export async function getWorkDailyPlanPageApi(params: WorkDailyPlanQuery) {
  return requestClient.get<PageResult<WorkDailyPlan>>('/work/daily-plans', { params });
}

export async function getWorkDailyPlanApi(id: string) {
  return requestClient.get<WorkDailyPlan>(`/work/daily-plans/${id}`);
}

export async function createWorkDailyPlanApi(data: CreateWorkDailyPlanInput) {
  return requestClient.post<WorkDailyPlan>('/work/daily-plans', data);
}

export async function updateWorkDailyPlanApi(id: string, data: Partial<CreateWorkDailyPlanInput>) {
  return requestClient.put<WorkDailyPlan>(`/work/daily-plans/${id}`, data);
}

export async function deleteWorkDailyPlanApi(id: string) {
  return requestClient.delete(`/work/daily-plans/${id}`);
}

export async function completeWorkDailyPlanApi(id: string) {
  return requestClient.post<WorkDailyPlan>(`/work/daily-plans/${id}/complete`);
}

export async function convertToWorkLogApi(input: ConvertToWorkLogInput) {
  return requestClient.post<{ workLogId: string }>('/work/daily-plans/convert-to-log', input);
}
