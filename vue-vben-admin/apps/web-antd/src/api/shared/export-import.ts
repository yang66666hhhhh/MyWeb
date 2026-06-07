import { requestClient } from '#/api/request';

export type ExportFormat = 'excel' | 'csv' | 'json';

export interface ExportQuery {
  format?: ExportFormat;
  startDate?: string;
  endDate?: string;
}

export interface ImportError {
  rowNumber: number;
  field: string;
  message: string;
}

export interface ImportPreview {
  totalRows: number;
  validRows: number;
  errorRows: number;
  previewRows: Record<string, any>[];
  errors: ImportError[];
}

export interface ImportResult {
  totalRows: number;
  successRows: number;
  failedRows: number;
  skippedRows: number;
  errors: ImportError[];
}

export const exportApi = {
  exportTasks: (params?: ExportQuery) =>
    requestClient.get('/export/tasks', { params, responseType: 'blob' }),

  exportWorkLogs: (params?: ExportQuery) =>
    requestClient.get('/export/worklogs', { params, responseType: 'blob' }),

  exportHabits: (params?: ExportQuery) =>
    requestClient.get('/export/habits', { params, responseType: 'blob' }),

  exportIncome: (params?: ExportQuery) =>
    requestClient.get('/export/income', { params, responseType: 'blob' }),

  exportExpense: (params?: ExportQuery) =>
    requestClient.get('/export/expense', { params, responseType: 'blob' }),
};

export const importApi = {
  previewTasks: (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return requestClient.post<ImportPreview>('/import/tasks/preview', formData);
  },

  importTasks: (rows: Record<string, any>[]) =>
    requestClient.post<ImportResult>('/import/tasks/execute', rows),

  previewWorkLogs: (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return requestClient.post<ImportPreview>('/import/worklogs/preview', formData);
  },

  importWorkLogs: (rows: Record<string, any>[]) =>
    requestClient.post<ImportResult>('/import/worklogs/execute', rows),

  previewHabits: (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return requestClient.post<ImportPreview>('/import/habits/preview', formData);
  },

  importHabits: (rows: Record<string, any>[]) =>
    requestClient.post<ImportResult>('/import/habits/execute', rows),

  previewIncome: (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return requestClient.post<ImportPreview>('/import/income/preview', formData);
  },

  importIncome: (rows: Record<string, any>[]) =>
    requestClient.post<ImportResult>('/import/income/execute', rows),

  previewExpense: (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return requestClient.post<ImportPreview>('/import/expense/preview', formData);
  },

  importExpense: (rows: Record<string, any>[]) =>
    requestClient.post<ImportResult>('/import/expense/execute', rows),
};
