<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Alert, Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const insights = ref([
  { id: '1', title: '工作效率洞察', type: '效率', value: '提升15%', description: '本周工作效率相比上周提升15%，建议继续保持', priority: '积极' },
  { id: '2', title: '学习时间分析', type: '学习', value: '稳定', description: '每日学习时间保持在2-3小时，符合预期', priority: '正常' },
  { id: '3', title: '健康提醒', type: '健康', value: '需关注', description: '本周运动时间不足，建议增加运动频率', priority: '建议' },
]);

const typeColors: Record<string, string> = {
  '效率': 'blue',
  '学习': 'green',
  '健康': 'orange',
};

const priorityColors: Record<string, string> = {
  '积极': 'success',
  '正常': 'processing',
  '建议': 'warning',
};
</script>

<template>
  <Page description="AI驱动的智能洞察分析" title="AI洞察">
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
        <Card><Statistic title="建议关注" :value="1" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="正常指标" :value="1" /></Card>
      </Col>
    </Row>

    <Card title="AI洞察">
      <template #extra><Button type="primary">刷新洞察</Button></template>
      <List :data-source="insights" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.title" :description="item.description" />
            <Space>
              <Tag :color="typeColors[item.type]">{{ item.type }}</Tag>
              <Tag :color="priorityColors[item.priority]">{{ item.priority }}</Tag>
              <span class="text-lg font-bold">{{ item.value }}</span>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>