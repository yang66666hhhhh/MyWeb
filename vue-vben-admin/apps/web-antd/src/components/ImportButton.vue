<script setup lang="ts">
import { ref, computed } from 'vue';
import { UploadOutlined, CheckCircleOutlined } from '@ant-design/icons-vue';
import {
  Button,
  Modal,
  Upload,
  Table,
  Alert,
  Statistic,
  Row,
  Col,
  message,
  Space,
  Typography,
} from 'ant-design-vue';
import type { ImportPreview, ImportResult } from '#/api/shared/export-import';
import { importApi } from '#/api/shared/export-import';

interface Props {
  module: 'tasks' | 'worklogs' | 'habits' | 'income' | 'expense';
  disabled?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  disabled: false,
});

const emit = defineEmits<{
  imported: [result: ImportResult];
}>();

const loading = ref(false);
const modalVisible = ref(false);
const step = ref<'upload' | 'preview' | 'result'>('upload');
const selectedFile = ref<File | null>(null);
const previewData = ref<ImportPreview | null>(null);
const importResult = ref<ImportResult | null>(null);

const previewColumns = computed(() => {
  if (!previewData.value?.previewRows?.length) return [];
  const firstRow = previewData.value.previewRows[0];
  if (!firstRow || typeof firstRow !== 'object') return [];
  return Object.keys(firstRow).map((key) => ({
    title: key,
    dataIndex: key,
    key,
    ellipsis: true,
    width: 120,
  }));
});

const apiMap = {
  tasks: { preview: importApi.previewTasks, execute: importApi.importTasks },
  worklogs: { preview: importApi.previewWorkLogs, execute: importApi.importWorkLogs },
  habits: { preview: importApi.previewHabits, execute: importApi.importHabits },
  income: { preview: importApi.previewIncome, execute: importApi.importIncome },
  expense: { preview: importApi.previewExpense, execute: importApi.importExpense },
};

function openModal() {
  modalVisible.value = true;
  step.value = 'upload';
  selectedFile.value = null;
  previewData.value = null;
  importResult.value = null;
}

function handleCancel() {
  modalVisible.value = false;
}

async function handleBeforeUpload(file: File) {
  selectedFile.value = file;
  await handlePreview();
  return false;
}

async function handlePreview() {
  if (!selectedFile.value) return;

  loading.value = true;
  try {
    const apiFn = apiMap[props.module].preview;
    previewData.value = await apiFn(selectedFile.value);
    step.value = 'preview';
  } catch (error: any) {
    message.error(error?.message || '预览失败');
  } finally {
    loading.value = false;
  }
}

async function handleImport() {
  if (!previewData.value?.previewRows?.length) return;

  loading.value = true;
  try {
    const apiFn = apiMap[props.module].execute;
    importResult.value = await apiFn(previewData.value.previewRows);
    step.value = 'result';
    emit('imported', importResult.value);
  } catch (error: any) {
    message.error(error?.message || '导入失败');
  } finally {
    loading.value = false;
  }
}

function handleClose() {
  modalVisible.value = false;
  if (importResult.value) {
    message.success(`导入完成: 成功 ${importResult.value.successRows} 条`);
  }
}
</script>

<template>
  <div>
    <Button :disabled="disabled" @click="openModal">
      <template #icon>
        <UploadOutlined />
      </template>
      导入
    </Button>

    <Modal
      v-model:open="modalVisible"
      title="数据导入"
      width="800px"
      :footer="null"
      @cancel="handleCancel"
    >
      <template v-if="step === 'upload'">
        <div style="text-align: center; padding: 40px 0;">
          <Upload
            :before-upload="handleBeforeUpload"
            :show-upload-list="false"
            accept=".xlsx,.xls,.csv,.json"
          >
            <div style="border: 2px dashed #d9d9d9; border-radius: 8px; padding: 40px; cursor: pointer; background: #fafafa;">
              <p style="font-size: 16px; margin-bottom: 8px;">
                点击或拖拽文件到此区域上传
              </p>
              <p style="color: #999; font-size: 14px;">
                支持 .xlsx、.xls、.csv、.json 格式
              </p>
            </div>
          </Upload>
        </div>
      </template>

      <template v-if="step === 'preview' && previewData">
        <Alert
          v-if="previewData.errorRows > 0"
          type="warning"
          :message="`发现 ${previewData.errorRows} 行数据存在错误`"
          show-icon
          style="margin-bottom: 16px;"
        />

        <Row :gutter="16" style="margin-bottom: 16px;">
          <Col :span="8">
            <Statistic title="总行数" :value="previewData.totalRows" />
          </Col>
          <Col :span="8">
            <Statistic title="有效行数" :value="previewData.validRows" :value-style="{ color: '#3f8600' }" />
          </Col>
          <Col :span="8">
            <Statistic title="错误行数" :value="previewData.errorRows" :value-style="{ color: '#cf1322' }" />
          </Col>
        </Row>

        <div v-if="previewData.errors.length > 0" style="margin-bottom: 16px;">
          <Typography.Title :level="5">错误详情</Typography.Title>
          <div style="max-height: 150px; overflow-y: auto;">
            <div v-for="(error, index) in previewData.errors" :key="index" style="color: #cf1322; font-size: 13px;">
              第 {{ error.rowNumber }} 行 - {{ error.field }}: {{ error.message }}
            </div>
          </div>
        </div>

        <Typography.Title :level="5">数据预览 (前10行)</Typography.Title>
        <Table
          :columns="previewColumns"
          :data-source="previewData.previewRows"
          :pagination="false"
          size="small"
          :scroll="{ x: 600 }"
          row-key="(_, index) => index"
        />

        <div style="text-align: right; margin-top: 16px;">
          <Space>
            <Button @click="step = 'upload'">重新选择</Button>
            <Button type="primary" :loading="loading" :disabled="previewData.validRows === 0" @click="handleImport">
              确认导入
            </Button>
          </Space>
        </div>
      </template>

      <template v-if="step === 'result' && importResult">
        <div style="text-align: center; padding: 20px 0;">
          <CheckCircleOutlined style="font-size: 48px; color: #52c41a; margin-bottom: 16px;" />
          <Typography.Title :level="4">导入完成</Typography.Title>
        </div>

        <Row :gutter="16" style="margin-bottom: 24px;">
          <Col :span="6">
            <Statistic title="总行数" :value="importResult.totalRows" />
          </Col>
          <Col :span="6">
            <Statistic title="成功" :value="importResult.successRows" :value-style="{ color: '#3f8600' }" />
          </Col>
          <Col :span="6">
            <Statistic title="失败" :value="importResult.failedRows" :value-style="{ color: '#cf1322' }" />
          </Col>
          <Col :span="6">
            <Statistic title="跳过" :value="importResult.skippedRows" />
          </Col>
        </Row>

        <div v-if="importResult.errors.length > 0" style="margin-bottom: 16px;">
          <Typography.Title :level="5">错误详情</Typography.Title>
          <div style="max-height: 200px; overflow-y: auto;">
            <div v-for="(error, index) in importResult.errors" :key="index" style="color: #cf1322; font-size: 13px;">
              第 {{ error.rowNumber }} 行 - {{ error.field }}: {{ error.message }}
            </div>
          </div>
        </div>

        <div style="text-align: right;">
          <Button type="primary" @click="handleClose">完成</Button>
        </div>
      </template>
    </Modal>
  </div>
</template>
