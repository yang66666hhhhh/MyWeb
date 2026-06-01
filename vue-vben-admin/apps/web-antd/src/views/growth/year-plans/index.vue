<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

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
  Row,
  Select,
  Space,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { CreateYearPlanInput, UpdateYearPlanInput, YearPlan } from '#/api/growth/extended';

import {
  createYearPlanApi,
  deleteYearPlanApi,
  getYearPlansApi,
  updateYearPlanApi,
} from '#/api/growth/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const formOpen = ref(false);
const submitting = ref(false);
const editingId = ref<null | string>(null);
const formData = ref<CreateYearPlanInput & UpdateYearPlanInput>({
  year: new Date().getFullYear(),
  title: '',
  description: '',
  category: '',
  tags: '',
});

const formRef = ref();
const formRules = {
  year: [{ required: true, message: '请输入年份', type: 'number' as const }],
  title: [{ required: true, message: '请输入目标标题', type: 'string' as const }],
  category: [{ required: true, message: '请选择分类', type: 'string' as const }],};

const { changePage, items, load, loading, query, search, total } = usePagedQuery<
  YearPlan,
  { category?: string; keyword?: string; page: number; pageSize: number; status?: number }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getYearPlansApi,
});

const categoryOptions = [
  { label: '全部分类', value: undefined },
  { label: '技术', value: '技术' },
  { label: '学业', value: '学业' },
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

const statusMap: Record<number, { color: string; label: string }> = {
  0: { color: 'default', label: '未开始' },
  1: { color: 'processing', label: '进行中' },
  2: { color: 'success', label: '已完成' },
  3: { color: 'error', label: '已取消' },
};

const categoryColors: Record<string, string> = {
  技术: 'blue',
  学业: 'green',
  健康: 'orange',
  成长: 'purple',
};

const columns = [
  { dataIndex: 'title', key: 'title', title: '年度目标' },
  { dataIndex: 'year', key: 'year', title: '年份', width: 80 },
  { dataIndex: 'category', key: 'category', title: '分类', width: 100 },
  { dataIndex: 'status', key: 'status', title: '状态', width: 100 },
  { key: 'progress', title: '进度', width: 160 },
  { dataIndex: 'createdAt', key: 'createdAt', title: '创建时间', width: 180 },
  { key: 'action', title: '操作', width: 200 },
];

const stats = computed(() => {
  const list = items.value as YearPlan[];
  const inProgress = list.filter((i) => i.status === 1).length;
  const completed = list.filter((i) => i.status === 2).length;
  const avgProgress = list.length
    ? Math.round(list.reduce((s, i) => s + i.progress, 0) / list.length)
    : 0;
  return { total: total.value, inProgress, completed, avgProgress };
});

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  void changePage(pagination.current ?? 1, pagination.pageSize ?? 10);
}

function openCreate() {
  editingId.value = null;
  formData.value = { year: new Date().getFullYear(), title: '', description: '', category: '', tags: '' };
  formOpen.value = true;
}

function openEdit(record: YearPlan) {
  editingId.value = record.id;
  formData.value = {
    year: record.year,
    title: record.title,
    description: record.description || '',
    category: record.category,
    tags: record.tags || '',
  };
  formOpen.value = true;
}

async function handleSubmit() {
    try { await formRef.value?.validate(); } catch { return; }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateYearPlanApi(editingId.value, formData.value);
      message.success('更新成功');
    } else {
      await createYearPlanApi(formData.value as CreateYearPlanInput);
      message.success('创建成功');
    }
    formOpen.value = false;
    await load();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '操作失败');
  } finally {
    submitting.value = false;
  }
}

async function handleDelete(id: string) {
  try {
    await deleteYearPlanApi(id);
    message.success('删除成功');
    await load();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '删除失败');
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="设定和追踪年度目标" title="年度计划">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="年度目标" :value="stats.total" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="进行中" :value="stats.inProgress" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已完成" :value="stats.completed" /></Card>
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
            <Tag :color="categoryColors[record.category]">{{ record.category }}</Tag>
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
              <Button size="small" type="link" @click="openEdit(record as YearPlan)">编辑</Button>
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
      :confirm-loading="submitting"
      :title="editingId ? '编辑年度目标' : '新增年度目标'"
      @ok="handleSubmit"
    >
      <Form ref="formRef" layout="vertical" :model="formData" :rules="formRules">
        <Form.Item label="年份" required>
          <InputNumber v-model:value="formData.year" :min="2020" :max="2030" class="w-full" />
        </Form.Item>
        <Form.Item label="目标标题" required>
          <Input v-model:value="formData.title" placeholder="请输入目标标题" />
        </Form.Item>
        <Form.Item label="分类" required>
          <Select v-model:value="formData.category" :options="categoryOptions.filter(o => o.value)" placeholder="请选择分类" />
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
