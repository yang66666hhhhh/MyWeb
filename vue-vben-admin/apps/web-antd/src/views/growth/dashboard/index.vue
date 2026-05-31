<script lang="ts" setup>
import type { DailyPlan } from '#/api/growth';

import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';

import { Page } from '@vben/common-ui';
import { useAccessStore } from '@vben/stores';

import {
  Button,
  Card,
  Col,
  Empty,
  List,
  ListItem,
  ListItemMeta,
  message,
  Progress,
  Row,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import { getDailyPlanPageApi } from '#/api/growth';
import { getRecentTasksApi, getRecentWorkLogsApi } from '#/api/dashboard';

const router = useRouter();
const accessStore = useAccessStore();
const loading = ref(false);

const todayPlans = ref<DailyPlan[]>([]);
const recentTasks = ref<any[]>([]);
const recentWorkLogs = ref<any[]>([]);

const today = new Date().toISOString().slice(0, 10);
const currentHour = new Date().getHours();

const completedCount = computed(
  () => todayPlans.value.filter((item) => item.status === 2).length,
);

const completionRate = computed(() => {
  if (todayPlans.value.length === 0) return 0;
  return Math.round((completedCount.value / todayPlans.value.length) * 100);
});

const greeting = computed(() => {
  if (currentHour < 6) return '夜深了，注意休息';
  if (currentHour < 9) return '早上好，新的一天开始了';
  if (currentHour < 12) return '上午好，保持专注';
  if (currentHour < 14) return '中午好，记得休息';
  if (currentHour < 18) return '下午好，继续加油';
  if (currentHour < 22) return '晚上好，回顾一下今天';
  return '夜深了，早点休息';
});

const weekDay = computed(() => {
  const days = ['周日', '周一', '周二', '周三', '周四', '周五', '周六'];
  return days[new Date().getDay()];
});

const statusMap: Record<number, { color: string; label: string }> = {
  0: { color: 'default', label: '待处理' },
  1: { color: 'processing', label: '进行中' },
  2: { color: 'success', label: '已完成' },
  3: { color: 'error', label: '已取消' },
};

const priorityMap: Record<number, { color: string; label: string }> = {
  1: { color: 'green', label: '低' },
  2: { color: 'orange', label: '中' },
  3: { color: 'red', label: '高' },
  4: { color: 'magenta', label: '紧急' },
};

const taskColumns = [
  { title: '任务', dataIndex: 'title', key: 'title', ellipsis: true },
  { title: '优先级', key: 'priority', width: 80 },
  { title: '状态', key: 'status', width: 80 },
];

const quickActions = computed(() => {
  const actions = [];
  if (accessStore.accessCodes.includes('WORK_TASK')) {
    actions.push({ icon: 'lucide:check-square', title: '工作任务', url: '/work/tasks' });
  }
  if (accessStore.accessCodes.includes('WORK_LOG')) {
    actions.push({ icon: 'lucide:clipboard-list', title: '工作日志', url: '/work/work-log' });
  }
  if (accessStore.accessCodes.includes('GROWTH_DAILY_PLAN')) {
    actions.push({ icon: 'lucide:calendar', title: '每日计划', url: '/growth/daily-plans' });
  }
  if (accessStore.accessCodes.includes('GROWTH_HABIT')) {
    actions.push({ icon: 'lucide:repeat', title: '习惯打卡', url: '/growth/habits' });
  }
  if (accessStore.accessCodes.includes('GROWTH_KNOWLEDGE')) {
    actions.push({ icon: 'lucide:book-open', title: '知识库', url: '/growth/knowledge-base' });
  }
  if (accessStore.accessCodes.includes('AI_ASSISTANT')) {
    actions.push({ icon: 'lucide:bot', title: 'AI 助手', url: '/ai/assistant' });
  }
  return actions;
});

const fetchData = async () => {
  loading.value = true;
  try {
    const [plansRes, tasksRes, logsRes] = await Promise.allSettled([
      getDailyPlanPageApi({ page: 1, pageSize: 10, startDate: today, endDate: today }),
      getRecentTasksApi({ pageSize: 5 }),
      getRecentWorkLogsApi({ pageSize: 5 }),
    ]);

    if (plansRes.status === 'fulfilled') {
      todayPlans.value = plansRes.value.items;
    }
    if (tasksRes.status === 'fulfilled') {
      recentTasks.value = tasksRes.value.items;
    }
    if (logsRes.status === 'fulfilled') {
      recentWorkLogs.value = logsRes.value.items;
    }
  } catch {
    message.error('加载数据失败');
  } finally {
    loading.value = false;
  }
};

const navTo = (url: string) => {
  router.push(url);
};

onMounted(() => {
  fetchData();
});
</script>

<template>
  <Page :title="`成长看板 - ${weekDay}`" :description="greeting">
    <div class="space-y-4">
      <Row :gutter="[16, 16]">
        <Col :lg="6" :md="12" :xs="24">
          <Card :loading="loading">
            <Statistic title="今日计划" :value="todayPlans.length" suffix="项" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card :loading="loading">
            <Statistic title="已完成" :value="completedCount" suffix="项" />
            <Progress :percent="completionRate" size="small" class="mt-2" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card :loading="loading">
            <Statistic title="待处理" :value="todayPlans.filter(p => p.status === 0 || p.status === 1).length" suffix="项" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card :loading="loading">
            <Statistic title="完成率" :value="completionRate" suffix="%" />
          </Card>
        </Col>
      </Row>

      <Row :gutter="[16, 16]">
        <Col :lg="16" :xs="24">
          <Card title="今日计划">
            <template #extra>
              <Button type="link" size="small" @click="navTo('/growth/daily-plans')">查看全部</Button>
            </template>
            <List v-if="todayPlans.length > 0" :data-source="todayPlans" :loading="loading" :split="false">
              <template #renderItem="{ item }">
                <ListItem>
                  <ListItemMeta :title="item.title" :description="item.description" />
                  <Tag :color="item.status === 2 ? 'success' : 'processing'">
                    {{ item.status === 2 ? '已完成' : '进行中' }}
                  </Tag>
                </ListItem>
              </template>
            </List>
            <Empty v-else description="今天暂无计划，去创建一个吧！">
              <Button type="primary" @click="navTo('/growth/daily-plans')">创建计划</Button>
            </Empty>
          </Card>
        </Col>

        <Col :lg="8" :xs="24">
          <Card title="快捷操作">
            <div class="grid grid-cols-2 gap-3">
              <div
                v-for="action in quickActions"
                :key="action.url"
                class="flex cursor-pointer items-center gap-3 rounded-lg p-3 transition-colors hover:bg-gray-50"
                @click="navTo(action.url)"
              >
                <div class="flex h-10 w-10 items-center justify-center rounded-lg bg-blue-50 text-blue-500">
                  <span :class="`icon-[${action.icon}]`" />
                </div>
                <span class="text-sm">{{ action.title }}</span>
              </div>
            </div>
          </Card>
        </Col>
      </Row>

      <Row :gutter="[16, 16]">
        <Col :lg="12" :xs="24">
          <Card title="最近任务">
            <template #extra>
              <Button type="link" size="small" @click="navTo('/work/tasks')">查看全部</Button>
            </template>
            <Table
              v-if="recentTasks.length > 0"
              :columns="taskColumns"
              :data-source="recentTasks"
              :pagination="false"
              size="small"
            >
              <template #bodyCell="{ column, record }">
                <template v-if="column.key === 'priority'">
                  <Tag :color="priorityMap[record.priority]?.color">
                    {{ priorityMap[record.priority]?.label }}
                  </Tag>
                </template>
                <template v-else-if="column.key === 'status'">
                  <Tag :color="statusMap[record.status]?.color">
                    {{ statusMap[record.status]?.label }}
                  </Tag>
                </template>
              </template>
            </Table>
            <Empty v-else description="暂无任务" />
          </Card>
        </Col>

        <Col :lg="12" :xs="24">
          <Card title="最近工作日志">
            <template #extra>
              <Button type="link" size="small" @click="navTo('/work/work-log')">查看全部</Button>
            </template>
            <List v-if="recentWorkLogs.length > 0" :data-source="recentWorkLogs" :split="false">
              <template #renderItem="{ item }">
                <ListItem>
                  <ListItemMeta
                    :title="item.title"
                    :description="`${item.workDate} | ${item.totalHours}h`"
                  />
                </ListItem>
              </template>
            </List>
            <Empty v-else description="暂无工作日志" />
          </Card>
        </Col>
      </Row>
    </div>
  </Page>
</template>
