import type { PageQuery, PageResult } from '#/types/api';

import { requestClient } from '#/api/request';

// OKR types
export interface Okr {
  id: string;
  userId?: string;
  title: string;
  description?: string;
  objective: string;
  keyResults: string;
  year: number;
  quarter: number;
  status: number;
  progress: number;
  tags?: string;
  createdAt: string;
}

export interface CreateOkrInput {
  title: string;
  description?: string;
  objective: string;
  keyResults: string;
  year: number;
  quarter: number;
  tags?: string;
}

export interface UpdateOkrInput {
  title?: string;
  description?: string;
  objective?: string;
  keyResults?: string;
  status?: number;
  progress?: number;
  tags?: string;
}

// Risk types
export interface RiskItem {
  id: string;
  userId?: string;
  title: string;
  description?: string;
  category: string;
  impact: number;
  probability: number;
  status: number;
  mitigationPlan?: string;
  identifiedDate?: string;
  resolvedDate?: string;
  tags?: string;
  createdAt: string;
}

export interface CreateRiskItemInput {
  title: string;
  description?: string;
  category: string;
  impact?: number;
  probability?: number;
  mitigationPlan?: string;
  identifiedDate?: string;
  tags?: string;
}

export interface UpdateRiskItemInput {
  title?: string;
  description?: string;
  category?: string;
  impact?: number;
  probability?: number;
  status?: number;
  mitigationPlan?: string;
  resolvedDate?: string;
  tags?: string;
}

// File types
export interface WorkFile {
  id: string;
  userId?: string;
  fileName: string;
  fileUrl: string;
  fileType: string;
  fileSize: number;
  category?: string;
  description?: string;
  tags?: string;
  createdAt: string;
}

export interface CreateWorkFileInput {
  fileName: string;
  fileUrl: string;
  fileType: string;
  fileSize: number;
  category?: string;
  description?: string;
  tags?: string;
}

export interface UpdateWorkFileInput {
  fileName?: string;
  fileUrl?: string;
  fileType?: string;
  fileSize?: number;
  category?: string;
  description?: string;
  tags?: string;
}

// Query types
export interface WorkExtendedQuery extends PageQuery {
  category?: string;
  status?: number;
  keyword?: string;
  year?: number;
  quarter?: number;
}

// API methods
export const workExtendedApi = {
  // OKR
  getOkrs: (params: WorkExtendedQuery) =>
    requestClient.get<PageResult<Okr>>('/work/okr', { params }),
  createOkr: (data: CreateOkrInput) =>
    requestClient.post<Okr>('/work/okr', data),
  updateOkr: (id: string, data: UpdateOkrInput) =>
    requestClient.put<Okr>(`/work/okr/${id}`, data),
  deleteOkr: (id: string) =>
    requestClient.delete(`/work/okr/${id}`),

  // Risks
  getRisks: (params: WorkExtendedQuery) =>
    requestClient.get<PageResult<RiskItem>>('/work/risks', { params }),
  createRisk: (data: CreateRiskItemInput) =>
    requestClient.post<RiskItem>('/work/risks', data),
  updateRisk: (id: string, data: UpdateRiskItemInput) =>
    requestClient.put<RiskItem>(`/work/risks/${id}`, data),
  deleteRisk: (id: string) =>
    requestClient.delete(`/work/risks/${id}`),

  // Files
  getFiles: (params: WorkExtendedQuery) =>
    requestClient.get<PageResult<WorkFile>>('/work/files', { params }),
  createFile: (data: CreateWorkFileInput) =>
    requestClient.post<WorkFile>('/work/files', data),
  updateFile: (id: string, data: UpdateWorkFileInput) =>
    requestClient.put<WorkFile>(`/work/files/${id}`, data),
  deleteFile: (id: string) =>
    requestClient.delete(`/work/files/${id}`),
};

// Export individual API methods
export const getOkrsApi = workExtendedApi.getOkrs;
export const createOkrApi = workExtendedApi.createOkr;
export const updateOkrApi = workExtendedApi.updateOkr;
export const deleteOkrApi = workExtendedApi.deleteOkr;

export const getRisksApi = workExtendedApi.getRisks;
export const createRiskApi = workExtendedApi.createRisk;
export const updateRiskApi = workExtendedApi.updateRisk;
export const deleteRiskApi = workExtendedApi.deleteRisk;

export const getFilesApi = workExtendedApi.getFiles;
export const createFileApi = workExtendedApi.createFile;
export const updateFileApi = workExtendedApi.updateFile;
export const deleteFileApi = workExtendedApi.deleteFile;
