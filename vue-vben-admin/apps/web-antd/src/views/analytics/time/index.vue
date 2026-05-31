<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, message, Row, Spin, Statistic } from 'ant-design-vue';

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
  } catch {
    message.error('加载时间数据失败');
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
    <Spin :spinning="loading">
      <Row :gutter="[16, 16]" class="mb-4">
        <Col :lg="6" :md="12" :xs="24">
          <Card>
            <Statistic title="日均工时" :value="overview.dailyWorkHours" suffix="小时" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card>
            <Statistic title="日均学习" :value="overview.dailyLearningHours" suffix="小时" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card>
            <Statistic title="日均休息" :value="overview.dailyRestHours" suffix="小时" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card>
            <Statistic title="时间利用率" :value="overview.timeUtilizationRate" suffix="%" />
          </Card>
        </Col>
      </Row>

      <Row :gutter="[16, 16]">
        <Col :lg="12" :xs="24">
          <Card title="时间分配">
            <div v-if="hourlyDistribution.length" class="h-64">
              <div
                v-for="item in hourlyDistribution"
                :key="item.hour"
                class="flex items-center justify-between border-b py-1"
              >
                <span>{{ item.hour }}:00</span>
                <span class="text-gray-500">{{ item.category }}</span>
                <span class="font-bold">{{ item.value }}分钟</span>
              </div>
            </div>
            <div v-else class="h-64 flex items-center justify-center text-gray-400">
              暂无数据
            </div>
          </Card>
        </Col>
        <Col :lg="12" :xs="24">
          <Card title="时间趋势">
            <div v-if="weeklyTrend.length" class="h-64">
              <div
                v-for="item in weeklyTrend"
                :key="item.date"
                class="flex items-center justify-between border-b py-1"
              >
                <span>{{ item.date }}</span>
                <span>工作: {{ item.workHours }}h</span>
                <span>学习: {{ item.learningHours }}h</span>
              </div>
            </div>
            <div v-else class="h-64 flex items-center justify-center text-gray-400">
              暂无数据
            </div>
          </Card>
        </Col>
      </Row>
    </Spin>
  </Page>
</template>
