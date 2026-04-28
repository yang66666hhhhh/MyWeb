<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
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

import type { ExamMaterial } from '#/api/growth';

import {
  createMaterialApi,
  deleteMaterialApi,
  getMaterialPageApi,
} from '#/api/growth';
import { usePagedQuery } from '#/composables/usePagedQuery';

const formOpen = ref(false);
const formLoading = ref(false);
const formState = ref({
  category: '讲义',
  fileName: '',
  fileUrl: '',
  subject: '数据结构',
  summary: '',
  tags: [] as string[],
  type: 'PDF',
});

const subjectOptions = [
  { label: '数据结构', value: '数据结构' },
  { label: '计算机网络', value: '计算机网络' },
  { label: '操作系统', value: '操作系统' },
  { label: '组成原理', value: '组成原理' },
  { label: '数学', value: '数学' },
  { label: '英语', value: '英语' },
  { label: '政治', value: '政治' },
];

const categoryOptions = [
  { label: '讲义', value: '讲义' },
  { label: '真题', value: '真题' },
  { label: '笔记', value: '笔记' },
  { label: '习题', value: '习题' },
  { label: '其他', value: '其他' },
];

const typeOptions = [
  { label: 'PDF', value: 'PDF' },
  { label: 'Word', value: 'Word' },
  { label: 'Markdown', value: 'Markdown' },
  { label: '其他', value: '其他' },
];

const subjectColorMap: Record<string, string> = {
  数据结构: 'blue',
  计算机网络: 'cyan',
  操作系统: 'orange',
  组成原理: 'purple',
  数学: 'red',
  英语: 'green',
  政治: 'gold',
};

const { changePage, items, load, loading, query, resetQuery, search, total } = usePagedQuery<
  ExamMaterial,
  { category?: string; keyword?: string; page: number; pageSize: number; subject?: string }
>({
  defaultQuery: { page: 1, pageSize: 10 },
  fetcher: getMaterialPageApi,
});

function openCreate() {
  formState.value = {
    category: '讲义',
    fileName: '',
    fileUrl: '',
    subject: '数据结构',
    summary: '',
    tags: [],
    type: 'PDF',
  };
  formOpen.value = true;
}

async function handleSubmit() {
  if (!formState.value.fileName?.trim()) {
    message.warning('请填写资料名称');
    return;
  }
  formLoading.value = true;
  try {
    await createMaterialApi(formState.value);
    message.success('资料已添加');
    formOpen.value = false;
    await load();
  } finally {
    formLoading.value = false;
  }
}

async function handleRemove(id: string) {
  await deleteMaterialApi(id);
  message.success('资料已删除');
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
  <Page description="管理学习资料、讲义、真题和笔记" title="资料库">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
          <Form :model="query" layout="inline">
            <Form.Item label="科目">
              <Select v-model:value="query.subject" :options="subjectOptions" allow-clear class="w-36" />
            </Form.Item>
            <Form.Item label="分类">
              <Select v-model:value="query.category" :options="categoryOptions" allow-clear class="w-36" />
            </Form.Item>
            <Form.Item label="关键词">
              <Input
                v-model:value="query.keyword"
                allow-clear
                placeholder="资料名称/描述"
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
          <Button type="primary" @click="openCreate">新增资料</Button>
        </div>
      </Card>

      <Card>
        <Table
          :columns="[
            { dataIndex: 'fileName', key: 'fileName', title: '资料名称', minWidth: 200 },
            { dataIndex: 'subject', key: 'subject', title: '科目', width: 120 },
            { dataIndex: 'category', key: 'category', title: '分类', width: 100 },
            { dataIndex: 'type', key: 'type', title: '格式', width: 100 },
            { dataIndex: 'summary', key: 'summary', title: '简介', minWidth: 200 },
            { dataIndex: 'createdAt', key: 'createdAt', title: '上传时间', width: 170 },
            { key: 'action', title: '操作', width: 200, fixed: 'right' },
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
            <template v-if="column.key === 'fileName'">
              <div class="space-y-1">
                <div class="font-medium flex items-center gap-2">
                  {{ record.fileName }}
                  <Tag v-if="record.type" class="text-xs">{{ record.type }}</Tag>
                </div>
                <div v-if="record.fileUrl" class="text-xs text-blue-500">
                  <a :href="record.fileUrl" target="_blank" class="hover:underline">查看附件</a>
                </div>
              </div>
            </template>
            <template v-else-if="column.key === 'subject'">
              <Tag :color="subjectColorMap[text] || 'default'">{{ text }}</Tag>
            </template>
            <template v-else-if="column.key === 'summary'">
              <span class="text-text-secondary text-sm line-clamp-2">{{ text || '-' }}</span>
            </template>
            <template v-else-if="column.key === 'tags'">
              <Space v-if="text?.length" wrap>
                <Tag v-for="tag in text" :key="tag" class="text-xs">{{ tag }}</Tag>
              </Space>
              <span v-else>-</span>
            </template>
            <template v-else-if="column.key === 'action'">
              <Space>
                <a v-if="record.fileUrl" :href="record.fileUrl" target="_blank" class="text-blue-500 hover:underline">下载</a>
                <Popconfirm title="确认删除？" @confirm="handleRemove(record.id)">
                  <Button danger size="small" type="link">删除</Button>
                </Popconfirm>
              </Space>
            </template>
          </template>
        </Table>
      </Card>
    </div>

    <Modal
      :confirm-loading="formLoading"
      v-model:open="formOpen"
      title="新增资料"
      width="600px"
      @cancel="formOpen = false"
      @ok="handleSubmit"
    >
      <Form :model="formState" layout="vertical">
        <Form.Item label="资料名称" required>
          <Input v-model:value="formState.fileName" placeholder="例如：数据结构重点讲义.pdf" />
        </Form.Item>
        <div class="grid grid-cols-2 gap-4">
          <Form.Item label="科目">
            <Select v-model:value="formState.subject" :options="subjectOptions" />
          </Form.Item>
          <Form.Item label="分类">
            <Select v-model:value="formState.category" :options="categoryOptions" />
          </Form.Item>
          <Form.Item label="文件格式">
            <Select v-model:value="formState.type" :options="typeOptions" />
          </Form.Item>
        </div>
        <Form.Item label="资料简介">
          <Input.TextArea
            v-model:value="formState.summary"
            :auto-size="{ minRows: 2, maxRows: 4 }"
            placeholder="简要描述资料内容"
          />
        </Form.Item>
        <Form.Item label="来源链接">
          <Input v-model:value="formState.fileUrl" placeholder="https://..." />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>
