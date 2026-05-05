import type { PageQuery, PageResult } from '#/types/api';
import { requestClient } from '#/api/request';

export interface WorkImportBatch {
  id: string;
  fileName: string;
  fileSize?: number;
  totalRows: number;
  successRows: number;
  failedRows: number;
  skippedRows: number;
  duplicateRows: number;
  status: number;
  importStrategy: number;
  startedAt?: string;
  finishedAt?: string;
  errorMessage?: string;
  createdAt: string;
}

export interface WorkImportBatchQuery extends PageQuery {
  status?: number;
  startDate?: string;
  endDate?: string;
}

export interface WorkImportPreviewItem {
  rowNumber: number;
  workDate?: string;
  projectName?: string;
  deviceNames?: string;
  taskTypeNames?: string;
  originalContent?: string;
  totalHours?: number;
  remark?: string;
  validationStatus: number;
  duplicateStatus?: number;
  errorMessage?: string;
}

export interface WorkImportPreviewResult {
  items: WorkImportPreviewItem[];
  totalRows: number;
  validRows: number;
  warningRows: number;
  errorRows: number;
  duplicateRows: number;
}

export interface WorkImportConfirmItem {
  rowNumber: number;
  projectName?: string;
  workDate?: string;
  originalContent?: string;
  totalHours?: number;
  validationStatus: number;
  duplicateStatus?: number;
}

export interface WorkImportConfirmDto {
  items: WorkImportConfirmItem[];
  importStrategy: number;
}

export interface WorkImportConfirmResult {
  successRows: number;
  failedRows: number;
  skippedRows: number;
}

export const workImportApi = {
  getPage: (params?: WorkImportBatchQuery) =>
    requestClient.get<PageResult<WorkImportBatch>>('/work/imports', { params }),

  previewWorkLog: (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return requestClient.post<WorkImportPreviewResult>('/work/imports/worklog/preview', formData);
  },

  executeWorkLog: (data: WorkImportConfirmDto) =>
    requestClient.post<WorkImportConfirmResult>('/work/imports/worklog/execute', data),

  getWorkLogTemplate: () =>
    requestClient.get('/work/imports/worklog/template', { responseType: 'blob' }),

  previewProject: (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return requestClient.post<WorkImportPreviewResult>('/work/imports/project/preview', formData);
  },

  executeProject: (data: WorkImportConfirmDto) =>
    requestClient.post<WorkImportConfirmResult>('/work/imports/project/execute', data),

  getProjectTemplate: () =>
    requestClient.get('/work/imports/project/template', { responseType: 'blob' }),

  previewDevice: (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return requestClient.post<WorkImportPreviewResult>('/work/imports/device/preview', formData);
  },

  executeDevice: (data: WorkImportConfirmDto) =>
    requestClient.post<WorkImportConfirmResult>('/work/imports/device/execute', data),

  getDeviceTemplate: () =>
    requestClient.get('/work/imports/device/template', { responseType: 'blob' }),

  previewTaskType: (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return requestClient.post<WorkImportPreviewResult>('/work/imports/tasktype/preview', formData);
  },

  executeTaskType: (data: WorkImportConfirmDto) =>
    requestClient.post<WorkImportConfirmResult>('/work/imports/tasktype/execute', data),

  getTaskTypeTemplate: () =>
    requestClient.get('/work/imports/tasktype/template', { responseType: 'blob' }),
};

export const getWorkImportBatchPageApi = workImportApi.getPage;
export const previewWorkImportApi = workImportApi.previewWorkLog;
export const confirmWorkImportApi = workImportApi.executeWorkLog;
export const getWorkImportTemplateUrl = workImportApi.getWorkLogTemplate;
