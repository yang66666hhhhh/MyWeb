import type { BaseEntity, PageQuery, PageResult } from '../types';

export interface WorkImportBatch extends BaseEntity {
  fileName: string;
  fileSize?: number;
  importType?: number;
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
}

export interface WorkImportBatchQuery extends PageQuery {
  keyword?: string;
  status?: number;
  startDate?: string;
  endDate?: string;
}

export interface WorkImportRow extends BaseEntity {
  batchId: string;
  rowNumber: number;
  rawDate?: string;
  rawWeekDay?: string;
  rawProject?: string;
  rawDevice?: string;
  rawTaskType?: string;
  rawContent?: string;
  rawHours?: string;
  rawRemark?: string;
  parsedDate?: string;
  parsedHours?: number;
  validationStatus: number;
  errorMessage?: string;
  importedWorkLogId?: string;
}

export interface WorkImportRowQuery extends PageQuery {
  batchId: string;
  validationStatus?: number;
}

export interface WorkImportPreviewItem {
  rowNumber: number;
  workDate: string;
  projectName: string;
  deviceNames: string;
  taskTypeNames: string;
  originalContent: string;
  totalHours?: number;
  remark?: string;
  validationStatus: number;
  errorMessage?: string;
  duplicateStatus?: number;
}

export interface WorkImportPreviewResult {
  items: WorkImportPreviewItem[];
  totalRows: number;
  validRows: number;
  warningRows: number;
  errorRows: number;
  duplicateRows: number;
}

export interface WorkImportConfirmInput {
  batchId: string;
  importStrategy: number;
}

export interface WorkImportConfirmResult {
  successRows: number;
  failedRows: number;
  skippedRows: number;
  duplicateRows: number;
}

export interface ExcelTemplateDownloadInput {
  fileName?: string;
}
