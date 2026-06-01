<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, message, Row, Spin, Statistic } from 'ant-design-vue';

import type {
  ExpenseBreakdown,
  FinanceAnalyticsOverview,
  MonthlyFinanceTrend,
} from '#/api/analytics/extended';

import {
  getExpenseBreakdownApi,
  getFinanceOverviewApi,
  getMonthlyFinanceTrendApi,
} from '#/api/analytics/extended';

const loading = ref(false);
const overview = ref<FinanceAnalyticsOverview>({
  monthlyBalance: 0,
  savingsRate: 0,
  investmentReturn: 0,
  budgetExecution: 0,
});
const monthlyTrend = ref<MonthlyFinanceTrend[]>([]);
const expenseBreakdown = ref<ExpenseBreakdown[]>([]);

async function fetchData() {
  loading.value = true;
  try {
    const [overviewRes, trendRes, expenseRes] = await Promise.allSettled([
      getFinanceOverviewApi(),
      getMonthlyFinanceTrendApi(),
      getExpenseBreakdownApi(),
    ]);
    if (overviewRes.status === 'fulfilled') overview.value = overviewRes.value;
    if (trendRes.status === 'fulfilled') monthlyTrend.value = trendRes.value;
    if (expenseRes.status === 'fulfilled') expenseBreakdown.value = expenseRes.value;
  } catch (e: any) {
    message.error(e?.message || '加载财务数据失败');
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  void fetchData();
});
</script>

<template>
  <Page description="分析财务状况和趋势" title="财务分析">
    <Spin :spinning="loading">
      <Row :gutter="[16, 16]" class="mb-4">
        <Col :lg="6" :md="12" :xs="24">
          <Card><Statistic title="本月结余" prefix="¥" :value="overview.monthlyBalance" /></Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card><Statistic title="储蓄率" :value="overview.savingsRate" suffix="%" /></Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card><Statistic title="投资收益" prefix="¥" :value="overview.investmentReturn" /></Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card><Statistic title="预算执行" :value="overview.budgetExecution" suffix="%" /></Card>
        </Col>
      </Row>

      <Row :gutter="[16, 16]">
        <Col :lg="12" :xs="24">
          <Card title="收支趋势">
            <div v-if="monthlyTrend.length" class="h-64">
              <div
                v-for="item in monthlyTrend"
                :key="item.month"
                class="flex items-center justify-between border-b py-1"
              >
                <span>{{ item.month }}</span>
                <span class="text-green-500">收入: ¥{{ item.income }}</span>
                <span class="text-red-500">支出: ¥{{ item.expense }}</span>
              </div>
            </div>
            <div v-else class="h-64 flex items-center justify-center text-gray-400">
              暂无数据
            </div>
          </Card>
        </Col>
        <Col :lg="12" :xs="24">
          <Card title="支出分类">
            <div v-if="expenseBreakdown.length" class="h-64">
              <div
                v-for="item in expenseBreakdown"
                :key="item.category"
                class="flex items-center justify-between border-b py-1"
              >
                <span>{{ item.category }}</span>
                <span>¥{{ item.amount }}</span>
                <span class="text-gray-500">{{ item.percentage }}%</span>
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
