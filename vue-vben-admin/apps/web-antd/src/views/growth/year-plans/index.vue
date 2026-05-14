<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Alert, Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const goals = ref([
  { id: '1', title: '掌握Vue 3 + TypeScript', category: '技术', deadline: '2024-06-30', progress: 75, status: '进行中' },
  { id: '2', title: '通过考研初试', category: '学业', deadline: '2024-12-31', progress: 40, status: '进行中' },
  { id: '3', title: '体重降到70kg', category: '健康', deadline: '2024-08-31', progress: 60, status: '进行中' },
  { id: '4', title: '读完12本书', category: '成长', deadline: '2024-12-31', progress: 25, status: '进行中' },
]);

const categoryColors: Record<string, string> = {
  '技术': 'blue',
  '学业': 'green',
  '健康': 'orange',
  '成长': 'purple',
};
</script>

<template>
  <Page description="设定和追踪年度目标" title="年度计划">
    <Alert
      class="mb-4"
      message="功能开发中"
      description="后端API正在开发中，当前为模拟数据"
      show-icon
      type="warning"
    />
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="年度目标" :value="goals.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="进行中" :value="4" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已完成" :value="0" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均进度" :value="50" suffix="%" /></Card>
      </Col>
    </Row>

    <Card title="年度目标">
      <template #extra><Button type="primary">添加目标</Button></template>
      <List :data-source="goals" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.title">
              <template #description>
                <div class="w-32">
                  <div class="mb-1 text-right text-sm">{{ item.progress }}%</div>
                  <div class="h-2 rounded-full bg-gray-200">
                    <div class="h-full rounded-full bg-blue-500" :style="{ width: `${item.progress}%` }"></div>
                  </div>
                </div>
              </template>
            </List.Item.Meta>
            <Space direction="vertical" size="small">
              <Tag :color="categoryColors[item.category]">{{ item.category }}</Tag>
              <span class="text-gray-400 text-sm">截止: {{ item.deadline }}</span>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
