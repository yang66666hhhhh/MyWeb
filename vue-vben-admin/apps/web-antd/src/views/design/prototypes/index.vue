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

const prototypes = ref([
  {
    id: '1',
    name: '移动端 App 原型',
    platform: 'Mobile',
    status: '已完成',
    version: 'v2.1',
    updatedAt: '2024-01-15',
    pages: 24,
  },
  {
    id: '2',
    name: 'Web 管理后台',
    platform: 'Web',
    status: '进行中',
    version: 'v1.5',
    updatedAt: '2024-01-12',
    pages: 18,
  },
  {
    id: '3',
    name: '小程序原型',
    platform: 'Mini Program',
    status: '待审核',
    version: 'v1.0',
    updatedAt: '2024-01-10',
    pages: 12,
  },
]);

const columns = [
  { title: '原型名称', dataIndex: 'name', key: 'name' },
  { title: '平台', dataIndex: 'platform', key: 'platform' },
  { title: '状态', dataIndex: 'status', key: 'status' },
  { title: '版本', dataIndex: 'version', key: 'version' },
  { title: '页面数', dataIndex: 'pages', key: 'pages' },
  { title: '更新时间', dataIndex: 'updatedAt', key: 'updatedAt' },
];

const platformColors: Record<string, string> = {
  Mobile: 'blue',
  Web: 'green',
  'Mini Program': 'orange',
};

const statusColors: Record<string, string> = {
  '已完成': 'green',
  '进行中': 'blue',
  '待审核': 'orange',
};
</script>

<template>
  <Page description="管理产品原型和交互设计" title="原型管理">
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
          <Statistic title="原型总数" :value="prototypes.length" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="已完成" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="进行中" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="总页面数" :value="54" />
        </Card>
      </Col>
    </Row>

    <Card title="原型列表">
      <template #extra>
        <Button type="primary">新建原型</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="prototypes"
        :loading="loading"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'platform'">
            <Tag :color="platformColors[record.platform] || 'default'">
              {{ record.platform }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="statusColors[record.status] || 'default'">
              {{ record.status }}
            </Tag>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>