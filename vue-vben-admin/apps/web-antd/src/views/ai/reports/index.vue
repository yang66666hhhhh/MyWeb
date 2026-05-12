<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Alert, Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const reports = ref([
  { id: '1', title: '2024年1月工作周报', type: '周报', period: '2024-W03', status: '已生成', createdAt: '2024-01-15' },
  { id: '2', title: '2023年度成长报告', type: '年报', period: '2023', status: '已生成', createdAt: '2024-01-01' },
  { id: '3', title: 'Q4 季度分析报告', type: '季报', period: '2023-Q4', status: '已生成', createdAt: '2024-01-05' },
]);

const typeColors: Record<string, string> = {
  '周报': 'blue',
  '月报': 'green',
  '季报': 'orange',
  '年报': 'purple',
};
</script>

<template>
  <Page description="AI生成的工作和成长报告" title="AI报告">
    <Alert
      class="mb-4"
      message="功能开发中"
      description="后端API正在开发中，当前为模拟数据"
      show-icon
      type="warning"
    />
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="报告总数" :value="reports.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="周报" :value="1" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="季报" :value="1" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="年报" :value="1" /></Card>
      </Col>
    </Row>

    <Card title="AI报告列表">
      <template #extra><Button type="primary">生成报告</Button></template>
      <List :data-source="reports" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.title" :description="`周期: ${item.period} | 生成时间: ${item.createdAt}`" />
            <Space>
              <Tag :color="typeColors[item.type]">{{ item.type }}</Tag>
              <Tag color="success">{{ item.status }}</Tag>
              <Button type="link">查看</Button>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
