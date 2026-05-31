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

import type { CreateReadingBookInput, ReadingBook, UpdateReadingBookInput } from '#/api/growth/extended';

import {
  createReadingBookApi,
  deleteReadingBookApi,
  getReadingBooksApi,
  updateReadingBookApi,
} from '#/api/growth/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const formOpen = ref(false);
const editingId = ref<null | string>(null);
const formData = ref<CreateReadingBookInput & UpdateReadingBookInput>({
  title: '',
  author: '',
  category: '',
  totalPages: 0,
  notes: '',
  tags: '',
});

const formRef = ref();
const formRules = {
  title: [{ required: true, message: '请输入书名', type: 'string' as const }],};

const { changePage, items, load, loading, query, search, total } = usePagedQuery<
  ReadingBook,
  { category?: string; keyword?: string; page: number; pageSize: number; status?: number }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getReadingBooksApi,
});

const categoryOptions = [
  { label: '全部分类', value: undefined },
  { label: '技术', value: '技术' },
  { label: '编程', value: '编程' },
  { label: '管理', value: '管理' },
  { label: '文学', value: '文学' },
  { label: '其他', value: '其他' },
];

const statusOptions = [
  { label: '全部状态', value: undefined },
  { label: '待阅读', value: 0 },
  { label: '阅读中', value: 1 },
  { label: '已读完', value: 2 },
  { label: '已放弃', value: 3 },
];

const statusMap: Record<number, { color: string; label: string }> = {
  0: { color: 'default', label: '待阅读' },
  1: { color: 'processing', label: '阅读中' },
  2: { color: 'success', label: '已读完' },
  3: { color: 'error', label: '已放弃' },
};

const columns = [
  { dataIndex: 'title', key: 'title', title: '书名' },
  { dataIndex: 'author', key: 'author', title: '作者', width: 120 },
  { dataIndex: 'category', key: 'category', title: '分类', width: 100 },
  { dataIndex: 'status', key: 'status', title: '状态', width: 100 },
  { key: 'progress', title: '阅读进度', width: 160 },
  { key: 'pages', title: '页数', width: 100 },
  { key: 'action', title: '操作', width: 200 },
];

const stats = computed(() => {
  const list = items.value as ReadingBook[];
  const completed = list.filter((i) => i.status === 2).length;
  const reading = list.filter((i) => i.status === 1).length;
  const pending = list.filter((i) => i.status === 0).length;
  return { total: total.value, completed, reading, pending };
});

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  void changePage(pagination.current ?? 1, pagination.pageSize ?? 10);
}

function openCreate() {
  editingId.value = null;
  formData.value = { title: '', author: '', category: '', totalPages: 0, notes: '', tags: '' };
  formOpen.value = true;
}

function openEdit(record: ReadingBook) {
  editingId.value = record.id;
  formData.value = {
    title: record.title,
    author: record.author || '',
    category: record.category || '',
    totalPages: record.totalPages,
    notes: record.notes || '',
    tags: record.tags || '',
  };
  formOpen.value = true;
}

async function handleSubmit() {
    try { await formRef.value?.validate(); } catch { return; }
  try {
    if (editingId.value) {
      await updateReadingBookApi(editingId.value, formData.value);
      message.success('更新成功');
    } else {
      await createReadingBookApi(formData.value as CreateReadingBookInput);
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
    await deleteReadingBookApi(id);
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
  <Page description="管理您的阅读清单和书籍笔记" title="阅读清单">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="书籍总数" :value="stats.total" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已读完" :value="stats.completed" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="阅读中" :value="stats.reading" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="待阅读" :value="stats.pending" /></Card>
      </Col>
    </Row>

    <Card class="mb-4">
      <Form layout="inline" :model="query">
        <Form.Item label="关键词">
          <Input v-model:value="query.keyword" allow-clear placeholder="书名或作者" @press-enter="search" />
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
            <Button @click="openCreate">添加书籍</Button>
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
            <Tag v-if="record.category" color="blue">{{ record.category }}</Tag>
            <span v-else>-</span>
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
          <template v-else-if="column.key === 'pages'">
            {{ record.currentPage }}/{{ record.totalPages }}
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button size="small" type="link" @click="openEdit(record as ReadingBook)">编辑</Button>
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
      :title="editingId ? '编辑书籍' : '添加书籍'"
      @ok="handleSubmit"
    >
      <Form ref="formRef" layout="vertical" :model="formData" :rules="formRules">
        <Form.Item label="书名" required>
          <Input v-model:value="formData.title" placeholder="请输入书名" />
        </Form.Item>
        <Form.Item label="作者">
          <Input v-model:value="formData.author" placeholder="作者名称" />
        </Form.Item>
        <Form.Item label="分类">
          <Select v-model:value="formData.category" :options="categoryOptions.filter(o => o.value)" placeholder="请选择分类" />
        </Form.Item>
        <Form.Item label="总页数">
          <InputNumber v-model:value="formData.totalPages" :min="0" class="w-full" />
        </Form.Item>
        <Form.Item label="笔记">
          <Input.TextArea v-model:value="formData.notes" placeholder="阅读笔记" />
        </Form.Item>
        <Form.Item label="标签">
          <Input v-model:value="formData.tags" placeholder="标签，逗号分隔" />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>
