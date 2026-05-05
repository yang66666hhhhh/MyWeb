import type { PageQuery, PageResult } from '#/types/api';
import { requestClient } from '#/api/request';
import type { WorkDailyPlan } from './types';

export type { WorkDailyPlan };

export interface WorkDailyPlanQuery extends PageQuery {
  planDate?: string;
  startDate?: string;
  endDate?: string;
  projectId?: string;
  status?: number;
  priority?: number;
}

export interface CreateWorkDailyPlanInput {
  planDate: string;
  projectId?: string;
  title: string;
  content?: string;
  priority?: number;
  startTime?: string;
  endTime?: string;
  estimatedHours?: number;
  remark?: string;
}

export interface UpdateWorkDailyPlanInput {
  planDate?: string;
  projectId?: string;
  title?: string;
  content?: string;
  priority?: number;
  status?: number;
  startTime?: string;
  endTime?: string;
  estimatedHours?: number;
  actualHours?: number;
  remark?: string;
}

export const dailyPlanApi = {
  getPage: (params?: WorkDailyPlanQuery) =>
    requestClient.get<PageResult<WorkDailyPlan>>('/work/daily-plans', { params }),

  getById: (id: string) =>
    requestClient.get<WorkDailyPlan>(`/work/daily-plans/${id}`),

  create: (data: CreateWorkDailyPlanInput) =>
    requestClient.post<WorkDailyPlan>('/work/daily-plans', data),

  update: (id: string, data: UpdateWorkDailyPlanInput) =>
    requestClient.put<WorkDailyPlan>(`/work/daily-plans/${id}`, data),

  delete: (id: string) =>
    requestClient.delete(`/work/daily-plans/${id}`),

  complete: (id: string) =>
    requestClient.post(`/work/daily-plans/${id}/complete`),

  convertToLog: (id: string) =>
    requestClient.post('/work/daily-plans/convert-to-log', { id }),
};

export const getWorkDailyPlanPageApi = dailyPlanApi.getPage;
export const getWorkDailyPlanApi = dailyPlanApi.getById;
export const createWorkDailyPlanApi = dailyPlanApi.create;
export const updateWorkDailyPlanApi = dailyPlanApi.update;
export const deleteWorkDailyPlanApi = dailyPlanApi.delete;
export const completeWorkDailyPlanApi = dailyPlanApi.complete;
export const convertToWorkLogApi = dailyPlanApi.convertToLog;
