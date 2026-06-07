<script setup lang="ts">
import { ref } from 'vue';
import { DownloadOutlined } from '@ant-design/icons-vue';
import { Button, Dropdown, Menu, MenuItem, message } from 'ant-design-vue';
import type { ExportFormat } from '#/api/shared/export-import';
import { exportApi } from '#/api/shared/export-import';

interface Props {
  module: 'tasks' | 'worklogs' | 'habits' | 'income' | 'expense';
  filename?: string;
  startDate?: string;
  endDate?: string;
  disabled?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  filename: 'export',
  disabled: false,
});

const loading = ref(false);

const formatLabels: Record<ExportFormat, string> = {
  excel: '导出 Excel',
  csv: '导出 CSV',
  json: '导出 JSON',
};

const apiMap = {
  tasks: exportApi.exportTasks,
  worklogs: exportApi.exportWorkLogs,
  habits: exportApi.exportHabits,
  income: exportApi.exportIncome,
  expense: exportApi.exportExpense,
};

async function handleExport(format: ExportFormat) {
  loading.value = true;
  try {
    const apiFn = apiMap[props.module];
    const response = await apiFn({
      format,
      startDate: props.startDate,
      endDate: props.endDate,
    });

    const blob = new Blob([response as any]);
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;

    const extensions: Record<ExportFormat, string> = {
      excel: '.xlsx',
      csv: '.csv',
      json: '.json',
    };
    link.download = `${props.filename}_${new Date().toISOString().slice(0, 10)}${extensions[format]}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);

    message.success('导出成功');
  } catch (error: any) {
    message.error(error?.message || '导出失败');
  } finally {
    loading.value = false;
  }
}
</script>

<template>
  <Dropdown :disabled="disabled || loading">
    <Button :loading="loading" :disabled="disabled">
      <template #icon>
        <DownloadOutlined />
      </template>
      导出
    </Button>
    <template #overlay>
      <Menu @click="({ key }: any) => handleExport(key as ExportFormat)">
        <MenuItem key="excel">
          {{ formatLabels.excel }}
        </MenuItem>
        <MenuItem key="csv">
          {{ formatLabels.csv }}
        </MenuItem>
        <MenuItem key="json">
          {{ formatLabels.json }}
        </MenuItem>
      </Menu>
    </template>
  </Dropdown>
</template>
