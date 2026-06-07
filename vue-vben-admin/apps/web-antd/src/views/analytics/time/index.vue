<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, message, Row, Statistic } from 'ant-design-vue';

import type {
  HourlyDistribution,
  TimeAnalyticsOverview,
  WeeklyTrend,
} from '#/api/analytics/extended';

import {
  getHourlyDistributionApi,
  getTimeOverviewApi,
  getWeeklyTrendApi,
} from '#/api/analytics/extended';

import TimeAllocationChart from '../components/TimeAllocationChart.vue';
import TimeUtilizationChart from '../components/TimeUtilizationChart.vue';

const loading = ref(false);
const overview = ref<TimeAnalyticsOverview>({
  dailyWorkHours: 0,
  dailyLearningHours: 0,
  dailyRestHours: 0,
  timeUtilizationRate: 0,
});
const hourlyDistribution = ref<HourlyDistribution[]>([]);
const weeklyTrend = ref<WeeklyTrend[]>([]);

async function fetchData() {
  loading.value = true;
  try {
    const [overviewRes, hourlyRes, trendRes] = await Promise.allSettled([
      getTimeOverviewApi(),
      getHourlyDistributionApi(),
      getWeeklyTrendApi(),
    ]);
    if (overviewRes.status === 'fulfilled') overview.value = overviewRes.value;
    if (hourlyRes.status === 'fulfilled') hourlyDistribution.value = hourlyRes.value;
    if (trendRes.status === 'fulfilled') weeklyTrend.value = trendRes.value;
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载时间数据失败');
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  void fetchData();
});
</script>

<template>
  <Page description="分析时间分配和使用效率" title="时间分析">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="日均工时" :value="overview.dailyWorkHours" suffix="小时" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="日均学习" :value="overview.dailyLearningHours" suffix="小时" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="日均休息" :value="overview.dailyRestHours" suffix="小时" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="时间利用率" :value="overview.timeUtilizationRate" suffix="%" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :lg="12" :xs="24">
        <Card title="时间分配">
          <TimeAllocationChart :data="hourlyDistribution" :loading="loading" />
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="每日时间利用效率">
          <TimeUtilizationChart :data="weeklyTrend" :loading="loading" />
        </Card>
      </Col>
    </Row>
  </Page>
</template>
