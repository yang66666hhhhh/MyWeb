<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const sessions = ref([
  { id: '1', title: '408数据结构答疑', messages: 12, lastActive: '2024-01-15 14:30', topic: '学习' },
  { id: '2', title: 'Vue 3 组件设计讨论', messages: 8, lastActive: '2024-01-15 10:20', topic: '技术' },
  { id: '3', title: '工作计划优化建议', messages: 15, lastActive: '2024-01-14 16:45', topic: '工作' },
]);

const topicColors: Record<string, string> = {
  '学习': 'blue',
  '技术': 'green',
  '工作': 'orange',
};
</script>

<template>
  <Page description="基于知识库的智能问答" title="知识问答">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="问答会话" :value="sessions.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="总消息数" :value="35" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="知识库文章" :value="24" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="今日问答" :value="2" /></Card>
      </Col>
    </Row>

    <Card title="问答会话">
      <template #extra><Button type="primary">新建会话</Button></template>
      <List :data-source="sessions" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.title" :description="`最后活跃: ${item.lastActive}`" />
            <Space>
              <Tag :color="topicColors[item.topic]">{{ item.topic }}</Tag>
              <span>{{ item.messages }} 条消息</span>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
