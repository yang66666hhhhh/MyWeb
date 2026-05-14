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

const projects = ref([
  {
    id: '1',
    name: '客户A系统实施',
    customer: '客户A',
    status: '进行中',
    progress: 65,
    startDate: '2024-01-01',
    endDate: '2024-03-31',
    manager: '张三',
  },
  {
    id: '2',
    name: '客户B数据迁移',
    customer: '客户B',
    status: '待开始',
    progress: 0,
    startDate: '2024-02-01',
    endDate: '2024-04-30',
    manager: '李四',
  },
  {
    id: '3',
    name: '客户C系统升级',
    customer: '客户C',
    status: '已完成',
    progress: 100,
    startDate: '2023-10-01',
    endDate: '2024-01-31',
    manager: '王五',
  },
]);

const columns = [
  { title: '项目名称', dataIndex: 'name', key: 'name' },
  { title: '客户', dataIndex: 'customer', key: 'customer' },
  { title: '状态', dataIndex: 'status', key: 'status' },
  { title: '进度', dataIndex: 'progress', key: 'progress' },
  { title: '开始日期', dataIndex: 'startDate', key: 'startDate' },
  { title: '结束日期', dataIndex: 'endDate', key: 'endDate' },
  { title: '负责人', dataIndex: 'manager', key: 'manager' },
];

const statusColors: Record<string, string> = {
  '进行中': 'blue',
  '待开始': 'default',
  '已完成': 'green',
  '已暂停': 'orange',
};
</script>

<template>
  <Page description="管理实施项目、跟踪进度和协调资源" title="项目看板">
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
          <Statistic title="项目总数" :value="projects.length" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="进行中" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="待开始" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="已完成" :value="1" />
        </Card>
      </Col>
    </Row>

    <Card title="项目列表">
      <template #extra>
        <Button type="primary">新建项目</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="projects"
        :loading="loading"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'status'">
            <Tag :color="statusColors[record.status] || 'default'">
              {{ record.status }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'progress'">
            <div class="flex items-center gap-2">
              <div class="h-2 w-20 rounded-full bg-gray-200">
                <div
                  class="h-full rounded-full bg-blue-500"
                  :style="{ width: `${record.progress}%` }"
                ></div>
              </div>
              <span>{{ record.progress }}%</span>
            </div>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>