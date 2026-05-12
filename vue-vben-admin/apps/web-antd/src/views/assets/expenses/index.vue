<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Alert, Button, Card, Col, Row, Statistic, Table, Tag } from 'ant-design-vue';

const loading = ref(false);

const expenses = ref([
  { id: '1', date: '2024-01-15', item: '房租', amount: 3000, category: '住房', note: '月租' },
  { id: '2', date: '2024-01-14', item: '餐饮', amount: 150, category: '餐饮', note: '午餐' },
  { id: '3', date: '2024-01-13', item: '交通', amount: 50, category: '交通', note: '地铁充值' },
  { id: '4', date: '2024-01-12', item: '购物', amount: 200, category: '购物', note: '日用品' },
]);

const columns = [
  { title: '日期', dataIndex: 'date', key: 'date' },
  { title: '项目', dataIndex: 'item', key: 'item' },
  { title: '金额', dataIndex: 'amount', key: 'amount' },
  { title: '分类', dataIndex: 'category', key: 'category' },
  { title: '备注', dataIndex: 'note', key: 'note' },
];

const categoryColors: Record<string, string> = {
  '住房': 'red',
  '餐饮': 'orange',
  '交通': 'blue',
  '购物': 'purple',
};
</script>

<template>
  <Page description="记录和管理支出" title="支出管理">
    <Alert
      class="mb-4"
      message="功能开发中"
      description="后端API正在开发中，当前为模拟数据"
      show-icon
      type="warning"
    />
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月支出" prefix="¥" :value="3400" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="住房" prefix="¥" :value="3000" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="餐饮" prefix="¥" :value="150" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="其他" prefix="¥" :value="250" /></Card>
      </Col>
    </Row>

    <Card title="支出记录">
      <template #extra><Button type="primary">记录支出</Button></template>
      <Table :columns="columns" :data-source="expenses" :loading="loading" row-key="id">
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'amount'">
            <span class="text-red-500">-¥{{ record.amount.toLocaleString() }}</span>
          </template>
          <template v-else-if="column.key === 'category'">
            <Tag :color="categoryColors[record.category]">{{ record.category }}</Tag>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>