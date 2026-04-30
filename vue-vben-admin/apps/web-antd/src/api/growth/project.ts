import type {
  BaseEntity,
  PageQuery,
  PageResult,
  ProjectStatus,
  ProjectType,
} from './types';

import { requestClient } from '#/api/request';

export interface Project extends BaseEntity {
  description?: string;
  endDate?: string;
  name: string;
  progress: number;
  startDate?: string;
  status: ProjectStatus;
  taskCount: number;
  type: ProjectType;
}

export interface ProjectQuery extends PageQuery {
  keyword?: string;
  status?: ProjectStatus;
  type?: ProjectType;
}

export type SaveProjectInput = Omit<Project, 'createdAt' | 'id' | 'updatedAt'>;

export async function getProjectPageApi(params: ProjectQuery) {
  return requestClient.get<PageResult<Project>>('/projects', { params });
}

export async function getProjectApi(id: string) {
  return requestClient.get<Project>(`/projects/${id}`);
}

export async function createProjectApi(data: SaveProjectInput) {
  return requestClient.post<Project>('/projects', data);
}

export async function updateProjectApi(id: string, data: SaveProjectInput) {
  return requestClient.put<Project>(`/projects/${id}`, data);
}

export async function deleteProjectApi(id: string) {
  return requestClient.delete(`/projects/${id}`);
}