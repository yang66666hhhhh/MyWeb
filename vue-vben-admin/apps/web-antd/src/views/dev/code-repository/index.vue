<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Alert, Button, Card, Col, Row, Statistic, Table, Tag } from 'ant-design-vue';

const loading = ref(false);

const repositories = ref([
  {
    id: '1',
    name: 'vue-vben-admin',
    description: 'Vue 3 + Vite + Ant Design Vue 管理后台',
    language: 'TypeScript',
    stars: 1250,
    forks: 380,
    lastUpdated: '2024-01-15',
  },
  {
    id: '2',
    name: 'dotnet-core-api',
    description: 'ASP.NET Core 10 Web API 模板',
    language: 'C#',
    stars: 890,
    forks: 210,
    lastUpdated: '2024-01-10',
  },
  {
    id: '3',
    name: 'python-ml-toolkit',
    description: '机器学习工具包',
    language: 'Python',
    stars: 560,
    forks: 120,
    lastUpdated: '2024-01-08',
  },
]);

const columns = [
  { title: '仓库名称', dataIndex: 'name', key: 'name' },
  { title: '描述', dataIndex: 'description', key: 'description' },
  { title: '语言', dataIndex: 'language', key: 'language' },
  { title: 'Stars', dataIndex: 'stars', key: 'stars' },
  { title: 'Forks', dataIndex: 'forks', key: 'forks' },
  { title: '最后更新', dataIndex: 'lastUpdated', key: 'lastUpdated' },
];

const languageColors: Record<string, string> = {
  TypeScript: 'blue',
  CSharp: 'purple',
  Python: 'green',
  JavaScript: 'yellow',
};
</script>

<template>
  <Page description="管理您的代码仓库，查看项目状态和统计信息" title="代码仓库">
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
          <Statistic title="仓库总数" :value="repositories.length" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="总 Stars" :value="2700" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="总 Forks" :value="710" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="活跃项目" :value="3" />
        </Card>
      </Col>
    </Row>

    <Card title="我的仓库">
      <template #extra>
        <Button type="primary">新建仓库</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="repositories"
        :loading="loading"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'language'">
            <Tag :color="languageColors[record.language] || 'default'">
              {{ record.language }}
            </Tag>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>