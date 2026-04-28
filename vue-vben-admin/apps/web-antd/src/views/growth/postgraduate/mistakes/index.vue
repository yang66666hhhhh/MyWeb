<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Descriptions,
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

import type { ExamMistake } from '#/api/growth';

import {
  createMistakeApi,
  deleteMistakeApi,
  getMistakePageApi,
  updateMistakeReviewStatusApi,
} from '#/api/growth';
import { usePagedQuery } from '#/composables/usePagedQuery';

const detailOpen = ref(false);
const formOpen = ref(false);
const selectedItem = ref<ExamMistake | null>(null);
const formLoading = ref(false);

const formState = ref({
  course: '数据结构',
  correctAnswer: '',
  errorReason: '',
  question: '',
  recordDate: '',
  reviewStatus: 'pending' as const,
  tags: [] as string[],
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

const reviewStatusOptions = [
  { label: '待复习', value: 'pending' },
  { label: '已复习', value: 'reviewed' },
  { label: '已掌握', value: 'mastered' },
];

const reviewStatusColorMap: Record<string, string> = {
  pending: 'warning',
  reviewed: 'processing',
  mastered: 'success',
};

const { changePage, items, load, loading, query, resetQuery, search, total } = usePagedQuery<
  ExamMistake,
  { course?: string; keyword?: string; page: number; pageSize: number; reviewStatus?: 'pending' | 'reviewed' | 'mastered' }
>({
  defaultQuery: { page: 1, pageSize: 10 },
  fetcher: getMistakePageApi,
});

function showDetail(record: ExamMistake) {
  selectedItem.value = record;
  detailOpen.value = true;
}

function openCreate() {
  formState.value = {
    course: '数据结构',
    correctAnswer: '',
    errorReason: '',
    question: '',
    recordDate: new Date().toISOString().slice(0, 10),
    reviewStatus: 'pending',
    tags: [],
  };
  formOpen.value = true;
}

async function handleSubmit() {
  if (!formState.value.question?.trim()) {
    message.warning('请填写题目');
    return;
  }
  formLoading.value = true;
  try {
    await createMistakeApi(formState.value);
    message.success('错题已添加');
    formOpen.value = false;
    await load();
  } finally {
    formLoading.value = false;
  }
}

async function updateStatus(id: string, status: ExamMistake['reviewStatus']) {
  await updateMistakeReviewStatusApi(id, status);
  message.success('状态已更新');
  await load();
}

async function handleRemove(id: string) {
  await deleteMistakeApi(id);
  message.success('错题已删除');
  await load();
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
  <Page description="记录错题、整理错误原因、定期复习" title="错题本">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
          <Form :model="query" layout="inline">
            <Form.Item label="科目">
              <Select v-model:value="query.course" :options="courseOptions" allow-clear class="w-36" />
            </Form.Item>
            <Form.Item label="复习状态">
              <Select v-model:value="query.reviewStatus" :options="reviewStatusOptions" allow-clear class="w-36" />
            </Form.Item>
            <Form.Item label="关键词">
              <Input
                v-model:value="query.keyword"
                allow-clear
                placeholder="题目/错误原因"
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
          <Button type="primary" @click="openCreate">新增错题</Button>
        </div>
      </Card>

      <Card>
        <Table
          :columns="[
            { dataIndex: 'question', key: 'question', title: '错题题目', minWidth: 200 },
            { dataIndex: 'course', key: 'course', title: '科目', width: 120 },
            { dataIndex: 'reviewStatus', key: 'reviewStatus', title: '状态', width: 100 },
            { dataIndex: 'reviewCount', key: 'reviewCount', title: '复习次数', width: 100 },
            { dataIndex: 'recordDate', key: 'recordDate', title: '记录日期', width: 120 },
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
            <template v-if="column.key === 'question'">
              <div class="font-medium line-clamp-2">{{ record.question }}</div>
              <div class="text-xs text-text-secondary mt-1">{{ record.errorReason }}</div>
            </template>
            <template v-else-if="column.key === 'course'">
              <Tag color="blue">{{ text }}</Tag>
            </template>
            <template v-else-if="column.key === 'reviewStatus'">
              <Tag :color="reviewStatusColorMap[text]">
                {{ reviewStatusOptions.find((o) => o.value === text)?.label }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'action'">
              <Space>
                <Button size="small" type="link" @click="showDetail(record)">详情</Button>
                <Button
                  v-if="record.reviewStatus === 'pending'"
                  size="small"
                  type="link"
                  @click="updateStatus(record.id, 'reviewed')"
                >
                  标记已复习
                </Button>
                <Button
                  v-if="record.reviewStatus === 'reviewed'"
                  size="small"
                  type="link"
                  @click="updateStatus(record.id, 'mastered')"
                >
                  标记已掌握
                </Button>
                <Popconfirm title="确认删除？" @confirm="handleRemove(record.id)">
                  <Button danger size="small" type="link">删除</Button>
                </Popconfirm>
              </Space>
            </template>
          </template>
        </Table>
      </Card>
    </div>

    <Modal v-model:open="detailOpen" title="错题详情" width="680px" :footer="null">
      <div v-if="selectedItem" class="space-y-4">
        <Descriptions bordered :column="1" size="small">
          <Descriptions.Item label="科目">
            <Tag color="blue">{{ selectedItem.course }}</Tag>
          </Descriptions.Item>
          <Descriptions.Item label="错题题目">{{ selectedItem.question }}</Descriptions.Item>
          <Descriptions.Item label="正确答案">{{ selectedItem.correctAnswer }}</Descriptions.Item>
          <Descriptions.Item label="错误原因">{{ selectedItem.errorReason }}</Descriptions.Item>
          <Descriptions.Item label="复习状态">
            <Tag :color="reviewStatusColorMap[selectedItem.reviewStatus]">
              {{ reviewStatusOptions.find((o) => o.value === selectedItem.reviewStatus)?.label }}
            </Tag>
          </Descriptions.Item>
          <Descriptions.Item label="复习次数">{{ selectedItem.reviewCount }} 次</Descriptions.Item>
          <Descriptions.Item label="记录日期">{{ selectedItem.recordDate }}</Descriptions.Item>
          <Descriptions.Item label="标签">
            <Space v-if="selectedItem.tags?.length" wrap>
              <Tag v-for="tag in selectedItem.tags" :key="tag">{{ tag }}</Tag>
            </Space>
            <span v-else>-</span>
          </Descriptions.Item>
        </Descriptions>
      </div>
    </Modal>

    <Modal
      :confirm-loading="formLoading"
      v-model:open="formOpen"
      title="新增错题"
      width="680px"
      @cancel="formOpen = false"
      @ok="handleSubmit"
    >
      <Form :model="formState" layout="vertical">
        <Form.Item label="科目">
          <Select v-model:value="formState.course" :options="courseOptions" />
        </Form.Item>
        <Form.Item label="错题题目" required>
          <Input.TextArea
            v-model:value="formState.question"
            :auto-size="{ minRows: 2, maxRows: 4 }"
            placeholder="记录题目内容"
          />
        </Form.Item>
        <Form.Item label="正确答案">
          <Input v-model:value="formState.correctAnswer" placeholder="正确答案" />
        </Form.Item>
        <Form.Item label="错误原因">
          <Input.TextArea
            v-model:value="formState.errorReason"
            :auto-size="{ minRows: 2, maxRows: 4 }"
            placeholder="分析错误原因"
          />
        </Form.Item>
        <Form.Item label="复习状态">
          <Select v-model:value="formState.reviewStatus" :options="reviewStatusOptions" />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>
