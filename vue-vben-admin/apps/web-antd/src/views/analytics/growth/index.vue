<script lang="ts" setup>
import type { DashboardOverview, TaskDistribution, TaskPriorityDistribution } from '#/api/analytics';
import type {
  GoalCompletion,
  HabitHeatmapData,
  LearningTimeTrend,
  SkillMastery,
} from '#/api/analytics/extended';

import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, message, Row, Statistic } from 'ant-design-vue';

import { analyticsApi } from '#/api/analytics';
import {
  getGoalCompletionApi,
  getHabitHeatmapApi,
  getLearningTimeTrendApi,
  getSkillMasteryApi,
} from '#/api/analytics/extended';

import GoalCompletionChart from '../components/GoalCompletionChart.vue';
import HabitHeatmapChart from '../components/HabitHeatmapChart.vue';
import LearningTimeChart from '../components/LearningTimeChart.vue';
import PriorityBarChart from '../components/PriorityBarChart.vue';
import SkillRadarChart from '../components/SkillRadarChart.vue';
import TaskPieChart from '../components/TaskPieChart.vue';

const loading = ref(false);
const overview = ref<DashboardOverview | null>(null);
const taskDistribution = ref<TaskDistribution[]>([]);
const priorityDistribution = ref<TaskPriorityDistribution[]>([]);
const skillMastery = ref<SkillMastery[]>([]);
const learningTimeTrend = ref<LearningTimeTrend[]>([]);
const goalCompletion = ref<GoalCompletion[]>([]);
const habitHeatmap = ref<HabitHeatmapData[]>([]);

async function fetchData() {
  loading.value = true;
  try {
    const [overviewRes, distRes, priorityRes, skillRes, learningRes, goalRes, heatmapRes] =
      await Promise.allSettled([
        analyticsApi.getDashboard(),
        analyticsApi.getTaskDistribution(),
        analyticsApi.getPriorityDistribution(),
        getSkillMasteryApi(),
        getLearningTimeTrendApi(),
        getGoalCompletionApi(),
        getHabitHeatmapApi(),
      ]);
    if (overviewRes.status === 'fulfilled') overview.value = overviewRes.value;
    if (distRes.status === 'fulfilled') taskDistribution.value = distRes.value;
    if (priorityRes.status === 'fulfilled') priorityDistribution.value = priorityRes.value;
    if (skillRes.status === 'fulfilled') skillMastery.value = skillRes.value;
    if (learningRes.status === 'fulfilled') learningTimeTrend.value = learningRes.value;
    if (goalRes.status === 'fulfilled') goalCompletion.value = goalRes.value;
    if (heatmapRes.status === 'fulfilled') habitHeatmap.value = heatmapRes.value;
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载成长数据失败');
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
        <Card title="技能掌握度">
          <SkillRadarChart :data="skillMastery" :loading="loading" />
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="学习时间趋势">
          <LearningTimeChart :data="learningTimeTrend" :loading="loading" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mt-4">
      <Col :lg="12" :xs="24">
        <Card title="目标完成率">
          <GoalCompletionChart :data="goalCompletion" :loading="loading" />
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="任务类型分布">
          <TaskPieChart :data="taskDistribution" :loading="loading" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mt-4">
      <Col :lg="12" :xs="24">
        <Card title="习惯打卡热力图">
          <HabitHeatmapChart :data="habitHeatmap" :loading="loading" />
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
