import type { BaseEntity, PageQuery, PageResult } from '../types';

export interface WorkProject extends BaseEntity {
  projectName: string;
  projectCode?: string;
  projectType?: number;
  customerName?: string;
  description?: string;
  startDate?: string;
  endDate?: string;
  status: number;
  sort?: number;
}

export interface WorkProjectQuery extends PageQuery {
  keyword?: string;
  status?: number;
  projectType?: number;
}

export interface CreateWorkProjectInput {
  projectName: string;
  projectCode?: string;
  projectType?: number;
  customerName?: string;
  description?: string;
  startDate?: string;
  endDate?: string;
  status?: number;
  sort?: number;
}

export interface UpdateWorkProjectInput extends Partial<CreateWorkProjectInput> {
  id: string;
}
