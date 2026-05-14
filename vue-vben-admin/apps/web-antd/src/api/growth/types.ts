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

export const HabitStatus = {
  Active: 0,
  Paused: 1,
  Completed: 2,
  Archived: 3,
} as const;

export type HabitStatus = (typeof HabitStatus)[keyof typeof HabitStatus];

export const ProjectStatus = {
  Planning: 0,
  Active: 1,
  Completed: 2,
  Paused: 3,
  Archived: 4,
} as const;

export type ProjectStatus = (typeof ProjectStatus)[keyof typeof ProjectStatus];

export const ProjectType = {
  Learning: 0,
  Skill: 1,
  Career: 2,
  Health: 3,
  Other: 4,
} as const;

export type ProjectType = (typeof ProjectType)[keyof typeof ProjectType];

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
  page: number;
  pageSize: number;
  total: number;
};
