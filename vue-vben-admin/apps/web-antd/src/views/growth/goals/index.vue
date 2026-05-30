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
  message,
  Modal,
  Popconfirm,
  Row,
  Select,
  Space,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { CreateGoalInput, Goal, UpdateGoalInput } from '#/api/growth/extended';

import {
  createGoalApi,
  deleteGoalApi,
  getGoalsApi,
  updateGoalApi,
} from '#/api/growth/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const formOpen = ref(false);
const editingId = ref<null | string>(null);
const formData = ref<CreateGoalInput & UpdateGoalInput>({
  title: '',
  description: '',
  category: '',
  priority: 2,
  startDate: undefined,
  dueDate: undefined,
  tags: '',
});

const { changePage, items, load, loading, query, search, total } = usePagedQuery<
  Goal,
  { category?: string; keyword?: string; page: number; pageSize: number; status?: number }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getGoalsApi,
});

const categoryOptions = [
  { label: '全部分类', value: undefined },
  { label: '学业', value: '学业' },
  { label: '职业', value: '职业' },
  { label: '健康', value: '健康' },
  { label: '成长', value: '成长' },
];

const statusOptions = [
  { label: '全部状态', value: undefined },
  { label: '未开始', value: 0 },
  { label: '进行中', value: 1 },
  { label: '已完成', value: 2 },
  { label: '已取消', value: 3 },
];

const priorityOptions = [
  { label: '低', value: 1 },
  { label: '中', value: 2 },
  { label: '高', value: 3 },
  { label: '紧急', value: 4 },
];

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
};

const columns = [
  { dataIndex: 'title', key: 'title', title: '目标' },
  { dataIndex: 'category', key: 'category', title: '分类', width: 100 },
  { dataIndex: 'priority', key: 'priority', title: '优先级', width: 100 },
  { dataIndex: 'status', key: 'status', title: '状态', width: 100 },
  { key: 'progress', title: '进度', width: 160 },
  { dataIndex: 'dueDate', key: 'dueDate', title: '截止日期', width: 140 },
  { key: 'action', title: '操作', width: 200 },
];

const stats = computed(() => {
  const list = items.value as Goal[];
  const completed = list.filter((i) => i.status === 2).length;
  const inProgress = list.filter((i) => i.status === 1).length;
  const avgProgress = list.length
    ? Math.round(list.reduce((s, i) => s + i.progress, 0) / list.length)
    : 0;
  return { total: total.value, completed, inProgress, avgProgress };
});

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  void changePage(pagination.current ?? 1, pagination.pageSize ?? 10);
}

function openCreate() {
  editingId.value = null;
  formData.value = { title: '', description: '', category: '', priority: 2, startDate: undefined, dueDate: undefined, tags: '' };
  formOpen.value = true;
}

function openEdit(record: Goal) {
  editingId.value = record.id;
  formData.value = {
    title: record.title,
    description: record.description || '',
    category: record.category,
    priority: record.priority,
    startDate: record.startDate,
    dueDate: record.dueDate,
    tags: record.tags || '',
  };
  formOpen.value = true;
}

async function handleSubmit() {
  try {
    if (editingId.value) {
      await updateGoalApi(editingId.value, formData.value);
      message.success('更新成功');
    } else {
      await createGoalApi(formData.value as CreateGoalInput);
      message.success('创建成功');
    }
    formOpen.value = false;
    await load();
  } catch {
    message.error('操作失败');
  }
}

async function handleDelete(id: string) {
  try {
    await deleteGoalApi(id);
    message.success('删除成功');
    await load();
  } catch {
    message.error('删除失败');
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="设定和追踪个人目标" title="目标管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="目标总数" :value="stats.total" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已完成" :value="stats.completed" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="进行中" :value="stats.inProgress" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均进度" :value="stats.avgProgress" suffix="%" /></Card>
      </Col>
    </Row>

    <Card class="mb-4">
      <Form layout="inline" :model="query">
        <Form.Item label="关键词">
          <Input v-model:value="query.keyword" allow-clear placeholder="目标名称" @press-enter="search" />
        </Form.Item>
        <Form.Item label="分类">
          <Select v-model:value="query.category" :options="categoryOptions" class="w-32" allow-clear />
        </Form.Item>
        <Form.Item label="状态">
          <Select v-model:value="query.status" :options="statusOptions" class="w-32" allow-clear />
        </Form.Item>
        <Form.Item>
          <Space>
            <Button type="primary" @click="search">查询</Button>
            <Button @click="openCreate">新增目标</Button>
          </Space>
        </Form.Item>
      </Form>
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
        row-key="id"
        @change="handleTableChange"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'category'">
            <Tag color="blue">{{ record.category }}</Tag>
          </template>
          <template v-else-if="column.key === 'priority'">
            <Tag :color="priorityMap[record.priority]?.color">{{ priorityMap[record.priority]?.label }}</Tag>
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="statusMap[record.status]?.color">{{ statusMap[record.status]?.label }}</Tag>
          </template>
          <template v-else-if="column.key === 'progress'">
            <div class="w-32">
              <div class="mb-1 text-right text-sm">{{ record.progress }}%</div>
              <div class="h-2 rounded-full bg-gray-200">
                <div class="h-full rounded-full bg-blue-500" :style="{ width: `${record.progress}%` }"></div>
              </div>
            </div>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button size="small" type="link" @click="openEdit(record as Goal)">编辑</Button>
              <Popconfirm title="确认删除？" @confirm="handleDelete(record.id)">
                <Button danger size="small" type="link">删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="formOpen"
      :title="editingId ? '编辑目标' : '新增目标'"
      @ok="handleSubmit"
    >
      <Form layout="vertical" :model="formData">
        <Form.Item label="目标标题" required>
          <Input v-model:value="formData.title" placeholder="请输入目标标题" />
        </Form.Item>
        <Form.Item label="分类" required>
          <Select v-model:value="formData.category" :options="categoryOptions.filter(o => o.value)" placeholder="请选择分类" />
        </Form.Item>
        <Form.Item label="优先级">
          <Select v-model:value="formData.priority" :options="priorityOptions" />
        </Form.Item>
        <Form.Item label="开始日期">
          <DatePicker v-model:value="formData.startDate" class="w-full" />
        </Form.Item>
        <Form.Item label="截止日期">
          <DatePicker v-model:value="formData.dueDate" class="w-full" />
        </Form.Item>
        <Form.Item label="描述">
          <Input.TextArea v-model:value="formData.description" placeholder="目标描述" />
        </Form.Item>
        <Form.Item label="标签">
          <Input v-model:value="formData.tags" placeholder="标签，逗号分隔" />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>
