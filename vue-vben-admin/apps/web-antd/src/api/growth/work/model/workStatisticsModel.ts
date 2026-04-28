import type { BaseEntity } from '../types';

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

export interface WorkStatisticsTrend {
  dates: string[];
  hours: number[];
  logs: number[];
}
