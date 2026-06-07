<script lang="ts" setup>
import type { WorkStatisticsOverview } from '#/api/work/statistics';

import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, message, Row, Statistic } from 'ant-design-vue';

import { statisticsApi } from '#/api/work/statistics';

import ProjectHoursPieChart from '../components/ProjectHoursPieChart.vue';
import TaskCompletionBarChart from '../components/TaskCompletionBarChart.vue';
import WorkEfficiencyChart from '../components/WorkEfficiencyChart.vue';

const loading = ref(false);
const overview = ref<WorkStatisticsOverview | null>(null);
const projectHours = ref<Array<{ logCount: number; percentage: number; projectId: string; projectName: string; totalHours: number }>>([]);
const dailyHours = ref<Array<{ date: string; hours: number; logCount: number }>>([]);

async function fetchData() {
  loading.value = true;
  try {
    const [overviewRes, projectRes, dailyRes] = await Promise.allSettled([
      statisticsApi.getOverview(),
      statisticsApi.getProjectHours(),
      statisticsApi.getDailyHours(),
    ]);
    overview.value = overviewRes.status === 'fulfilled' ? overviewRes.value : null;
    projectHours.value = projectRes.status === 'fulfilled' ? projectRes.value : [];
    dailyHours.value = dailyRes.status === 'fulfilled' ? dailyRes.value : [];
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载失败，请稍后重试');
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  void fetchData();
});
</script>

<template>
  <Page description="分析工作效率和产出" title="工作分析">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="总工时" :value="overview?.totalHours ?? 0" suffix="小时" :precision="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="日志总数" :value="overview?.totalLogs ?? 0" suffix="条" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="项目数" :value="overview?.totalProjects ?? 0" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="设备数" :value="overview?.totalDevices ?? 0" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="今日工时" :value="overview?.todayHours ?? 0" suffix="小时" :precision="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="今日日志" :value="overview?.todayLogs ?? 0" suffix="条" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="缺失数据" :value="overview?.missingDataCount ?? 0" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="待补充" :value="overview?.pendingSupplementCount ?? 0" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mb-4">
      <Col :span="24">
        <Card title="工作效率趋势">
          <WorkEfficiencyChart :data="dailyHours" :loading="loading" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :lg="12" :xs="24">
        <Card title="项目工时分布">
          <ProjectHoursPieChart :data="projectHours" :loading="loading" />
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="任务完成率">
          <TaskCompletionBarChart :data="dailyHours" :loading="loading" />
        </Card>
      </Col>
    </Row>
  </Page>
</template>
