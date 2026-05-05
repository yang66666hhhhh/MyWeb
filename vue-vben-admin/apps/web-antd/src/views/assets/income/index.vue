<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, Row, Statistic, Table, Tag } from 'ant-design-vue';

const loading = ref(false);

const income = ref([
  { id: '1', date: '2024-01-15', source: '工资', amount: 15000, category: '固定收入', note: '月工资' },
  { id: '2', date: '2024-01-10', source: '兼职', amount: 3000, category: '兼职收入', note: '周末兼职' },
  { id: '3', date: '2024-01-05', source: '投资收益', amount: 500, category: '投资', note: '基金分红' },
]);

const columns = [
  { title: '日期', dataIndex: 'date', key: 'date' },
  { title: '来源', dataIndex: 'source', key: 'source' },
  { title: '金额', dataIndex: 'amount', key: 'amount' },
  { title: '分类', dataIndex: 'category', key: 'category' },
  { title: '备注', dataIndex: 'note', key: 'note' },
];

const categoryColors: Record<string, string> = {
  '固定收入': 'blue',
  '兼职收入': 'green',
  '投资': 'purple',
};
</script>

<template>
  <Page description="记录和管理收入" title="收入管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月收入" :value="18500" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="固定收入" :value="15000" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="兼职收入" :value="3000" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="投资收益" :value="500" prefix="¥" /></Card>
      </Col>
    </Row>

    <Card title="收入记录">
      <template #extra><Button type="primary">记录收入</Button></template>
      <Table :columns="columns" :data-source="income" :loading="loading" row-key="id">
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'amount'">
            <span class="text-green-500">+¥{{ record.amount.toLocaleString() }}</span>
          </template>
          <template v-else-if="column.key === 'category'">
            <Tag :color="categoryColors[record.category]">{{ record.category }}</Tag>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>
