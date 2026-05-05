<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const sleepRecords = ref([
  { id: '1', date: '2024-01-15', bedtime: '23:00', wakeTime: '07:00', duration: 8, quality: '良好' },
  { id: '2', date: '2024-01-14', bedtime: '00:30', wakeTime: '07:30', duration: 7, quality: '一般' },
  { id: '3', date: '2024-01-13', bedtime: '22:30', wakeTime: '06:30', duration: 8, quality: '良好' },
  { id: '4', date: '2024-01-12', bedtime: '01:00', wakeTime: '08:00', duration: 7, quality: '较差' },
]);

const qualityColors: Record<string, string> = {
  '良好': 'success',
  '一般': 'processing',
  '较差': 'warning',
};
</script>

<template>
  <Page description="追踪睡眠质量和规律" title="睡眠追踪">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="记录天数" :value="sleepRecords.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均时长" :value="7.5" suffix="小时" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="最佳睡眠" value="良好" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="规律性" value="一般" /></Card>
      </Col>
    </Row>

    <Card title="睡眠记录">
      <template #extra><Button type="primary">记录睡眠</Button></template>
      <List :data-source="sleepRecords" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.date" :description="`就寝: ${item.bedtime} | 起床: ${item.wakeTime}`" />
            <Space>
              <Tag :color="qualityColors[item.quality]">{{ item.quality }}</Tag>
              <span>{{ item.duration }}小时</span>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
