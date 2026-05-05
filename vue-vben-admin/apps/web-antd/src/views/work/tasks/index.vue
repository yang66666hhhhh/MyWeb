<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const tasks = ref([
  { id: '1', title: '完成用户管理模块', project: 'WebSite', priority: '高', status: '进行中', dueDate: '2024-01-20', assignee: 'Jack' },
  { id: '2', title: '修复登录bug', project: 'WebSite', priority: '高', status: '待处理', dueDate: '2024-01-18', assignee: 'Jack' },
  { id: '3', title: '编写API文档', project: 'WebSite', priority: '中', status: '进行中', dueDate: '2024-01-25', assignee: 'Lisa' },
  { id: '4', title: '数据库优化', project: 'WebSite', priority: '低', status: '已完成', dueDate: '2024-01-15', assignee: 'Jack' },
]);

const statusColors: Record<string, string> = {
  '待处理': 'default',
  '进行中': 'processing',
  '已完成': 'success',
  '已取消': 'error',
};

const priorityColors: Record<string, string> = {
  '高': 'red',
  '中': 'orange',
  '低': 'green',
};
</script>

<template>
  <Page description="管理和追踪工作任务" title="工作任务">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="任务总数" :value="tasks.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="待处理" :value="1" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="进行中" :value="2" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已完成" :value="1" /></Card>
      </Col>
    </Row>

    <Card title="任务列表">
      <template #extra><Button type="primary">新建任务</Button></template>
      <List :data-source="tasks" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.title" :description="`项目: ${item.project} | 负责人: ${item.assignee}`" />
            <Space>
              <Tag :color="priorityColors[item.priority]">{{ item.priority }}</Tag>
              <Tag :color="statusColors[item.status]">{{ item.status }}</Tag>
              <span class="text-gray-400">{{ item.dueDate }}</span>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
