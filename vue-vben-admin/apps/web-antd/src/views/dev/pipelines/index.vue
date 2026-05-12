<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Alert,
  Button,
  Card,
  Col,
  Row,
  Space,
  Statistic,
  Steps,
  Table,
  Tag,
} from 'ant-design-vue';

const loading = ref(false);

const pipelines = ref([
  {
    id: '1',
    name: 'CI/CD Pipeline',
    repository: 'vue-vben-admin',
    status: 'success',
    branch: 'main',
    duration: '2m 30s',
    triggeredAt: '2024-01-15 14:30',
  },
  {
    id: '2',
    name: 'Build & Test',
    repository: 'dotnet-core-api',
    status: 'running',
    branch: 'develop',
    duration: '1m 15s',
    triggeredAt: '2024-01-15 14:25',
  },
  {
    id: '3',
    name: 'Deploy to Staging',
    repository: 'dotnet-core-api',
    status: 'failed',
    branch: 'release/1.0',
    duration: '3m 45s',
    triggeredAt: '2024-01-15 14:20',
  },
]);

const columns = [
  { title: '流水线名称', dataIndex: 'name', key: 'name' },
  { title: '仓库', dataIndex: 'repository', key: 'repository' },
  { title: '状态', dataIndex: 'status', key: 'status' },
  { title: '分支', dataIndex: 'branch', key: 'branch' },
  { title: '耗时', dataIndex: 'duration', key: 'duration' },
  { title: '触发时间', dataIndex: 'triggeredAt', key: 'triggeredAt' },
];

const statusColors: Record<string, string> = {
  success: 'green',
  running: 'blue',
  failed: 'red',
  pending: 'default',
};

const statusIcons: Record<string, string> = {
  success: '✓',
  running: '⟳',
  failed: '✗',
  pending: '○',
};
</script>

<template>
  <Page description="查看和管理 CI/CD 流水线" title="流水线">
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
          <Statistic title="成功" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="运行中" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="失败" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="总流水线" :value="3" />
        </Card>
      </Col>
    </Row>

    <Card title="流水线列表">
      <template #extra>
        <Button type="primary">新建流水线</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="pipelines"
        :loading="loading"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'status'">
            <Space>
              <span>{{ statusIcons[record.status] }}</span>
              <Tag :color="statusColors[record.status]">
                {{ record.status }}
              </Tag>
            </Space>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>