<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Alert,
  Button,
  Card,
  Col,
  Row,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

const loading = ref(false);

const courses = ref([
  {
    id: '1',
    name: 'Vue 3 高级编程',
    category: '前端开发',
    students: 45,
    status: '进行中',
    startDate: '2024-01-01',
    endDate: '2024-06-30',
  },
  {
    id: '2',
    name: 'ASP.NET Core 微服务',
    category: '后端开发',
    students: 32,
    status: '进行中',
    startDate: '2024-02-01',
    endDate: '2024-07-31',
  },
  {
    id: '3',
    name: '数据库设计原理',
    category: '数据库',
    students: 28,
    status: '已完成',
    startDate: '2023-09-01',
    endDate: '2024-01-31',
  },
]);

const columns = [
  { title: '课程名称', dataIndex: 'name', key: 'name' },
  { title: '分类', dataIndex: 'category', key: 'category' },
  { title: '学生数', dataIndex: 'students', key: 'students' },
  { title: '状态', dataIndex: 'status', key: 'status' },
  { title: '开始日期', dataIndex: 'startDate', key: 'startDate' },
  { title: '结束日期', dataIndex: 'endDate', key: 'endDate' },
];

const categoryColors: Record<string, string> = {
  '前端开发': 'blue',
  '后端开发': 'green',
  '数据库': 'purple',
};

const statusColors: Record<string, string> = {
  '进行中': 'blue',
  '已完成': 'green',
  '待开始': 'default',
};
</script>

<template>
  <Page description="管理课程内容、教学计划和学生信息" title="课程管理">
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
          <Statistic title="课程总数" :value="courses.length" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="进行中" :value="2" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="学生总数" :value="105" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="已完成课程" :value="1" />
        </Card>
      </Col>
    </Row>

    <Card title="课程列表">
      <template #extra>
        <Button type="primary">新建课程</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="courses"
        :loading="loading"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'category'">
            <Tag :color="categoryColors[record.category] || 'default'">
              {{ record.category }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="statusColors[record.status] || 'default'">
              {{ record.status }}
            </Tag>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>