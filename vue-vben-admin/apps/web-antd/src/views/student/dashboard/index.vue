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
  Space,
  Statistic,
  type TableColumnsType,
  Table,
  Tag,
  message,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import {
  getMaterialPageApi,
  getMistakePageApi,
  getPostgraduateTaskPageApi,
  type ExamMaterial,
  type ExamMistake,
  type PostgraduateTask,
  updatePostgraduateTaskApi,
} from '#/api/student';

interface SubjectInsight {
  materialCount: number;
  mistakeCount: number;
  name: string;
  pendingTaskCount: number;
  reviewCount: number;
  score: number;
}

const loading = ref(false);
const tasks = ref<PostgraduateTask[]>([]);
const mistakes = ref<ExamMistake[]>([]);
const materials = ref<ExamMaterial[]>([]);

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

const totalTaskCount = computed(() => tasks.value.length);
const pendingTaskCount = computed(() => tasks.value.filter((item) => item.status !== 2).length);
const dueReviewCount = computed(() =>
  mistakes.value.filter((item) =>
    item.status !== 2 &&
    (!item.nextReviewDate || dayjs(item.nextReviewDate).isSame(dayjs(), 'day') || dayjs(item.nextReviewDate).isBefore(dayjs(), 'day')),
  ).length,
);
const materialCount = computed(() => materials.value.length);
const completionRate = computed(() => {
  if (tasks.value.length === 0) {
    return 0;
  }
  return Math.round((tasks.value.filter((item) => item.status === 2).length / tasks.value.length) * 100);
});

const todayTasks = computed(() =>
  tasks.value
    .filter((item) =>
      item.status !== 2 &&
      (!item.dueDate || dayjs(item.dueDate).isSame(dayjs(), 'day') || dayjs(item.dueDate).isBefore(dayjs(), 'day')),
    )
    .slice(0, 8),
);

const reviewMistakes = computed(() =>
  mistakes.value
    .filter((item) =>
      item.status !== 2 &&
      (!item.nextReviewDate || dayjs(item.nextReviewDate).isSame(dayjs(), 'day') || dayjs(item.nextReviewDate).isBefore(dayjs(), 'day')),
    )
    .slice(0, 6),
);

const subjectInsights = computed<SubjectInsight[]>(() => {
  const names = new Set<string>();
  for (const item of mistakes.value) names.add(item.subject);
  for (const item of materials.value) names.add(item.subject);

  return [...names]
    .filter(Boolean)
    .map((name) => {
      const mistakeCount = mistakes.value.filter((item) => item.subject === name && item.status !== 2).length;
      const reviewCount = mistakes.value.filter((item) => item.subject === name && item.status !== 2 && item.nextReviewDate && !dayjs(item.nextReviewDate).isAfter(dayjs(), 'day')).length;
      const materialCount = materials.value.filter((item) => item.subject === name).length;
      const pendingTaskCount = tasks.value.filter((item) => item.status !== 2 && [item.title, item.description].some((text) => text?.includes(name))).length;
      const score = Math.min(100, materialCount * 12 + Math.max(0, 40 - mistakeCount * 6) + Math.max(0, 20 - reviewCount * 5));
      return { materialCount, mistakeCount, name, pendingTaskCount, reviewCount, score };
    })
    .sort((a, b) => a.score - b.score);
});

async function fetchAll() {
  loading.value = true;
  try {
    const [taskItems, mistakeItems, materialItems] = await Promise.all([
      fetchAllPages(getPostgraduateTaskPageApi),
      fetchAllPages(getMistakePageApi),
      fetchAllPages(getMaterialPageApi),
    ]);
    tasks.value = taskItems;
    mistakes.value = mistakeItems;
    materials.value = materialItems;
  } catch {
    message.error('加载学习总览失败');
  } finally {
    loading.value = false;
  }
}

async function fetchAllPages<T>(
  loader: (params: { page: number; pageSize: number }) => Promise<{ items: T[]; total: number }>,
) {
  const items: T[] = [];
  let page = 1;
  const pageSize = 100;
  while (true) {
    const result = await loader({ page, pageSize });
    items.push(...result.items);
    if (items.length >= result.total || result.items.length < pageSize) {
      break;
    }
    page += 1;
  }
  return items;
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
    await fetchAll();
  } catch {
    message.error('更新任务失败');
  }
}

onMounted(() => {
  void fetchAll();
});
</script>

<template>
  <Page description="聚合计划、错题、资料和复习压力" title="学习总览">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="待办任务" :value="pendingTaskCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="到期复习" :value="dueReviewCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="学习资料" :value="materialCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="任务完成率" :value="completionRate" suffix="%" /></Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="14" :xs="24">
        <Card title="今日重点">
          <template #extra>
            <Button :loading="loading" type="primary" @click="fetchAll">刷新</Button>
          </template>
          <Table
            :columns="taskColumns"
            :data-source="todayTasks"
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
          <Empty v-if="todayTasks.length === 0 && !loading" description="今天没有到期任务" />
        </Card>
      </Col>

      <Col :lg="10" :xs="24">
        <Card title="到期错题">
          <div v-if="reviewMistakes.length === 0 && !loading" class="py-8">
            <Empty description="暂无到期复习" />
          </div>
          <div v-else class="space-y-3">
            <div
              v-for="mistake in reviewMistakes"
              :key="mistake.id"
              class="rounded border border-gray-200 p-3"
            >
              <div class="mb-2 flex items-center justify-between gap-2">
                <Space :size="4" wrap>
                  <Tag color="blue">{{ mistake.subject }}</Tag>
                  <Tag v-if="mistake.nextReviewDate" color="orange">{{ mistake.nextReviewDate }}</Tag>
                  <Tag v-else>未排期</Tag>
                </Space>
                <span class="text-text-secondary text-xs">{{ mistake.reviewCount }} 次</span>
              </div>
              <div class="line-clamp-2 text-sm">{{ mistake.question }}</div>
            </div>
          </div>
        </Card>
      </Col>
    </Row>

    <Card title="科目状态">
      <Row :gutter="[16, 16]">
        <Col v-for="subject in subjectInsights" :key="subject.name" :lg="8" :md="12" :xs="24">
          <div class="rounded border border-gray-200 p-3">
            <div class="mb-2 flex items-center justify-between">
              <div class="font-medium">{{ subject.name }}</div>
              <Tag :color="subject.score >= 70 ? 'green' : subject.score >= 45 ? 'orange' : 'red'">
                {{ subject.score }} 分
              </Tag>
            </div>
            <Progress :percent="subject.score" :show-info="false" />
            <div class="text-text-secondary mt-3 grid grid-cols-3 gap-2 text-xs">
              <span>错题 {{ subject.mistakeCount }}</span>
              <span>复习 {{ subject.reviewCount }}</span>
              <span>资料 {{ subject.materialCount }}</span>
            </div>
          </div>
        </Col>
      </Row>
      <Empty v-if="subjectInsights.length === 0 && !loading" description="暂无科目数据" />
    </Card>

    <div class="text-text-secondary mt-3 text-xs">
      当前共 {{ totalTaskCount }} 个学习任务，科目状态根据资料覆盖、未掌握错题和到期复习综合计算。
    </div>
  </Page>
</template>
