<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  DatePicker,
  Descriptions,
  Drawer,
  Form,
  Input,
  Modal,
  Popconfirm,
  Select,
  Space,
  Table,
  Tag,
  message,
} from 'ant-design-vue';
import dayjs from 'dayjs';
import { useRouter } from 'vue-router';

import type { WorkDailyPlan } from '#/api/growth/work';
import {
  completeWorkDailyPlanApi,
  convertToWorkLogApi,
  createWorkDailyPlanApi,
  deleteWorkDailyPlanApi,
  getWorkDailyPlanApi,
  getWorkDailyPlanPageApi,
  updateWorkDailyPlanApi,
} from '#/api/growth/work';
import { usePagedQuery } from '#/composables/usePagedQuery';
import {
  WorkDailyPlanPriority,
  WorkDailyPlanPriorityColor,
  WorkDailyPlanPriorityLabel,
  WorkDailyPlanStatus,
  WorkDailyPlanStatusColor,
  WorkDailyPlanStatusLabel,
} from '#/enums/workEnum';

const router = useRouter();

const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(null);
const selectedItem = ref<WorkDailyPlan | null>(null);

const projectOptions = [
  { label: '生产线升级项目', value: 'project-1' },
  { label: '质量改进项目', value: 'project-2' },
];

const statusOptions = [
  { label: '全部状态', value: undefined },
  { label: '待执行', value: 0 },
  { label: '进行中', value: 1 },
  { label: '已完成', value: 2 },
  { label: '已取消', value: 3 },
];

const priorityOptions = [
  { label: '全部优先级', value: undefined },
  { label: '低', value: 1 },
  { label: '中', value: 2 },
  { label: '高', value: 3 },
  { label: '紧急', value: 4 },
];

const formState = ref({
  content: '',
  endTime: '',
  estimatedHours: 0,
  planDate: dayjs().format('YYYY-MM-DD'),
  priority: 2,
  projectId: '',
  remark: '',
  startTime: '',
  status: 0,
  title: '',
});

const columns = [
  { dataIndex: 'planDate', key: 'planDate', title: '计划日期', width: 120 },
  { dataIndex: 'title', key: 'title', title: '计划标题', minWidth: 200 },
  { dataIndex: 'projectName', key: 'projectName', title: '项目', width: 140 },
  { dataIndex: 'priority', key: 'priority', title: '优先级', width: 90 },
  { dataIndex: 'status', key: 'status', title: '状态', width: 100 },
  { dataIndex: 'estimatedHours', key: 'estimatedHours', title: '预计时长', width: 100 },
  { dataIndex: 'startTime', key: 'startTime', title: '开始时间', width: 90 },
  { dataIndex: 'endTime', key: 'endTime', title: '结束时间', width: 90 },
  { key: 'action', title: '操作', width: 220, fixed: 'right' },
];

const { changePage, items, load, loading, query, resetQuery, search, total } = usePagedQuery<
  WorkDailyPlan,
  {
    keyword?: string;
    planDate?: string;
    startDate?: string;
    endDate?: string;
    projectId?: string;
    status?: number;
    priority?: number;
    page: number;
    pageSize: number;
  }
>({
  defaultQuery: { page: 1, pageSize: 10 },
  fetcher: getWorkDailyPlanPageApi,
});

function onDateChange(val: string) {
  query.planDate = val || undefined;
}

function openCreate() {
  editingId.value = null;
  formState.value = {
    content: '',
    endTime: '',
    estimatedHours: 0,
    planDate: dayjs().format('YYYY-MM-DD'),
    priority: 2,
    projectId: '',
    remark: '',
    startTime: '',
    status: 0,
    title: '',
  };
  formOpen.value = true;
}

async function openEdit(record: WorkDailyPlan) {
  editingId.value = record.id;
  const detail = await getWorkDailyPlanApi(record.id);
  if (detail) {
    formState.value = {
      content: detail.content || '',
      endTime: detail.endTime || '',
      estimatedHours: detail.estimatedHours || 0,
      planDate: typeof detail.planDate === 'string' ? detail.planDate : detail.planDate?.toString() || '',
      priority: detail.priority,
      projectId: detail.projectId || '',
      remark: detail.remark || '',
      startTime: detail.startTime || '',
      status: detail.status,
      title: detail.title,
    };
  }
  formOpen.value = true;
}

function showDetail(record: WorkDailyPlan) {
  selectedItem.value = record;
  detailOpen.value = true;
}

async function handleComplete(id: string) {
  await completeWorkDailyPlanApi(id);
  message.success('计划已完成');
  await load();
}

async function handleConvertToLog(record: WorkDailyPlan) {
  await convertToWorkLogApi({
    originalContent: record.content,
    planId: record.id,
    workDate: dayjs().format('YYYY-MM-DD'),
  });
  message.success('已转换为工作日志');
  await load();
}

async function handleRemove(id: string) {
  await deleteWorkDailyPlanApi(id);
  message.success('计划已删除');
  await load();
}

async function handleSubmit() {
  if (!formState.value.title.trim()) {
    message.warning('请填写计划标题');
    return;
  }
  if (editingId.value) {
    await updateWorkDailyPlanApi(editingId.value, formState.value);
    message.success('计划已更新');
  } else {
    await createWorkDailyPlanApi(formState.value);
    message.success('计划已创建');
  }
  formOpen.value = false;
  await load();
}

function resetFilters() {
  resetQuery();
  void load();
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="管理工作计划，支持从计划转换为工作日志" title="每日计划">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
          <Form :model="query" layout="inline">
            <Form.Item label="计划日期">
              <DatePicker
                style="width: 160px"
                :value="query.planDate"
                format="YYYY-MM-DD"
                value-format="YYYY-MM-DD"
                allow-clear
                @change="onDateChange"
              />
            </Form.Item>
            <Form.Item label="状态">
              <Select v-model:value="query.status" :options="statusOptions" allow-clear class="w-32" />
            </Form.Item>
            <Form.Item label="优先级">
              <Select v-model:value="query.priority" :options="priorityOptions" allow-clear class="w-32" />
            </Form.Item>
            <Form.Item label="关键词">
              <Input v-model:value="query.keyword" allow-clear placeholder="标题/内容" style="width: 180px" @press-enter="search" />
            </Form.Item>
            <Form.Item>
              <Space>
                <Button type="primary" @click="search">查询</Button>
                <Button @click="resetFilters">重置</Button>
              </Space>
            </Form.Item>
          </Form>
          <Button type="primary" @click="openCreate">新增计划</Button>
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
          :scroll="{ x: 1300 }"
          row-key="id"
          @change="changePage($event.current ?? 1, $event.pageSize ?? 10)"
        >
          <template #bodyCell="{ column, record, text }">
            <template v-if="column.key === 'title'">
              <div class="space-y-1">
                <div class="font-medium">{{ record.title }}</div>
                <div v-if="record.content" class="text-text-secondary line-clamp-1 text-xs">
                  {{ record.content }}
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'priority'">
              <Tag :color="WorkDailyPlanPriorityColor[text as WorkDailyPlanPriority]">
                {{ WorkDailyPlanPriorityLabel[text as WorkDailyPlanPriority] }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'status'">
              <Tag :color="WorkDailyPlanStatusColor[text as WorkDailyPlanStatus]">
                {{ WorkDailyPlanStatusLabel[text as WorkDailyPlanStatus] }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'estimatedHours'">
              {{ text ? `${text}h` : '-' }}
            </template>
            <template v-else-if="column.key === 'action'">
              <Space>
                <Button v-if="record.status !== 2" size="small" type="link" @click="showDetail(record)">详情</Button>
                <Button v-if="record.status !== 2" size="small" type="link" @click="openEdit(record)">编辑</Button>
                <Button v-if="record.status !== 2" size="small" type="link" @click="handleComplete(record.id)">完成</Button>
                <Button v-if="record.status === 2" size="small" type="link" @click="handleConvertToLog(record)">转日志</Button>
                <Popconfirm title="确认删除？" @confirm="handleRemove(record.id)">
                  <Button danger size="small" type="link">删除</Button>
                </Popconfirm>
              </Space>
            </template>
          </template>
        </Table>
      </Card>
    </div>

    <Modal
      :open="formOpen"
      :title="editingId ? '编辑计划' : '新增计划'"
      width="600px"
      @cancel="formOpen = false"
      @ok="handleSubmit"
    >
      <Form :model="formState" layout="vertical">
        <Form.Item label="计划标题" required>
          <Input v-model:value="formState.title" placeholder="计划标题" />
        </Form.Item>
        <div class="grid grid-cols-2 gap-4">
          <Form.Item label="计划日期">
            <DatePicker
              style="width: 100%"
              :value="formState.planDate ? dayjs(formState.planDate) : null"
              format="YYYY-MM-DD"
              value-format="YYYY-MM-DD"
              @change="(_, d) => formState.planDate = d"
            />
          </Form.Item>
          <Form.Item label="状态">
            <Select v-model:value="formState.status" :options="statusOptions.slice(1)" />
          </Form.Item>
          <Form.Item label="优先级">
            <Select v-model:value="formState.priority" :options="priorityOptions.slice(1)" />
          </Form.Item>
          <Form.Item label="预计时长">
            <Input v-model:value="formState.estimatedHours" type="number" addon-after="小时" />
          </Form.Item>
        </div>
        <Form.Item label="计划内容">
          <Input.TextArea v-model:value="formState.content" :auto-size="{ minRows: 2, maxRows: 4 }" placeholder="计划内容" />
        </Form.Item>
        <Form.Item label="备注">
          <Input.TextArea v-model:value="formState.remark" :auto-size="{ minRows: 2, maxRows: 4 }" placeholder="备注" />
        </Form.Item>
      </Form>
    </Modal>

    <Drawer v-model:open="detailOpen" title="计划详情" width="500px" :footer="null">
      <div v-if="selectedItem">
        <Descriptions bordered :column="1" size="small">
          <Descriptions.Item label="计划日期">{{ selectedItem.planDate }}</Descriptions.Item>
          <Descriptions.Item label="计划标题">{{ selectedItem.title }}</Descriptions.Item>
          <Descriptions.Item label="项目">{{ selectedItem.projectName || '-' }}</Descriptions.Item>
          <Descriptions.Item label="优先级">
            <Tag :color="WorkDailyPlanPriorityColor[selectedItem.priority as WorkDailyPlanPriority]">
              {{ WorkDailyPlanPriorityLabel[selectedItem.priority as WorkDailyPlanPriority] }}
            </Tag>
          </Descriptions.Item>
          <Descriptions.Item label="状态">
            <Tag :color="WorkDailyPlanStatusColor[selectedItem.status as WorkDailyPlanStatus]">
              {{ WorkDailyPlanStatusLabel[selectedItem.status as WorkDailyPlanStatus] }}
            </Tag>
          </Descriptions.Item>
          <Descriptions.Item label="预计时长">{{ selectedItem.estimatedHours ? `${selectedItem.estimatedHours}h` : '-' }}</Descriptions.Item>
          <Descriptions.Item label="计划内容">{{ selectedItem.content || '-' }}</Descriptions.Item>
          <Descriptions.Item label="备注">{{ selectedItem.remark || '-' }}</Descriptions.Item>
        </Descriptions>
      </div>
    </Drawer>
  </Page>
</template>
