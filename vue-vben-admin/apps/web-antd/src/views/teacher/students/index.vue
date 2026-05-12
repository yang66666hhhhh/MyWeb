<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Alert,
  Button,
  Card,
  Col,
  Row,
  Space,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

const loading = ref(false);

const students = ref([
  {
    id: '1',
    name: '张三',
    email: 'zhangsan@example.com',
    course: 'Vue 3 高级编程',
    progress: 75,
    status: '在读',
    enrolledAt: '2024-01-05',
  },
  {
    id: '2',
    name: '李四',
    email: 'lisi@example.com',
    course: 'ASP.NET Core 微服务',
    progress: 60,
    status: '在读',
    enrolledAt: '2024-02-10',
  },
  {
    id: '3',
    name: '王五',
    email: 'wangwu@example.com',
    course: '数据库设计原理',
    progress: 100,
    status: '已毕业',
    enrolledAt: '2023-09-15',
  },
]);

const columns = [
  { title: '学生姓名', dataIndex: 'name', key: 'name' },
  { title: '邮箱', dataIndex: 'email', key: 'email' },
  { title: '课程', dataIndex: 'course', key: 'course' },
  { title: '进度', dataIndex: 'progress', key: 'progress' },
  { title: '状态', dataIndex: 'status', key: 'status' },
  { title: '入学时间', dataIndex: 'enrolledAt', key: 'enrolledAt' },
];

const statusColors: Record<string, string> = {
  '在读': 'blue',
  '已毕业': 'green',
  '休学': 'orange',
};
</script>

<template>
  <Page description="管理学生信息、学习进度和成绩" title="学生管理">
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
          <Statistic title="学生总数" :value="students.length" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="在读学生" :value="2" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="已毕业" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="平均进度" :value="78" suffix="%" />
        </Card>
      </Col>
    </Row>

    <Card title="学生列表">
      <template #extra>
        <Button type="primary">添加学生</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="students"
        :loading="loading"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'progress'">
            <div class="flex items-center gap-2">
              <div class="h-2 w-20 rounded-full bg-gray-200">
                <div
                  class="h-full rounded-full bg-blue-500"
                  :style="{ width: `${record.progress}%` }"
                />
              </div>
              <span>{{ record.progress }}%</span>
            </div>
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