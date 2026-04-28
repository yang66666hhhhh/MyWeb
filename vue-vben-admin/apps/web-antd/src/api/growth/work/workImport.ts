import type { BaseEntity, PageQuery, PageResult } from '../../types';

import { createId, mockDelay } from '../mock-utils';

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

const importBatches: WorkImportBatch[] = [
  {
    createdAt: `${today()}T10:00:00`,
    duplicateRows: 2,
    errorMessage: undefined,
    failedRows: 1,
    fileName: '工作记录_2026_04.xlsx',
    fileSize: 24576,
    id: 'batch-1',
    importStrategy: 0,
    importType: 0,
    status: 2,
    successRows: 15,
    totalRows: 18,
    skippedRows: 0,
    startedAt: `${today()}T10:00:00`,
    finishedAt: `${today()}T10:00:05`,
  },
];

const importRows: WorkImportRow[] = [
  {
    batchId: 'batch-1',
    createdAt: `${today()}T10:00:00`,
    errorMessage: undefined,
    id: 'row-1',
    importedWorkLogId: 'worklog-imported-1',
    parsedDate: '2026-04-28',
    parsedHours: 4,
    rawContent: '完成设备调试',
    rawDate: '2026/04/28',
    rawDevice: 'A线体',
    rawHours: '4',
    rawProject: '生产线升级项目',
    rawRemark: '',
    rawTaskType: '调试',
    rawWeekDay: '周一',
    rowNumber: 1,
    validationStatus: 0,
  },
  {
    batchId: 'batch-1',
    createdAt: `${today()}T10:00:00`,
    errorMessage: undefined,
    id: 'row-2',
    importedWorkLogId: 'worklog-imported-2',
    parsedDate: '2026-04-28',
    parsedHours: 2,
    rawContent: '问题处理',
    rawDate: '2026/04/28',
    rawDevice: 'B线体',
    rawHours: '2',
    rawProject: '生产线升级项目',
    rawRemark: '',
    rawTaskType: '问题处理',
    rawWeekDay: '周一',
    rowNumber: 2,
    validationStatus: 0,
  },
  {
    batchId: 'batch-1',
    createdAt: `${today()}T10:00:00`,
    errorMessage: '日期格式错误',
    id: 'row-3',
    importedWorkLogId: undefined,
    parsedDate: undefined,
    parsedHours: undefined,
    rawContent: '',
    rawDate: '2026/04/32',
    rawDevice: 'C线体',
    rawHours: '',
    rawProject: '生产线升级项目',
    rawRemark: '缺失数据',
    rawTaskType: '',
    rawWeekDay: '周一',
    rowNumber: 3,
    validationStatus: 2,
  },
];

export async function getWorkImportBatchPageApi(params: WorkImportBatchQuery) {
  return mockDelay<PageResult<WorkImportBatch>>(
    createPageResult(importBatches.filter((item) => {
      const inDate =
        (!params.startDate || item.createdAt >= params.startDate) &&
        (!params.endDate || item.createdAt <= params.endDate);
      const inStatus = params.status === undefined || item.status === params.status;
      return inDate && inStatus;
    }), params),
  );
}

export async function getWorkImportBatchApi(id: string) {
  return mockDelay(importBatches.find((item) => item.id === id));
}

export async function getWorkImportRowPageApi(params: WorkImportRowQuery) {
  const filtered = importRows.filter((item) => {
    const inBatch = item.batchId === params.batchId;
    const inStatus = params.validationStatus === undefined || item.validationStatus === params.validationStatus;
    return inBatch && inStatus;
  });
  return mockDelay<PageResult<WorkImportRow>>(createPageResult(filtered, params));
}

export async function previewWorkImportApi(file: File): Promise<WorkImportPreviewResult> {
  await mockDelay(500);
  return {
    duplicateRows: 1,
    errorRows: 1,
    items: [
      {
        duplicateStatus: 0,
        errorMessage: undefined,
        deviceNames: 'A线体,B线体',
        originalContent: '完成设备调试和参数优化',
        projectName: '生产线升级项目',
        remark: '',
        rowNumber: 1,
        taskTypeNames: '调试,维护',
        totalHours: 4,
        validationStatus: 0,
        workDate: '2026-04-28',
      },
      {
        duplicateStatus: 1,
        errorMessage: undefined,
        deviceNames: 'C线体',
        originalContent: '日常巡检',
        projectName: '生产线升级项目',
        remark: '',
        rowNumber: 2,
        taskTypeNames: '维护',
        totalHours: 1,
        validationStatus: 0,
        workDate: '2026-04-28',
      },
      {
        duplicateStatus: 0,
        errorMessage: '日期格式错误：2026/04/32 不是有效日期',
        deviceNames: 'D线体',
        originalContent: '',
        projectName: '生产线升级项目',
        remark: '缺失数据',
        rowNumber: 3,
        taskTypeNames: '',
        totalHours: undefined,
        validationStatus: 2,
        workDate: '2026-04-32',
      },
    ],
    totalRows: 3,
    validRows: 2,
    warningRows: 0,
  };
}

export async function confirmWorkImportApi(input: WorkImportConfirmInput): Promise<WorkImportConfirmResult> {
  await mockDelay(1000);
  return {
    duplicateRows: 1,
    failedRows: 0,
    skippedRows: 1,
    successRows: 1,
  };
}

export function getWorkImportTemplateUrl(): string {
  return '/api/work/import/template';
}
