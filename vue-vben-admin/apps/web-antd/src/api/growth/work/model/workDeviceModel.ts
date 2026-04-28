import type { BaseEntity, PageQuery, PageResult } from '../types';

export interface WorkDevice extends BaseEntity {
  projectId?: string;
  deviceName: string;
  deviceCode?: string;
  deviceType?: number;
  description?: string;
  status: number;
}

export interface WorkDeviceQuery extends PageQuery {
  keyword?: string;
  projectId?: string;
  deviceType?: number;
  status?: number;
}

export interface CreateWorkDeviceInput {
  projectId?: string;
  deviceName: string;
  deviceCode?: string;
  deviceType?: number;
  description?: string;
  status?: number;
}

export interface UpdateWorkDeviceInput extends Partial<CreateWorkDeviceInput> {
  id: string;
}
