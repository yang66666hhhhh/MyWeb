<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, Row, Space, Statistic, Table, Tag } from 'ant-design-vue';

const loading = ref(false);

const plans = ref([
  { id: '1', week: '2024-W03', goals: '完成用户模块开发', tasks: 5, completed: 3, status: '进行中' },
  { id: '2', week: '2024-W02', goals: '完成认证系统', tasks: 4, completed: 4, status: '已完成' },
  { id: '3', week: '2024-W01', goals: '项目初始化和架构设计', tasks: 6, completed: 6, status: '已完成' },
]);

const columns = [
  { title: '周次', dataIndex: 'week', key: 'week' },
  { title: '目标', dataIndex: 'goals', key: 'goals' },
  { title: '任务数', dataIndex: 'tasks', key: 'tasks' },
  { title: '已完成', dataIndex: 'completed', key: 'completed' },
  { title: '状态', dataIndex: 'status', key: 'status' },
];

const statusColors: Record<string, string> = {
  '进行中': 'processing',
  '已完成': 'success',
};
</script>

<template>
  <Page description="规划和管理每周工作" title="周计划">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本周目标" :value="5" suffix="个" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已完成" :value="3" suffix="个" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="完成率" :value="60" suffix="%" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本周工时" :value="32" suffix="小时" /></Card>
      </Col>
    </Row>

    <Card title="周计划列表">
      <template #extra><Button type="primary">新建周计划</Button></template>
      <Table :columns="columns" :data-source="plans" :loading="loading" row-key="id">
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'status'">
            <Tag :color="statusColors[record.status]">{{ record.status }}</Tag>
          </template>
          <template v-else-if="column.key === 'completed'">
            <span>{{ record.completed }}/{{ record.tasks }}</span>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>
