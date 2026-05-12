<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Alert, Button, Card, Col, Input, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);
const query = ref('');

const insights = ref([
  { id: '1', title: '工作效率趋势', type: '趋势', value: '上升12%', description: '近一周工作效率相比上周提升12%', priority: '积极' },
  { id: '2', title: '时间分配分析', type: '分析', value: '需优化', description: '会议时间占比过高(35%)，建议减少到20%', priority: '建议' },
  { id: '3', title: '任务完成预测', type: '预测', value: '按时完成', description: '当前进度下，本月目标可按时完成', priority: '正常' },
  { id: '4', title: '学习投入分析', type: '分析', value: '稳定', description: '每日学习时间保持在2小时左右', priority: '正常' },
]);

const typeColors: Record<string, string> = {
  '趋势': 'blue',
  '分析': 'purple',
  '预测': 'green',
};

const priorityColors: Record<string, string> = {
  '积极': 'success',
  '建议': 'warning',
  '正常': 'processing',
};
</script>

<template>
  <Page description="AI驱动的数据洞察分析" title="数据洞察">
    <Alert
      class="mb-4"
      message="功能开发中"
      description="后端API正在开发中，当前为模拟数据"
      show-icon
      type="warning"
    />
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="洞察数量" :value="insights.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="积极信号" :value="1" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="建议优化" :value="1" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="正常指标" :value="2" /></Card>
      </Col>
    </Row>

    <Card title="AI洞察">
      <template #extra>
        <Space>
          <Input v-model:value="query" placeholder="输入分析需求..." style="width: 300px" />
          <Button type="primary" :loading="loading">分析</Button>
        </Space>
      </template>
      <div v-for="insight in insights" :key="insight.id" class="mb-4 p-4 border rounded-lg">
        <div class="flex items-center justify-between mb-2">
          <span class="text-lg font-bold">{{ insight.title }}</span>
          <Space>
            <Tag :color="typeColors[insight.type]">{{ insight.type }}</Tag>
            <Tag :color="priorityColors[insight.priority]">{{ insight.priority }}</Tag>
          </Space>
        </div>
        <div class="text-2xl font-bold mb-2">{{ insight.value }}</div>
        <div class="text-gray-500">{{ insight.description }}</div>
      </div>
    </Card>
  </Page>
</template>
