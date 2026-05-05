<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, List, Row, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const goals = ref([
  { id: '1', title: '完成毕业设计', category: '学业', deadline: '2024-05-30', progress: 80, status: '进行中' },
  { id: '2', title: '获得实习offer', category: '职业', deadline: '2024-03-31', progress: 100, status: '已完成' },
  { id: '3', title: '通过英语六级', category: '学业', deadline: '2024-06-15', progress: 60, status: '进行中' },
]);

const statusColors: Record<string, string> = {
  '已完成': 'success',
  '进行中': 'processing',
  '待开始': 'default',
};
</script>

<template>
  <Page description="设定和追踪个人目标" title="目标管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="目标总数" :value="goals.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已完成" :value="1" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="进行中" :value="2" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="完成率" :value="33" suffix="%" /></Card>
      </Col>
    </Row>

    <Card title="我的目标">
      <template #extra><Button type="primary">添加目标</Button></template>
      <List :data-source="goals" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.title" :description="`截止: ${item.deadline}`" />
            <div class="flex items-center gap-4">
              <Tag :color="statusColors[item.status]">{{ item.status }}</Tag>
              <div class="w-32">
                <div class="mb-1 text-right text-sm">{{ item.progress }}%</div>
                <div class="h-2 rounded-full bg-gray-200">
                  <div class="h-full rounded-full bg-blue-500" :style="{ width: `${item.progress}%` }" />
                </div>
              </div>
            </div>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
