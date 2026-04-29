<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  DatePicker,
  Descriptions,
  Drawer,
  Form,
  Input,
  Modal,
  Popconfirm,
  Row,
  Select,
  Space,
  Statistic,
  Table,
  Tag,
  message,
} from 'ant-design-vue';
import type { Dayjs } from 'dayjs';
import dayjs from 'dayjs';
import { storeToRefs } from 'pinia';

import type { WorkLog, WorkLogStatus } from '#/api/growth/work';
import {
  createWorkLogApi,
  deleteWorkLogApi,
  getWorkLogApi,
  getWorkLogPageApi,
  updateWorkLogApi,
} from '#/api/growth/work';
import { usePagedQuery } from '#/composables/usePagedQuery';
import {
  WorkLogSourceType,
  WorkLogSourceTypeLabel,
  WorkLogStatusColor,
  WorkLogStatusLabel,
} from '#/enums/workEnum';

import WorkLogModal from './WorkLogModal.vue';

const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(null);
const selectedItem = ref<WorkLog | null>(null);

const projectOptions = [
  { label: '生产线升级项目', value: 'project-1' },
  { label: '质量改进项目', value: 'project-2' },
  { label: '设备维护项目', value: 'project-3' },
];

const statusOptions = [
  { label: '全部状态', value: undefined },
  { label: '正常', value: 0 },
  { label: '缺失数据', value: 1 },
  { label: '待补充', value: 2 },
];

const sourceTypeOptions = [
  { label: '全部来源', value: undefined },
  { label: '手动', value: 0 },
  { label: 'Excel导入', value: 1 },
  { label: '计划转换', value: 2 },
];

const columns = [
  { dataIndex: 'workDate', key: 'workDate', title: '日期', width: 120 },
  { dataIndex: 'weekDay', key: 'weekDay', title: '星期', width: 80 },
  { dataIndex: 'title', key: 'title', title: '标题', minWidth: 180 },
  { dataIndex: 'projectName', key: 'projectName', title: '项目', width: 140 },
  { dataIndex: 'deviceNames', key: 'deviceNames', title: '设备', width: 140 },
  { dataIndex: 'taskTypeNames', key: 'taskTypeNames', title: '任务类型', width: 120 },
  { dataIndex: 'totalHours', key: 'totalHours', title: '耗时', width: 80 },
  { dataIndex: 'status', key: 'status', title: '状态', width: 100 },
  { dataIndex: 'sourceType', key: 'sourceType', title: '来源', width: 100 },
  { key: 'action', title: '操作', width: 200, fixed: 'right' },
];

const { changePage, items, load, loading, query, resetQuery, search, total } = usePagedQuery<
  WorkLog,
  {
    keyword?: string;
    workDate?: string;
    startDate?: string;
    endDate?: string;
    projectId?: string;
    sourceType?: number;
    status?: number;
    page: number;
    pageSize: number;
  }
>({
  defaultQuery: { page: 1, pageSize: 10 },
  fetcher: getWorkLogPageApi,
});

function onDateRangeChange(values: [string, string] | [Dayjs, Dayjs] | null) {
  const start = values?.[0];
  const end = values?.[1];
  query.startDate = start && typeof start !== 'string' ? start.format('YYYY-MM-DD') : undefined;
  query.endDate = end && typeof end !== 'string' ? end.format('YYYY-MM-DD') : undefined;
}

function openCreate() {
  editingId.value = null;
  formOpen.value = true;
}

function openEdit(record: WorkLog) {
  editingId.value = record.id;
  formOpen.value = true;
}

function showDetail(record: WorkLog) {
  selectedItem.value = record;
  detailOpen.value = true;
}

async function handleRemove(id: string) {
  await deleteWorkLogApi(id);
  message.success('工作日志已删除');
  await load();
}

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  void changePage(pagination.current ?? 1, pagination.pageSize ?? 10);
}

function resetFilters() {
  resetQuery();
  void load();
}

function handleFormOpenChange(value: boolean) {
  if (!value) {
    editingId.value = null;
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="记录日常工作内容，支持手动录入和Excel导入" title="工作日志">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
          <Form :model="query" layout="inline">
            <Form.Item label="日期范围">
              <DatePicker.RangePicker style="width: 240px" @change="onDateRangeChange" />
            </Form.Item>
            <Form.Item label="项目">
              <Select v-model:value="query.projectId" :options="projectOptions" allow-clear class="w-40" />
            </Form.Item>
            <Form.Item label="来源">
              <Select v-model:value="query.sourceType" :options="sourceTypeOptions" allow-clear class="w-32" />
            </Form.Item>
            <Form.Item label="状态">
              <Select v-model:value="query.status" :options="statusOptions" allow-clear class="w-32" />
            </Form.Item>
            <Form.Item label="关键词">
              <Input
                v-model:value="query.keyword"
                allow-clear
                placeholder="标题/内容"
                style="width: 180px"
                @press-enter="search"
              />
            </Form.Item>
            <Form.Item>
              <Space>
                <Button type="primary" @click="search">查询</Button>
                <Button @click="resetFilters">重置</Button>
              </Space>
            </Form.Item>
          </Form>
          <Button type="primary" @click="openCreate">新增日志</Button>
        </div>
      </Card>

      <Card>
        <Table
          :columns="columns"
          :data-source="items"
          :loading="loading"
          :pagination="{
            current: query.page,
            pageSize: query.pageSize,
            showSizeChanger: true,
            showTotal: (value: number) => `共 ${value} 条`,
            total,
          }"
          :scroll="{ x: 1400 }"
          row-key="id"
          @change="handleTableChange"
        >
          <template #bodyCell="{ column, record, text }">
            <template v-if="column.key === 'title'">
              <div class="space-y-1">
                <div class="font-medium">{{ record.title }}</div>
                <div v-if="record.originalContent" class="text-text-secondary line-clamp-1 text-xs">
                  {{ record.originalContent }}
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'deviceNames'">
              <Space v-if="text?.length" wrap>
                <Tag v-for="name in text" :key="name" color="blue">{{ name }}</Tag>
              </Space>
              <span v-else>-</span>
            </template>
            <template v-else-if="column.key === 'taskTypeNames'">
              <Space v-if="text?.length" wrap>
                <Tag v-for="name in text" :key="name" color="purple">{{ name }}</Tag>
              </Space>
              <span v-else>-</span>
            </template>
            <template v-else-if="column.key === 'totalHours'">
              {{ text !== undefined ? `${text}h` : '-' }}
            </template>
            <template v-else-if="column.key === 'status'">
              <Tag :color="WorkLogStatusColor[text as WorkLogStatus]">
                {{ WorkLogStatusLabel[text as WorkLogStatus] }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'sourceType'">
              {{ WorkLogSourceTypeLabel[text as WorkLogSourceType] }}
            </template>
            <template v-else-if="column.key === 'action'">
              <Space>
                <Button size="small" type="link" @click="showDetail(record)">详情</Button>
                <Button size="small" type="link" @click="openEdit(record)">编辑</Button>
                <Popconfirm title="确认删除？" @confirm="handleRemove(record.id)">
                  <Button danger size="small" type="link">删除</Button>
                </Popconfirm>
              </Space>
            </template>
          </template>
        </Table>
      </Card>
    </div>

    <WorkLogModal
      v-model:open="formOpen"
      :id="editingId"
      @success="load"
      @update:open="handleFormOpenChange"
    />

    <Drawer v-model:open="detailOpen" title="工作日志详情" width="640px" :footer="null">
      <div v-if="selectedItem" class="space-y-4">
        <Descriptions bordered :column="1" size="small">
          <Descriptions.Item label="日期">{{ selectedItem.workDate }} {{ selectedItem.weekDay }}</Descriptions.Item>
          <Descriptions.Item label="项目">{{ selectedItem.projectName }}</Descriptions.Item>
          <Descriptions.Item label="标题">{{ selectedItem.title }}</Descriptions.Item>
          <Descriptions.Item label="设备">
            <Space v-if="selectedItem.deviceNames?.length" wrap>
              <Tag v-for="name in selectedItem.deviceNames" :key="name">{{ name }}</Tag>
            </Space>
            <span v-else>-</span>
          </Descriptions.Item>
          <Descriptions.Item label="任务类型">
            <Space v-if="selectedItem.taskTypeNames?.length" wrap>
              <Tag v-for="name in selectedItem.taskTypeNames" :key="name">{{ name }}</Tag>
            </Space>
            <span v-else>-</span>
          </Descriptions.Item>
          <Descriptions.Item label="耗时">{{ selectedItem.totalHours }}h</Descriptions.Item>
          <Descriptions.Item label="状态">
            <Tag :color="WorkLogStatusColor[selectedItem.status as WorkLogStatus]">
              {{ WorkLogStatusLabel[selectedItem.status as WorkLogStatus] }}
            </Tag>
          </Descriptions.Item>
          <Descriptions.Item label="来源">{{ WorkLogSourceTypeLabel[selectedItem.sourceType as WorkLogSourceType] }}</Descriptions.Item>
          <Descriptions.Item label="工作总结">{{ selectedItem.summary || '-' }}</Descriptions.Item>
          <Descriptions.Item label="原始内容">{{ selectedItem.originalContent || '-' }}</Descriptions.Item>
          <Descriptions.Item label="备注">{{ selectedItem.remark || '-' }}</Descriptions.Item>
        </Descriptions>
      </div>
    </Drawer>
  </Page>
</template>
