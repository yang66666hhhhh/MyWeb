import type { BaseEntity, HabitStatus, PageQuery, PageResult } from './types';

import {
  createId,
  createPageResult,
  includesKeyword,
  mockDelay,
  today,
} from './mock-utils';

export interface Habit extends BaseEntity {
  checkInCount: number;
  currentStreak: number;
  description?: null | string;
  habitType: string;
  longestStreak: number;
  lastCheckInDate?: null | string;
  name: string;
  status: HabitStatus;
  targetFrequency: string;
  todayCompleted: boolean;
}

export interface HabitQuery extends PageQuery {
  habitType?: string;
  keyword?: string;
  status?: HabitStatus;
}

export interface SaveHabitInput {
  description?: null | string;
  habitType: string;
  name: string;
  status?: HabitStatus;
  targetFrequency: string;
}

const habits: Habit[] = [
  {
    checkInCount: 12,
    createdAt: `${today()}T08:00:00`,
    currentStreak: 5,
    description: '保持 408 学习节奏',
    habitType: '学习',
    id: 'habit-1',
    lastCheckInDate: today(),
    longestStreak: 9,
    name: '晚间 408 学习',
    status: 1,
    targetFrequency: '每天',
    todayCompleted: true,
  },
  {
    checkInCount: 6,
    createdAt: `${today()}T08:00:00`,
    currentStreak: 2,
    description: '保持身体状态',
    habitType: '生活',
    id: 'habit-2',
    lastCheckInDate: null,
    longestStreak: 4,
    name: '运动 30 分钟',
    status: 1,
    targetFrequency: '每周 4 次',
    todayCompleted: false,
  },
];

function filterHabits(query: HabitQuery) {
  return habits.filter((item) => {
    const inKeyword =
      includesKeyword(item.name, query.keyword) ||
      includesKeyword(item.description, query.keyword);
    const inStatus = query.status === undefined || item.status === query.status;
    const inType = !query.habitType || item.habitType === query.habitType;
    return inKeyword && inStatus && inType;
  });
}

export async function getHabitPageApi(params: HabitQuery) {
  return mockDelay<PageResult<Habit>>(
    createPageResult(filterHabits(params), params),
  );
}

export async function getHabitApi(id: string) {
  return mockDelay(habits.find((item) => item.id === id));
}

export async function createHabitApi(data: SaveHabitInput) {
  const item: Habit = {
    ...data,
    checkInCount: 0,
    createdAt: new Date().toISOString(),
    currentStreak: 0,
    id: createId('habit'),
    lastCheckInDate: null,
    longestStreak: 0,
    status: data.status ?? 1,
    todayCompleted: false,
  };
  habits.unshift(item);
  return mockDelay(item);
}

export async function updateHabitApi(id: string, data: SaveHabitInput) {
  const index = habits.findIndex((item) => item.id === id);
  const current = habits[index];
  if (current) {
    habits[index] = {
      ...current,
      ...data,
      status: data.status ?? current.status,
      updatedAt: new Date().toISOString(),
    };
  }
  return mockDelay(habits[index]);
}

export async function checkInHabitApi(id: string) {
  const habit = habits.find((item) => item.id === id);
  if (habit && !habit.todayCompleted) {
    habit.todayCompleted = true;
    habit.checkInCount += 1;
    habit.currentStreak += 1;
    habit.longestStreak = Math.max(habit.longestStreak, habit.currentStreak);
    habit.lastCheckInDate = today();
    habit.updatedAt = new Date().toISOString();
  }
  return mockDelay(habit);
}

export async function updateHabitStatusApi(id: string, status: HabitStatus) {
  const habit = habits.find((item) => item.id === id);
  if (habit) {
    habit.status = status;
    habit.updatedAt = new Date().toISOString();
  }
  return mockDelay(habit);
}

export async function deleteHabitApi(id: string) {
  const index = habits.findIndex((item) => item.id === id);
  if (index >= 0) {
    habits.splice(index, 1);
  }
  return mockDelay(true);
}
