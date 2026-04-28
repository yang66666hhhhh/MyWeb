<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  DatePicker,
  Form,
  Input,
  message,
  Modal,
  Popconfirm,
  Select,
  Space,
  Table,
  Tag,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import type { PostgraduateTask, SavePostgraduateTaskInput } from '#/api/growth';

import { createPostgraduateTaskApi, getPostgraduateTaskPageApi } from '#/api/growth';
import { usePagedQuery } from '#/composables/usePagedQuery';

const formOpen = ref(false);
const editingId = ref<null | string>(null);
const formLoading = ref(false);
const formState = ref<SavePostgraduateTaskInput>({
  course: '数据结构',
  dueDate: dayjs().format('YYYY-MM-DD'),
  progress: 0,
  status: 'pending',
  title: '',
});

const courseOptions = [
  { label: '数据结构', value: '数据结构' },
  { label: '计算机网络', value: '计算机网络' },
  { label: '操作系统', value: '操作系统' },
  { label: '组成原理', value: '组成原理' },
  { label: '数学', value: '数学' },
  { label: '英语', value: '英语' },
  { label: '政治', value: '政治' },
];

const statusOptions = [
  { label: '待完成', value: 'pending' },
  { label: '进行中', value: 'reviewing' },
  { label: '已完成', value: 'completed' },
];

const statusColorMap: Record<string, string> = {
  pending: 'default',
  reviewing: 'processing',
  completed: 'success',
};

const { changePage, items, load, loading, query, resetQuery, search, total } = usePagedQuery<
  PostgraduateTask,
  { course?: string; keyword?: string; page: number; pageSize: number }
>({
  defaultQuery: { page: 1, pageSize: 10 },
  fetcher: getPostgraduateTaskPageApi,
});

function openCreate() {
  editingId.value = null;
  formState.value = {
    course: '数据结构',
    dueDate: dayjs().format('YYYY-MM-DD'),
    progress: 0,
    status: 'pending',
    title: '',
  };
  formOpen.value = true;
}

async function handleSubmit() {
  if (!formState.value.title?.trim()) {
    message.warning('请填写任务标题');
    return;
  }
  formLoading.value = true;
  try {
    await createPostgraduateTaskApi(formState.value);
    message.success('学习任务已创建');
    formOpen.value = false;
    await load();
  } finally {
    formLoading.value = false;
  }
}

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  void changePage(pagination.current ?? 1, pagination.pageSize ?? 10);
}

function resetFilters() {
  resetQuery();
  void load();
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="管理学习计划、复习任务和进度跟踪" title="学习计划">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
          <Form :model="query" layout="inline">
            <Form.Item label="科目">
              <Select v-model:value="query.course" :options="courseOptions" allow-clear class="w-36" />
            </Form.Item>
            <Form.Item label="关键词">
              <Input
                v-model:value="query.keyword"
                allow-clear
                placeholder="任务标题"
                style="width: 200px"
                @press-enter="search"
              />
            </Form.Item>
            <Form.Item>
              <Space>
                <Button type="primary" @click="search">查询</Button>
                <Button @click="resetFilters">重置</Button>
              </Space>
            </Form.Item>
          </Form>
          <Button type="primary" @click="openCreate">新增任务</Button>
        </div>
      </Card>

      <Card>
        <Table
          :columns="[
            { dataIndex: 'title', key: 'title', title: '任务标题', minWidth: 200 },
            { dataIndex: 'course', key: 'course', title: '科目', width: 120 },
            { dataIndex: 'status', key: 'status', title: '状态', width: 100 },
            { dataIndex: 'progress', key: 'progress', title: '进度', width: 160 },
            { dataIndex: 'dueDate', key: 'dueDate', title: '截止日期', width: 120 },
            { dataIndex: 'createdAt', key: 'createdAt', title: '创建时间', width: 170 },
          ]"
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
          <template #bodyCell="{ column, record, text }">
            <template v-if="column.key === 'title'">
              <div class="font-medium">{{ record.title }}</div>
            </template>
            <template v-else-if="column.key === 'course'">
              <Tag color="blue">{{ text }}</Tag>
            </template>
            <template v-else-if="column.key === 'status'">
              <Tag :color="statusColorMap[text]">
                {{ statusOptions.find((o) => o.value === text)?.label }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'progress'">
              <div class="flex items-center gap-2">
                <div class="w-24 h-2 bg-gray-200 rounded-full overflow-hidden">
                  <div
                    class="h-full bg-blue-500 rounded-full transition-all"
                    :style="{ width: `${text}%` }"
                  />
                </div>
                <span class="text-sm">{{ text }}%</span>
              </div>
            </template>
            <template v-else-if="column.key === 'dueDate'">
              {{ text || '-' }}
            </template>
            <template v-else-if="column.key === 'createdAt'">
              {{ text?.replace('T', ' ').slice(0, 16) || '-' }}
            </template>
          </template>
        </Table>
      </Card>
    </div>

    <Modal
      :confirm-loading="formLoading"
      v-model:open="formOpen"
      title="新增学习任务"
      width="560px"
      @cancel="formOpen = false"
      @ok="handleSubmit"
    >
      <Form :model="formState" layout="vertical">
        <Form.Item label="任务标题" required>
          <Input v-model:value="formState.title" placeholder="例如：复习链表章节" />
        </Form.Item>
        <div class="grid grid-cols-2 gap-4">
          <Form.Item label="科目">
            <Select v-model:value="formState.course" :options="courseOptions" />
          </Form.Item>
          <Form.Item label="状态">
            <Select v-model:value="formState.status" :options="statusOptions" />
          </Form.Item>
          <Form.Item label="截止日期">
            <DatePicker
              style="width: 100%"
              :value="formState.dueDate ? dayjs(formState.dueDate) : undefined"
              format="YYYY-MM-DD"
              @change="(_, dateStr) => (formState.dueDate = dateStr as string)"
            />
          </Form.Item>
          <Form.Item label="进度">
            <div class="flex items-center gap-2">
              <Input
                v-model:value="formState.progress"
                type="number"
                :min="0"
                :max="100"
                class="w-20"
              />
              <span>%</span>
            </div>
          </Form.Item>
        </div>
      </Form>
    </Modal>
  </Page>
</template>
