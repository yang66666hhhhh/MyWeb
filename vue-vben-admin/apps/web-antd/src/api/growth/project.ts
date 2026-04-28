import type {
  BaseEntity,
  PageQuery,
  PageResult,
  ProjectStatus,
  ProjectType,
} from './types';

import {
  createId,
  createPageResult,
  includesKeyword,
  mockDelay,
  today,
} from './mock-utils';

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

const projects: Project[] = [
  {
    createdAt: `${today()}T08:00:00`,
    description: 'Vue Vben Admin + ASP.NET Core Web API 的个人管理系统。',
    endDate: '',
    id: 'project-1',
    name: '个人管理系统',
    progress: 35,
    startDate: today(),
    status: 1,
    taskCount: 12,
    type: 2,
  },
  {
    createdAt: `${today()}T08:30:00`,
    description: '跟踪在职备考的学习计划、阶段目标和资料整理。',
    endDate: '',
    id: 'project-study',
    name: '在职备考',
    progress: 42,
    startDate: today(),
    status: 1,
    taskCount: 8,
    type: 1,
  },
];

function filterProjects(query: ProjectQuery) {
  return projects.filter((item) => {
    const inKeyword =
      includesKeyword(item.name, query.keyword) ||
      includesKeyword(item.description, query.keyword);
    const inStatus = query.status === undefined || item.status === query.status;
    const inType = query.type === undefined || item.type === query.type;
    return inKeyword && inStatus && inType;
  });
}

export async function getProjectPageApi(params: ProjectQuery) {
  return mockDelay<PageResult<Project>>(
    createPageResult(filterProjects(params), params),
  );
}

export async function getProjectApi(id: string) {
  return mockDelay(projects.find((item) => item.id === id));
}

export async function createProjectApi(data: SaveProjectInput) {
  const item: Project = {
    ...data,
    createdAt: new Date().toISOString(),
    id: createId('project'),
  };
  projects.unshift(item);
  return mockDelay(item);
}

export async function updateProjectApi(id: string, data: SaveProjectInput) {
  const index = projects.findIndex((item) => item.id === id);
  const current = projects[index];
  if (current) {
    projects[index] = {
      ...current,
      ...data,
      updatedAt: new Date().toISOString(),
    };
  }
  return mockDelay(projects[index]);
}

export async function deleteProjectApi(id: string) {
  const index = projects.findIndex((item) => item.id === id);
  if (index >= 0) {
    projects.splice(index, 1);
  }
  return mockDelay(true);
}
