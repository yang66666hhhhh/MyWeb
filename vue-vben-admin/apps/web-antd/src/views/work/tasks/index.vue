<script lang="ts" setup>
import { computed, onMounted, reactive, ref } from 'vue';
import { useRouter } from 'vue-router';

import { Page } from '@vben/common-ui';
import { useAccessStore } from '@vben/stores';

import {
  Button,
  Card,
  Col,
  DatePicker,
  Form,
  Input,
  message,
  Modal,
  Popconfirm,
  Row,
  Select,
  Space,
  Table,
  Tag,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import { taskApi, type CreateTaskItemInput, type TaskItem, type TaskItemQuery, type UpdateTaskItemInput } from '#/api/growth/task';
import { projectApi } from '#/api/work/project';
import type { WorkProject } from '#/api/work/project';
import ExportButton from '#/components/ExportButton.vue';
import ImportButton from '#/components/ImportButton.vue';

const props = withDefaults(
  defineProps<{
    context?: 'default' | 'implementation';
  }>(),
  {
    context: 'default',
  },
);

const router = useRouter();
const accessStore = useAccessStore();
const loading = ref(false);
const submitting = ref(false);
const formOpen = ref(false);
const editingId = ref<null | string>(null);
const items = ref<TaskItem[]>([]);
const total = ref(0);
const projects = ref<WorkProject[]>([]);

const query = reactive({
  page: 1,
  pageSize: 20,
  source: 'Work',
  taskType: 'Work',
  status: undefined as number | undefined,
  priority: undefined as number | undefined,
  keyword: '',
});

const formState = reactive<CreateTaskItemInput & { projectId?: string; status?: number }>({
  planDate: dayjs().format('YYYY-MM-DD'),
  title: '',
  description: '',
  taskType: 'Work',
  source: 'Work',
  projectId: undefined,
  priority: 2,
  startTime: '',
  endTime: '',
  estimatedHours: undefined,
  remark: '',
});

const statusMap: Record<number, { color: string; label: string }> = {
  0: { color: 'default', label: '待处理' },
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

const canViewProjects = computed(() => accessStore.accessCodes.includes('WORK_PROJECT'));
const canCreateTask = computed(() => accessStore.accessCodes.includes('WORK_TASK'));
const canEditTask = computed(() => accessStore.accessCodes.includes('WORK_TASK'));
const canDeleteTask = computed(() => accessStore.accessCodes.includes('WORK_TASK'));
const canCompleteTask = computed(() => accessStore.accessCodes.includes('WORK_TASK'));

const columns = computed<any[]>(() => [
  { title: '任务标题', dataIndex: 'title', key: 'title', minWidth: 220 },
  ...(canViewProjects.value ? [{ title: '项目', dataIndex: 'projectName', key: 'projectName', width: 150 }] : []),
  { title: '优先级', dataIndex: 'priority', key: 'priority', width: 80 },
  { title: '状态', dataIndex: 'status', key: 'status', width: 100 },
  { title: '计划日期', dataIndex: 'planDate', key: 'planDate', width: 120 },
  { title: '截止日期', dataIndex: 'endTime', key: 'endTime', width: 120 },
  { title: '预计工时', dataIndex: 'estimatedHours', key: 'estimatedHours', width: 100 },
  { key: 'action', title: '操作', width: 200, fixed: 'right' },
]);

const statusOptions = [
  { label: '全部状态', value: undefined },
  { label: '待处理', value: 0 },
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

const formRef = ref();
const formRules = {
  title: [{ required: true, message: '请输入任务标题', type: 'string' as const, trigger: 'blur' as const }],
};

const summaryText = computed(() => {
  const totalCount = items.value.length;
  const completedCount = items.value.filter((item) => Number(item.status) === 2).length;
  return `共 ${totalCount} 条任务，已完成 ${completedCount} 条`;
});

const pageTitle = computed(() =>
  props.context === 'implementation' ? '实施任务' : '工作任务',
);

const pageDescription = computed(() =>
  props.context === 'implementation'
    ? '跟踪实施项目任务、交付节点和日常推进事项'
    : '管理和追踪工作任务',
);

const createButtonText = computed(() =>
  props.context === 'implementation' ? '新建实施任务' : '新建任务',
);

const emptyProjectText = computed(() =>
  props.context === 'implementation' ? '未关联实施项目' : '未关联项目',
);

async function fetchProjects() {
  if (!canViewProjects.value) {
    projects.value = [];
    return;
  }

  try {
    const res = await projectApi.getPage({ page: 1, pageSize: 100 });
    projects.value = res.items;
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载失败，请稍后重试');
  }
}

async function fetchPage() {
  loading.value = true;
  try {
    const params: TaskItemQuery = {
      ...query,
    };
    const result = await taskApi.getPage(params);
    items.value = result.items;
    total.value = result.total;
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载失败');
  } finally {
    loading.value = false;
  }
}

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  query.page = pagination.current ?? 1;
  query.pageSize = pagination.pageSize ?? 20;
  fetchPage();
}

function search() {
  query.page = 1;
  fetchPage();
}

function resetFilters() {
  query.status = undefined;
  query.priority = undefined;
  query.keyword = '';
  query.page = 1;
  fetchPage();
}

function openCreate() {
  editingId.value = null;
  Object.assign(formState, {
    planDate: dayjs().format('YYYY-MM-DD'),
    title: '',
    description: '',
    taskType: 'Work',
    source: 'Work',
    projectId: undefined,
    priority: 2,
    startTime: '',
    endTime: '',
    estimatedHours: undefined,
    remark: '',
  });
  formOpen.value = true;
}

function goToWeeklyReport() {
  void router.push('/implementation/weekly-report');
}

function openEdit(record: Record<string, any>) {
  const task = record as TaskItem;
  editingId.value = task.id;
  Object.assign(formState, {
    planDate: task.planDate || dayjs().format('YYYY-MM-DD'),
    title: task.title,
    description: task.description || '',
    taskType: task.type,
    source: task.source,
    projectId: canViewProjects.value ? task.projectId : undefined,
    priority: task.priority ? Number(task.priority) : 2,
    status: task.status ? Number(task.status) : undefined,
    startTime: task.startTime || '',
    endTime: task.endTime || '',
    estimatedHours: task.estimatedHours,
    remark: task.remark || '',
  });
  formOpen.value = true;
}

async function handleSave() {
  try { await formRef.value?.validate(); } catch { return; }

  submitting.value = true;
  try {
    if (editingId.value) {
      const data: UpdateTaskItemInput = {
        title: formState.title,
        description: formState.description,
        projectId: canViewProjects.value ? formState.projectId : undefined,
        priority: formState.priority,
        status: formState.status as any,
        startTime: formState.startTime || undefined,
        endTime: formState.endTime || undefined,
        estimatedHours: formState.estimatedHours,
        remark: formState.remark,
      };
      await taskApi.update(editingId.value, data);
      message.success('更新成功');
    } else {
      await taskApi.create(formState as CreateTaskItemInput);
      message.success('创建成功');
    }
    formOpen.value = false;
    await fetchPage();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '操作失败');
  } finally {
    submitting.value = false;
  }
}

async function handleRemove(id: string) {
  try {
    await taskApi.delete(id);
    message.success('已删除');
    fetchPage();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '删除失败');
  }
}

async function handleComplete(id: string) {
  try {
    await taskApi.complete(id);
    message.success('任务已完成');
    fetchPage();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '操作失败');
  }
}

onMounted(() => {
  void fetchProjects();
  fetchPage();
});
</script>

<template>
  <Page :description="pageDescription" :title="pageTitle">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <Form layout="inline">
            <Form.Item label="关键词">
              <Input v-model:value="query.keyword" placeholder="搜索任务..." style="width: 160px" @pressEnter="search" />
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
            <ExportButton module="tasks" filename="工作任务" />
            <ImportButton module="tasks" @imported="fetchPage" />
            <Button v-if="props.context === 'implementation'" @click="goToWeeklyReport">
              查看实施周报
            </Button>
            <Button v-if="canCreateTask" type="primary" @click="openCreate">{{ createButtonText }}</Button>
          </div>
        </div>
      </Card>

      <Card>
        <Table
          :columns="columns"
          :data-source="items"
          :loading="loading"
          :locale="{ emptyText: '暂无数据' }"
          :pagination="{ current: query.page, pageSize: query.pageSize, showSizeChanger: true, showTotal: (v: number) => `共 ${v} 条`, total }"
          :scroll="{ x: 1100 }"
          row-key="id"
          @change="handleTableChange"
        >
          <template #bodyCell="{ column, record, text }">
            <template v-if="column.key === 'title'">
              <div>
                <div class="font-medium">{{ record.title }}</div>
                <div v-if="canViewProjects" class="text-gray-400 text-xs">
                  {{ record.projectName || emptyProjectText }}
                </div>
                <div v-if="record.description" class="text-gray-400 text-xs">{{ record.description }}</div>
              </div>
            </template>
            <template v-else-if="column.key === 'priority'">
              <Tag :color="priorityMap[Number(text)]?.color">{{ priorityMap[Number(text)]?.label }}</Tag>
            </template>
            <template v-else-if="column.key === 'status'">
              <Tag :color="statusMap[text]?.color">{{ statusMap[text]?.label }}</Tag>
            </template>
            <template v-else-if="column.key === 'estimatedHours'">
              <span v-if="text">{{ text }}h</span>
              <span v-else class="text-gray-400">-</span>
            </template>
            <template v-else-if="column.key === 'action'">
              <Space>
                <Button v-if="canCompleteTask && record.status !== 2" size="small" type="link" @click="handleComplete(record.id)">完成</Button>
                <Button v-if="canEditTask" size="small" type="link" @click="openEdit(record)">编辑</Button>
                <Popconfirm v-if="canDeleteTask" title="确认删除？" @confirm="handleRemove(record.id)">
                  <Button danger size="small" type="link">删除</Button>
                </Popconfirm>
              </Space>
            </template>
          </template>
        </Table>
      </Card>
    </div>

    <Modal
      v-model:open="formOpen"
      :title="editingId ? '编辑任务' : createButtonText"
      width="600px"
      :confirm-loading="submitting"
      @ok="handleSave"
    >
      <Form ref="formRef" :model="formState" :rules="formRules" layout="vertical">
        <Form.Item label="任务标题" required>
          <Input v-model:value="formState.title" placeholder="请输入任务标题" />
        </Form.Item>

        <Form.Item label="任务描述">
          <Input.TextArea v-model:value="formState.description" :rows="3" placeholder="请输入任务描述" />
        </Form.Item>

        <Row :gutter="16">
          <Col v-if="canViewProjects" :span="12">
            <Form.Item label="所属项目">
              <Select v-model:value="formState.projectId" :options="projects.map(p => ({ label: p.projectName, value: p.id }))" allow-clear placeholder="选择项目" style="width: 100%" />
            </Form.Item>
          </Col>
          <Col :span="canViewProjects ? 12 : 24">
            <Form.Item label="优先级">
              <Select v-model:value="formState.priority" :options="priorityOptions.filter(o => o.value !== undefined)" style="width: 100%" />
            </Form.Item>
          </Col>
        </Row>

        <Row :gutter="16">
          <Col :span="12">
            <Form.Item label="计划日期">
              <DatePicker :value="formState.planDate ? dayjs(formState.planDate) : undefined" format="YYYY-MM-DD" style="width: 100%" @change="(_, value) => formState.planDate = Array.isArray(value) ? value[0] : value || ''" />
            </Form.Item>
          </Col>
          <Col :span="12">
            <Form.Item label="截止日期">
              <DatePicker :value="formState.endTime ? dayjs(formState.endTime) : undefined" format="YYYY-MM-DD" style="width: 100%" @change="(_, value) => formState.endTime = Array.isArray(value) ? value[0] : value || ''" />
            </Form.Item>
          </Col>
        </Row>

        <Row :gutter="16">
          <Col :span="12">
            <Form.Item label="预计工时">
              <Input v-model:value="formState.estimatedHours" type="number" placeholder="小时" />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item label="备注">
          <Input.TextArea v-model:value="formState.remark" :rows="2" placeholder="备注信息" />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>
