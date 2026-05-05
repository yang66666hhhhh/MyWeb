<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const skills = ref([
  { id: '1', name: 'Vue 3', category: '前端', level: '高级', progress: 90, lastPracticed: '2024-01-15' },
  { id: '2', name: 'TypeScript', category: '前端', level: '中级', progress: 75, lastPracticed: '2024-01-14' },
  { id: '3', name: 'ASP.NET Core', category: '后端', level: '中级', progress: 70, lastPracticed: '2024-01-13' },
  { id: '4', name: 'MySQL', category: '数据库', level: '中级', progress: 65, lastPracticed: '2024-01-12' },
  { id: '5', name: 'Docker', category: 'DevOps', level: '初级', progress: 40, lastPracticed: '2024-01-10' },
]);

const categoryColors: Record<string, string> = {
  '前端': 'blue',
  '后端': 'green',
  '数据库': 'orange',
  'DevOps': 'purple',
};

const levelColors: Record<string, string> = {
  '初级': 'default',
  '中级': 'processing',
  '高级': 'success',
};
</script>

<template>
  <Page description="管理和追踪您的技能成长" title="技能管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="技能总数" :value="skills.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="高级技能" :value="1" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="中级技能" :value="3" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均进度" :value="68" suffix="%" /></Card>
      </Col>
    </Row>

    <Card title="我的技能">
      <template #extra><Button type="primary">添加技能</Button></template>
      <List :data-source="skills" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.name">
              <template #description>
                <Space>
                  <Tag :color="categoryColors[item.category]">{{ item.category }}</Tag>
                  <Tag :color="levelColors[item.level]">{{ item.level }}</Tag>
                </Space>
              </template>
            </List.Item.Meta>
            <div class="flex items-center gap-4">
              <div class="w-32">
                <div class="mb-1 text-right text-sm">{{ item.progress }}%</div>
                <div class="h-2 rounded-full bg-gray-200">
                  <div class="h-full rounded-full bg-blue-500" :style="{ width: `${item.progress}%` }" />
                </div>
              </div>
              <span class="text-gray-400">{{ item.lastPracticed }}</span>
            </div>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
