<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Alert,
  Button,
  Card,
  Col,
  Row,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

const loading = ref(false);

const issues = ref([
  {
    id: '1',
    title: '修复登录页面样式问题',
    repository: 'vue-vben-admin',
    status: 'open',
    priority: 'high',
    assignee: 'Jack',
    createdAt: '2024-01-15',
  },
  {
    id: '2',
    title: '添加用户导出功能',
    repository: 'dotnet-core-api',
    status: 'in-progress',
    priority: 'medium',
    assignee: 'Lisa',
    createdAt: '2024-01-12',
  },
  {
    id: '3',
    title: '优化数据库查询性能',
    repository: 'dotnet-core-api',
    status: 'resolved',
    priority: 'high',
    assignee: 'Jack',
    createdAt: '2024-01-10',
  },
]);

const columns = [
  { title: '标题', dataIndex: 'title', key: 'title' },
  { title: '仓库', dataIndex: 'repository', key: 'repository' },
  { title: '状态', dataIndex: 'status', key: 'status' },
  { title: '优先级', dataIndex: 'priority', key: 'priority' },
  { title: '负责人', dataIndex: 'assignee', key: 'assignee' },
  { title: '创建时间', dataIndex: 'createdAt', key: 'createdAt' },
];

const statusColors: Record<string, string> = {
  open: 'blue',
  'in-progress': 'orange',
  resolved: 'green',
  closed: 'default',
};

const priorityColors: Record<string, string> = {
  high: 'red',
  medium: 'yellow',
  low: 'green',
};
</script>

<template>
  <Page description="跟踪和管理项目中的问题和任务" title="问题跟踪">
    <Alert
      class="mb-4"
      message="功能开发中"
      description="后端API正在开发中，当前为模拟数据"
      show-icon
      type="warning"
    />
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="待处理" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="进行中" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="已解决" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="总数" :value="3" />
        </Card>
      </Col>
    </Row>

    <Card title="问题列表">
      <template #extra>
        <Button type="primary">新建问题</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="issues"
        :loading="loading"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'status'">
            <Tag :color="statusColors[record.status]">
              {{ record.status }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'priority'">
            <Tag :color="priorityColors[record.priority]">
              {{ record.priority }}
            </Tag>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>