<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const resources = ref([
  { id: '1', name: '云服务器', type: '基础设施', provider: '阿里云', cost: 200, status: '使用中', renewDate: '2024-02-15' },
  { id: '2', name: '域名', type: '网络', provider: '万网', cost: 60, status: '使用中', renewDate: '2024-12-31' },
  { id: '3', name: 'GitHub Pro', type: '开发工具', provider: 'GitHub', cost: 40, status: '使用中', renewDate: '2024-06-01' },
  { id: '4', name: 'Figma', type: '设计工具', provider: 'Figma', cost: 100, status: '使用中', renewDate: '2024-03-15' },
]);

const typeColors: Record<string, string> = {
  '基础设施': 'red',
  '网络': 'blue',
  '开发工具': 'green',
  '设计工具': 'purple',
};
</script>

<template>
  <Page description="管理订阅和资源" title="资源管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="资源总数" :value="resources.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="月费用" :value="400" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="年费用" :value="4800" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="即将到期" :value="1" /></Card>
      </Col>
    </Row>

    <Card title="资源列表">
      <template #extra><Button type="primary">添加资源</Button></template>
      <List :data-source="resources" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.name" :description="`提供商: ${item.provider} | 到期: ${item.renewDate}`" />
            <Space>
              <Tag :color="typeColors[item.type]">{{ item.type }}</Tag>
              <Tag color="success">{{ item.status }}</Tag>
              <span>¥{{ item.cost }}/月</span>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
