<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Alert, Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const workouts = ref([
  { id: '1', date: '2024-01-15', type: '跑步', duration: 30, calories: 350, note: '5公里' },
  { id: '2', date: '2024-01-14', type: '力量训练', duration: 45, calories: 280, note: '上肢训练' },
  { id: '3', date: '2024-01-13', type: '瑜伽', duration: 60, calories: 200, note: '放松恢复' },
  { id: '4', date: '2024-01-12', type: '游泳', duration: 40, calories: 400, note: '1500米' },
]);

const typeColors: Record<string, string> = {
  '跑步': 'blue',
  '力量训练': 'red',
  '瑜伽': 'purple',
  '游泳': 'cyan',
};
</script>

<template>
  <Page description="记录和管理您的健身活动" title="健身管理">
    <Alert
      class="mb-4"
      message="功能开发中"
      description="后端API正在开发中，当前为模拟数据"
      show-icon
      type="warning"
    />
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月运动" :value="workouts.length" suffix="次" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="总时长" :value="175" suffix="分钟" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="消耗热量" :value="1230" suffix="卡" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="连续天数" :value="4" suffix="天" /></Card>
      </Col>
    </Row>

    <Card title="运动记录">
      <template #extra><Button type="primary">记录运动</Button></template>
      <List :data-source="workouts" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="`${item.type} - ${item.date}`" :description="item.note" />
            <Space>
              <Tag :color="typeColors[item.type]">{{ item.type }}</Tag>
              <span>{{ item.duration }}分钟</span>
              <span>{{ item.calories }}卡</span>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
