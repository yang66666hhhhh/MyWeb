<script lang="ts" setup>
import { Table, Tag, Tooltip } from 'ant-design-vue';

import {
  WorkImportValidationStatus,
  WorkImportValidationStatusColor,
  WorkImportValidationStatusLabel,
} from '#/enums/workEnum';

defineProps<{
  data: Array<{
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
  }>;
}>();

const columns = [
  { dataIndex: 'rowNumber', key: 'rowNumber', title: '行号', width: 60 },
  { dataIndex: 'workDate', key: 'workDate', title: '日期', width: 110 },
  { dataIndex: 'projectName', key: 'projectName', title: '项目', width: 140 },
  { dataIndex: 'deviceNames', key: 'deviceNames', title: '设备', width: 140 },
  { dataIndex: 'taskTypeNames', key: 'taskTypeNames', title: '任务类型', width: 120 },
  { dataIndex: 'originalContent', key: 'originalContent', title: '工作内容', minWidth: 180 },
  { dataIndex: 'totalHours', key: 'totalHours', title: '耗时', width: 80 },
  { dataIndex: 'remark', key: 'remark', title: '备注', width: 120 },
  { dataIndex: 'validationStatus', key: 'validationStatus', title: '状态', width: 80 },
  { dataIndex: 'errorMessage', key: 'errorMessage', title: '错误信息', width: 200 },
];
</script>

<template>
  <Table
    :columns="columns"
    :data-source="data"
    :pagination="false"
    :scroll="{ x: 1300 }"
    row-key="rowNumber"
    size="small"
  >
    <template #bodyCell="{ column, record }">
      <template v-if="column.key === 'validationStatus'">
        <Tag :color="WorkImportValidationStatusColor[record.validationStatus as WorkImportValidationStatus]">
          {{ WorkImportValidationStatusLabel[record.validationStatus as WorkImportValidationStatus] }}
        </Tag>
      </template>
      <template v-else-if="column.key === 'errorMessage'">
        <Tooltip v-if="record.errorMessage" :title="record.errorMessage">
          <span class="text-red-500 cursor-help">{{ record.errorMessage?.slice(0, 30) }}{{ record.errorMessage?.length > 30 ? '...' : '' }}</span>
        </Tooltip>
        <span v-else>-</span>
      </template>
      <template v-else-if="column.key === 'originalContent'">
        <span class="line-clamp-2">{{ record.originalContent || '-' }}</span>
      </template>
      <template v-else-if="column.key === 'duplicateStatus'">
        <Tag v-if="record.duplicateStatus === 1" color="orange">重复</Tag>
        <span v-else>-</span>
      </template>
    </template>
  </Table>
</template>
