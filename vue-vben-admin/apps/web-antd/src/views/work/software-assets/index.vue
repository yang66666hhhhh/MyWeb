<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, Row, Space, Statistic, Table, Tag } from 'ant-design-vue';

const loading = ref(false);

const assets = ref([
  { id: '1', name: 'Visual Studio Code', type: 'IDE', license: '免费', version: '1.85', status: '使用中', assignedTo: 'Jack' },
  { id: '2', name: 'JetBrains Rider', type: 'IDE', license: '付费', version: '2023.3', status: '使用中', assignedTo: 'Jack' },
  { id: '3', name: 'Figma', type: '设计工具', license: '付费', version: '最新', status: '使用中', assignedTo: 'Lisa' },
  { id: '4', name: 'Postman', type: 'API工具', license: '免费', version: '10.x', status: '可用', assignedTo: '' },
]);

const columns = [
  { title: '软件名称', dataIndex: 'name', key: 'name' },
  { title: '类型', dataIndex: 'type', key: 'type' },
  { title: '许可', dataIndex: 'license', key: 'license' },
  { title: '版本', dataIndex: 'version', key: 'version' },
  { title: '状态', dataIndex: 'status', key: 'status' },
  { title: '使用人', dataIndex: 'assignedTo', key: 'assignedTo' },
];

const statusColors: Record<string, string> = {
  '使用中': 'processing',
  '可用': 'success',
  '已过期': 'error',
};

const licenseColors: Record<string, string> = {
  '免费': 'green',
  '付费': 'orange',
};
</script>

<template>
  <Page description="管理团队软件资产和许可证" title="软件资产">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="软件总数" :value="assets.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="使用中" :value="3" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="付费许可" :value="2" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="可用" :value="1" /></Card>
      </Col>
    </Row>

    <Card title="软件资产列表">
      <template #extra><Button type="primary">添加软件</Button></template>
      <Table :columns="columns" :data-source="assets" :loading="loading" row-key="id">
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'status'">
            <Tag :color="statusColors[record.status]">{{ record.status }}</Tag>
          </template>
          <template v-else-if="column.key === 'license'">
            <Tag :color="licenseColors[record.license]">{{ record.license }}</Tag>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>
