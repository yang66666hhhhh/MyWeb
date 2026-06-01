<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';

import { Page } from '@vben/common-ui';
import { useAccessStore, useUserStore } from '@vben/stores';

import {
  Button,
  Card,
  Col,
  List,
  ListItem,
  ListItemMeta,
  message,
  Row,
  Space,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { RecentTask, RecentWorkLog, TodayPlan } from '#/api/dashboard';

import { dashboardApi } from '#/api/dashboard';

const router = useRouter();
const accessStore = useAccessStore();
const userStore = useUserStore();
const loading = ref(false);

const overview = ref({
  todayTasks: 0,
  todayCompletedTasks: 0,
  todayWorkHours: 0,
  weekTasks: 0,
  weekCompletedTasks: 0,
  weekWorkHours: 0,
  totalProjects: 0,
  totalHabits: 0,
  totalKnowledge: 0,
});

const recentTasks = ref<RecentTask[]>([]);
const recentWorkLogs = ref<RecentWorkLog[]>([]);
const todayPlans = ref<TodayPlan[]>([]);

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
  { title: '日期', dataIndex: 'planDate', key: 'planDate', width: 100 },
];

const quickNavItems = [
  { icon: 'lucide:check-square', title: '工作任务', url: '/work/tasks', feature: 'WORK_TASK' },
  { icon: 'lucide:clipboard-list', title: '工作日志', url: '/work/work-log', feature: 'WORK_LOG' },
  { icon: 'lucide:folder-kanban', title: '工作项目', url: '/work/project', feature: 'WORK_PROJECT' },
  { icon: 'lucide:calendar-heart', title: '每日计划', url: '/growth/daily-plans', feature: 'GROWTH_DAILY_PLAN' },
  { icon: 'lucide:repeat', title: '习惯打卡', url: '/growth/habits', feature: 'GROWTH_HABIT' },
  { icon: 'lucide:book-open', title: '知识库', url: '/growth/knowledge-base', feature: 'GROWTH_KNOWLEDGE' },
  { icon: 'lucide:bot', title: 'AI 助手', url: '/ai/assistant', feature: 'AI_ASSISTANT' },
  { icon: 'lucide:wallet', title: '财务资产', url: '/assets/dashboard', feature: 'ASSET_DASHBOARD' },
];

const filteredNavItems = quickNavItems.filter(
  (item) => !item.feature || accessStore.accessCodes.includes(item.feature),
);

const completedToday = computed(() => todayPlans.value.filter((p) => p.status === 2).length);
const completionRate = computed(() =>
  todayPlans.value.length > 0 ? Math.round((completedToday.value / todayPlans.value.length) * 100) : 0,
);

const fetchData = async () => {
  loading.value = true;
  try {
    const [overviewData, tasksData, logsData, plansData] = await Promise.allSettled([
      dashboardApi.getOverview(),
      dashboardApi.getRecentTasks({ pageSize: 5 }),
      dashboardApi.getRecentWorkLogs({ pageSize: 5 }),
      dashboardApi.getTodayPlans({ pageSize: 10 }),
    ]);

    if (overviewData.status === 'fulfilled') {
      overview.value = overviewData.value;
    }
    if (tasksData.status === 'fulfilled') {
      recentTasks.value = tasksData.value.items;
    }
    if (logsData.status === 'fulfilled') {
      recentWorkLogs.value = logsData.value.items;
    }
    if (plansData.status === 'fulfilled') {
      todayPlans.value = plansData.value.items;
    }
  } catch (e: any) {
    message.error(e?.message || '加载失败，请稍后重试');
  } finally {
    loading.value = false;
  }
};

const navTo = (url: string) => {
  router.push(url);
};

const getGreeting = () => {
  const hour = new Date().getHours();
  if (hour < 6) return '夜深了';
  if (hour < 9) return '早上好';
  if (hour < 12) return '上午好';
  if (hour < 14) return '中午好';
  if (hour < 18) return '下午好';
  if (hour < 22) return '晚上好';
  return '夜深了';
};

onMounted(() => {
  fetchData();
});
</script>

<template>
  <Page :title="`${getGreeting()}，${userStore.userInfo?.realName || ''}`" description="开始您一天的工作吧！">
    <div class="space-y-4">
      <Row :gutter="[16, 16]">
        <Col :lg="6" :md="12" :xs="24">
          <Card :loading="loading">
            <Statistic title="今日任务" :value="overview.todayTasks" suffix="个" />
            <div class="mt-2 text-sm text-gray-500">
              已完成 {{ overview.todayCompletedTasks }}
            </div>
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card :loading="loading">
            <Statistic title="今日工时" :value="overview.todayWorkHours" suffix="小时" :precision="1" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card :loading="loading">
            <Statistic title="本周任务" :value="overview.weekTasks" suffix="个" />
            <div class="mt-2 text-sm text-gray-500">
              已完成 {{ overview.weekCompletedTasks }}
            </div>
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card :loading="loading">
            <Statistic title="本周工时" :value="overview.weekWorkHours" suffix="小时" :precision="1" />
          </Card>
        </Col>
      </Row>

      <Row :gutter="[16, 16]">
        <Col :lg="16" :xs="24">
          <Card title="今日计划" :loading="loading">
            <template #extra>
              <Space>
                <span class="text-sm text-gray-500">
                  {{ completedToday }}/{{ todayPlans.length }} 完成 ({{ completionRate }}%)
                </span>
                <Button type="link" size="small" @click="navTo('/growth/daily-plans')">查看全部</Button>
              </Space>
            </template>
            <Table
              v-if="todayPlans.length > 0"
              :columns="taskColumns"
              :data-source="todayPlans"
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
            <div v-else class="py-8 text-center text-gray-400">
              今天暂无计划，去创建一个吧！
            </div>
          </Card>

          <Card title="最近任务" class="mt-4" :loading="loading">
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
            <div v-else class="py-8 text-center text-gray-400">
              暂无任务
            </div>
          </Card>
        </Col>

        <Col :lg="8" :xs="24">
          <Card title="快捷导航">
            <div class="grid grid-cols-2 gap-3">
              <div
                v-for="item in filteredNavItems"
                :key="item.url"
                class="flex cursor-pointer items-center gap-3 rounded-lg p-3 transition-colors hover:bg-gray-50"
                @click="navTo(item.url)"
              >
                <div class="flex h-10 w-10 items-center justify-center rounded-lg bg-blue-50 text-blue-500">
                  <span :class="`icon-[lucide:${item.icon.replace('lucide:', '')}]`" />
                </div>
                <span class="text-sm">{{ item.title }}</span>
              </div>
            </div>
          </Card>

          <Card title="最近工作日志" class="mt-4" :loading="loading">
            <template #extra>
              <Button type="link" size="small" @click="navTo('/work/work-log')">查看全部</Button>
            </template>
            <List v-if="recentWorkLogs.length > 0" :data-source="recentWorkLogs" :split="false">
              <template #renderItem="{ item }">
                <ListItem>
                  <ListItemMeta
                    :title="item.title"
                    :description="`${item.workDate} | ${item.totalHours}h${item.projectName ? ` | ${item.projectName}` : ''}`"
                  />
                </ListItem>
              </template>
            </List>
            <div v-else class="py-8 text-center text-gray-400">
              暂无工作日志
            </div>
          </Card>
        </Col>
      </Row>
    </div>
  </Page>
</template>
