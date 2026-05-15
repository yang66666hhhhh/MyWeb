<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  DatePicker,
  Form,
  Input,
  Modal,
  Popconfirm,
  Row,
  Select,
  Space,
  Statistic,
  type TableColumnsType,
  Table,
  Tag,
  message,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import {
  createPostgraduateTaskApi,
  deletePostgraduateTaskApi,
  getPostgraduateTaskPageApi,
  type PostgraduateTask,
  type SavePostgraduateTaskInput,
  updatePostgraduateTaskApi,
} from '#/api/growth/postgraduate';

import type { Dayjs } from 'dayjs';

interface TaskFormState {
  description: string;
  dueDate: string;
  priority: number;
  status: number;
  title: string;
  type: number;
}

const loading = ref(false);
const formOpen = ref(false);
const editingId = ref<null | string>(null);
const keyword = ref('');
const status = ref<number | undefined>();
const type = ref<number | undefined>();
const tasks = ref<PostgraduateTask[]>([]);

const statusOptions = [
  { label: '全部状态', value: undefined },
  { label: '待开始', value: 0 },
  { label: '进行中', value: 1 },
  { label: '已完成', value: 2 },
  { label: '已取消', value: 3 },
];

const typeOptions = [
  { label: '学习任务', value: 0 },
  { label: '复习任务', value: 1 },
  { label: '冲刺训练', value: 2 },
];

const priorityOptions = [
  { label: '低', value: 1 },
  { label: '中', value: 2 },
  { label: '高', value: 3 },
  { label: '紧急', value: 4 },
];

const statusLabels: Record<number, string> = {
  0: '待开始',
  1: '进行中',
  2: '已完成',
  3: '已取消',
};

const statusColors: Record<number, string> = {
  0: 'default',
  1: 'processing',
  2: 'success',
  3: 'error',
};

const typeLabels: Record<number, string> = {
  0: '学习任务',
  1: '复习任务',
  2: '冲刺训练',
};

const priorityLabels: Record<number, string> = {
  1: '低',
  2: '中',
  3: '高',
  4: '紧急',
};

const priorityColors: Record<number, string> = {
  1: 'green',
  2: 'blue',
  3: 'orange',
  4: 'red',
};

const columns: TableColumnsType<PostgraduateTask> = [
  { title: '任务标题', dataIndex: 'title', key: 'title', minWidth: 220 },
  { title: '类型', dataIndex: 'type', key: 'type', width: 110 },
  { title: '状态', dataIndex: 'status', key: 'status', width: 110 },
  { title: '优先级', dataIndex: 'priority', key: 'priority', width: 100 },
  { title: '截止日期', dataIndex: 'dueDate', key: 'dueDate', width: 120 },
  { key: 'action', title: '操作', width: 200, fixed: 'right' },
];

const formState = ref<TaskFormState>({
  description: '',
  dueDate: '',
  priority: 2,
  status: 0,
  title: '',
  type: 0,
});

const totalCount = computed(() => tasks.value.length);
const inProgressCount = computed(() => tasks.value.filter((item) => item.status === 1).length);
const completedCount = computed(() => tasks.value.filter((item) => item.status === 2).length);
const upcomingCount = computed(() => {
  const now = dayjs();
  return tasks.value.filter((item) =>
    item.status !== 2 &&
    item.dueDate &&
    dayjs(item.dueDate).isValid() &&
    dayjs(item.dueDate).diff(now, 'day') <= 7 &&
    dayjs(item.dueDate).diff(now, 'day') >= 0,
  ).length;
});

const dueDateValue = computed(() =>
  formState.value.dueDate ? dayjs(formState.value.dueDate) : undefined,
);

async function fetchTasks() {
  loading.value = true;
  try {
    const allTasks: PostgraduateTask[] = [];
    let page = 1;
    const pageSize = 100;

    while (true) {
      const result = await getPostgraduateTaskPageApi({
        keyword: keyword.value || undefined,
        page,
        pageSize,
        status: status.value,
        type: type.value,
      });

      allTasks.push(...result.items);
      if (allTasks.length >= result.total || result.items.length < pageSize) {
        break;
      }
      page += 1;
    }

    tasks.value = allTasks;
  } catch {
    message.error('加载学习任务失败');
  } finally {
    loading.value = false;
  }
}

function resetFilters() {
  keyword.value = '';
  status.value = undefined;
  type.value = undefined;
  void fetchTasks();
}

function openCreate() {
  editingId.value = null;
  formState.value = {
    description: '',
    dueDate: '',
    priority: 2,
    status: 0,
    title: '',
    type: 0,
  };
  formOpen.value = true;
}

function openEdit(task: PostgraduateTask) {
  editingId.value = task.id;
  formState.value = {
    description: task.description || '',
    dueDate: task.dueDate || '',
    priority: task.priority,
    status: task.status,
    title: task.title,
    type: task.type,
  };
  formOpen.value = true;
}

function toTask(record: Record<string, any>) {
  return record as PostgraduateTask;
}

function handleDueDateChange(_: Dayjs | string, dateString: string) {
  formState.value.dueDate = dateString;
}

async function handleSave() {
  if (!formState.value.title.trim()) {
    message.warning('请填写任务标题');
    return;
  }

  const payload: SavePostgraduateTaskInput = {
    description: formState.value.description || undefined,
    dueDate: formState.value.dueDate || undefined,
    priority: formState.value.priority,
    status: formState.value.status,
    title: formState.value.title,
    type: formState.value.type,
  };

  try {
    if (editingId.value) {
      await updatePostgraduateTaskApi(editingId.value, payload);
      message.success('学习任务已更新');
    } else {
      await createPostgraduateTaskApi(payload);
      message.success('学习任务已创建');
    }
    formOpen.value = false;
    await fetchTasks();
  } catch {
    message.error('保存学习任务失败');
  }
}

async function handleDelete(id: string) {
  try {
    await deletePostgraduateTaskApi(id);
    message.success('学习任务已删除');
    await fetchTasks();
  } catch {
    message.error('删除学习任务失败');
  }
}

async function markCompleted(task: PostgraduateTask) {
  try {
    await updatePostgraduateTaskApi(task.id, {
      description: task.description || undefined,
      dueDate: task.dueDate || undefined,
      priority: task.priority,
      status: 2,
      title: task.title,
      type: task.type,
    });
    message.success('任务已标记为完成');
    await fetchTasks();
  } catch {
    message.error('更新任务状态失败');
  }
}

onMounted(() => {
  void fetchTasks();
});
</script>

<template>
  <Page description="制定学习计划、跟踪学习进度" title="学习计划">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="学习任务" :value="totalCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="进行中" :value="inProgressCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已完成" :value="completedCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="7天内截止" :value="upcomingCount" /></Card>
      </Col>
    </Row>

    <Card title="学习任务列表">
      <template #extra>
        <Space>
          <Input
            v-model:value="keyword"
            allow-clear
            placeholder="搜索任务标题"
            style="width: 180px"
            @press-enter="fetchTasks"
          />
          <Select
            v-model:value="status"
            :options="statusOptions"
            allow-clear
            placeholder="状态"
            style="width: 120px"
          />
          <Select
            v-model:value="type"
            :options="typeOptions"
            allow-clear
            placeholder="类型"
            style="width: 130px"
          />
          <Button type="primary" @click="fetchTasks">查询</Button>
          <Button @click="resetFilters">重置</Button>
          <Button type="primary" @click="openCreate">新建计划</Button>
        </Space>
      </template>

      <Table
        :columns="columns"
        :data-source="tasks"
        :loading="loading"
        :pagination="{ pageSize: 10, showSizeChanger: true, showTotal: (value: number) => `共 ${value} 条` }"
        :scroll="{ x: 980 }"
        row-key="id"
      >
        <template #bodyCell="{ column, record, text }">
          <template v-if="column.key === 'title'">
            <div class="font-medium">{{ record.title }}</div>
            <div v-if="record.description" class="text-text-secondary line-clamp-1 text-xs">
              {{ record.description }}
            </div>
          </template>
          <template v-else-if="column.key === 'type'">
            <Tag color="cyan">{{ typeLabels[Number(text)] || '学习任务' }}</Tag>
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="statusColors[Number(text)] || 'default'">
              {{ statusLabels[Number(text)] || '待开始' }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'priority'">
            <Tag :color="priorityColors[Number(text)] || 'default'">
              {{ priorityLabels[Number(text)] || '中' }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'dueDate'">
            <span v-if="text">{{ text }}</span>
            <span v-else class="text-text-secondary">-</span>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button v-if="record.status !== 2" size="small" type="link" @click="markCompleted(toTask(record))">
                完成
              </Button>
              <Button size="small" type="link" @click="openEdit(toTask(record))">编辑</Button>
              <Popconfirm title="确认删除该学习任务？" @confirm="handleDelete(record.id)">
                <Button danger size="small" type="link">删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="formOpen"
      :title="editingId ? '编辑学习任务' : '新建学习任务'"
      width="620px"
      @ok="handleSave"
    >
      <Form :model="formState" layout="vertical">
        <Form.Item label="任务标题" required>
          <Input v-model:value="formState.title" placeholder="例如：完成二叉树遍历专题" />
        </Form.Item>
        <Form.Item label="任务描述">
          <Input.TextArea
            v-model:value="formState.description"
            :rows="3"
            placeholder="记录本次学习目标、章节范围或复习重点"
          />
        </Form.Item>
        <Row :gutter="16">
          <Col :span="8">
            <Form.Item label="任务类型">
              <Select v-model:value="formState.type" :options="typeOptions" />
            </Form.Item>
          </Col>
          <Col :span="8">
            <Form.Item label="状态">
              <Select v-model:value="formState.status" :options="statusOptions.filter((item) => item.value !== undefined)" />
            </Form.Item>
          </Col>
          <Col :span="8">
            <Form.Item label="优先级">
              <Select v-model:value="formState.priority" :options="priorityOptions" />
            </Form.Item>
          </Col>
        </Row>
        <Form.Item label="截止日期">
          <DatePicker
            :value="dueDateValue"
            format="YYYY-MM-DD"
            style="width: 100%"
            @change="handleDueDateChange"
          />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>
