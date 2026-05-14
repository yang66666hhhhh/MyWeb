import type { WorkDevice } from './types';

import type { PageQuery, PageResult } from '#/types/api';

import { requestClient } from '#/api/request';

export type { WorkDevice };

export interface WorkDeviceQuery extends PageQuery {
  projectId?: string;
  status?: number;
  deviceType?: number;
}

export interface CreateWorkDeviceInput {
  projectId?: string;
  deviceName: string;
  deviceCode?: string;
  deviceType?: number;
  description?: string;
}

export interface UpdateWorkDeviceInput {
  projectId?: string;
  deviceName?: string;
  deviceCode?: string;
  deviceType?: number;
  description?: string;
  status?: number;
}

export const deviceApi = {
  getPage: (params?: WorkDeviceQuery) =>
    requestClient.get<PageResult<WorkDevice>>('/work/devices', { params }),

  getById: (id: string) =>
    requestClient.get<WorkDevice>(`/work/devices/${id}`),

  create: (data: CreateWorkDeviceInput) =>
    requestClient.post<WorkDevice>('/work/devices', data),

  update: (id: string, data: UpdateWorkDeviceInput) =>
    requestClient.put<WorkDevice>(`/work/devices/${id}`, data),

  delete: (id: string) =>
    requestClient.delete(`/work/devices/${id}`),
};

export const getWorkDevicePageApi = deviceApi.getPage;
export const getWorkDeviceApi = deviceApi.getById;
export const createWorkDeviceApi = deviceApi.create;
export const updateWorkDeviceApi = deviceApi.update;
export const deleteWorkDeviceApi = deviceApi.delete;
