<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
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

const plans = ref([
  {
    id: '1',
    subject: '数据结构',
    topic: '二叉树遍历',
    status: '进行中',
    priority: '高',
    dueDate: '2024-01-20',
    progress: 60,
  },
  {
    id: '2',
    subject: '操作系统',
    topic: '进程调度',
    status: '待开始',
    priority: '中',
    dueDate: '2024-01-25',
    progress: 0,
  },
  {
    id: '3',
    subject: '计算机网络',
    topic: 'TCP/IP 协议',
    status: '已完成',
    priority: '高',
    dueDate: '2024-01-15',
    progress: 100,
  },
]);

const columns = [
  { title: '科目', dataIndex: 'subject', key: 'subject' },
  { title: '主题', dataIndex: 'topic', key: 'topic' },
  { title: '状态', dataIndex: 'status', key: 'status' },
  { title: '优先级', dataIndex: 'priority', key: 'priority' },
  { title: '截止日期', dataIndex: 'dueDate', key: 'dueDate' },
  { title: '进度', dataIndex: 'progress', key: 'progress' },
];

const statusColors: Record<string, string> = {
  '进行中': 'blue',
  '待开始': 'default',
  '已完成': 'green',
};

const priorityColors: Record<string, string> = {
  '高': 'red',
  '中': 'orange',
  '低': 'green',
};

const subjectColors: Record<string, string> = {
  '数据结构': 'blue',
  '操作系统': 'purple',
  '计算机网络': 'green',
  '计算机组成原理': 'orange',
};
</script>

<template>
  <Page description="制定学习计划、跟踪学习进度" title="学习计划">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="学习主题" :value="plans.length" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="进行中" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="已完成" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="平均进度" :value="53" suffix="%" />
        </Card>
      </Col>
    </Row>

    <Card title="学习计划列表">
      <template #extra>
        <Button type="primary">新建计划</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="plans"
        :loading="loading"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'subject'">
            <Tag :color="subjectColors[record.subject] || 'default'">
              {{ record.subject }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="statusColors[record.status] || 'default'">
              {{ record.status }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'priority'">
            <Tag :color="priorityColors[record.priority] || 'default'">
              {{ record.priority }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'progress'">
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
        </template>
      </Table>
    </Card>
  </Page>
</template>
