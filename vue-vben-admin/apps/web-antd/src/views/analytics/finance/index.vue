<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, message, Row, Statistic } from 'ant-design-vue';

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

import ExpensePieChart from '../components/ExpensePieChart.vue';
import FinanceTrendChart from '../components/FinanceTrendChart.vue';

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
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载财务数据失败');
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
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="本月结余" prefix="¥" :value="overview.monthlyBalance" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="储蓄率" :value="overview.savingsRate" suffix="%" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="投资收益" prefix="¥" :value="overview.investmentReturn" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="预算执行" :value="overview.budgetExecution" suffix="%" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :lg="12" :xs="24">
        <Card title="收支趋势对比">
          <FinanceTrendChart :data="monthlyTrend" :loading="loading" />
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="支出分类占比">
          <ExpensePieChart :data="expenseBreakdown" :loading="loading" />
        </Card>
      </Col>
    </Row>
  </Page>
</template>
