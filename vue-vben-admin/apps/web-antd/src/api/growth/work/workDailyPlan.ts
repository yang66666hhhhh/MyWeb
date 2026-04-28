import type { BaseEntity, PageQuery, PageResult } from '../../types';

import { createId, createPageResult, mockDelay, today } from '../mock-utils';

export interface WorkDailyPlan extends BaseEntity {
  planDate: string;
  title: string;
  content?: string;
  projectId?: string;
  projectName?: string;
  priority: number;
  status: number;
  startTime?: string;
  endTime?: string;
  estimatedHours?: number;
  actualHours?: number;
  convertedWorkLogId?: string;
  remark?: string;
}

export interface WorkDailyPlanQuery extends PageQuery {
  keyword?: string;
  planDate?: string;
  startDate?: string;
  endDate?: string;
  projectId?: string;
  status?: number;
  priority?: number;
}

export interface CreateWorkDailyPlanInput {
  planDate: string;
  title: string;
  content?: string;
  projectId?: string;
  priority?: number;
  status?: number;
  startTime?: string;
  endTime?: string;
  estimatedHours?: number;
  remark?: string;
}

export interface ConvertToWorkLogInput {
  planId: string;
  workDate: string;
  originalContent?: string;
  totalHours?: number;
}

const dailyPlans: WorkDailyPlan[] = [
  {
    actualHours: undefined,
    content: '完成A线体设备调试任务',
    convertedWorkLogId: undefined,
    createdAt: `${today()}T08:00:00`,
    endTime: '12:00',
    estimatedHours: 4,
    id: 'plan-1',
    planDate: today(),
    priority: 3,
    projectId: 'project-1',
    projectName: '生产线升级项目',
    remark: '',
    startTime: '08:00',
    status: 0,
    title: 'A线体设备调试',
    updatedAt: undefined,
  },
  {
    actualHours: undefined,
    content: '处理B线体异常问题',
    convertedWorkLogId: undefined,
    createdAt: `${today()}T08:30:00`,
    endTime: '17:00',
    estimatedHours: 8,
    id: 'plan-2',
    planDate: today(),
    priority: 4,
    projectId: 'project-1',
    projectName: '生产线升级项目',
    remark: '紧急',
    startTime: '09:00',
    status: 1,
    title: 'B线体问题处理',
    updatedAt: undefined,
  },
  {
    actualHours: undefined,
    content: '日常巡检设备',
    convertedWorkLogId: undefined,
    createdAt: `${today()}T09:00:00`,
    endTime: '18:00',
    estimatedHours: 1,
    id: 'plan-3',
    planDate: today(),
    priority: 1,
    projectId: 'project-2',
    projectName: '质量改进项目',
    remark: '',
    startTime: '17:00',
    status: 0,
    title: '设备日常巡检',
    updatedAt: undefined,
  },
];

function filterDailyPlans(query: WorkDailyPlanQuery) {
  return dailyPlans.filter((item) => {
    const inKeyword =
      !query.keyword ||
      item.title.includes(query.keyword) ||
      item.content?.includes(query.keyword);
    const inDate =
      (!query.startDate || item.planDate >= query.startDate) &&
      (!query.endDate || item.planDate <= query.endDate) &&
      (!query.planDate || item.planDate === query.planDate);
    const inProject = !query.projectId || item.projectId === query.projectId;
    const inStatus = query.status === undefined || item.status === query.status;
    const inPriority = query.priority === undefined || item.priority === query.priority;
    return inKeyword && inDate && inProject && inStatus && inPriority;
  });
}

export async function getWorkDailyPlanPageApi(params: WorkDailyPlanQuery) {
  return mockDelay<PageResult<WorkDailyPlan>>(
    createPageResult(filterDailyPlans(params), params),
  );
}

export async function getWorkDailyPlanApi(id: string) {
  return mockDelay(dailyPlans.find((item) => item.id === id));
}

export async function createWorkDailyPlanApi(data: CreateWorkDailyPlanInput) {
  const item: WorkDailyPlan = {
    ...data,
    createdAt: new Date().toISOString(),
    id: createId('plandaily'),
    priority: data.priority ?? 2,
    status: data.status ?? 0,
    updatedAt: undefined,
  };
  dailyPlans.unshift(item);
  return mockDelay(item);
}

export async function updateWorkDailyPlanApi(id: string, data: Partial<CreateWorkDailyPlanInput>) {
  const index = dailyPlans.findIndex((item) => item.id === id);
  if (dailyPlans[index]) {
    dailyPlans[index] = {
      ...dailyPlans[index],
      ...data,
      updatedAt: new Date().toISOString(),
    } as WorkDailyPlan;
  }
  return mockDelay(dailyPlans[index]);
}

export async function deleteWorkDailyPlanApi(id: string) {
  const index = dailyPlans.findIndex((item) => item.id === id);
  if (index >= 0) dailyPlans.splice(index, 1);
  return mockDelay(true);
}

export async function completeWorkDailyPlanApi(id: string) {
  const index = dailyPlans.findIndex((item) => item.id === id);
  if (dailyPlans[index]) {
    dailyPlans[index].status = 2;
    dailyPlans[index].updatedAt = new Date().toISOString();
  }
  return mockDelay(dailyPlans[index]);
}

export async function convertToWorkLogApi(input: ConvertToWorkLogInput) {
  return mockDelay({ workLogId: createId('worklogconverted') });
}
