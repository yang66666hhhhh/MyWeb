<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const books = ref([
  { id: '1', title: '深入理解TypeScript', author: '某作者', status: '已读完', rating: 5, category: '技术' },
  { id: '2', title: 'Vue.js设计与实现', author: '某作者', status: '阅读中', rating: 4, category: '技术' },
  { id: '3', title: '代码整洁之道', author: 'Robert C. Martin', status: '已读完', rating: 5, category: '编程' },
  { id: '4', title: '人月神话', author: 'Frederick P. Brooks', status: '待阅读', rating: 0, category: '管理' },
]);

const statusColors: Record<string, string> = {
  '已读完': 'success',
  '阅读中': 'processing',
  '待阅读': 'default',
};
</script>

<template>
  <Page description="管理您的阅读清单和书籍笔记" title="阅读清单">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="书籍总数" :value="books.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已读完" :value="2" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="阅读中" :value="1" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="待阅读" :value="1" /></Card>
      </Col>
    </Row>

    <Card title="我的书单">
      <template #extra><Button type="primary">添加书籍</Button></template>
      <List :data-source="books" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.title" :description="`作者: ${item.author}`" />
            <Space>
              <Tag :color="statusColors[item.status]">{{ item.status }}</Tag>
              <Tag>{{ item.category }}</Tag>
              <span v-if="item.rating">⭐ {{ item.rating }}/5</span>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
