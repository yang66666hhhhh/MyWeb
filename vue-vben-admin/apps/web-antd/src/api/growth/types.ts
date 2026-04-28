export interface PageQuery {
  page?: number;
  pageSize?: number;
}

export interface PageResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
}

export interface BaseEntity {
  createdAt?: string;
  id: string;
  updatedAt?: null | string;
}

export type DailyPlanStatus = 0 | 1 | 2 | 3;
export type DailyPlanPriority = 1 | 2 | 3 | 4 | 5;
export type HabitStatus = 0 | 1;
export type ProjectStatus = 0 | 1 | 2 | 3;
export type ProjectType = 0 | 1 | 2 | 3;
