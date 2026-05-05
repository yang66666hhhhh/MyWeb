import type { PageQuery, PageResult } from '#/types/api';
import { requestClient } from '#/api/request';
import type { WorkLog } from './types';

export type { WorkLog };

export interface WorkLogQuery extends PageQuery {
  workDate?: string;
  startDate?: string;
  endDate?: string;
  projectId?: string;
  templateId?: string;
  status?: number;
  sourceType?: number;
}

export interface CreateWorkLogInput {
  workDate: string;
  projectId: string;
  title: string;
  originalContent?: string;
  summary?: string;
  totalHours: number;
  remark?: string;
  templateId?: string;
  dynamicValues?: DynamicFieldInput[];
}

export interface UpdateWorkLogInput {
  workDate?: string;
  projectId?: string;
  title?: string;
  originalContent?: string;
  summary?: string;
  totalHours?: number;
  status?: number;
  remark?: string;
  templateId?: string;
  dynamicValues?: DynamicFieldInput[];
}

export type SaveWorkLogInput = CreateWorkLogInput | UpdateWorkLogInput;

export interface DynamicFieldInput {
  fieldName: string;
  stringValue?: string;
  numberValue?: number;
  dateValue?: string;
}

export interface WorkLogDynamicValueDto {
  id: string;
  workLogId: string;
  fieldName: string;
  stringValue?: string;
  numberValue?: number;
  dateValue?: string;
}

export interface WorkLogDetail extends WorkLog {
  dynamicValues?: WorkLogDynamicValueDto[];
  templateName?: string;
}

export const workLogApi = {
  getPage: (params?: WorkLogQuery) =>
    requestClient.get<PageResult<WorkLogDetail>>('/work/logs', { params }),

  getById: (id: string) =>
    requestClient.get<WorkLogDetail>(`/work/logs/${id}`),

  create: (data: CreateWorkLogInput) =>
    requestClient.post<WorkLogDetail>('/work/logs', data),

  update: (id: string, data: UpdateWorkLogInput) =>
    requestClient.put<WorkLogDetail>(`/work/logs/${id}`, data),

  delete: (id: string) =>
    requestClient.delete(`/work/logs/${id}`),
};

export const getWorkLogPageApi = workLogApi.getPage;
export const getWorkLogApi = workLogApi.getById;
export const createWorkLogApi = workLogApi.create;
export const updateWorkLogApi = workLogApi.update;
export const deleteWorkLogApi = workLogApi.delete;
