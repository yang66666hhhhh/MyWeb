<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Alert, Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const moods = ref([
  { id: '1', date: '2024-01-15', mood: '开心', energy: 8, note: '完成了一个重要项目' },
  { id: '2', date: '2024-01-14', mood: '平静', energy: 6, note: '正常工作日' },
  { id: '3', date: '2024-01-13', mood: '疲惫', energy: 4, note: '加班到很晚' },
  { id: '4', date: '2024-01-12', mood: '兴奋', energy: 9, note: '学到了新技术' },
]);

const moodColors: Record<string, string> = {
  '开心': 'green',
  '平静': 'blue',
  '疲惫': 'orange',
  '兴奋': 'purple',
  '焦虑': 'red',
};
</script>

<template>
  <Page description="追踪和分析您的情绪变化" title="心情追踪">
    <Alert
      class="mb-4"
      message="功能开发中"
      description="后端API正在开发中，当前为模拟数据"
      show-icon
      type="warning"
    />
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="记录天数" :value="moods.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均能量" :value="7" suffix="/10" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="最常见心情" value="开心" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="连续记录" :value="4" suffix="天" /></Card>
      </Col>
    </Row>

    <Card title="心情记录">
      <template #extra><Button type="primary">记录心情</Button></template>
      <List :data-source="moods" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.date" :description="item.note" />
            <Space>
              <Tag :color="moodColors[item.mood]">{{ item.mood }}</Tag>
              <span>能量: {{ item.energy }}/10</span>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
