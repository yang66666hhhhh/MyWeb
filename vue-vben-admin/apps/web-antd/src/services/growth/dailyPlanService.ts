import { growthApi } from '../../api/growth/dailyPlan';
import type { DailyPlanItem, DailyPlanQuery, DailyPlanCreate, DailyPlanUpdate } from '../../api/growth/dailyPlan';
import type { PageResult } from '../../types/api';

export async function getDailyPlans(query: DailyPlanQuery): Promise<PageResult<DailyPlanItem>> {
  const res = await growthApi.getDailyPlans(query);
  return res.data;
}

export async function getDailyPlanById(id: string): Promise<DailyPlanItem> {
  const res = await growthApi.getDailyPlanById(id);
  return res.data;
}

export async function createDailyPlan(data: DailyPlanCreate): Promise<DailyPlanItem> {
  const res = await growthApi.createDailyPlan(data);
  return res.data;
}

export async function updateDailyPlan(id: string, data: DailyPlanUpdate): Promise<DailyPlanItem> {
  const res = await growthApi.updateDailyPlan(id, data);
  return res.data;
}

export async function completeDailyPlan(id: string): Promise<void> {
  await growthApi.completeDailyPlan(id);
}

export async function deleteDailyPlan(id: string): Promise<void> {
  await growthApi.deleteDailyPlan(id);
}
