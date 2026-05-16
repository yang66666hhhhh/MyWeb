<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Empty,
  Progress,
  Row,
  Statistic,
  type TableColumnsType,
  Table,
  Tag,
  message,
} from 'ant-design-vue';

import { getExamDashboardApi, type ExamDashboard, type PostgraduateTask } from '#/api/student/postgraduate';

const loading = ref(false);
const dashboard = ref<ExamDashboard>({
  materialCount: 0,
  mistakeCount: 0,
  recentRecords: [],
  reviewTaskCount: 0,
  subjects: [],
  todayTasks: [],
  weeklyHours: 0,
});

const statusLabels: Record<number, string> = {
  0: '待开始',
  1: '进行中',
  2: '已完成',
  3: '已取消',
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
  1: '复习任务',
  2: '冲刺训练',
};

const taskColumns: TableColumnsType<PostgraduateTask> = [
  { title: '任务标题', dataIndex: 'title', key: 'title', minWidth: 220 },
  { title: '类型', dataIndex: 'type', key: 'type', width: 110 },
  { title: '优先级', dataIndex: 'priority', key: 'priority', width: 100 },
  { title: '状态', dataIndex: 'status', key: 'status', width: 100 },
  { title: '截止日期', dataIndex: 'dueDate', key: 'dueDate', width: 120 },
];

const averageProgress = computed(() => {
  if (dashboard.value.subjects.length === 0) {
    return 0;
  }
  return Math.round(
    dashboard.value.subjects.reduce((sum, item) => sum + item.progress, 0) /
      dashboard.value.subjects.length,
  );
});

async function fetchDashboard() {
  loading.value = true;
  try {
    dashboard.value = await getExamDashboardApi();
  } catch {
    message.error('加载考研备考面板失败');
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  void fetchDashboard();
});
</script>

<template>
  <Page description="备考研究生入学考试，跟踪各科目复习进度" title="考研备考">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="今日任务" :value="dashboard.todayTasks.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="待复习错题" :value="dashboard.mistakeCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="资料总数" :value="dashboard.materialCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="平均进度" :value="averageProgress" suffix="%" /></Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="12" :xs="24">
        <Card title="科目进度">
          <div
            v-for="subject in dashboard.subjects"
            :key="subject.id"
            class="mb-4 last:mb-0"
          >
            <div class="mb-2 flex items-center justify-between">
              <div class="flex items-center gap-2">
                <Tag :color="subject.color || 'blue'">{{ subject.name }}</Tag>
                <span class="text-text-secondary text-xs">
                  本周 {{ subject.weeklyHours }}h / 目标 {{ subject.targetHours }}h
                </span>
              </div>
              <span>{{ subject.progress }}%</span>
            </div>
            <Progress :percent="subject.progress" :show-info="false" />
          </div>
          <Empty v-if="dashboard.subjects.length === 0" description="暂无科目进度数据" />
        </Card>
      </Col>

      <Col :lg="12" :xs="24">
        <Card title="最近学习记录">
          <div v-if="dashboard.recentRecords.length === 0" class="py-6">
            <Empty description="暂无学习记录" />
          </div>
          <div v-else class="space-y-3">
            <div
              v-for="record in dashboard.recentRecords"
              :key="record.id"
              class="rounded border border-gray-200 p-3"
            >
              <div class="mb-1 flex items-center justify-between">
                <Tag color="cyan">{{ record.subject }}</Tag>
                <span class="text-text-secondary text-xs">{{ record.recordDate }}</span>
              </div>
              <div class="mb-1 text-sm">{{ record.summary }}</div>
              <div class="text-text-secondary text-xs">{{ record.durationMinutes }} 分钟</div>
            </div>
          </div>
        </Card>
      </Col>
    </Row>

    <Card title="今日任务清单">
      <template #extra>
        <Button :loading="loading" type="primary" @click="fetchDashboard">刷新面板</Button>
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
        </template>
      </Table>
      <Empty v-if="dashboard.todayTasks.length === 0 && !loading" description="当前没有到期或待办的任务" />
    </Card>
  </Page>
</template>
