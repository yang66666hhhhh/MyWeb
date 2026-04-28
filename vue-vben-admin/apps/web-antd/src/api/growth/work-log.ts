import type { BaseEntity, PageQuery, PageResult } from './types';

import {
  createId,
  createPageResult,
  includesKeyword,
  mockDelay,
  today,
} from './mock-utils';

export interface WorkLog extends BaseEntity {
  category: string;
  content: string;
  durationMinutes: number;
  issue?: string;
  logDate: string;
  nextPlan?: string;
  projectId?: string;
  projectName?: string;
  summary: string;
  title: string;
  tags?: string[];
}

export interface WorkLogQuery extends PageQuery {
  endDate?: string;
  keyword?: string;
  projectId?: string;
  startDate?: string;
}

export type SaveWorkLogInput = Omit<WorkLog, 'createdAt' | 'id' | 'updatedAt'>;

const workLogs: WorkLog[] = [
  {
    category: '开发',
    content: '整理个人管理系统 growth 模块，梳理 DailyPlan 与成长模块页面结构。',
    createdAt: `${today()}T09:00:00`,
    durationMinutes: 120,
    id: 'work-log-1',
    issue: '后端模块尚未全部实现，需要先用前端 mock 跑通交互。',
    logDate: today(),
    nextPlan: '补齐工作日志、知识库、项目管理接口。',
    projectId: 'project-1',
    projectName: '个人管理系统',
    summary: '完成前端模块结构设计。',
    tags: ['Vue', 'Vben', 'ASP.NET Core'],
    title: '个人管理系统前端结构设计',
  },
  {
    category: '学习',
    content: '完成英语阅读精读并整理错题，顺手复盘今日节奏。',
    createdAt: `${today()}T21:00:00`,
    durationMinutes: 90,
    id: 'work-log-2',
    issue: '晚间精力波动较大，专注时段需要继续优化。',
    logDate: today(),
    nextPlan: '补一篇英语阅读，继续整理知识库笔记。',
    projectId: 'project-study',
    projectName: '在职备考',
    summary: '学习节奏基本稳定，错题记录需要继续细化。',
    tags: ['英语', '复盘'],
    title: '英语阅读与复盘',
  },
];

function filterWorkLogs(query: WorkLogQuery) {
  return workLogs.filter((item) => {
    const inDateRange =
      (!query.startDate || item.logDate >= query.startDate) &&
      (!query.endDate || item.logDate <= query.endDate);
    const inProject = !query.projectId || item.projectId === query.projectId;
    const inKeyword =
      includesKeyword(item.title, query.keyword) ||
      includesKeyword(item.content, query.keyword) ||
      includesKeyword(item.summary, query.keyword);
    return inDateRange && inProject && inKeyword;
  });
}

export async function getWorkLogPageApi(params: WorkLogQuery) {
  return mockDelay<PageResult<WorkLog>>(
    createPageResult(filterWorkLogs(params), params),
  );
}

export async function getWorkLogApi(id: string) {
  return mockDelay(workLogs.find((item) => item.id === id));
}

export async function createWorkLogApi(data: SaveWorkLogInput) {
  const item: WorkLog = {
    ...data,
    createdAt: new Date().toISOString(),
    id: createId('work-log'),
  };
  workLogs.unshift(item);
  return mockDelay(item);
}

export async function updateWorkLogApi(id: string, data: SaveWorkLogInput) {
  const index = workLogs.findIndex((item) => item.id === id);
  const current = workLogs[index];
  if (current) {
    workLogs[index] = {
      ...current,
      ...data,
      updatedAt: new Date().toISOString(),
    };
  }
  return mockDelay(workLogs[index]);
}

export async function deleteWorkLogApi(id: string) {
  const index = workLogs.findIndex((item) => item.id === id);
  if (index >= 0) {
    workLogs.splice(index, 1);
  }
  return mockDelay(true);
}
