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
  Table,
  Tag,
} from 'ant-design-vue';

const loading = ref(false);

const customers = ref([
  {
    id: '1',
    name: '客户A',
    industry: '制造业',
    contact: '张经理',
    phone: '13800138001',
    email: 'customerA@example.com',
    status: '活跃',
    projects: 2,
  },
  {
    id: '2',
    name: '客户B',
    industry: '金融业',
    contact: '李总监',
    phone: '13800138002',
    email: 'customerB@example.com',
    status: '活跃',
    projects: 1,
  },
  {
    id: '3',
    name: '客户C',
    industry: '零售业',
    contact: '王主管',
    phone: '13800138003',
    email: 'customerC@example.com',
    status: '潜在',
    projects: 0,
  },
]);

const columns = [
  { title: '客户名称', dataIndex: 'name', key: 'name' },
  { title: '行业', dataIndex: 'industry', key: 'industry' },
  { title: '联系人', dataIndex: 'contact', key: 'contact' },
  { title: '电话', dataIndex: 'phone', key: 'phone' },
  { title: '邮箱', dataIndex: 'email', key: 'email' },
  { title: '状态', dataIndex: 'status', key: 'status' },
  { title: '项目数', dataIndex: 'projects', key: 'projects' },
];

const statusColors: Record<string, string> = {
  '活跃': 'green',
  '潜在': 'blue',
  '已流失': 'red',
};

const industryColors: Record<string, string> = {
  '制造业': 'blue',
  '金融业': 'purple',
  '零售业': 'orange',
};
</script>

<template>
  <Page description="管理客户信息、联系人和合作项目" title="客户管理">
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
          <Statistic title="客户总数" :value="customers.length" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="活跃客户" :value="2" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="潜在客户" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="总项目数" :value="3" />
        </Card>
      </Col>
    </Row>

    <Card title="客户列表">
      <template #extra>
        <Button type="primary">添加客户</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="customers"
        :loading="loading"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'status'">
            <Tag :color="statusColors[record.status] || 'default'">
              {{ record.status }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'industry'">
            <Tag :color="industryColors[record.industry] || 'default'">
              {{ record.industry }}
            </Tag>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>