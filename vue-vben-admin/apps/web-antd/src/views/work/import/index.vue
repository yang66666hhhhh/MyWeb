<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Alert,
  Button,
  Card,
  Col,
  Descriptions,
  Progress,
  Result,
  Row,
  Space,
  Statistic,
  Steps,
  Table,
  Tag,
  Upload,
  message,
} from 'ant-design-vue';
import { DownloadOutlined, UploadOutlined } from '@ant-design/icons-vue';
import type { UploadFile } from 'ant-design-vue';

import type { WorkImportBatch, WorkImportPreviewItem } from '#/api/growth/work';
import {
  confirmWorkImportApi,
  getWorkImportBatchPageApi,
  getWorkImportTemplateUrl,
  previewWorkImportApi,
} from '#/api/growth/work';
import { usePagedQuery } from '#/composables/usePagedQuery';
import {
  WorkImportStatus,
  WorkImportStatusColor,
  WorkImportStatusLabel,
  WorkImportStrategy,
  WorkImportStrategyLabel,
  WorkImportValidationStatus,
  WorkImportValidationStatusColor,
  WorkImportValidationStatusLabel,
} from '#/enums/workEnum';

import ImportPreviewTable from './ImportPreviewTable.vue';
import ImportResult from './ImportResult.vue';

const currentStep = ref(0);
const previewData = ref<WorkImportPreviewItem[]>([]);
const previewLoading = ref(false);
const confirmLoading = ref(false);
const importResult = ref<{
  success: boolean;
  successRows: number;
  failedRows: number;
  skippedRows: number;
  duplicateRows: number;
} | null>(null);

const fileList = ref<UploadFile[]>([]);
const selectedBatch = ref<WorkImportBatch | null>(null);

const { changePage, items, load, loading, query, resetQuery, search, total } = usePagedQuery<
  WorkImportBatch,
  { page: number; pageSize: number; status?: number }
>({
  defaultQuery: { page: 1, pageSize: 5 },
  fetcher: getWorkImportBatchPageApi,
});

const previewStats = computed(() => {
  const valid = previewData.value.filter((i) => i.validationStatus === WorkImportValidationStatus.Valid).length;
  const warning = previewData.value.filter((i) => i.validationStatus === WorkImportValidationStatus.Warning).length;
  const error = previewData.value.filter((i) => i.validationStatus === WorkImportValidationStatus.Error).length;
  const duplicate = previewData.value.filter((i) => i.duplicateStatus === 1).length;
  return { valid, warning, error, duplicate, total: previewData.value.length };
});

function handleTemplateDownload() {
  const url = getWorkImportTemplateUrl();
  const link = document.createElement('a');
  link.href = url;
  link.download = '工作记录导入模板.xlsx';
  link.click();
}

async function handleFileChange(info: { file: UploadFile }) {
  const file = info.file;
  if (!file.originFileObj) return;

  previewLoading.value = true;
  try {
    const result = await previewWorkImportApi(file.originFileObj as File);
    previewData.value = result.Items;
    currentStep.value = 2;
    message.success('文件解析成功，请预览数据');
  } catch {
    message.error('文件解析失败');
  } finally {
    previewLoading.value = false;
  }
}

async function handleConfirm() {
  confirmLoading.value = true;
  try {
    const result = await confirmWorkImportApi({
      batchId: 'temp-batch',
      importStrategy: WorkImportStrategy.SkipDuplicate,
    });
    importResult.value = {
      success: true,
      successRows: result.successRows,
      failedRows: result.failedRows,
      skippedRows: result.skippedRows,
      duplicateRows: result.duplicateRows,
    };
    currentStep.value = 3;
    message.success('导入完成');
  } catch {
    message.error('导入失败');
  } finally {
    confirmLoading.value = false;
  }
}

function handleBackToUpload() {
  currentStep.value = 0;
  previewData.value = [];
  fileList.value = [];
  importResult.value = null;
}

function handleReset() {
  currentStep.value = 0;
  previewData.value = [];
  fileList.value = [];
  importResult.value = null;
  void load();
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="导入Excel工作记录数据，支持预览校验和批次管理" title="工作导入">
    <div class="space-y-4">
      <Card>
        <Steps :current="currentStep" size="small">
          <Steps.Step title="下载模板" description="获取Excel导入模板" />
          <Steps.Step title="上传文件" description="上传工作记录Excel" />
          <Steps.Step title="预览校验" description="检查数据并确认" />
          <Steps.Step title="导入结果" description="查看导入结果" />
        </Steps>
      </Card>

      <Card v-if="currentStep === 0">
        <div class="flex flex-col items-center justify-center py-8">
          <Alert class="w-full max-w-xl mb-6" message="导入说明" show-icon type="info">
            <template #description>
              <ul class="list-disc pl-4 space-y-1 text-sm">
                <li>请先下载导入模板，按照模板格式填写工作记录</li>
                <li>日期格式支持 yyyy/MM/dd 或 yyyy-MM-dd</li>
                <li>项目名称不存在时系统会自动创建</li>
                <li>设备/任务类型支持多个，用逗号分隔</li>
                <li>工作内容为空且备注为"缺失数据"时，将导入为缺失状态</li>
              </ul>
            </template>
          </Alert>
          <Button type="primary" size="large" @click="handleTemplateDownload">
            <DownloadOutlined />
            下载导入模板
          </Button>
          <div class="mt-4">
            <Button @click="currentStep = 1">已有数据文件，继续</Button>
          </div>
        </div>
      </Card>

      <Card v-if="currentStep === 1">
        <div class="flex flex-col items-center justify-center py-8">
          <Upload
            v-model:file-list="fileList"
            :before-upload="() => false"
            :max-count="1"
            accept=".xlsx,.xls"
            @change="handleFileChange"
          >
            <Button type="primary" size="large" :loading="previewLoading">
              <UploadOutlined />
              选择Excel文件
            </Button>
          </Upload>
          <div class="mt-4 text-text-secondary text-sm">
            支持 .xlsx 和 .xls 格式
          </div>
          <div class="mt-4">
            <Button @click="currentStep = 0">返回上一步</Button>
          </div>
        </div>
      </Card>

      <Card v-if="currentStep === 2 && previewData.length > 0">
        <div class="mb-4">
          <Alert type="info" show-icon>
            <template #message>数据预览</template>
            <template #description>
              <Row :gutter="16" class="mt-2">
                <Col :span="6">
                  <Statistic title="总行数" :value="previewStats.total" />
                </Col>
                <Col :span="6">
                  <Statistic title="有效" :value="previewStats.valid" :value-style="{ color: '#52c41a' }" />
                </Col>
                <Col :span="6">
                  <Statistic title="警告" :value="previewStats.warning" :value-style="{ color: '#faad14' }" />
                </Col>
                <Col :span="6">
                  <Statistic title="错误" :value="previewStats.error" :value-style="{ color: '#ff4d4f' }" />
                </Col>
              </Row>
            </template>
          </Alert>
        </div>

        <ImportPreviewTable :data="previewData" />

        <div class="mt-4 flex justify-end gap-3">
          <Button @click="handleBackToUpload">重新上传</Button>
          <Button type="primary" :loading="confirmLoading" @click="handleConfirm">
            确认导入
          </Button>
        </div>
      </Card>

      <Card v-if="currentStep === 3 && importResult">
        <ImportResult :result="importResult" @reset="handleReset" />
      </Card>

      <Card title="导入历史">
        <Table
          :columns="[
            { dataIndex: 'fileName', key: 'fileName', title: '文件名' },
            { dataIndex: 'createdAt', key: 'createdAt', title: '导入时间', width: 170 },
            { dataIndex: 'status', key: 'status', title: '状态', width: 100 },
            { dataIndex: 'totalRows', key: 'totalRows', title: '总行数', width: 80 },
            { dataIndex: 'successRows', key: 'successRows', title: '成功', width: 80 },
            { dataIndex: 'failedRows', key: 'failedRows', title: '失败', width: 80 },
            { dataIndex: 'duplicateRows', key: 'duplicateRows', title: '跳过', width: 80 },
          ]"
          :data-source="items"
          :loading="loading"
          :pagination="{
            current: query.page,
            pageSize: query.pageSize,
            showSizeChanger: true,
            showTotal: (value: number) => `共 ${value} 条`,
            total,
          }"
          row-key="id"
          @change="changePage($event.current ?? 1, $event.pageSize ?? 10)"
        >
          <template #bodyCell="{ column, record, text }">
            <template v-if="column.key === 'fileName'">
              <div class="font-medium">{{ record.fileName }}</div>
              <div class="text-xs text-text-secondary">
                {{ (record.fileSize ?? 0 / 1024).toFixed(1) }} KB
              </div>
            </template>
            <template v-else-if="column.key === 'createdAt'">
              {{ text?.replace('T', ' ').slice(0, 16) }}
            </template>
            <template v-else-if="column.key === 'status'">
              <Tag :color="WorkImportStatusColor[text as WorkImportStatus]">
                {{ WorkImportStatusLabel[text as WorkImportStatus] }}
              </Tag>
            </template>
          </template>
        </Table>
      </Card>
    </div>
  </Page>
</template>
