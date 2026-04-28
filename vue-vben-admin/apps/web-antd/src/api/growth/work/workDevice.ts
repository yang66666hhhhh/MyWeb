import type { BaseEntity, PageQuery, PageResult } from '../../types';

import { createId, createPageResult, mockDelay, today } from '../mock-utils';

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

const devices: WorkDevice[] = [
  {
    createdAt: `${today()}T08:00:00`,
    description: '主生产线A',
    deviceCode: 'DEVICE-A',
    deviceName: 'A线体',
    deviceType: 0,
    id: 'device-1',
    projectId: 'project-1',
    status: 0,
    updatedAt: undefined,
  },
  {
    createdAt: `${today()}T08:00:00`,
    description: '主生产线B',
    deviceCode: 'DEVICE-B',
    deviceName: 'B线体',
    deviceType: 0,
    id: 'device-2',
    projectId: 'project-1',
    status: 0,
    updatedAt: undefined,
  },
  {
    createdAt: `${today()}T08:00:00`,
    description: '主生产线C',
    deviceCode: 'DEVICE-C',
    deviceName: 'C线体',
    deviceType: 0,
    id: 'device-3',
    projectId: 'project-1',
    status: 0,
    updatedAt: undefined,
  },
  {
    createdAt: `${today()}T09:00:00`,
    description: '测试设备1',
    deviceCode: 'TEST-001',
    deviceName: '测试设备1',
    deviceType: 2,
    id: 'device-4',
    projectId: 'project-2',
    status: 0,
    updatedAt: undefined,
  },
];

function filterDevices(query: WorkDeviceQuery) {
  return devices.filter((item) => {
    const inKeyword =
      !query.keyword ||
      item.deviceName.includes(query.keyword) ||
      item.deviceCode?.includes(query.keyword);
    const inProject = !query.projectId || item.projectId === query.projectId;
    const inType = query.deviceType === undefined || item.deviceType === query.deviceType;
    const inStatus = query.status === undefined || item.status === query.status;
    return inKeyword && inProject && inType && inStatus;
  });
}

export async function getWorkDevicePageApi(params: WorkDeviceQuery) {
  return mockDelay<PageResult<WorkDevice>>(createPageResult(filterDevices(params), params));
}

export async function getWorkDeviceApi(id: string) {
  return mockDelay(devices.find((item) => item.id === id));
}

export async function createWorkDeviceApi(data: CreateWorkDeviceInput) {
  const item: WorkDevice = {
    ...data,
    createdAt: new Date().toISOString(),
    id: createId('device'),
    status: data.status ?? 0,
    updatedAt: undefined,
  };
  devices.unshift(item);
  return mockDelay(item);
}

export async function updateWorkDeviceApi(id: string, data: Partial<CreateWorkDeviceInput>) {
  const index = devices.findIndex((item) => item.id === id);
  if (devices[index]) {
    devices[index] = {
      ...devices[index],
      ...data,
      updatedAt: new Date().toISOString(),
    } as WorkDevice;
  }
  return mockDelay(devices[index]);
}

export async function deleteWorkDeviceApi(id: string) {
  const index = devices.findIndex((item) => item.id === id);
  if (index >= 0) devices.splice(index, 1);
  return mockDelay(true);
}
