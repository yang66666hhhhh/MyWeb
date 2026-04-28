import type { BaseEntity, PageQuery, PageResult } from '../../types';

import { createId, createPageResult, mockDelay, today } from '../mock-utils';

export interface WorkLog extends BaseEntity {
  workDate: string;
  weekDay: string;
  projectId: string;
  projectName: string;
  deviceIds: string[];
  deviceNames: string[];
  taskTypeIds: string[];
  taskTypeNames: string[];
  title: string;
  originalContent: string;
  summary: string;
  totalHours: number;
  status: number;
  sourceType: number;
  importBatchId?: string;
  remark?: string;
}

export interface WorkLogQuery extends PageQuery {
  keyword?: string;
  workDate?: string;
  startDate?: string;
  endDate?: string;
  projectId?: string;
  deviceId?: string;
  taskTypeId?: string;
  sourceType?: number;
  status?: number;
}

export interface CreateWorkLogInput {
  workDate: string;
  weekDay: string;
  projectId: string;
  deviceIds: string[];
  taskTypeIds: string[];
  title: string;
  originalContent?: string;
  summary?: string;
  totalHours?: number;
  status?: number;
  sourceType?: number;
  remark?: string;
}

const workLogs: WorkLog[] = [
  {
    createdAt: `${today()}T09:00:00`,
    deviceIds: ['device-1'],
    deviceNames: ['A线体'],
    id: 'worklog-1',
    importBatchId: undefined,
    originalContent: '完成设备调试和参数优化',
    projectId: 'project-1',
    projectName: '生产线升级项目',
    remark: '',
    sourceType: 0,
    status: 0,
    summary: '今日主要完成A线体调试，产出达标',
    taskTypeIds: ['tasktype-1'],
    taskTypeNames: ['调试'],
    title: 'A线体设备调试',
    totalHours: 4,
    updatedAt: undefined,
    weekDay: '周一',
    workDate: today(),
  },
  {
    createdAt: `${today()}T10:30:00`,
    deviceIds: ['device-2'],
    deviceNames: ['B线体'],
    id: 'worklog-2',
    importBatchId: undefined,
    originalContent: '',
    projectId: 'project-1',
    projectName: '生产线升级项目',
    remark: '缺失数据',
    sourceType: 1,
    status: 1,
    summary: '',
    taskTypeIds: [],
    taskTypeNames: [],
    title: 'B线体数据记录',
    totalHours: 0,
    updatedAt: undefined,
    weekDay: '周一',
    workDate: today(),
  },
  {
    createdAt: `${today()}T14:00:00`,
    deviceIds: ['device-1', 'device-3'],
    deviceNames: ['A线体', 'C线体'],
    id: 'worklog-3',
    importBatchId: undefined,
    originalContent: '生产异常问题处理',
    projectId: 'project-2',
    projectName: '质量改进项目',
    remark: '',
    sourceType: 0,
    status: 0,
    summary: '解决了2个质量问题',
    taskTypeIds: ['tasktype-2'],
    taskTypeNames: ['问题处理'],
    title: '质量问题处理',
    totalHours: 3,
    updatedAt: undefined,
    weekDay: '周一',
    workDate: today(),
  },
];

function filterWorkLogs(query: WorkLogQuery) {
  return workLogs.filter((item) => {
    const inKeyword =
      !query.keyword ||
      item.title.includes(query.keyword) ||
      item.originalContent.includes(query.keyword);
    const inDate =
      (!query.startDate || item.workDate >= query.startDate) &&
      (!query.endDate || item.workDate <= query.endDate) &&
      (!query.workDate || item.workDate === query.workDate);
    const inProject = !query.projectId || item.projectId === query.projectId;
    const inSourceType = query.sourceType === undefined || item.sourceType === query.sourceType;
    const inStatus = query.status === undefined || item.status === query.status;
    return inKeyword && inDate && inProject && inSourceType && inStatus;
  });
}

export async function getWorkLogPageApi(params: WorkLogQuery) {
  return mockDelay<PageResult<WorkLog>>(createPageResult(filterWorkLogs(params), params));
}

export async function getWorkLogApi(id: string) {
  return mockDelay(workLogs.find((item) => item.id === id));
}

export async function createWorkLogApi(data: CreateWorkLogInput) {
  const item: WorkLog = {
    ...data,
    createdAt: new Date().toISOString(),
    deviceIds: data.deviceIds || [],
    deviceNames: [],
    id: createId('worklog'),
    projectName: data.projectId,
    status: data.status ?? 0,
    summary: data.summary ?? '',
    taskTypeIds: data.taskTypeIds || [],
    taskTypeNames: [],
    updatedAt: undefined,
  };
  workLogs.unshift(item);
  return mockDelay(item);
}

export async function updateWorkLogApi(id: string, data: Partial<CreateWorkLogInput>) {
  const index = workLogs.findIndex((item) => item.id === id);
  if (workLogs[index]) {
    workLogs[index] = {
      ...workLogs[index],
      ...data,
      updatedAt: new Date().toISOString(),
    } as WorkLog;
  }
  return mockDelay(workLogs[index]);
}

export async function deleteWorkLogApi(id: string) {
  const index = workLogs.findIndex((item) => item.id === id);
  if (index >= 0) workLogs.splice(index, 1);
  return mockDelay(true);
}
