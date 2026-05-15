<script lang="ts" setup>
import { computed, onMounted, reactive, ref } from 'vue';
import { useRouter } from 'vue-router';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Form,
  Input,
  InputNumber,
  message,
  Modal,
  Popconfirm,
  Progress,
  Row,
  Select,
  Space,
  Table,
  Tag,
} from 'ant-design-vue';
import dayjs from 'dayjs';
import weekOfYear from 'dayjs/plugin/weekOfYear';

import { weeklyPlanApi, type CreateWeeklyPlanInput, type CreateWeeklyPlanTaskInput, type WeeklyPlan, type WeeklyPlanTask } from '#/api/work/weeklyPlan';

dayjs.extend(weekOfYear);

const props = withDefaults(
  defineProps<{
    context?: 'default' | 'implementation';
  }>(),
  {
    context: 'default',
  },
);

const router = useRouter();
const loading = ref(false);
const formOpen = ref(false);
const taskFormOpen = ref(false);
const editingId = ref<null | string>(null);
const editingTaskId = ref<null | string>(null);
const currentPlanId = ref<null | string>(null);
const items = ref<WeeklyPlan[]>([]);
const total = ref(0);

const currentYear = dayjs().year();
const currentWeek = dayjs().week();

const query = reactive({
  page: 1,
  pageSize: 20,
  year: currentYear,
  status: undefined as number | undefined,
  keyword: '',
});

const formState = reactive({
  year: currentYear,
  weekNumber: currentWeek,
  goals: '',
  status: 0,
});

const taskFormState = reactive({
  title: '',
  description: '',
  priority: 2,
  estimatedHours: undefined as number | undefined,
});

const statusMap: Record<number, { color: string; label: string }> = {
  0: { color: 'default', label: '草稿' },
  1: { color: 'processing', label: '进行中' },
  2: { color: 'success', label: '已完成' },
  3: { color: 'error', label: '已取消' },
};

const priorityMap: Record<number, { color: string; label: string }> = {
  1: { color: 'green', label: '低' },
  2: { color: 'orange', label: '中' },
  3: { color: 'red', label: '高' },
  4: { color: 'magenta', label: '紧急' },
};

const taskStatusMap: Record<number, { color: string; label: string }> = {
  0: { color: 'default', label: '待处理' },
  1: { color: 'processing', label: '进行中' },
  2: { color: 'success', label: '已完成' },
  3: { color: 'error', label: '已取消' },
};

const taskColumns: any[] = [
  { title: '任务标题', dataIndex: 'title', key: 'title' },
  { title: '优先级', dataIndex: 'priority', key: 'priority', width: 80 },
  { title: '状态', dataIndex: 'status', key: 'status', width: 100 },
  { title: '预计工时', dataIndex: 'estimatedHours', key: 'estimatedHours', width: 100 },
  { key: 'action', title: '操作', width: 150 },
];

const weekOptions = computed(() => {
  const options = [];
  for (let i = 1; i <= 53; i++) {
    options.push({ label: `第${i}周`, value: i });
  }
  return options;
});

const yearOptions = computed(() => {
  const options = [];
  for (let i = currentYear - 2; i <= currentYear + 1; i++) {
    options.push({ label: `${i}年`, value: i });
  }
  return options;
});

const statusOptions = [
  { label: '全部状态', value: undefined },
  { label: '草稿', value: 0 },
  { label: '进行中', value: 1 },
  { label: '已完成', value: 2 },
  { label: '已取消', value: 3 },
];

const pageTitle = computed(() =>
  props.context === 'implementation' ? '实施周计划' : '周计划',
);

const pageDescription = computed(() =>
  props.context === 'implementation'
    ? '规划实施项目周目标、交付节奏和关键任务'
    : '规划和管理每周工作',
);

const createButtonText = computed(() =>
  props.context === 'implementation' ? '新建实施周计划' : '新建周计划',
);

function goToImplementationReport(plan: WeeklyPlan) {
  const query = new URLSearchParams({
    endDate: plan.endDate,
    startDate: plan.startDate,
  });
  void router.push(`/implementation/weekly-report?${query.toString()}`);
}

async function fetchPage() {
  loading.value = true;
  try {
    const result = await weeklyPlanApi.getPage(query);
    items.value = result.items;
    total.value = result.total;
  } catch {
    message.error('加载失败');
  } finally {
    loading.value = false;
  }
}

function search() {
  query.page = 1;
  fetchPage();
}

function resetFilters() {
  query.status = undefined;
  query.keyword = '';
  query.page = 1;
  fetchPage();
}

function openCreate() {
  editingId.value = null;
  Object.assign(formState, {
    year: currentYear,
    weekNumber: currentWeek,
    goals: '',
    status: 0,
  });
  formOpen.value = true;
}

function openEdit(record: WeeklyPlan) {
  editingId.value = record.id;
  Object.assign(formState, {
    year: record.year,
    weekNumber: record.weekNumber,
    goals: record.goals,
    status: record.status,
  });
  formOpen.value = true;
}

async function handleSave() {
  if (!formState.goals) {
    message.error('请填写目标');
    return;
  }

  try {
    if (editingId.value) {
      await weeklyPlanApi.update(editingId.value, { goals: formState.goals, status: formState.status });
      message.success('更新成功');
    } else {
      await weeklyPlanApi.create(formState as CreateWeeklyPlanInput);
      message.success('创建成功');
    }
    formOpen.value = false;
    await fetchPage();
  } catch (e: any) {
    message.error(e?.message || '操作失败');
  }
}

async function handleDelete(id: string) {
  try {
    await weeklyPlanApi.delete(id);
    message.success('删除成功');
    await fetchPage();
  } catch {
    message.error('删除失败');
  }
}

function openTaskForm(plan: WeeklyPlan) {
  currentPlanId.value = plan.id;
  editingTaskId.value = null;
  Object.assign(taskFormState, {
    title: '',
    description: '',
    priority: 2,
    estimatedHours: undefined,
  });
  taskFormOpen.value = true;
}

function openTaskEdit(plan: WeeklyPlan, task: Record<string, any>) {
  const weeklyTask = task as WeeklyPlanTask;
  currentPlanId.value = plan.id;
  editingTaskId.value = weeklyTask.id;
  Object.assign(taskFormState, {
    title: weeklyTask.title,
    description: weeklyTask.description || '',
    priority: weeklyTask.priority,
    estimatedHours: weeklyTask.estimatedHours,
  });
  taskFormOpen.value = true;
}

async function handleTaskSave() {
  if (!taskFormState.title) {
    message.error('请填写任务标题');
    return;
  }

  try {
    if (editingTaskId.value) {
      await weeklyPlanApi.updateTask(editingTaskId.value, taskFormState);
      message.success('更新成功');
    } else if (currentPlanId.value) {
      await weeklyPlanApi.addTask(currentPlanId.value, taskFormState as CreateWeeklyPlanTaskInput);
      message.success('添加成功');
    }
    taskFormOpen.value = false;
    await fetchPage();
  } catch {
    message.error('操作失败');
  }
}

async function handleTaskDelete(taskId: string) {
  try {
    await weeklyPlanApi.deleteTask(taskId);
    message.success('删除成功');
    await fetchPage();
  } catch {
    message.error('删除失败');
  }
}

function getTaskCompletionRate(plan: WeeklyPlan): number {
  if (!plan.tasks || plan.tasks.length === 0) return 0;
  const completed = plan.tasks.filter(t => t.status === 2).length;
  return Math.round((completed / plan.tasks.length) * 100);
}

function formatDateRange(plan: WeeklyPlan): string {
  return `${plan.startDate} ~ ${plan.endDate}`;
}

function getTaskSummary(plan: WeeklyPlan): string {
  if (!plan.tasks || plan.tasks.length === 0) return '0/0';
  const completed = plan.tasks.filter(t => t.status === 2).length;
  return `${completed}/${plan.tasks.length}`;
}

onMounted(() => {
  fetchPage();
});
</script>

<template>
  <Page :description="pageDescription" :title="pageTitle">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <Form layout="inline">
            <Form.Item label="年份">
              <Select v-model:value="query.year" :options="yearOptions" style="width: 100px" />
            </Form.Item>
            <Form.Item label="状态">
              <Select v-model:value="query.status" :options="statusOptions" allow-clear class="w-32" />
            </Form.Item>
            <Form.Item>
              <Space>
                <Button type="primary" @click="search">查询</Button>
                <Button @click="resetFilters">重置</Button>
              </Space>
            </Form.Item>
          </Form>
          <div class="flex items-center gap-3">
            <span class="text-gray-500 text-sm">共 {{ total }} 条</span>
            <Button type="primary" @click="openCreate">{{ createButtonText }}</Button>
          </div>
        </div>
      </Card>

      <Card v-for="plan in items" :key="plan.id" class="mb-4">
        <template #title>
          <div class="flex items-center justify-between">
            <Space>
              <Tag color="blue">{{ plan.weekCode }}</Tag>
              <span>{{ formatDateRange(plan) }}</span>
            </Space>
            <Tag :color="statusMap[plan.status]?.color">{{ statusMap[plan.status]?.label }}</Tag>
          </div>
        </template>
        <template #extra>
          <Space>
            <Button
              v-if="props.context === 'implementation'"
              size="small"
              type="link"
              @click="goToImplementationReport(plan)"
            >
              去生成周报
            </Button>
            <Button size="small" type="link" @click="openTaskForm(plan)">添加任务</Button>
            <Button size="small" type="link" @click="openEdit(plan)">编辑</Button>
            <Popconfirm title="确认删除？" @confirm="handleDelete(plan.id)">
              <Button danger size="small" type="link">删除</Button>
            </Popconfirm>
          </Space>
        </template>

        <Row :gutter="16" class="mb-4">
          <Col :span="16">
            <div class="font-medium text-lg mb-2">目标</div>
            <div class="text-gray-600">{{ plan.goals || '暂无目标' }}</div>
          </Col>
          <Col :span="8">
            <div class="font-medium mb-2">完成进度</div>
            <Progress :percent="getTaskCompletionRate(plan)" size="small" />
            <div class="text-gray-500 text-sm mt-1">{{ getTaskSummary(plan) }} 个任务已完成</div>
          </Col>
        </Row>

        <Table :columns="taskColumns" :data-source="plan.tasks" size="small" row-key="id">
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'priority'">
              <Tag :color="priorityMap[record.priority]?.color">{{ priorityMap[record.priority]?.label }}</Tag>
            </template>
            <template v-else-if="column.key === 'status'">
              <Tag :color="taskStatusMap[record.status]?.color">{{ taskStatusMap[record.status]?.label }}</Tag>
            </template>
            <template v-else-if="column.key === 'estimatedHours'">
              <span v-if="record.estimatedHours">{{ record.estimatedHours }}h</span>
              <span v-else class="text-gray-400">-</span>
            </template>
            <template v-else-if="column.key === 'action'">
              <Space>
                <Button size="small" type="link" @click="openTaskEdit(plan, record)">编辑</Button>
                <Popconfirm title="确认删除？" @confirm="handleTaskDelete(record.id)">
                  <Button danger size="small" type="link">删除</Button>
                </Popconfirm>
              </Space>
            </template>
          </template>
        </Table>
      </Card>
    </div>

    <Modal v-model:open="formOpen" :title="editingId ? '编辑周计划' : createButtonText" @ok="handleSave">
      <Form :model="formState" layout="vertical">
        <Row :gutter="16">
          <Col :span="12">
            <Form.Item label="年份" required>
              <InputNumber v-model:value="formState.year" :min="2000" :max="2100" style="width: 100%" />
            </Form.Item>
          </Col>
          <Col :span="12">
            <Form.Item label="周数" required>
              <Select v-model:value="formState.weekNumber" :options="weekOptions" style="width: 100%" />
            </Form.Item>
          </Col>
        </Row>
        <Form.Item label="目标">
          <Input.TextArea v-model:value="formState.goals" :rows="3" placeholder="请输入本周目标" />
        </Form.Item>
      </Form>
    </Modal>

    <Modal v-model:open="taskFormOpen" :title="editingTaskId ? '编辑任务' : '添加任务'" @ok="handleTaskSave">
      <Form :model="taskFormState" layout="vertical">
        <Form.Item label="任务标题" required>
          <Input v-model:value="taskFormState.title" placeholder="请输入任务标题" />
        </Form.Item>
        <Form.Item label="任务描述">
          <Input.TextArea v-model:value="taskFormState.description" :rows="2" placeholder="任务描述（可选）" />
        </Form.Item>
        <Row :gutter="16">
          <Col :span="12">
            <Form.Item label="优先级">
              <Select v-model:value="taskFormState.priority" :options="Object.entries(priorityMap).map(([v, m]) => ({ label: m.label, value: Number(v) }))" style="width: 100%" />
            </Form.Item>
          </Col>
          <Col :span="12">
            <Form.Item label="预计工时">
              <InputNumber v-model:value="taskFormState.estimatedHours" :min="0" :max="24" placeholder="小时" style="width: 100%" />
            </Form.Item>
          </Col>
        </Row>
      </Form>
    </Modal>
  </Page>
</template>
