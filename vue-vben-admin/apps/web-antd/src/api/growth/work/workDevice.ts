import type { BaseEntity, PageQuery, PageResult } from '../../types';

import { requestClient } from '#/api/request';

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

export async function getWorkDevicePageApi(params: WorkDeviceQuery) {
  return requestClient.get<PageResult<WorkDevice>>('/work/devices', { params });
}

export async function getWorkDeviceApi(id: string) {
  return requestClient.get<WorkDevice>(`/work/devices/${id}`);
}

export async function createWorkDeviceApi(data: CreateWorkDeviceInput) {
  return requestClient.post<WorkDevice>('/work/devices', data);
}

export async function updateWorkDeviceApi(id: string, data: Partial<CreateWorkDeviceInput>) {
  return requestClient.put<WorkDevice>(`/work/devices/${id}`, data);
}

export async function deleteWorkDeviceApi(id: string) {
  return requestClient.delete(`/work/devices/${id}`);
}
