<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, Progress, Row, Statistic, Table, Tag } from 'ant-design-vue';

const loading = ref(false);

const budgets = ref([
  { id: '1', category: '住房', budget: 3000, spent: 3000, status: '已用完' },
  { id: '2', category: '餐饮', budget: 2000, spent: 1500, status: '正常' },
  { id: '3', category: '交通', budget: 500, spent: 350, status: '正常' },
  { id: '4', category: '购物', budget: 1000, spent: 800, status: '接近' },
  { id: '5', category: '娱乐', budget: 500, spent: 200, status: '正常' },
]);

const columns = [
  { title: '分类', dataIndex: 'category', key: 'category' },
  { title: '预算', dataIndex: 'budget', key: 'budget' },
  { title: '已花费', dataIndex: 'spent', key: 'spent' },
  { title: '进度', dataIndex: 'progress', key: 'progress' },
  { title: '状态', dataIndex: 'status', key: 'status' },
];

const statusColors: Record<string, string> = {
  '正常': 'success',
  '接近': 'warning',
  '已用完': 'error',
};
</script>

<template>
  <Page description="设定和追踪预算" title="预算管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="总预算" :value="7000" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已花费" :value="5850" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="剩余" :value="1150" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="使用率" :value="84" suffix="%" /></Card>
      </Col>
    </Row>

    <Card title="预算列表">
      <template #extra><Button type="primary">设置预算</Button></template>
      <Table :columns="columns" :data-source="budgets" :loading="loading" row-key="id">
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'progress'">
            <Progress :percent="Math.round((record.spent / record.budget) * 100)" :status="record.spent > record.budget ? 'exception' : 'active'" />
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="statusColors[record.status]">{{ record.status }}</Tag>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>
