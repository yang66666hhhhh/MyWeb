<script lang="ts" setup>
import type { WorkStatisticsOverview } from '#/api/work';

import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';

import { Page } from '@vben/common-ui';
import { useAccessStore } from '@vben/stores';

import { Card, Col, message, Row, Statistic } from 'ant-design-vue';

import { getOverviewApi, getWorkDailyPlanPageApi, getWorkLogPageApi } from '#/api/work';

const loading = ref(false);
const router = useRouter();
const accessStore = useAccessStore();
const overview = ref<WorkStatisticsOverview>({
  missingDataCount: 0,
  pendingSupplementCount: 0,
  todayHours: 0,
  todayLogs: 0,
  totalDevices: 0,
  totalHours: 0,
  totalLogs: 0,
  totalProjects: 0,
});

const accessCodes = computed(() => new Set(accessStore.accessCodes));
const canViewStatistics = computed(() => accessCodes.value.has('WORK_STATISTICS'));
const canViewProjects = computed(() => accessCodes.value.has('WORK_PROJECT'));
const canViewDevices = computed(() => accessCodes.value.has('WORK_DEVICE'));

const quickEntries = computed(() => [
  {
    bg: 'bg-blue-100',
    code: 'WORK_TASK',
    description: '管理工作计划',
    icon: '日',
    path: '/work/daily-plans',
    text: 'text-blue-600',
    title: '每日计划',
  },
  {
    bg: 'bg-green-100',
    code: 'WORK_LOG',
    description: '记录日常工作',
    icon: '志',
    path: '/work/work-log',
    text: 'text-green-600',
    title: '工作日志',
  },
  {
    bg: 'bg-orange-100',
    code: 'WORK_IMPORT',
    description: 'Excel批量导入',
    icon: '导',
    path: '/work/import',
    text: 'text-orange-600',
    title: '工作导入',
  },
  {
    bg: 'bg-purple-100',
    code: 'WORK_PROJECT',
    description: '管理工作项目',
    icon: '项',
    path: '/work/project',
    text: 'text-purple-600',
    title: '项目管理',
  },
].filter((item) => accessCodes.value.has(item.code)));

function getToday() {
  return new Date().toISOString().slice(0, 10);
}

async function load() {
  loading.value = true;
  try {
    if (canViewStatistics.value) {
      overview.value = await getOverviewApi();
      return;
    }

    const today = getToday();
    const [logs, todayLogs, todayPlans] = await Promise.all([
      getWorkLogPageApi({ page: 1, pageSize: 100 }),
      getWorkLogPageApi({ page: 1, pageSize: 100, workDate: today }),
      getWorkDailyPlanPageApi({ page: 1, pageSize: 1, planDate: today }),
    ]);

    overview.value = {
      missingDataCount: 0,
      pendingSupplementCount: todayPlans.total,
      todayHours: todayLogs.items.reduce((sum, item) => sum + Number(item.totalHours || 0), 0),
      todayLogs: todayLogs.total,
      totalDevices: 0,
      totalHours: logs.items.reduce((sum, item) => sum + Number(item.totalHours || 0), 0),
      totalLogs: logs.total,
      totalProjects: 0,
    };
  } catch {
    message.error('加载数据失败');
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="工作模块总览，快速入口和关键指标" title="工作看板">
    <div class="space-y-4">
      <Row :gutter="[16, 16]">
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="条" title="工作日志总数" :value="overview.totalLogs" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="小时" title="总工作时长" :value="overview.totalHours" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="条" title="今日工作日志" :value="overview.todayLogs" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="小时" title="今日工作时长" :value="overview.todayHours" />
          </Card>
        </Col>
      </Row>

      <Row v-if="canViewProjects || canViewDevices || canViewStatistics" :gutter="[16, 16]">
        <Col v-if="canViewProjects" :lg="8" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="个" title="项目总数" :value="overview.totalProjects" />
          </Card>
        </Col>
        <Col v-if="canViewDevices" :lg="8" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="台" title="设备总数" :value="overview.totalDevices" />
          </Card>
        </Col>
        <Col v-if="canViewStatistics" :lg="8" :md="24" :xs="24">
          <Card hoverable>
            <Row :gutter="16">
              <Col :span="12">
                <Statistic :loading="loading" title="缺失数据" :value="overview.missingDataCount" />
              </Col>
              <Col :span="12">
                <Statistic :loading="loading" title="待补充" :value="overview.pendingSupplementCount" />
              </Col>
            </Row>
          </Card>
        </Col>
      </Row>

      <Card title="快捷入口">
        <Row :gutter="[12, 12]">
          <Col v-for="entry in quickEntries" :key="entry.path" :lg="6" :md="12" :xs="24">
            <Card hoverable class="cursor-pointer transition-all hover:shadow-lg" @click="router.push(entry.path)">
              <div class="flex items-center gap-4">
                <div :class="[entry.bg, entry.text]" class="w-12 h-12 rounded-lg flex items-center justify-center text-xl">
                  {{ entry.icon }}
                </div>
                <div>
                  <div class="font-medium">{{ entry.title }}</div>
                  <div class="text-xs text-text-secondary">{{ entry.description }}</div>
                </div>
              </div>
            </Card>
          </Col>
        </Row>
      </Card>
    </div>
  </Page>
</template>
