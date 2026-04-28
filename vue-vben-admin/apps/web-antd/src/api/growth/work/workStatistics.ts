import { mockDelay, today } from '../mock-utils';

export interface WorkStatisticsOverview {
  totalLogs: number;
  totalHours: number;
  totalProjects: number;
  totalDevices: number;
  todayLogs: number;
  todayHours: number;
  missingDataCount: number;
  pendingSupplementCount: number;
}

export interface WorkStatisticsDailyHours {
  date: string;
  hours: number;
  logCount: number;
}

export interface WorkStatisticsProjectHours {
  projectId: string;
  projectName: string;
  totalHours: number;
  logCount: number;
  percentage: number;
}

export interface WorkStatisticsTaskTypeDistribution {
  taskTypeId: string;
  taskTypeName: string;
  totalHours: number;
  logCount: number;
  percentage: number;
}

export interface WorkStatisticsDeviceRanking {
  deviceId: string;
  deviceName: string;
  totalHours: number;
  logCount: number;
  ranking: number;
}

export interface WorkStatisticsQuery {
  startDate?: string;
  endDate?: string;
  projectId?: string;
}

export async function getWorkStatisticsOverviewApi(params?: WorkStatisticsQuery) {
  return mockDelay<WorkStatisticsOverview>({
    missingDataCount: 5,
    pendingSupplementCount: 3,
    todayHours: 8.5,
    todayLogs: 6,
    totalDevices: 12,
    totalHours: 168.5,
    totalLogs: 45,
    totalProjects: 5,
  });
}

export async function getWorkStatisticsDailyHoursApi(params?: WorkStatisticsQuery) {
  const dates = [];
  const hours = [];
  const logs = [];
  for (let i = 6; i >= 0; i--) {
    const date = new Date();
    date.setDate(date.getDate() - i);
    const dateStr = date.toISOString().slice(0, 10);
    dates.push(dateStr);
    hours.push(Math.round(Math.random() * 6 + 4) * 1.0);
    logs.push(Math.floor(Math.random() * 5 + 3));
  }
  return mockDelay<WorkStatisticsDailyHours[]>(
    dates.map((date, index) => ({
      date,
      hours: hours[index],
      logCount: logs[index],
    })),
  );
}

export async function getWorkStatisticsProjectHoursApi(params?: WorkStatisticsQuery) {
  return mockDelay<WorkStatisticsProjectHours[]>([
    { logCount: 20, percentage: 45, projectId: 'project-1', projectName: '生产线升级项目', totalHours: 75 },
    { logCount: 12, percentage: 25, projectId: 'project-2', projectName: '质量改进项目', totalHours: 42 },
    { logCount: 8, percentage: 18, projectId: 'project-3', projectName: '设备维护项目', totalHours: 30 },
    { logCount: 5, percentage: 12, projectId: 'project-4', projectName: '其他项目', totalHours: 21.5 },
  ]);
}

export async function getWorkStatisticsTaskTypeDistributionApi(params?: WorkStatisticsQuery) {
  return mockDelay<WorkStatisticsTaskTypeDistribution[]>([
    { logCount: 15, percentage: 35, taskTypeId: 'tasktype-1', taskTypeName: '调试', totalHours: 52 },
    { logCount: 12, percentage: 28, taskTypeId: 'tasktype-2', taskTypeName: '问题处理', totalHours: 42 },
    { logCount: 10, percentage: 23, taskTypeId: 'tasktype-3', taskTypeName: '维护', totalHours: 35 },
    { logCount: 8, percentage: 14, taskTypeId: 'tasktype-4', taskTypeName: '其他', totalHours: 39.5 },
  ]);
}

export async function getWorkStatisticsDeviceRankingApi(params?: WorkStatisticsQuery) {
  return mockDelay<WorkStatisticsDeviceRanking[]>([
    { deviceId: 'device-1', deviceName: 'A线体', logCount: 15, ranking: 1, totalHours: 45 },
    { deviceId: 'device-2', deviceName: 'B线体', logCount: 12, ranking: 2, totalHours: 36 },
    { deviceId: 'device-3', deviceName: 'C线体', logCount: 10, ranking: 3, totalHours: 30 },
    { deviceId: 'device-4', deviceName: 'D线体', logCount: 8, ranking: 4, totalHours: 24 },
    { deviceId: 'device-5', deviceName: 'E线体', logCount: 6, ranking: 5, totalHours: 18 },
  ]);
}
