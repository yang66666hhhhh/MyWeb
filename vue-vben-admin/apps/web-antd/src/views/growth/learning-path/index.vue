<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Alert, Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const paths = ref([
  { id: '1', title: '前端开发工程师', steps: 12, completedSteps: 8, status: '进行中', category: '技术' },
  { id: '2', title: '考研408备考', steps: 20, completedSteps: 7, status: '进行中', category: '学业' },
  { id: '3', title: '英语能力提升', steps: 10, completedSteps: 4, status: '进行中', category: '语言' },
]);

const statusColors: Record<string, string> = {
  '进行中': 'processing',
  '已完成': 'success',
  '待开始': 'default',
};
</script>

<template>
  <Page description="规划和追踪学习路径" title="学习路径">
    <Alert
      class="mb-4"
      message="功能开发中"
      description="后端API正在开发中，当前为模拟数据"
      show-icon
      type="warning"
    />
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="学习路径" :value="paths.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="总步骤" :value="42" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已完成" :value="19" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="完成率" :value="45" suffix="%" /></Card>
      </Col>
    </Row>

    <Card title="学习路径">
      <template #extra><Button type="primary">新建路径</Button></template>
      <List :data-source="paths" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.title">
              <template #description>
                <Space>
                  <Tag :color="statusColors[item.status]">{{ item.status }}</Tag>
                  <Tag>{{ item.category }}</Tag>
                </Space>
              </template>
            </List.Item.Meta>
            <div class="flex items-center gap-4">
              <span>{{ item.completedSteps }}/{{ item.steps }} 步骤</span>
              <div class="w-32">
                <div class="h-2 rounded-full bg-gray-200">
                  <div class="h-full rounded-full bg-blue-500" :style="{ width: `${(item.completedSteps / item.steps) * 100}%` }"></div>
                </div>
              </div>
            </div>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>