import type { PageQuery, PageResult } from '#/types/api';
import { requestClient } from '#/api/request';

export interface SoftwareAsset {
  id: string;
  userId?: string;
  name: string;
  version?: string;
  type: number;
  licenseType: number;
  status: number;
  vendor?: string;
  purchaseDate?: string;
  expireDate?: string;
  cost?: number;
  description?: string;
  assignedTo?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface SoftwareAssetQuery extends PageQuery {
  keyword?: string;
  type?: number;
  licenseType?: number;
  status?: number;
}

export interface CreateSoftwareAssetInput {
  name: string;
  version?: string;
  type: number;
  licenseType: number;
  status: number;
  vendor?: string;
  purchaseDate?: string;
  expireDate?: string;
  cost?: number;
  description?: string;
  assignedTo?: string;
}

export interface UpdateSoftwareAssetInput {
  name?: string;
  version?: string;
  type?: number;
  licenseType?: number;
  status?: number;
  vendor?: string;
  purchaseDate?: string;
  expireDate?: string;
  cost?: number;
  description?: string;
  assignedTo?: string;
}

export const softwareAssetApi = {
  getPage: (params: SoftwareAssetQuery) =>
    requestClient.get<PageResult<SoftwareAsset>>('/work/software-assets', { params }),

  getById: (id: string) =>
    requestClient.get<SoftwareAsset>(`/work/software-assets/${id}`),

  create: (data: CreateSoftwareAssetInput) =>
    requestClient.post<SoftwareAsset>('/work/software-assets', data),

  update: (id: string, data: UpdateSoftwareAssetInput) =>
    requestClient.put<SoftwareAsset>(`/work/software-assets/${id}`, data),

  delete: (id: string) =>
    requestClient.delete(`/work/software-assets/${id}`),
};

export const getSoftwareAssetPageApi = softwareAssetApi.getPage;
export const getSoftwareAssetApi = softwareAssetApi.getById;
export const createSoftwareAssetApi = softwareAssetApi.create;
export const updateSoftwareAssetApi = softwareAssetApi.update;
export const deleteSoftwareAssetApi = softwareAssetApi.delete;
