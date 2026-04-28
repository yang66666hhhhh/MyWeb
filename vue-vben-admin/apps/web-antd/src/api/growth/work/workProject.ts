import type { BaseEntity, PageQuery, PageResult } from '../../types';

import { createId, createPageResult, mockDelay, today } from '../mock-utils';

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

const projects: WorkProject[] = [
  {
    createdAt: `${today()}T08:00:00`,
    customerName: '内部',
    description: '生产线自动化升级改造',
    endDate: '',
    id: 'project-1',
    projectCode: 'PRJ-001',
    projectName: '生产线升级项目',
    projectType: 0,
    sort: 1,
    startDate: today(),
    status: 0,
    updatedAt: undefined,
  },
  {
    createdAt: `${today()}T08:30:00`,
    customerName: '内部',
    description: '提升产品质量和良品率',
    endDate: '',
    id: 'project-2',
    projectCode: 'PRJ-002',
    projectName: '质量改进项目',
    projectType: 1,
    sort: 2,
    startDate: today(),
    status: 0,
    updatedAt: undefined,
  },
];

function filterProjects(query: WorkProjectQuery) {
  return projects.filter((item) => {
    const inKeyword =
      !query.keyword ||
      item.projectName.includes(query.keyword) ||
      item.projectCode?.includes(query.keyword);
    const inStatus = query.status === undefined || item.status === query.status;
    const inType = query.projectType === undefined || item.projectType === query.projectType;
    return inKeyword && inStatus && inType;
  });
}

export async function getWorkProjectPageApi(params: WorkProjectQuery) {
  return mockDelay<PageResult<WorkProject>>(createPageResult(filterProjects(params), params));
}

export async function getWorkProjectApi(id: string) {
  return mockDelay(projects.find((item) => item.id === id));
}

export async function createWorkProjectApi(data: CreateWorkProjectInput) {
  const item: WorkProject = {
    ...data,
    createdAt: new Date().toISOString(),
    id: createId('project'),
    status: data.status ?? 0,
    updatedAt: undefined,
  };
  projects.unshift(item);
  return mockDelay(item);
}

export async function updateWorkProjectApi(id: string, data: Partial<CreateWorkProjectInput>) {
  const index = projects.findIndex((item) => item.id === id);
  if (projects[index]) {
    projects[index] = {
      ...projects[index],
      ...data,
      updatedAt: new Date().toISOString(),
    } as WorkProject;
  }
  return mockDelay(projects[index]);
}

export async function deleteWorkProjectApi(id: string) {
  const index = projects.findIndex((item) => item.id === id);
  if (index >= 0) projects.splice(index, 1);
  return mockDelay(true);
}
