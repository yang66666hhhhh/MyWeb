<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Form,
  Input,
  List,
  message,
  Modal,
  Popconfirm,
  Select,
  Space,
  Tag,
} from 'ant-design-vue';

import type { CreateCustomReportInput, CustomReport } from '#/api/analytics/extended';

import {
  createCustomReportApi,
  deleteCustomReportApi,
  getCustomReportsApi,
} from '#/api/analytics/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const formOpen = ref(false);
const formRef = ref();
const formData = ref<CreateCustomReportInput>({
  title: '',
  description: '',
  type: undefined,
});

const formRules = {
  title: [{ required: true, message: '请输入报表名称', type: 'string' as const, trigger: 'blur' as const }],
};

const { items, load, loading, query, search, total, changePage } = usePagedQuery<
  CustomReport,
  { keyword?: string; page: number; pageSize: number; type?: string }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getCustomReportsApi,
});

const typeOptions = [
  { label: '全部类型', value: undefined },
  { label: '月报', value: '月报' },
  { label: '周报', value: '周报' },
  { label: '年报', value: '年报' },
  { label: '自定义', value: '自定义' },
];

const typeColors: Record<string, string> = {
  '月报': 'blue',
  '周报': 'green',
  '年报': 'purple',
  '自定义': 'orange',
};

function openCreate() {
  formData.value = { title: '', description: '', type: undefined };
  formOpen.value = true;
}

async function handleSubmit() {
  try {
    await formRef.value?.validate();
  } catch {
    return;
  }
  try {
    await createCustomReportApi(formData.value);
    message.success('创建成功');
    formOpen.value = false;
    await load();
  } catch {
    message.error('创建失败');
  }
}

async function handleDelete(id: string) {
  try {
    await deleteCustomReportApi(id);
    message.success('删除成功');
    await load();
  } catch {
    message.error('删除失败');
  }
}

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  void changePage(pagination.current ?? 1, pagination.pageSize ?? 10);
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="创建和查看自定义报表" title="自定义报表">
    <Card class="mb-4">
      <Form layout="inline" :model="query">
        <Form.Item label="关键词">
          <Input v-model:value="query.keyword" allow-clear placeholder="报表名称" @press-enter="search" />
        </Form.Item>
        <Form.Item label="类型">
          <Select v-model:value="query.type" :options="typeOptions" class="w-32" allow-clear />
        </Form.Item>
        <Form.Item>
          <Space>
            <Button type="primary" @click="search">查询</Button>
            <Button @click="openCreate">创建报表</Button>
          </Space>
        </Form.Item>
      </Form>
    </Card>

    <Card title="报表列表">
      <List
        :data-source="items"
        :loading="loading"
        :pagination="{
          current: query.page,
          pageSize: query.pageSize,
          total,
          showSizeChanger: true,
          showTotal: (value: number) => `共 ${value} 条`,
          onChange: (page: number, pageSize: number) => handleTableChange({ current: page, pageSize }),
        }"
      >
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta
              :title="item.title"
              :description="item.description || `创建时间: ${item.createdAt}`"
            />
            <div class="flex items-center gap-2">
              <Tag v-if="item.type" :color="typeColors[item.type]">{{ item.type }}</Tag>
              <Button type="link">查看</Button>
              <Popconfirm title="确认删除？" @confirm="handleDelete(item.id)">
                <Button danger type="link">删除</Button>
              </Popconfirm>
            </div>
          </List.Item>
        </template>
      </List>
    </Card>

    <Modal v-model:open="formOpen" title="创建报表" @ok="handleSubmit">
      <Form ref="formRef" layout="vertical" :model="formData" :rules="formRules">
        <Form.Item label="报表名称" required>
          <Input v-model:value="formData.title" placeholder="请输入报表名称" />
        </Form.Item>
        <Form.Item label="类型">
          <Select v-model:value="formData.type" :options="typeOptions.filter(o => o.value)" placeholder="请选择类型" />
        </Form.Item>
        <Form.Item label="描述">
          <Input.TextArea v-model:value="formData.description" placeholder="报表描述" />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>
