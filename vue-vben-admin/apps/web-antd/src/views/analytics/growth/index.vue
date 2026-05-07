<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, Row, Statistic } from 'ant-design-vue';

import { analyticsApi } from '#/api/analytics';
import type { DashboardOverview, TaskDistribution, TaskPriorityDistribution } from '#/api/analytics';

import TaskPieChart from './components/TaskPieChart.vue';
import PriorityBarChart from './components/PriorityBarChart.vue';

const loading = ref(false);
const overview = ref<DashboardOverview | null>(null);
const taskDistribution = ref<TaskDistribution[]>([]);
const priorityDistribution = ref<TaskPriorityDistribution[]>([]);

async function fetchData() {
  loading.value = true;
  try {
    const [overviewData, distData, priorityData] = await Promise.all([
      analyticsApi.getDashboard(),
      analyticsApi.getTaskDistribution(),
      analyticsApi.getPriorityDistribution(),
    ]);
    overview.value = overviewData;
    taskDistribution.value = distData;
    priorityDistribution.value = priorityData;
  } catch {
    // Handle error
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  void fetchData();
});
</script>

<template>
  <Page description="分析个人成长数据和趋势" title="成长分析">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="总任务数" :value="overview?.totalTasks ?? 0" suffix="个" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="已完成" :value="overview?.completedTasks ?? 0" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="进行中" :value="overview?.pendingTasks ?? 0" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="完成率" :value="overview?.completionRate ?? 0" suffix="%" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :lg="12" :xs="24">
        <Card title="任务类型分布">
          <TaskPieChart :data="taskDistribution" :loading="loading" />
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="优先级分布">
          <PriorityBarChart :data="priorityDistribution" :loading="loading" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mt-4">
      <Col :span="24">
        <Card title="今日概览">
          <Row :gutter="16">
            <Col :span="8">
              <Statistic title="今日创建任务" :value="overview?.todayTasks ?? 0" />
            </Col>
            <Col :span="8">
              <Statistic title="今日完成" :value="overview?.todayCompletedTasks ?? 0" />
            </Col>
            <Col :span="8">
              <Statistic title="今日工时" :value="overview?.todayWorkHours ?? 0" suffix="小时" />
            </Col>
          </Row>
        </Card>
      </Col>
    </Row>
  </Page>
</template>