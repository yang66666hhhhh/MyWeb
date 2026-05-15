import type { WorkProject } from './types';

import type { PageQuery, PageResult } from '#/types/api';

import { requestClient } from '#/api/request';

export type { WorkProject };

export interface WorkProjectQuery extends PageQuery {
  status?: number;
  projectType?: number;
}

export interface CreateWorkProjectInput {
  projectName: string;
  projectCode?: string;
  projectType?: number;
  customerName?: string;
  location?: string;
  description?: string;
  startDate?: string;
  endDate?: string;
}

export interface UpdateWorkProjectInput {
  projectName?: string;
  projectCode?: string;
  projectType?: number;
  customerName?: string;
  location?: string;
  description?: string;
  startDate?: string;
  endDate?: string;
  status?: number;
}

export const projectApi = {
  getPage: (params?: WorkProjectQuery) =>
    requestClient.get<PageResult<WorkProject>>('/work/projects', { params }),

  getById: (id: string) =>
    requestClient.get<WorkProject>(`/work/projects/${id}`),

  create: (data: CreateWorkProjectInput) =>
    requestClient.post<WorkProject>('/work/projects', data),

  update: (id: string, data: UpdateWorkProjectInput) =>
    requestClient.put<WorkProject>(`/work/projects/${id}`, data),

  delete: (id: string) =>
    requestClient.delete(`/work/projects/${id}`),
};

export const getWorkProjectPageApi = projectApi.getPage;
export const getWorkProjectApi = projectApi.getById;
export const createWorkProjectApi = projectApi.create;
export const updateWorkProjectApi = projectApi.update;
export const deleteWorkProjectApi = projectApi.delete;
