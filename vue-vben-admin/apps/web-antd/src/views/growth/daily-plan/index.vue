<script lang="ts" setup>
import { computed, onMounted } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Alert,
  Button,
  Card,
  DatePicker,
  Form,
  Input,
  message,
  Popconfirm,
  Select,
  Space,
  Table,
  Tag,
} from 'ant-design-vue';
import dayjs from 'dayjs';
import { storeToRefs } from 'pinia';

import type { DailyPlan, DailyPlanPriority, DailyPlanStatus } from '#/api/growth';

import { useDailyPlanStore } from '#/store/modules/daily-plan';

import DailyPlanForm from './components/DailyPlanForm.vue';

const dailyPlanStore = useDailyPlanStore();
const { editingId, formOpen, items, loading, query, total } =
  storeToRefs(dailyPlanStore);

const statusMap: Record<DailyPlanStatus, { color: string; label: string }> = {
  0: { color: 'default', label: '未开始' },
  1: { color: 'processing', label: '进行中' },
  2: { color: 'success', label: '已完成' },
  3: { color: 'error', label: '已取消' },
};

const priorityMap: Record<DailyPlanPriority, { color: string; label: string }> = {
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
  { dataIndex: 'completedAt', key: 'completedAt', title: '完成时间', width: 170 },
  { dataIndex: 'createdAt', key: 'createdAt', title: '创建时间', width: 170 },
  { key: 'action', title: '操作', width: 250, fixed: 'right' },
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
  { label: '紧急+', value: 5 },
];

const summaryText = computed(() => {
  const completedCount = items.value.filter((item) => item.status === 2).length;
  return `当前筛选结果 ${items.value.length} 条，已完成 ${completedCount} 条`;
});

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  query.value.page = pagination.current ?? 1;
  query.value.pageSize = pagination.pageSize ?? 10;
  void dailyPlanStore.fetchPage();
}

function formatDateTime(value?: null | string) {
  return value ? value.replace('T', ' ').slice(0, 16) : '-';
}

function formatTimeRange(record: DailyPlan) {
  if (!record.startTime && !record.endTime) {
    return '未设置';
  }
  return `${record.startTime || '--:--'} - ${record.endTime || '--:--'}`;
}

function onDateChange(val: null | string | dayjs.Dayjs) {
  const nextDate =
    val && typeof val !== 'string' ? val.format('YYYY-MM-DD') : undefined;
  query.value.date = nextDate;
  query.value.startDate = nextDate;
  query.value.endDate = nextDate;
}

async function handleRemove(id: string) {
  await dailyPlanStore.remove(id);
  message.success('每日计划已删除');
}

async function handleChangeStatus(record: Record<string, any>, status: DailyPlanStatus) {
  const plan = record as DailyPlan;
  await dailyPlanStore.changeStatus(plan, status);
  message.success(`"${plan.title}" 状态已更新`);
}

function resetFilters() {
  dailyPlanStore.$reset();
  void dailyPlanStore.fetchPage();
}

onMounted(() => {
  void dailyPlanStore.fetchPage();
});
</script>

<template>
  <Page
    description="优先对接现有 DailyPlan 后端接口，保留开始时间和结束时间的前端预留能力。"
    title="每日计划"
  >
    <div class="space-y-4">
      <Alert
        message="当前真实后端已对接列表、新增、编辑、删除、完成和状态更新；开始时间、结束时间与优先级筛选先由前端兼容处理。"
        type="info"
        show-icon
      />

      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
          <Form :model="query" layout="inline">
            <Form.Item label="计划日期">
              <DatePicker
                style="width: 180px"
                :value="query.date ? dayjs(query.date) : undefined"
                format="YYYY-MM-DD"
                allow-clear
                @change="onDateChange"
              />
            </Form.Item>
            <Form.Item label="状态">
              <Select
                v-model:value="query.status"
                :options="statusOptions"
                allow-clear
                class="w-36"
              />
            </Form.Item>
            <Form.Item label="优先级">
              <Select
                v-model:value="query.priority"
                :options="priorityOptions"
                allow-clear
                class="w-36"
              />
            </Form.Item>
            <Form.Item label="关键词">
              <Input
                v-model:value="query.keyword"
                allow-clear
                placeholder="标题、内容、备注"
                style="width: 220px"
                @press-enter="dailyPlanStore.search"
              />
            </Form.Item>
            <Form.Item>
              <Space>
                <Button type="primary" @click="dailyPlanStore.search">查询</Button>
                <Button @click="resetFilters">重置</Button>
              </Space>
            </Form.Item>
          </Form>

          <div class="flex items-center justify-between gap-3">
            <span class="text-text-secondary text-sm">{{ summaryText }}</span>
            <Button type="primary" @click="dailyPlanStore.openCreate">新增计划</Button>
          </div>
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
          :scroll="{ x: 1180 }"
          row-key="id"
          @change="handleTableChange"
        >
          <template #bodyCell="{ column, record, text }">
            <template v-if="column.key === 'title'">
              <div class="space-y-1">
                <div class="font-medium">{{ record.title }}</div>
                <div class="text-text-secondary line-clamp-2 text-xs">
                  {{ record.description || '暂无计划内容' }}
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'timeRange'">
              {{ formatTimeRange(record as DailyPlan) }}
            </template>
            <template v-else-if="column.key === 'priority'">
              <Tag :color="priorityMap[text as DailyPlanPriority].color">
                {{ priorityMap[text as DailyPlanPriority].label }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'status'">
              <Tag :color="statusMap[text as DailyPlanStatus].color">
                {{ statusMap[text as DailyPlanStatus].label }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'remark'">
              <span class="text-text-secondary">{{ text || '-' }}</span>
            </template>
            <template v-else-if="column.key === 'completedAt'">
              <span class="text-text-secondary">{{ formatDateTime(text) }}</span>
            </template>
            <template v-else-if="column.key === 'createdAt'">
              {{ formatDateTime(text) }}
            </template>
            <template v-else-if="column.key === 'action'">
              <Space>
                <Button
                  v-if="record.status === 0"
                  size="small"
                  type="link"
                  @click="handleChangeStatus(record, 1)"
                >
                  开始
                </Button>
                <Button size="small" type="link" @click="dailyPlanStore.openEdit(record.id)">
                  编辑
                </Button>
                <Button
                  :disabled="record.status === 2"
                  size="small"
                  type="link"
                  @click="handleChangeStatus(record, 2)"
                >
                  完成
                </Button>
                <Button
                  :disabled="record.status === 2 || record.status === 3"
                  size="small"
                  type="link"
                  @click="handleChangeStatus(record, 3)"
                >
                  取消
                </Button>
                <Popconfirm title="确认删除这条每日计划？" @confirm="handleRemove(record.id)">
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
      @success="dailyPlanStore.fetchPage"
      @update:open="
        (value) => {
          if (!value) dailyPlanStore.closeForm();
        }
      "
    />
  </Page>
</template>
