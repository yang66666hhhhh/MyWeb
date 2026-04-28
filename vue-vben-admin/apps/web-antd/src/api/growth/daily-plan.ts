import { requestClient } from '#/api/request';

import type {
  BaseEntity,
  DailyPlanPriority,
  DailyPlanStatus,
  PageQuery,
  PageResult,
} from './types';

const DAILY_PLAN_SCHEDULE_STORAGE_KEY = 'growth.daily-plan.schedule';

interface DailyPlanApiPayload {
  description?: null | string;
  planDate: string;
  priority?: DailyPlanPriority;
  remark?: null | string;
  status?: DailyPlanStatus;
  title: string;
}

interface DailyPlanScheduleMap {
  [id: string]: {
    endTime?: string;
    startTime?: string;
  };
}

export interface DailyPlan extends BaseEntity {
  completedAt?: null | string;
  description?: null | string;
  endTime?: null | string;
  planDate: string;
  priority: DailyPlanPriority;
  remark?: null | string;
  startTime?: null | string;
  status: DailyPlanStatus;
  title: string;
}

export interface DailyPlanQuery extends PageQuery {
  date?: string;
  endDate?: string;
  keyword?: string;
  priority?: DailyPlanPriority;
  startDate?: string;
  status?: DailyPlanStatus;
}

export interface CreateDailyPlanInput {
  description?: null | string;
  endTime?: null | string;
  planDate: string;
  priority?: DailyPlanPriority;
  remark?: null | string;
  startTime?: null | string;
  status?: DailyPlanStatus;
  title: string;
}

export interface UpdateDailyPlanInput {
  description?: null | string;
  endTime?: null | string;
  planDate: string;
  priority: DailyPlanPriority;
  remark?: null | string;
  startTime?: null | string;
  status: DailyPlanStatus;
  title: string;
}

function readScheduleMap(): DailyPlanScheduleMap {
  if (typeof window === 'undefined') {
    return {};
  }
  try {
    const raw = window.localStorage.getItem(DAILY_PLAN_SCHEDULE_STORAGE_KEY);
    return raw ? (JSON.parse(raw) as DailyPlanScheduleMap) : {};
  } catch {
    return {};
  }
}

function writeScheduleMap(scheduleMap: DailyPlanScheduleMap) {
  if (typeof window === 'undefined') {
    return;
  }
  window.localStorage.setItem(
    DAILY_PLAN_SCHEDULE_STORAGE_KEY,
    JSON.stringify(scheduleMap),
  );
}

function persistSchedule(
  id: string,
  schedule: { endTime?: null | string; startTime?: null | string },
) {
  const scheduleMap = readScheduleMap();
  scheduleMap[id] = {
    endTime: schedule.endTime || undefined,
    startTime: schedule.startTime || undefined,
  };
  writeScheduleMap(scheduleMap);
}

function clearSchedule(id: string) {
  const scheduleMap = readScheduleMap();
  if (scheduleMap[id]) {
    delete scheduleMap[id];
    writeScheduleMap(scheduleMap);
  }
}

function enrichDailyPlan<T extends DailyPlan | undefined | null>(plan: T): T {
  if (!plan) {
    return plan;
  }
  const scheduleMap = readScheduleMap();
  const schedule = scheduleMap[plan.id];
  return {
    ...plan,
    endTime: plan.endTime ?? schedule?.endTime ?? null,
    startTime: plan.startTime ?? schedule?.startTime ?? null,
  };
}

function normalizeDailyPlanPage(result: PageResult<DailyPlan>) {
  return {
    ...result,
    items: result.items.map((item) => enrichDailyPlan(item) as DailyPlan),
  };
}

function toDailyPlanPayload(
  data: CreateDailyPlanInput | UpdateDailyPlanInput,
): DailyPlanApiPayload {
  return {
    description: data.description ?? null,
    planDate: data.planDate,
    priority: data.priority,
    remark: data.remark ?? null,
    status: data.status,
    title: data.title,
  };
}

export async function getDailyPlanPageApi(params: DailyPlanQuery) {
  const { date, priority, ...rest } = params;
  const requestParams = {
    ...rest,
    ...(date ? { endDate: date, startDate: date } : {}),
  };
  const result = await requestClient.get<PageResult<DailyPlan>>('/daily-plans', {
    params: requestParams,
  });
  return normalizeDailyPlanPage(result);
}

export async function getDailyPlanApi(id: string) {
  const result = await requestClient.get<DailyPlan>(`/daily-plans/${id}`);
  return enrichDailyPlan(result) as DailyPlan;
}

export async function createDailyPlanApi(data: CreateDailyPlanInput) {
  const result = await requestClient.post<DailyPlan>(
    '/daily-plans',
    toDailyPlanPayload(data),
  );
  persistSchedule(result.id, data);
  return enrichDailyPlan(result) as DailyPlan;
}

export async function updateDailyPlanApi(
  id: string,
  data: UpdateDailyPlanInput,
) {
  const result = await requestClient.put<DailyPlan>(
    `/daily-plans/${id}`,
    toDailyPlanPayload(data),
  );
  persistSchedule(id, data);
  return enrichDailyPlan(result) as DailyPlan;
}

export async function completeDailyPlanApi(id: string) {
  const result = await requestClient.request<DailyPlan>(
    `/daily-plans/${id}/complete`,
    {
      method: 'PATCH',
    },
  );
  return enrichDailyPlan(result) as DailyPlan;
}

export async function updateDailyPlanStatusApi(
  plan: DailyPlan,
  status: DailyPlanStatus,
) {
  return updateDailyPlanApi(plan.id, {
    description: plan.description,
    endTime: plan.endTime,
    planDate: plan.planDate,
    priority: plan.priority,
    remark: plan.remark,
    startTime: plan.startTime,
    status,
    title: plan.title,
  });
}

export async function deleteDailyPlanApi(id: string) {
  await requestClient.delete(`/daily-plans/${id}`);
  clearSchedule(id);
  return true;
}
