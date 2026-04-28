import type { BaseEntity, PageQuery, PageResult } from '../../types';

export interface WorkDailyPlan extends BaseEntity {
  planDate: string;
  title: string;
  content?: string;
  projectId?: string;
  projectName?: string;
  priority: number;
  status: number;
  startTime?: string;
  endTime?: string;
  estimatedHours?: number;
  actualHours?: number;
  convertedWorkLogId?: string;
  remark?: string;
}

export interface WorkDailyPlanQuery extends PageQuery {
  keyword?: string;
  planDate?: string;
  startDate?: string;
  endDate?: string;
  projectId?: string;
  status?: number;
  priority?: number;
}

export interface CreateWorkDailyPlanInput {
  planDate: string;
  title: string;
  content?: string;
  projectId?: string;
  priority?: number;
  status?: number;
  startTime?: string;
  endTime?: string;
  estimatedHours?: number;
  remark?: string;
}

export interface UpdateWorkDailyPlanInput extends Partial<CreateWorkDailyPlanInput> {
  id: string;
}

export interface ConvertToWorkLogInput {
  planId: string;
  workDate: string;
  originalContent?: string;
  totalHours?: number;
}
