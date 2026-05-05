<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, List, Row, Space, Statistic, Tag } from 'ant-design-vue';

const loading = ref(false);

const files = ref([
  { id: '1', name: '项目需求文档.docx', type: '文档', size: '2.5MB', uploadedBy: 'Jack', uploadedAt: '2024-01-15', category: '需求' },
  { id: '2', name: '系统架构图.png', type: '图片', size: '1.2MB', uploadedBy: 'Jack', uploadedAt: '2024-01-14', category: '设计' },
  { id: '3', name: 'API接口文档.md', type: '文档', size: '156KB', uploadedBy: 'Lisa', uploadedAt: '2024-01-13', category: '技术' },
  { id: '4', name: '测试报告.xlsx', type: '表格', size: '890KB', uploadedBy: 'Jack', uploadedAt: '2024-01-12', category: '测试' },
]);

const typeColors: Record<string, string> = {
  '文档': 'blue',
  '图片': 'green',
  '表格': 'orange',
  '代码': 'purple',
};

const categoryColors: Record<string, string> = {
  '需求': 'blue',
  '设计': 'purple',
  '技术': 'cyan',
  '测试': 'green',
};
</script>

<template>
  <Page description="管理项目文件和文档" title="文件管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="文件总数" :value="files.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="文档" :value="2" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="图片" :value="1" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="总大小" value="4.7MB" /></Card>
      </Col>
    </Row>

    <Card title="文件列表">
      <template #extra><Button type="primary">上传文件</Button></template>
      <List :data-source="files" :loading="loading">
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta :title="item.name" :description="`上传者: ${item.uploadedBy} | ${item.uploadedAt}`" />
            <Space>
              <Tag :color="typeColors[item.type]">{{ item.type }}</Tag>
              <Tag :color="categoryColors[item.category]">{{ item.category }}</Tag>
              <span class="text-gray-400">{{ item.size }}</span>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>
  </Page>
</template>
