<script lang="ts" setup>
import type {
  HabitsAnalyticsOverview,
  HabitHeatmapData,
  HabitTrend,
} from '#/api/analytics/extended';

import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, message, Row, Statistic } from 'ant-design-vue';

import {
  getHabitsOverviewApi,
  getHabitHeatmapApi,
  getHabitTrendsApi,
} from '#/api/analytics/extended';

import HabitCompletionChart from '../components/HabitCompletionChart.vue';
import HabitHeatmapChart from '../components/HabitHeatmapChart.vue';

const loading = ref(false);
const overview = ref<HabitsAnalyticsOverview>({
  activeHabits: 0,
  monthlyCheckIns: 0,
  longestStreak: 0,
  completionRate: 0,
});
const habitTrends = ref<HabitTrend[]>([]);
const habitHeatmap = ref<HabitHeatmapData[]>([]);

async function fetchData() {
  loading.value = true;
  try {
    const [overviewRes, trendsRes, heatmapRes] = await Promise.allSettled([
      getHabitsOverviewApi(),
      getHabitTrendsApi(),
      getHabitHeatmapApi(),
    ]);
    if (overviewRes.status === 'fulfilled') overview.value = overviewRes.value;
    if (trendsRes.status === 'fulfilled') habitTrends.value = trendsRes.value;
    if (heatmapRes.status === 'fulfilled') habitHeatmap.value = heatmapRes.value;
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载习惯数据失败');
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  void fetchData();
});
</script>

<template>
  <Page description="分析习惯养成和坚持情况" title="习惯分析">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="活跃习惯" :value="overview.activeHabits" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="本月打卡" :value="overview.monthlyCheckIns" suffix="次" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="最长连续" :value="overview.longestStreak" suffix="天" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="完成率" :value="overview.completionRate" suffix="%" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :span="24">
        <Card title="习惯打卡热力图">
          <HabitHeatmapChart :data="habitHeatmap" :loading="loading" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mt-4">
      <Col :span="24">
        <Card title="习惯完成率趋势">
          <HabitCompletionChart :data="habitTrends" :loading="loading" />
        </Card>
      </Col>
    </Row>
  </Page>
</template>
