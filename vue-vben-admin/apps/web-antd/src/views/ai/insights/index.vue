<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Form,
  Input,
  List,
  message,
  Popconfirm,
  Row,
  Select,
  Space,
  Statistic,
  Tag,
} from 'ant-design-vue';

import type { AiInsightItem } from '#/api/ai/extended';

import {
  deleteInsightApi,
  generateInsightApi,
  getInsightsApi,
} from '#/api/ai/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const generating = ref(false);
const generateForm = ref({
  title: '',
  category: '',
  source: '',
});

const { items, load, loading, query, search, total, changePage } = usePagedQuery<
  AiInsightItem,
  { keyword?: string; page: number; pageSize: number; type?: string }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getInsightsApi,
});

const categoryOptions = [
  { label: '全部分类', value: undefined },
  { label: '趋势', value: '趋势' },
  { label: '分析', value: '分析' },
  { label: '预测', value: '预测' },
];

const categoryColors: Record<string, string> = {
  '趋势': 'blue',
  '分析': 'purple',
  '预测': 'green',
};

async function handleGenerate() {
  generating.value = true;
  try {
    await generateInsightApi(generateForm.value);
    message.success('洞察生成成功');
    generateForm.value = { title: '', category: '', source: '' };
    await load();
  } catch (e: any) {
    message.error(e?.message || '生成失败');
  } finally {
    generating.value = false;
  }
}

async function handleDelete(id: string) {
  try {
    await deleteInsightApi(id);
    message.success('删除成功');
    await load();
  } catch (e: any) {
    message.error(e?.message || '删除失败');
  }
}

function handlePageChange(page: number, pageSize: number) {
  void changePage(page, pageSize);
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="AI驱动的数据洞察分析" title="数据洞察">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="洞察数量" :value="total" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="当前页" :value="items.length" /></Card>
      </Col>
    </Row>

    <Card class="mb-4">
      <Form layout="inline" :model="query">
        <Form.Item label="关键词">
          <Input v-model:value="query.keyword" allow-clear placeholder="搜索洞察" @press-enter="search" />
        </Form.Item>
        <Form.Item label="分类">
          <Select v-model:value="query.type" :options="categoryOptions" class="w-32" allow-clear />
        </Form.Item>
        <Form.Item>
          <Button type="primary" @click="search">查询</Button>
        </Form.Item>
      </Form>
    </Card>

    <Card class="mb-4" title="生成新洞察">
      <Space>
        <Input v-model:value="generateForm.title" placeholder="洞察标题（可选）" style="width: 200px" />
        <Select v-model:value="generateForm.category" :options="categoryOptions.filter(o => o.value)" placeholder="分类" class="w-32" />
        <Input v-model:value="generateForm.source" placeholder="数据来源（可选）" style="width: 200px" />
        <Button type="primary" :loading="generating" @click="handleGenerate">生成洞察</Button>
      </Space>
    </Card>

    <Card title="AI洞察">
      <List
        :data-source="items"
        :loading="loading"
        :pagination="{
          current: query.page,
          pageSize: query.pageSize,
          total,
          showSizeChanger: true,
          showTotal: (value: number) => `共 ${value} 条`,
          onChange: handlePageChange,
        }"
      >
        <template #renderItem="{ item }">
          <List.Item>
            <div class="w-full">
              <div class="flex items-center justify-between mb-2">
                <span class="text-lg font-bold">{{ item.title }}</span>
                <Space>
                  <Tag v-if="item.category" :color="categoryColors[item.category] || 'default'">
                    {{ item.category }}
                  </Tag>
                  <span class="text-gray-400">{{ item.createdAt }}</span>
                  <Popconfirm title="确认删除？" @confirm="handleDelete(item.id)">
                    <Button danger type="link">删除</Button>
                  </Popconfirm>
                </Space>
              </div>
              <div v-if="item.content" class="text-gray-500">{{ item.content }}</div>
              <div v-if="item.source" class="text-gray-400 mt-1">来源: {{ item.source }}</div>
            </div>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
