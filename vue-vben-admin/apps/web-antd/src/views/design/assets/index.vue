<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Alert,
  Button,
  Card,
  Col,
  Image,
  Row,
  Space,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

const loading = ref(false);

const assets = ref([
  {
    id: '1',
    name: 'Logo 设计',
    type: '图片',
    format: 'SVG',
    size: '120 KB',
    updatedAt: '2024-01-15',
    tags: ['品牌', 'Logo'],
  },
  {
    id: '2',
    name: 'UI 组件库',
    type: '设计系统',
    format: 'Figma',
    size: '2.5 MB',
    updatedAt: '2024-01-12',
    tags: ['组件', 'UI'],
  },
  {
    id: '3',
    name: '图标集',
    type: '图标',
    format: 'PNG',
    size: '500 KB',
    updatedAt: '2024-01-10',
    tags: ['图标', '资源'],
  },
]);

const columns = [
  { title: '资源名称', dataIndex: 'name', key: 'name' },
  { title: '类型', dataIndex: 'type', key: 'type' },
  { title: '格式', dataIndex: 'format', key: 'format' },
  { title: '大小', dataIndex: 'size', key: 'size' },
  { title: '更新时间', dataIndex: 'updatedAt', key: 'updatedAt' },
  { title: '标签', dataIndex: 'tags', key: 'tags' },
];

const typeColors: Record<string, string> = {
  '图片': 'blue',
  '设计系统': 'purple',
  '图标': 'green',
};
</script>

<template>
  <Page description="管理设计资源、组件库和品牌资产" title="设计资产">
    <Alert
      class="mb-4"
      message="功能开发中"
      description="后端API正在开发中，当前为模拟数据"
      show-icon
      type="warning"
    />
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="资源总数" :value="assets.length" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="图片资源" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="设计系统" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="图标资源" :value="1" />
        </Card>
      </Col>
    </Row>

    <Card title="设计资产列表">
      <template #extra>
        <Button type="primary">上传资源</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="assets"
        :loading="loading"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'type'">
            <Tag :color="typeColors[record.type] || 'default'">
              {{ record.type }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'tags'">
            <Space>
              <Tag v-for="tag in record.tags" :key="tag">{{ tag }}</Tag>
            </Space>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>