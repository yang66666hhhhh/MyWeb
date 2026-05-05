<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, Row, Statistic, Table, Tag } from 'ant-design-vue';

const loading = ref(false);

const investments = ref([
  { id: '1', name: '沪深300ETF', type: '基金', amount: 20000, currentValue: 21500, return: 7.5, status: '持有中' },
  { id: '2', name: '余额宝', type: '货币基金', amount: 10000, currentValue: 10050, return: 0.5, status: '持有中' },
  { id: '3', name: '定期存款', type: '存款', amount: 50000, currentValue: 50000, return: 2.5, status: '持有中' },
]);

const columns = [
  { title: '名称', dataIndex: 'name', key: 'name' },
  { title: '类型', dataIndex: 'type', key: 'type' },
  { title: '投入金额', dataIndex: 'amount', key: 'amount' },
  { title: '当前价值', dataIndex: 'currentValue', key: 'currentValue' },
  { title: '收益率', dataIndex: 'return', key: 'return' },
  { title: '状态', dataIndex: 'status', key: 'status' },
];

const typeColors: Record<string, string> = {
  '基金': 'blue',
  '货币基金': 'green',
  '存款': 'orange',
};
</script>

<template>
  <Page description="管理投资组合" title="投资管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="投资总额" :value="80000" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="当前价值" :value="81550" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="总收益" :value="1550" prefix="¥" valueStyle="color: #3f8600" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="收益率" :value="1.94" suffix="%" valueStyle="color: #3f8600" /></Card>
      </Col>
    </Row>

    <Card title="投资组合">
      <template #extra><Button type="primary">添加投资</Button></template>
      <Table :columns="columns" :data-source="investments" :loading="loading" row-key="id">
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'type'">
            <Tag :color="typeColors[record.type]">{{ record.type }}</Tag>
          </template>
          <template v-else-if="column.key === 'return'">
            <span :class="record.return >= 0 ? 'text-green-500' : 'text-red-500'">
              {{ record.return >= 0 ? '+' : '' }}{{ record.return }}%
            </span>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>
