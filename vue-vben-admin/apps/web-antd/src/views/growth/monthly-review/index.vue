<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const reviews = ref([
  { id: '1', month: '2024-01', highlights: '完成3个项目，学习Vue 3', challenges: '时间管理待改进', rating: 4 },
  { id: '2', month: '2023-12', highlights: '掌握TypeScript，建立健身习惯', challenges: '工作压力大', rating: 3 },
  { id: '3', month: '2023-11', highlights: '开始学习新技术栈', challenges: '拖延症', rating: 3 },
]);
</script>

<template>
  <Page description="每月回顾成长和规划" title="月度复盘">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已复盘" :value="reviews.length" suffix="个月" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均评分" :value="3.3" suffix="/5" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="最佳月份" value="2024-01" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="待复盘" :value="1" suffix="个月" /></Card>
      </Col>
    </Row>

    <Card title="月度复盘记录">
      <template #extra><Button type="primary">新建复盘</Button></template>
      <List :data-source="reviews" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.month">
              <template #description>
                <div>
                  <div class="mb-1"><strong>亮点:</strong> {{ item.highlights }}</div>
                  <div><strong>挑战:</strong> {{ item.challenges }}</div>
                </div>
              </template>
            </List.Item.Meta>
            <Space>
              <span>⭐ {{ item.rating }}/5</span>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
