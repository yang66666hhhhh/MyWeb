<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import type { GanttBarObject } from '@infectoone/vue-ganttastic';
import { GGanttChart, GGanttRow } from '@infectoone/vue-ganttastic';
import { Page } from '@vben/common-ui';
import {
  Button,
  Card,
  DatePicker,
  Empty,
  Form,
  Modal,
  Radio,
  Select,
  Space,
  message,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import type { TaskItem } from '#/api/growth/task';
import { taskApi } from '#/api/growth/task';
import { projectApi } from '#/api/work/project';

type ZoomLevel = 'day' | 'month' | 'week';

type GanttBar = GanttBarObject;

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

const statusConfig: Record<number, { bgColor: string; color: string; label: string }> = {
  0: { bgColor: '#d9d9d9', color: 'default', label: '待处理' },
  1: { bgColor: '#1890ff', color: 'processing', label: '进行中' },
  2: { bgColor: '#52c41a', color: 'success', label: '已完成' },
  3: { bgColor: '#ff4d4f', color: 'error', label: '已取消' },
};

const priorityConfig: Record<number, { color: string; label: string }> = {
  1: { color: 'green', label: '低' },
  2: { color: 'orange', label: '中' },
  3: { color: 'red', label: '高' },
  4: { color: 'magenta', label: '紧急' },
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

const precisionMap: Record<ZoomLevel, 'date' | 'day' | 'hour' | 'month' | 'week'> = {
  day: 'hour',
  month: 'day',
  week: 'date',
};

const chartRange = computed(() => {
  const current = currentDate.value;
  let start: dayjs.Dayjs;
  let end: dayjs.Dayjs;

  switch (zoomLevel.value) {
    case 'day': {
      start = current.subtract(2, 'day').startOf('day');
      end = current.add(5, 'day').endOf('day');
      break;
    }
    case 'month': {
      start = current.startOf('month').subtract(1, 'month');
      end = current.startOf('month').add(4, 'month').endOf('month');
      break;
    }
    case 'week': {
      start = current.startOf('week').subtract(1, 'week');
      end = current.startOf('week').add(5, 'week').endOf('week');
      break;
    }
  }

  return {
    start: start.format('YYYY-MM-DD HH:mm'),
    end: end.format('YYYY-MM-DD HH:mm'),
  };
});

const ganttRows = computed(() => {
  const barStart = 'startDate';
  const barEnd = 'endDate';

  const grouped = new Map<string, { bars: GanttBar[]; label: string }>();

  for (const task of tasks.value) {
    if (!task.startTime && !task.planDate) continue;

    const taskStart = task.startTime
      ? dayjs(task.startTime)
      : dayjs(task.planDate).startOf('day');
    const taskEnd = task.endTime
      ? dayjs(task.endTime)
      : taskStart.add(Math.max(task.estimatedHours ? Math.ceil(task.estimatedHours / 8) : 1, 1), 'day');

    const status = typeof task.status === 'number' ? task.status : 0;
    const config = statusConfig[status] ?? statusConfig[0]!;

    const bar: GanttBar = {
      [barStart]: taskStart.format('YYYY-MM-DD HH:mm'),
      [barEnd]: taskEnd.format('YYYY-MM-DD HH:mm'),
      ganttBarConfig: {
        id: task.id,
        immobile: status === 2 || status === 3,
        label: task.title,
        hasHandles: true,
        style: {
          background: config.bgColor,
          borderRadius: '4px',
          color: '#fff',
        },
      },
    };

    const groupKey = task.projectName || '未分组';
    if (!grouped.has(groupKey)) {
      grouped.set(groupKey, { bars: [], label: groupKey });
    }
    grouped.get(groupKey)!.bars.push(bar);
  }

  return [...grouped.values()];
});

const barStart = 'startDate';
const barEnd = 'endDate';

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
  } catch (error: unknown) {
    message.error((error instanceof Error ? error.message : null) || '加载任务失败');
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
      currentDate.value = currentDate.value.subtract(3, 'day');
      break;
    }
    case 'month': {
      currentDate.value = currentDate.value.subtract(1, 'month');
      break;
    }
    case 'week': {
      currentDate.value = currentDate.value.subtract(1, 'week');
      break;
    }
  }
}

function navigateNext() {
  switch (zoomLevel.value) {
    case 'day': {
      currentDate.value = currentDate.value.add(3, 'day');
      break;
    }
    case 'month': {
      currentDate.value = currentDate.value.add(1, 'month');
      break;
    }
    case 'week': {
      currentDate.value = currentDate.value.add(1, 'week');
      break;
    }
  }
}

function handleDateChange(date: unknown) {
  if (date && dayjs.isDayjs(date)) {
    currentDate.value = date;
  }
}

function handleClickBar(event: { bar: GanttBar; e: MouseEvent }) {
  const task = tasks.value.find((t) => t.id === event.bar.ganttBarConfig.id);
  if (task) {
    Modal.info({
      title: task.title,
      content: `
        项目: ${task.projectName || '-'}
        状态: ${statusConfig[task.status]?.label || '-'}
        优先级: ${priorityConfig[task.priority]?.label || '-'}
        工时: ${task.estimatedHours || '-'}h
        描述: ${task.description || '-'}
      `,
      width: 400,
    });
  }
}

function handleDblClickBar(event: { bar: GanttBar; e: MouseEvent }) {
  const task = tasks.value.find((t) => t.id === event.bar.ganttBarConfig.id);
  if (task) {
    message.info(`编辑任务: ${task.title}`);
  }
}

async function handleDragEndBar(event: {
  bar: GanttBar;
  e: MouseEvent;
  movedBars?: Map<GanttBar, { oldEnd: string; oldStart: string }>;
}) {
  const bar = event.bar;
  const taskId = bar.ganttBarConfig.id;

  try {
    await taskApi.update(taskId, {
      startTime: bar[barStart],
      endTime: bar[barEnd],
    });
    message.success('任务时间已更新');
  } catch {
    message.error('更新失败');
    fetchData();
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
          <GGanttChart
            :chart-start="chartRange.start"
            :chart-end="chartRange.end"
            :precision="precisionMap[zoomLevel]"
            :bar-start="barStart"
            :bar-end="barEnd"
            :row-height="40"
            grid
            push-on-overlap
            color-scheme="creamy"
            width="100%"
            label-column-title="项目分组"
            label-column-width="160px"
            @click-bar="handleClickBar"
            @dblclick-bar="handleDblClickBar"
            @dragend-bar="handleDragEndBar"
          >
            <GGanttRow
              v-for="row in ganttRows"
              :key="row.label"
              :label="row.label"
              :bars="row.bars"
            />
          </GGanttChart>
        </div>

        <div class="mt-4 flex gap-4">
          <div v-for="(config, status) in statusConfig" :key="status" class="flex items-center gap-1">
            <div
              class="h-3 w-3 rounded"
              :style="{ backgroundColor: config.bgColor }"
            ></div>
            <span class="text-xs text-gray-500">{{ config.label }}</span>
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
  min-height: 400px;
}

:deep(.g-gantt-chart) {
  font-family: inherit;
}

:deep(.g-gantt-row-label) {
  font-size: 13px;
  padding: 0 12px;
}

:deep(.g-gantt-bar) {
  cursor: pointer;
  transition: opacity 0.2s;
}

:deep(.g-gantt-bar:hover) {
  opacity: 0.85;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
}

:deep(.g-gantt-bar-label) {
  font-size: 12px;
  font-weight: 500;
}
</style>
