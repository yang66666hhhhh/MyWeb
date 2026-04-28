import type { BaseEntity, PageQuery, PageResult } from '../../types';

import { requestClient } from '#/api/request';

export interface WorkProject extends BaseEntity {
  projectName: string;
  projectCode?: string;
  projectType?: number;
  customerName?: string;
  description?: string;
  startDate?: string;
  endDate?: string;
  status: number;
  sort?: number;
}

export interface WorkProjectQuery extends PageQuery {
  keyword?: string;
  status?: number;
  projectType?: number;
}

export interface CreateWorkProjectInput {
  projectName: string;
  projectCode?: string;
  projectType?: number;
  customerName?: string;
  description?: string;
  startDate?: string;
  endDate?: string;
  status?: number;
  sort?: number;
}

export async function getWorkProjectPageApi(params: WorkProjectQuery) {
  return requestClient.get<PageResult<WorkProject>>('/work/projects', { params });
}

export async function getWorkProjectApi(id: string) {
  return requestClient.get<WorkProject>(`/work/projects/${id}`);
}

export async function createWorkProjectApi(data: CreateWorkProjectInput) {
  return requestClient.post<WorkProject>('/work/projects', data);
}

export async function updateWorkProjectApi(id: string, data: Partial<CreateWorkProjectInput>) {
  return requestClient.put<WorkProject>(`/work/projects/${id}`, data);
}

export async function deleteWorkProjectApi(id: string) {
  return requestClient.delete(`/work/projects/${id}`);
}
