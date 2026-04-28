import type { BaseEntity, PageQuery, PageResult } from '../types';

export interface WorkTaskType extends BaseEntity {
  typeName: string;
  typeCode?: string;
  description?: string;
  sort?: number;
  enabled: boolean;
}

export interface WorkTaskTypeQuery extends PageQuery {
  keyword?: string;
  enabled?: boolean;
}

export interface CreateWorkTaskTypeInput {
  typeName: string;
  typeCode?: string;
  description?: string;
  sort?: number;
  enabled?: boolean;
}

export interface UpdateWorkTaskTypeInput extends Partial<CreateWorkTaskTypeInput> {
  id: string;
}
