<script lang="ts" setup>
import { computed, onMounted, reactive, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  DatePicker,
  Form,
  message,
  Popconfirm,
  Select,
  Space,
  Table,
  Tag,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import { taskApi } from '#/api/growth';
import type { TaskItem, TaskItemQuery } from '#/api/growth';

import DailyPlanForm from './components/DailyPlanForm.vue';

const loading = ref(false);
const formOpen = ref(false);
const editingId = ref<null | string>(null);
const items = ref<any[]>([]);
const total = ref(0);

const query = reactive({
  page: 1,
  pageSize: 10,
  date: undefined as string | undefined,
  status: undefined as number | undefined,
  priority: undefined as number | undefined,
  keyword: '',
});

const statusMap: Record<number, { color: string; label: string }> = {
  0: { color: 'default', label: '未开始' },
  1: { color: 'processing', label: '进行中' },
  2: { color: 'success', label: '已完成' },
  3: { color: 'error', label: '已取消' },
};

const priorityMap: Record<number, { color: string; label: string }> = {
  1: { color: 'default', label: '低' },
  2: { color: 'blue', label: '中' },
  3: { color: 'orange', label: '高' },
  4: { color: 'red', label: '紧急' },
  5: { color: 'magenta', label: '紧急+' },
};

const columns: any[] = [
  { dataIndex: 'planDate', key: 'planDate', title: '日期', width: 120 },
  { dataIndex: 'title', key: 'title', title: '计划标题', minWidth: 220 },
  { key: 'timeRange', title: '时段', width: 150 },
  { dataIndex: 'priority', key: 'priority', title: '优先级', width: 100 },
  { dataIndex: 'status', key: 'status', title: '状态', width: 110 },
  { dataIndex: 'remark', key: 'remark', title: '备注', minWidth: 180 },
  { key: 'action', title: '操作', width: 200, fixed: 'right' },
];

const statusOptions = [
  { label: '全部状态', value: undefined },
  { label: '未开始', value: 0 },
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

const summaryText = computed(() => {
  const completedCount = items.value.filter((item) => item.status === 2).length;
  return `共 ${items.value.length} 条，已完成 ${completedCount} 条`;
});

async function fetchPage() {
  loading.value = true;
  try {
    const params: TaskItemQuery = {
      ...query,
      taskType: 'Personal',
      source: 'Growth',
    };
    const result = await taskApi.getPage(params);
    items.value = result.items;
    total.value = result.total;
  } catch {
    message.error('加载失败');
  } finally {
    loading.value = false;
  }
}

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  query.page = pagination.current ?? 1;
  query.pageSize = pagination.pageSize ?? 10;
  fetchPage();
}

function formatTimeRange(record: any) {
  if (!record.startTime && !record.endTime) return '未设置';
  return `${record.startTime || '--:--'} - ${record.endTime || '--:--'}`;
}

function onDateChange(val: null | string | dayjs.Dayjs) {
  const nextDate = val && typeof val !== 'string' ? val.format('YYYY-MM-DD') : undefined;
  query.date = nextDate;
  fetchPage();
}

function search() {
  query.page = 1;
  fetchPage();
}

function resetFilters() {
  query.date = undefined;
  query.status = undefined;
  query.priority = undefined;
  query.keyword = '';
  query.page = 1;
  fetchPage();
}

function openCreate() {
  editingId.value = null;
  formOpen.value = true;
}

function openEdit(id: string) {
  editingId.value = id;
  formOpen.value = true;
}

async function handleRemove(id: string) {
  try {
    await taskApi.delete(id);
    message.success('已删除');
    fetchPage();
  } catch {
    message.error('删除失败');
  }
}

async function handleChangeStatus(record: Record<string, any>, status: number) {
  const task = record as TaskItem;
  try {
    await taskApi.update(task.id, { status } as any);
    message.success('状态已更新');
    fetchPage();
  } catch {
    message.error('更新失败');
  }
}

onMounted(() => fetchPage());
</script>

<template>
  <Page description="管理每日计划和任务" title="每日计划">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <Form layout="inline">
            <Form.Item label="日期">
              <DatePicker
                style="width: 160px"
                :value="query.date ? dayjs(query.date) : undefined"
                format="YYYY-MM-DD"
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
            <Form.Item>
              <Space>
                <Button type="primary" @click="search">查询</Button>
                <Button @click="resetFilters">重置</Button>
              </Space>
            </Form.Item>
          </Form>
          <div class="flex items-center gap-3">
            <span class="text-gray-500 text-sm">{{ summaryText }}</span>
            <Button type="primary" @click="openCreate">新增计划</Button>
          </div>
        </div>
      </Card>

      <Card>
        <Table
          :columns="columns"
          :data-source="items"
          :loading="loading"
          :pagination="{ current: query.page, pageSize: query.pageSize, showSizeChanger: true, showTotal: (v: number) => `共 ${v} 条`, total }"
          :scroll="{ x: 1100 }"
          row-key="id"
          @change="handleTableChange"
        >
          <template #bodyCell="{ column, record, text }">
            <template v-if="column.key === 'title'">
              <div>
                <div class="font-medium">{{ record.title }}</div>
                <div class="text-gray-400 text-xs">{{ record.description || '暂无内容' }}</div>
              </div>
            </template>
            <template v-else-if="column.key === 'timeRange'">
              {{ formatTimeRange(record) }}
            </template>
            <template v-else-if="column.key === 'priority'">
              <Tag :color="priorityMap[text]?.color">{{ priorityMap[text]?.label }}</Tag>
            </template>
            <template v-else-if="column.key === 'status'">
              <Tag :color="statusMap[text]?.color">{{ statusMap[text]?.label }}</Tag>
            </template>
            <template v-else-if="column.key === 'action'">
              <Space>
                <Button v-if="record.status === 0" size="small" type="link" @click="handleChangeStatus(record, 1)">开始</Button>
                <Button size="small" type="link" @click="openEdit(record.id)">编辑</Button>
                <Button :disabled="record.status === 2" size="small" type="link" @click="handleChangeStatus(record, 2)">完成</Button>
                <Button :disabled="record.status === 2 || record.status === 3" size="small" type="link" @click="handleChangeStatus(record, 3)">取消</Button>
                <Popconfirm title="确认删除？" @confirm="handleRemove(record.id)">
                  <Button danger size="small" type="link">删除</Button>
                </Popconfirm>
              </Space>
            </template>
          </template>
        </Table>
      </Card>
    </div>

    <DailyPlanForm
      v-model:open="formOpen"
      :id="editingId"
      @success="fetchPage"
      @update:open="(v: boolean) => { if (!v) { editingId = null; formOpen = false; } }"
    />
  </Page>
</template>
