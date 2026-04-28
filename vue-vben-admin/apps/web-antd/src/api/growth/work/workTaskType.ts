import type { BaseEntity, PageQuery, PageResult } from '../../types';

import { createId, createPageResult, mockDelay, today } from '../mock-utils';

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

const taskTypes: WorkTaskType[] = [
  {
    createdAt: `${today()}T08:00:00`,
    description: '设备调试工作',
    enabled: true,
    id: 'tasktype-1',
    sort: 1,
    typeCode: 'TT-001',
    typeName: '调试',
    updatedAt: undefined,
  },
  {
    createdAt: `${today()}T08:00:00`,
    description: '问题处理和解决',
    enabled: true,
    id: 'tasktype-2',
    sort: 2,
    typeCode: 'TT-002',
    typeName: '问题处理',
    updatedAt: undefined,
  },
  {
    createdAt: `${today()}T08:00:00`,
    description: '日常维护保养',
    enabled: true,
    id: 'tasktype-3',
    sort: 3,
    typeCode: 'TT-003',
    typeName: '维护',
    updatedAt: undefined,
  },
  {
    createdAt: `${today()}T08:00:00`,
    description: '生产作业工作',
    enabled: true,
    id: 'tasktype-4',
    sort: 4,
    typeCode: 'TT-004',
    typeName: '生产',
    updatedAt: undefined,
  },
  {
    createdAt: `${today()}T08:00:00`,
    description: '质量检测工作',
    enabled: true,
    id: 'tasktype-5',
    sort: 5,
    typeCode: 'TT-005',
    typeName: '检测',
    updatedAt: undefined,
  },
];

function filterTaskTypes(query: WorkTaskTypeQuery) {
  return taskTypes.filter((item) => {
    const inKeyword =
      !query.keyword ||
      item.typeName.includes(query.keyword) ||
      item.typeCode?.includes(query.keyword);
    const inEnabled = query.enabled === undefined || item.enabled === query.enabled;
    return inKeyword && inEnabled;
  });
}

export async function getWorkTaskTypePageApi(params: WorkTaskTypeQuery) {
  return mockDelay<PageResult<WorkTaskType>>(createPageResult(filterTaskTypes(params), params));
}

export async function getWorkTaskTypeApi(id: string) {
  return mockDelay(taskTypes.find((item) => item.id === id));
}

export async function createWorkTaskTypeApi(data: CreateWorkTaskTypeInput) {
  const item: WorkTaskType = {
    ...data,
    createdAt: new Date().toISOString(),
    enabled: data.enabled ?? true,
    id: createId('tasktype'),
    updatedAt: undefined,
  };
  taskTypes.unshift(item);
  return mockDelay(item);
}

export async function updateWorkTaskTypeApi(id: string, data: Partial<CreateWorkTaskTypeInput>) {
  const index = taskTypes.findIndex((item) => item.id === id);
  if (taskTypes[index]) {
    taskTypes[index] = {
      ...taskTypes[index],
      ...data,
      updatedAt: new Date().toISOString(),
    } as WorkTaskType;
  }
  return mockDelay(taskTypes[index]);
}

export async function deleteWorkTaskTypeApi(id: string) {
  const index = taskTypes.findIndex((item) => item.id === id);
  if (index >= 0) taskTypes.splice(index, 1);
  return mockDelay(true);
}
