<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Alert,
  Button,
  Card,
  Col,
  Progress,
  Row,
  Space,
  Statistic,
  Steps,
  Table,
  Tag,
  Timeline,
} from 'ant-design-vue';

const loading = ref(false);

const subjects = ref([
  {
    id: '1',
    name: '政治',
    progress: 45,
    status: '进行中',
    nextReview: '2024-01-20',
  },
  {
    id: '2',
    name: '英语',
    progress: 60,
    status: '进行中',
    nextReview: '2024-01-18',
  },
  {
    id: '3',
    name: '数学',
    progress: 35,
    status: '进行中',
    nextReview: '2024-01-22',
  },
  {
    id: '4',
    name: '专业课',
    progress: 50,
    status: '进行中',
    nextReview: '2024-01-25',
  },
]);

const milestones = [
  {
    date: '2024-03-01',
    title: '基础阶段结束',
    description: '完成所有科目的基础知识学习',
  },
  {
    date: '2024-06-01',
    title: '强化阶段结束',
    description: '完成重点难点的深入学习',
  },
  {
    date: '2024-09-01',
    title: '冲刺阶段开始',
    description: '开始模拟考试和真题训练',
  },
  {
    date: '2024-12-01',
    title: '考试',
    description: '研究生入学考试',
  },
];

const columns = [
  { title: '科目', dataIndex: 'name', key: 'name' },
  { title: '进度', dataIndex: 'progress', key: 'progress' },
  { title: '状态', dataIndex: 'status', key: 'status' },
  { title: '下次复习', dataIndex: 'nextReview', key: 'nextReview' },
];

const subjectColors: Record<string, string> = {
  '政治': 'red',
  '英语': 'blue',
  '数学': 'green',
  '专业课': 'purple',
};
</script>

<template>
  <Page description="备考研究生入学考试，跟踪各科目复习进度" title="考研备考">
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
          <Statistic title="考试科目" :value="4" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="平均进度" :value="48" suffix="%" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="距离考试" :value="320" suffix="天" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="复习阶段" value="基础" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="12" :xs="24">
        <Card title="科目进度">
          <div v-for="subject in subjects" :key="subject.id" class="mb-4">
            <div class="mb-2 flex items-center justify-between">
              <Tag :color="subjectColors[subject.name] || 'default'">
                {{ subject.name }}
              </Tag>
              <span>{{ subject.progress }}%</span>
            </div>
            <Progress :percent="subject.progress" :show-info="false" />
          </div>
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="备考里程碑">
          <Timeline>
            <Timeline.Item v-for="milestone in milestones" :key="milestone.date">
              <div>
                <div class="font-bold">{{ milestone.title }}</div>
                <div class="text-gray-500">{{ milestone.date }}</div>
                <div>{{ milestone.description }}</div>
              </div>
            </Timeline.Item>
          </Timeline>
        </Card>
      </Col>
    </Row>

    <Card title="学习计划">
      <template #extra>
        <Button type="primary">制定计划</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="subjects"
        :loading="loading"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'name'">
            <Tag :color="subjectColors[record.name] || 'default'">
              {{ record.name }}
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