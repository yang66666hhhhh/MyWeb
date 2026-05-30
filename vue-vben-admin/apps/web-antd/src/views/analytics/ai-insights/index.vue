<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, List, message, Row, Space, Statistic, Tag } from 'ant-design-vue';

import type { AiInsight } from '#/api/analytics/extended';

import { generateAiInsightApi, getAiInsightsApi } from '#/api/analytics/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const generating = ref(false);

const { items, load, loading, total, changePage, query } = usePagedQuery<
  AiInsight,
  { keyword?: string; page: number; pageSize: number; type?: string }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getAiInsightsApi,
});

const categoryColors: Record<string, string> = {
  '效率': 'blue',
  '学习': 'green',
  '健康': 'orange',
  '工作': 'purple',
};

async function handleGenerate() {
  generating.value = true;
  try {
    await generateAiInsightApi();
    message.success('洞察生成成功');
    await load();
  } catch {
    message.error('生成失败');
  } finally {
    generating.value = false;
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
  <Page description="AI驱动的智能洞察分析" title="AI洞察">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="洞察数量" :value="total" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="当前页" :value="items.length" /></Card>
      </Col>
    </Row>

    <Card title="AI洞察">
      <template #extra>
        <Button type="primary" :loading="generating" @click="handleGenerate">刷新洞察</Button>
      </template>
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
            <List.Item.Meta :title="item.title" :description="item.content" />
            <Space>
              <Tag v-if="item.category" :color="categoryColors[item.category] || 'default'">
                {{ item.category }}
              </Tag>
              <span class="text-gray-400">{{ item.createdAt }}</span>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
