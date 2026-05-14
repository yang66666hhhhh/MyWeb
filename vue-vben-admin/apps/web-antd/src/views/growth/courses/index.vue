<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Alert, Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const courses = ref([
  { id: '1', name: 'Vue 3 高级编程', platform: 'Udemy', progress: 75, status: '学习中', category: '前端' },
  { id: '2', name: '数据结构与算法', platform: 'Coursera', progress: 40, status: '学习中', category: '计算机科学' },
  { id: '3', name: '英语口语', platform: '多邻国', progress: 60, status: '学习中', category: '语言' },
]);

const statusColors: Record<string, string> = {
  '学习中': 'processing',
  '已完成': 'success',
  '待开始': 'default',
};
</script>

<template>
  <Page description="管理在线课程学习进度" title="课程管理">
    <Alert
      class="mb-4"
      message="功能开发中"
      description="后端API正在开发中，当前为模拟数据"
      show-icon
      type="warning"
    />
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="课程总数" :value="courses.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="学习中" :value="3" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已完成" :value="0" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均进度" :value="58" suffix="%" /></Card>
      </Col>
    </Row>

    <Card title="我的课程">
      <template #extra><Button type="primary">添加课程</Button></template>
      <List :data-source="courses" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.name" :description="`平台: ${item.platform}`" />
            <Space>
              <Tag :color="statusColors[item.status]">{{ item.status }}</Tag>
              <Tag>{{ item.category }}</Tag>
              <div class="w-32">
                <div class="mb-1 text-right text-sm">{{ item.progress }}%</div>
                <div class="h-2 rounded-full bg-gray-200">
                  <div class="h-full rounded-full bg-blue-500" :style="{ width: `${item.progress}%` }"></div>
                </div>
              </div>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>