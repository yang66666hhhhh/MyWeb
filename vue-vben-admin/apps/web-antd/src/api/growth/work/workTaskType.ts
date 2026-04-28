import type { BaseEntity, PageQuery, PageResult } from '../../types';

import { requestClient } from '#/api/request';

export interface WorkTaskType extends BaseEntity {
  typeName: string;
  typeCode?: string;
  description?: string;
  sort?: number;
  enabled: boolean;
}

export interface WorkTaskTypeQuery extends PageQuery {
  keyword?: string;
  enabled?: boolean;
}

export interface CreateWorkTaskTypeInput {
  typeName: string;
  typeCode?: string;
  description?: string;
  sort?: number;
  enabled?: boolean;
}

export async function getWorkTaskTypePageApi(params: WorkTaskTypeQuery) {
  return requestClient.get<PageResult<WorkTaskType>>('/work/task-types', { params });
}

export async function getWorkTaskTypeApi(id: string) {
  return requestClient.get<WorkTaskType>(`/work/task-types/${id}`);
}

export async function createWorkTaskTypeApi(data: CreateWorkTaskTypeInput) {
  return requestClient.post<WorkTaskType>('/work/task-types', data);
}

export async function updateWorkTaskTypeApi(id: string, data: Partial<CreateWorkTaskTypeInput>) {
  return requestClient.put<WorkTaskType>(`/work/task-types/${id}`, data);
}

export async function deleteWorkTaskTypeApi(id: string) {
  return requestClient.delete(`/work/task-types/${id}`);
}
