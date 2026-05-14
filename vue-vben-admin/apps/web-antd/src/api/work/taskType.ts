import type { WorkTaskType } from './types';

import type { PageQuery, PageResult } from '#/types/api';

import { requestClient } from '#/api/request';

export type { WorkTaskType };

export interface WorkTaskTypeQuery extends PageQuery {
  enabled?: boolean | number;
}

export interface CreateWorkTaskTypeInput {
  typeName: string;
  typeCode?: string;
  description?: string;
  sort?: number;
  enabled?: boolean;
}

export interface UpdateWorkTaskTypeInput {
  typeName?: string;
  typeCode?: string;
  description?: string;
  sort?: number;
  enabled?: boolean;
}

export const taskTypeApi = {
  getPage: (params?: WorkTaskTypeQuery) =>
    requestClient.get<PageResult<WorkTaskType>>('/work/task-types', { params }),

  getById: (id: string) =>
    requestClient.get<WorkTaskType>(`/work/task-types/${id}`),

  create: (data: CreateWorkTaskTypeInput) =>
    requestClient.post<WorkTaskType>('/work/task-types', data),

  update: (id: string, data: UpdateWorkTaskTypeInput) =>
    requestClient.put<WorkTaskType>(`/work/task-types/${id}`, data),

  delete: (id: string) =>
    requestClient.delete(`/work/task-types/${id}`),
};

export const getWorkTaskTypePageApi = taskTypeApi.getPage;
export const getWorkTaskTypeApi = taskTypeApi.getById;
export const createWorkTaskTypeApi = taskTypeApi.create;
export const updateWorkTaskTypeApi = taskTypeApi.update;
export const deleteWorkTaskTypeApi = taskTypeApi.delete;
