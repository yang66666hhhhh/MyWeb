import { workStatisticsApi } from '../../api/work/statistics';
import type { WorkStatisticsOverview, WorkDailyHours, WorkProjectHours, WorkTaskTypeDistribution, WorkDeviceRanking } from '../../api/work/statistics';

export async function getOverview(): Promise<WorkStatisticsOverview> {
  const res = await workStatisticsApi.getOverview();
  return res.data;
}

export async function getDailyHours(params: { startDate?: string; endDate?: string }): Promise<WorkDailyHours[]> {
  const res = await workStatisticsApi.getDailyHours(params);
  return res.data;
}

export async function getProjectHours(params: { startDate?: string; endDate?: string }): Promise<WorkProjectHours[]> {
  const res = await workStatisticsApi.getProjectHours(params);
  return res.data;
}

export async function getTaskTypeDistribution(params: { startDate?: string; endDate?: string }): Promise<WorkTaskTypeDistribution[]> {
  const res = await workStatisticsApi.getTaskTypeDistribution(params);
  return res.data;
}

export async function getDeviceRanking(params: { startDate?: string; endDate?: string }): Promise<WorkDeviceRanking[]> {
  const res = await workStatisticsApi.getDeviceRanking(params);
  return res.data;
}
