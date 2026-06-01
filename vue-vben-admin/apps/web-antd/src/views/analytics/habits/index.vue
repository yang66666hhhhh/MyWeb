<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, message, Row, Spin, Statistic } from 'ant-design-vue';

import type { HabitsAnalyticsOverview, HabitTrend } from '#/api/analytics/extended';

import { getHabitsOverviewApi, getHabitTrendsApi } from '#/api/analytics/extended';

const loading = ref(false);
const overview = ref<HabitsAnalyticsOverview>({
  activeHabits: 0,
  monthlyCheckIns: 0,
  longestStreak: 0,
  completionRate: 0,
});
const habitTrends = ref<HabitTrend[]>([]);

async function fetchData() {
  loading.value = true;
  try {
    const [overviewRes, trendsRes] = await Promise.allSettled([
      getHabitsOverviewApi(),
      getHabitTrendsApi(),
    ]);
    if (overviewRes.status === 'fulfilled') overview.value = overviewRes.value;
    if (trendsRes.status === 'fulfilled') habitTrends.value = trendsRes.value;
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
    <Spin :spinning="loading">
      <Row :gutter="[16, 16]" class="mb-4">
        <Col :lg="6" :md="12" :xs="24">
          <Card><Statistic title="活跃习惯" :value="overview.activeHabits" /></Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card><Statistic title="本月打卡" :value="overview.monthlyCheckIns" suffix="次" /></Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card><Statistic title="最长连续" :value="overview.longestStreak" suffix="天" /></Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card><Statistic title="完成率" :value="overview.completionRate" suffix="%" /></Card>
        </Col>
      </Row>

      <Row :gutter="[16, 16]">
        <Col :lg="12" :xs="24">
          <Card title="习惯打卡趋势">
            <div v-if="habitTrends.length" class="h-64">
              <div
                v-for="item in habitTrends"
                :key="item.date"
                class="flex items-center justify-between border-b py-1"
              >
                <span>{{ item.date }}</span>
                <span>打卡: {{ item.checkIns }}次</span>
                <span>完成率: {{ item.completionRate }}%</span>
              </div>
            </div>
            <div v-else class="h-64 flex items-center justify-center text-gray-400">
              暂无数据
            </div>
          </Card>
        </Col>
        <Col :lg="12" :xs="24">
          <Card title="习惯完成率">
            <div v-if="habitTrends.length" class="h-64">
              <div
                v-for="item in habitTrends"
                :key="item.date"
                class="flex items-center justify-between border-b py-1"
              >
                <span>{{ item.date }}</span>
                <div class="w-32">
                  <div class="mb-1 text-right text-sm">{{ item.completionRate }}%</div>
                  <div class="h-2 rounded-full bg-gray-200">
                    <div
                      class="h-full rounded-full bg-green-500"
                      :style="{ width: `${item.completionRate}%` }"
                    ></div>
                  </div>
                </div>
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
