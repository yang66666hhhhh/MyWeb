<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  DatePicker,
  Empty,
  Form,
  Popover,
  Radio,
  Select,
  Space,
  Tag,
  Tooltip,
  message,
} from 'ant-design-vue';

import type { TaskItem } from '#/api/growth/task';

import { taskApi } from '#/api/growth/task';
import { projectApi } from '#/api/work/project';
import dayjs from 'dayjs';

type ZoomLevel = 'day' | 'week' | 'month';

const loading = ref(false);
const tasks = ref<TaskItem[]>([]);
const projects = ref<Array<{ label: string; value: string }>>([]);
const total = ref(0);
const zoomLevel = ref<ZoomLevel>('week');
const currentDate = ref(dayjs());

const queryParams = ref({
  projectId: undefined as string | undefined,
  status: undefined as number | undefined,
});

const statusConfig: Record<string, { color: string; label: string; bgColor: string }> = {
  Pending: { color: 'default', label: '待处理', bgColor: '#d9d9d9' },
  InProgress: { color: 'processing', label: '进行中', bgColor: '#1890ff' },
  Completed: { color: 'success', label: '已完成', bgColor: '#52c41a' },
  Cancelled: { color: 'error', label: '已取消', bgColor: '#ff4d4f' },
};

const priorityConfig: Record<string, { color: string; label: string }> = {
  Low: { color: 'green', label: '低' },
  Medium: { color: 'orange', label: '中' },
  High: { color: 'red', label: '高' },
  Urgent: { color: 'magenta', label: '紧急' },
};

const statusOptions = [
  { label: '全部状态', value: undefined },
  { label: '待处理', value: 0 },
  { label: '进行中', value: 1 },
  { label: '已完成', value: 2 },
  { label: '已取消', value: 3 },
];

const zoomOptions = [
  { label: '日', value: 'day' },
  { label: '周', value: 'week' },
  { label: '月', value: 'month' },
];

const timelineRange = computed(() => {
  const current = currentDate.value;
  let start: dayjs.Dayjs;
  let end: dayjs.Dayjs;

  switch (zoomLevel.value) {
    case 'day': {
      start = current.subtract(3, 'day');
      end = current.add(10, 'day');
      break;
    }
    case 'week': {
      start = current.startOf('week').subtract(1, 'week');
      end = current.startOf('week').add(6, 'week');
      break;
    }
    case 'month': {
      start = current.startOf('month').subtract(1, 'month');
      end = current.startOf('month').add(4, 'month');
      break;
    }
  }

  return { start, end };
});

const timelineDays = computed(() => {
  const { start, end } = timelineRange.value;
  const days: dayjs.Dayjs[] = [];
  let current = start;
  while (current.isBefore(end) || current.isSame(end, 'day')) {
    days.push(current);
    current = current.add(1, 'day');
  }
  return days;
});

const totalDays = computed(() => timelineDays.value.length);

const headerGroups = computed(() => {
  const { start, end } = timelineRange.value;
  const groups: Array<{ label: string; span: number }> = [];

  switch (zoomLevel.value) {
    case 'day': {
      let current = start;
      while (current.isBefore(end)) {
        groups.push({
          label: current.format('MM/DD'),
          span: 1,
        });
        current = current.add(1, 'day');
      }
      break;
    }
    case 'week': {
      let current = start.startOf('week');
      while (current.isBefore(end)) {
        const weekEnd = current.add(6, 'day');
        groups.push({
          label: `${current.format('MM/DD')} - ${weekEnd.format('MM/DD')}`,
          span: 7,
        });
        current = current.add(7, 'day');
      }
      break;
    }
    case 'month': {
      let current = start.startOf('month');
      while (current.isBefore(end)) {
        const daysInMonth = current.daysInMonth();
        groups.push({
          label: current.format('YYYY年MM月'),
          span: daysInMonth,
        });
        current = current.add(1, 'month');
      }
      break;
    }
  }

  return groups;
});

const dayHeaders = computed(() => {
  return timelineDays.value.map((day) => ({
    label: day.format('DD'),
    isWeekend: day.day() === 0 || day.day() === 6,
    isToday: day.isSame(dayjs(), 'day'),
  }));
});

const taskBars = computed(() => {
  const { start } = timelineRange.value;
  const dayWidth = 36;

  return tasks.value
    .filter((task) => task.planDate || task.startTime)
    .map((task) => {
      const taskStart = task.startTime
        ? dayjs(task.startTime)
        : dayjs(task.planDate);
      const taskEnd = task.endTime
        ? dayjs(task.endTime)
        : taskStart.add(Math.max(task.estimatedHours ? Math.ceil(task.estimatedHours / 8) : 1, 1), 'day');

      const offsetDays = taskStart.diff(start, 'day');
      const duration = Math.max(taskEnd.diff(taskStart, 'day'), 1);

      const left = offsetDays * dayWidth;
      const width = duration * dayWidth;

      const status = task.status || 'Pending';
      const config = statusConfig[status] ?? statusConfig.Pending;

      return {
        ...task,
        left,
        width: Math.max(width, dayWidth),
        bgColor: config?.bgColor ?? '#d9d9d9',
        statusLabel: config?.label ?? status,
        startDate: taskStart.format('MM/DD'),
        endDate: taskEnd.format('MM/DD'),
        duration,
      };
    });
});

async function loadProjects() {
  try {
    const res = await projectApi.getPage({ page: 1, pageSize: 100 });
    projects.value = res.items.map((p) => ({
      label: p.projectName,
      value: p.id,
    }));
  } catch {
    // ignore
  }
}

async function fetchData() {
  loading.value = true;
  try {
    const res = await taskApi.getPage({
      page: 1,
      pageSize: 200,
      source: 'Work',
      taskType: 'Work',
      status: queryParams.value.status,
      projectId: queryParams.value.projectId,
    });
    tasks.value = res.items;
    total.value = res.total;
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载任务失败');
  } finally {
    loading.value = false;
  }
}

function handleZoomChange(level: ZoomLevel) {
  zoomLevel.value = level;
}

function goToToday() {
  currentDate.value = dayjs();
}

function navigatePrev() {
  switch (zoomLevel.value) {
    case 'day': {
      currentDate.value = currentDate.value.subtract(7, 'day');
      break;
    }
    case 'week': {
      currentDate.value = currentDate.value.subtract(2, 'week');
      break;
    }
    case 'month': {
      currentDate.value = currentDate.value.subtract(1, 'month');
      break;
    }
  }
}

function navigateNext() {
  switch (zoomLevel.value) {
    case 'day': {
      currentDate.value = currentDate.value.add(7, 'day');
      break;
    }
    case 'week': {
      currentDate.value = currentDate.value.add(2, 'week');
      break;
    }
    case 'month': {
      currentDate.value = currentDate.value.add(1, 'month');
      break;
    }
  }
}

function handleDateChange(date: unknown) {
  if (date && dayjs.isDayjs(date)) {
    currentDate.value = date;
  }
}

onMounted(() => {
  loadProjects();
  fetchData();
});
</script>

<template>
  <Page description="使用甘特图视图管理项目任务进度" title="甘特图">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <Form layout="inline">
            <Form.Item label="项目">
              <Select
                v-model:value="queryParams.projectId"
                :options="projects"
                allow-clear
                placeholder="全部项目"
                style="width: 200px"
              />
            </Form.Item>
            <Form.Item label="状态">
              <Select
                v-model:value="queryParams.status"
                :options="statusOptions"
                allow-clear
                style="width: 120px"
              />
            </Form.Item>
            <Form.Item>
              <Space>
                <Button type="primary" @click="fetchData">查询</Button>
                <Button
                  @click="
                    queryParams = { projectId: undefined, status: undefined };
                    fetchData();
                  "
                >
                  重置
                </Button>
              </Space>
            </Form.Item>
          </Form>
          <span class="text-sm text-gray-500">共 {{ total }} 个任务</span>
        </div>
      </Card>

      <Card>
        <template #title>
          <div class="flex items-center justify-between">
            <span>甘特图视图</span>
            <Space>
              <Button size="small" @click="navigatePrev">&lt;</Button>
              <DatePicker
                :value="currentDate"
                size="small"
                style="width: 130px"
                @change="handleDateChange"
              />
              <Button size="small" @click="goToToday">今天</Button>
              <Button size="small" @click="navigateNext">&gt;</Button>
              <Radio.Group
                :value="zoomLevel"
                button-style="solid"
                size="small"
                @change="(e: any) => handleZoomChange(e.target.value)"
              >
                <Radio.Button
                  v-for="opt in zoomOptions"
                  :key="opt.value"
                  :value="opt.value"
                >
                  {{ opt.label }}
                </Radio.Button>
              </Radio.Group>
            </Space>
          </div>
        </template>

        <div v-if="loading" class="flex items-center justify-center py-20">
          <div class="text-gray-400">加载中...</div>
        </div>

        <Empty v-else-if="tasks.length === 0" description="暂无任务数据" />

        <div v-else class="gantt-container">
          <div class="gantt-wrapper">
            <div class="gantt-sidebar">
              <div class="sidebar-header">
                <span>任务名称</span>
              </div>
              <div
                v-for="task in tasks"
                :key="task.id"
                class="sidebar-row"
              >
                <Tooltip :title="task.title">
                  <span class="task-name">{{ task.title }}</span>
                </Tooltip>
                <Tag
                  :color="statusConfig[task.status]?.color || 'default'"
                  size="small"
                >
                  {{ statusConfig[task.status]?.label || task.status }}
                </Tag>
              </div>
            </div>

            <div class="gantt-timeline">
              <div class="timeline-header">
                <div
                  v-for="(group, idx) in headerGroups"
                  :key="idx"
                  class="header-group"
                  :style="{ width: `${group.span * 36}px` }"
                >
                  {{ group.label }}
                </div>
              </div>
              <div class="timeline-days">
                <div
                  v-for="(day, idx) in dayHeaders"
                  :key="idx"
                  class="day-cell"
                  :class="{
                    'weekend': day.isWeekend,
                    'today': day.isToday,
                  }"
                >
                  {{ day.label }}
                </div>
              </div>
              <div class="timeline-body">
                <div
                  v-for="task in taskBars"
                  :key="task.id"
                  class="task-row"
                >
                  <div class="row-grid" :style="{ width: `${totalDays * 36}px` }">
                    <div
                      v-for="(_, idx) in timelineDays"
                      :key="idx"
                      class="grid-cell"
                      :class="{ 'weekend': dayHeaders[idx]?.isWeekend }"
                    />
                  </div>
                  <Popover trigger="hover" placement="right">
                    <template #content>
                      <div class="p-2" style="min-width: 200px">
                        <div class="font-medium">{{ task.title }}</div>
                        <div class="mt-1 text-sm text-gray-500">
                          项目: {{ task.projectName || '-' }}
                        </div>
                        <div class="text-sm text-gray-500">
                          时间: {{ task.startDate }} ~ {{ task.endDate }}
                        </div>
                        <div class="text-sm text-gray-500">
                          工时: {{ task.estimatedHours || '-' }}h
                        </div>
                        <div class="mt-1">
                          <Tag
                            :color="priorityConfig[task.priority]?.color"
                            size="small"
                          >
                            {{ priorityConfig[task.priority]?.label || task.priority }}
                          </Tag>
                          <Tag
                            :color="statusConfig[task.status]?.color"
                            size="small"
                          >
                            {{ statusConfig[task.status]?.label || task.status }}
                          </Tag>
                        </div>
                      </div>
                    </template>
                    <div
                      class="task-bar"
                      :style="{
                        left: `${task.left}px`,
                        width: `${task.width}px`,
                        backgroundColor: task.bgColor,
                      }"
                    >
                      <span class="task-bar-label">{{ task.title }}</span>
                    </div>
                  </Popover>
                </div>
              </div>
            </div>
          </div>
        </div>
      </Card>
    </div>
  </Page>
</template>

<style scoped>
.gantt-container {
  overflow-x: auto;
  border: 1px solid #f0f0f0;
  border-radius: 6px;
}

.gantt-wrapper {
  display: flex;
  min-width: fit-content;
}

.gantt-sidebar {
  flex-shrink: 0;
  width: 240px;
  border-right: 2px solid #f0f0f0;
  background: #fafafa;
}

.sidebar-header {
  display: flex;
  align-items: center;
  height: 58px;
  padding: 0 12px;
  font-weight: 600;
  border-bottom: 1px solid #f0f0f0;
  background: #fafafa;
}

.sidebar-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 40px;
  padding: 0 12px;
  border-bottom: 1px solid #f0f0f0;
}

.task-name {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  max-width: 140px;
  font-size: 13px;
}

.gantt-timeline {
  flex: 1;
  overflow-x: auto;
}

.timeline-header {
  display: flex;
  height: 28px;
  border-bottom: 1px solid #f0f0f0;
  background: #fafafa;
}

.header-group {
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  font-weight: 500;
  color: #666;
  border-right: 1px solid #f0f0f0;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.timeline-days {
  display: flex;
  height: 30px;
  border-bottom: 1px solid #f0f0f0;
}

.day-cell {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 36px;
  font-size: 11px;
  color: #666;
  border-right: 1px solid #f0f0f0;
}

.day-cell.weekend {
  background: #fff8f0;
  color: #fa8c16;
}

.day-cell.today {
  background: #e6f7ff;
  color: #1890ff;
  font-weight: 600;
}

.timeline-body {
  position: relative;
}

.task-row {
  position: relative;
  height: 40px;
  border-bottom: 1px solid #f0f0f0;
}

.row-grid {
  position: absolute;
  top: 0;
  left: 0;
  display: flex;
  height: 100%;
}

.grid-cell {
  width: 36px;
  height: 100%;
  border-right: 1px solid #f0f0f0;
}

.grid-cell.weekend {
  background: #fff8f0;
}

.task-bar {
  position: absolute;
  top: 6px;
  height: 28px;
  border-radius: 4px;
  display: flex;
  align-items: center;
  padding: 0 8px;
  cursor: pointer;
  transition: opacity 0.2s;
  z-index: 1;
  overflow: hidden;
}

.task-bar:hover {
  opacity: 0.85;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

.task-bar-label {
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
  font-size: 12px;
  color: #fff;
  font-weight: 500;
}
</style>
