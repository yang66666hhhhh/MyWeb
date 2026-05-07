export const DailyPlanPriority = {
  Low: 1,
  Medium: 2,
  High: 3,
  Urgent: 4,
} as const;

export type DailyPlanPriority = (typeof DailyPlanPriority)[keyof typeof DailyPlanPriority];

export const DailyPlanStatus = {
  Pending: 0,
  InProgress: 1,
  Completed: 2,
  Cancelled: 3,
} as const;

export type DailyPlanStatus = (typeof DailyPlanStatus)[keyof typeof DailyPlanStatus];

export interface BaseEntity {
  id: string;
  createdAt: string;
  updatedAt?: string;
}

export type PageQuery = {
  page?: number;
  pageSize?: number;
};

export type PageResult<T> = {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
};