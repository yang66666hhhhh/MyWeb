<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Empty,
  Progress,
  Row,
  Space,
  Statistic,
  type TableColumnsType,
  Table,
  Tag,
  message,
} from 'ant-design-vue';

import {
  getExamDashboardApi,
  type ExamDashboard,
  type PostgraduateTask,
  updatePostgraduateTaskApi,
} from '#/api/student';

const loading = ref(false);
const dashboard = ref<ExamDashboard>({
  materialCount: 0,
  mistakeCount: 0,
  overdueTaskCount: 0,
  pendingTaskCount: 0,
  recentRecords: [],
  reviewTaskCount: 0,
  subjectCount: 0,
  subjects: [],
  todayReviewCount: 0,
  todayTasks: [],
  weeklyHours: 0,
});

const statusLabels: Record<number, string> = {
  0: '待开始',
  1: '进行中',
  2: '已完成',
  3: '已逾期',
};

const statusColors: Record<number, string> = {
  0: 'default',
  1: 'processing',
  2: 'success',
  3: 'error',
};

const priorityLabels: Record<number, string> = {
  1: '低',
  2: '中',
  3: '高',
  4: '紧急',
};

const typeLabels: Record<number, string> = {
  0: '学习任务',
  1: '刷题训练',
  2: '复习任务',
  3: '模拟考试',
};

const taskColumns: TableColumnsType<PostgraduateTask> = [
  { title: '任务标题', dataIndex: 'title', key: 'title', minWidth: 240 },
  { title: '类型', dataIndex: 'type', key: 'type', width: 110 },
  { title: '优先级', dataIndex: 'priority', key: 'priority', width: 100 },
  { title: '状态', dataIndex: 'status', key: 'status', width: 100 },
  { title: '截止日期', dataIndex: 'dueDate', key: 'dueDate', width: 120 },
  { key: 'action', title: '操作', width: 90, fixed: 'right' },
];

async function fetchDashboard() {
  loading.value = true;
  try {
    dashboard.value = await getExamDashboardApi();
  } catch {
    message.error('加载学习总览失败');
  } finally {
    loading.value = false;
  }
}

function toTask(record: Record<string, any>) {
  return record as PostgraduateTask;
}

async function markTaskCompleted(task: PostgraduateTask) {
  try {
    await updatePostgraduateTaskApi(task.id, {
      description: task.description || undefined,
      dueDate: task.dueDate || undefined,
      priority: task.priority,
      status: 2,
      title: task.title,
      type: task.type,
    });
    message.success('任务已完成');
    await fetchDashboard();
  } catch {
    message.error('更新任务失败');
  }
}

onMounted(() => {
  void fetchDashboard();
});
</script>

<template>
  <Page description="聚合计划、记录、错题、资料和科目进度" title="学习总览">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="待办任务" :value="dashboard.pendingTaskCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="今日复习" :value="dashboard.todayReviewCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本周学习" :value="dashboard.weeklyHours" suffix="h" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="覆盖科目" :value="dashboard.subjectCount" /></Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="14" :xs="24">
        <Card title="今日重点">
          <template #extra>
            <Button :loading="loading" type="primary" @click="fetchDashboard">刷新</Button>
          </template>
          <Table
            :columns="taskColumns"
            :data-source="dashboard.todayTasks"
            :loading="loading"
            :pagination="false"
            :scroll="{ x: 860 }"
            row-key="id"
          >
            <template #bodyCell="{ column, record, text }">
              <template v-if="column.key === 'title'">
                <div class="font-medium">{{ record.title }}</div>
                <div v-if="record.description" class="text-text-secondary line-clamp-1 text-xs">
                  {{ record.description }}
                </div>
              </template>
              <template v-else-if="column.key === 'type'">
                <Tag color="cyan">{{ typeLabels[Number(text)] || '学习任务' }}</Tag>
              </template>
              <template v-else-if="column.key === 'priority'">
                <Tag color="blue">{{ priorityLabels[Number(text)] || '中' }}</Tag>
              </template>
              <template v-else-if="column.key === 'status'">
                <Tag :color="statusColors[Number(text)] || 'default'">
                  {{ statusLabels[Number(text)] || '待开始' }}
                </Tag>
              </template>
              <template v-else-if="column.key === 'dueDate'">
                <span>{{ text || '-' }}</span>
              </template>
              <template v-else-if="column.key === 'action'">
                <Button size="small" type="link" @click="markTaskCompleted(toTask(record))">完成</Button>
              </template>
            </template>
          </Table>
          <Empty v-if="dashboard.todayTasks.length === 0 && !loading" description="今天没有到期任务" />
        </Card>
      </Col>

      <Col :lg="10" :xs="24">
        <Card title="最近学习记录">
          <div v-if="dashboard.recentRecords.length === 0 && !loading" class="py-8">
            <Empty description="暂无学习记录" />
          </div>
          <div v-else class="space-y-3">
            <div
              v-for="record in dashboard.recentRecords"
              :key="record.id"
              class="rounded border border-gray-200 p-3"
            >
              <div class="mb-2 flex items-center justify-between gap-2">
                <Space :size="4" wrap>
                  <Tag color="blue">{{ record.subject }}</Tag>
                  <Tag>{{ record.recordDate }}</Tag>
                </Space>
                <span class="text-text-secondary text-xs">{{ record.durationMinutes }} 分钟</span>
              </div>
              <div class="font-medium">{{ record.summary }}</div>
              <div v-if="record.taskTitle" class="text-text-secondary mt-1 text-xs">{{ record.taskTitle }}</div>
            </div>
          </div>
        </Card>
      </Col>
    </Row>

    <Card title="科目进度">
      <Row :gutter="[16, 16]">
        <Col v-for="subject in dashboard.subjects" :key="subject.id" :lg="8" :md="12" :xs="24">
          <div class="rounded border border-gray-200 p-3">
            <div class="mb-2 flex items-center justify-between">
              <Tag :color="subject.color || 'blue'">{{ subject.name }}</Tag>
              <span class="text-sm">{{ subject.progress }}%</span>
            </div>
            <Progress :percent="subject.progress" :show-info="false" />
            <div class="text-text-secondary mt-3 grid grid-cols-3 gap-2 text-xs">
              <span>本周 {{ subject.weeklyHours }}h</span>
              <span>错题 {{ subject.mistakeCount }}</span>
              <span>资料 {{ subject.materialCount }}</span>
            </div>
          </div>
        </Col>
      </Row>
      <Empty v-if="dashboard.subjects.length === 0 && !loading" description="暂无科目数据" />
    </Card>
  </Page>
</template>
