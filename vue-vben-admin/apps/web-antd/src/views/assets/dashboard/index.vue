<script lang="ts" setup>
import type { EchartsUIType } from '@vben/plugins/echarts';

import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { EchartsUI, useEcharts } from '@vben/plugins/echarts';

import { Card, Col, message, Progress, Row, Statistic, Table, Tag } from 'ant-design-vue';

import type { AssetOverview, BudgetExecution, CategoryStat, Expense, Income, MonthlyTrend } from '#/api/assets';

import { assetApi } from '#/api/assets';

const loading = ref(false);
const overview = ref<AssetOverview>({
  totalIncome: 0,
  totalExpense: 0,
  totalInvestment: 0,
  netAsset: 0,
  monthlyIncome: 0,
  monthlyExpense: 0,
  savingsRate: 0,
});

const incomeTrend = ref<MonthlyTrend[]>([]);
const expenseTrend = ref<MonthlyTrend[]>([]);
const expenseCategories = ref<CategoryStat[]>([]);
const budgetExecution = ref<BudgetExecution[]>([]);

const recentTransactions = ref<Array<{
  amount: number;
  category: string;
  date: string;
  id: string;
  title: string;
  type: 'expense' | 'income';
}>>([]);

const trendChartRef = ref<EchartsUIType>();
const { renderEcharts: renderTrendChart } = useEcharts(trendChartRef);

const pieChartRef = ref<EchartsUIType>();
const { renderEcharts: renderPieChart } = useEcharts(pieChartRef);

const fetchOverview = async () => {
  loading.value = true;
  try {
    const data = await assetApi.getAssetOverview();
    overview.value = data;
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载资产数据失败');
  } finally {
    loading.value = false;
  }
};

const fetchChartData = async () => {
  try {
    const [trendRes, categoryRes, budgetRes] = await Promise.allSettled([
      assetApi.getIncomeTrend(6),
      assetApi.getExpenseTrend(6),
      assetApi.getExpenseCategoryStats(),
      assetApi.getBudgetExecution(),
    ]);

    if (trendRes.status === 'fulfilled') {
      incomeTrend.value = trendRes.value;
    }

    if (categoryRes.status === 'fulfilled') {
      expenseTrend.value = categoryRes.value;
    }

    if (budgetRes.status === 'fulfilled') {
      expenseCategories.value = budgetRes.value;
    }

    renderTrendChart({
      tooltip: { trigger: 'axis' as const },
      legend: { data: ['收入', '支出'] },
      grid: { left: '3%', right: '4%', bottom: '3%', containLabel: true },
      xAxis: {
        type: 'category' as const,
        data: incomeTrend.value.map((item) => item.month),
        axisTick: { show: false },
      },
      yAxis: { type: 'value' as const, axisTick: { show: false } },
      series: [
        {
          name: '收入',
          type: 'line' as const,
          smooth: true,
          areaStyle: {},
          itemStyle: { color: '#52c41a' },
          data: incomeTrend.value.map((item) => item.amount),
        },
        {
          name: '支出',
          type: 'line' as const,
          smooth: true,
          areaStyle: {},
          itemStyle: { color: '#ff4d4f' },
          data: expenseTrend.value.map((item) => item.amount),
        },
      ],
    });

    renderPieChart({
      tooltip: { trigger: 'item' as const },
      legend: { bottom: '0%', left: 'center' },
      series: [
        {
          type: 'pie' as const,
          radius: ['40%', '70%'],
          label: { show: true, formatter: '{b}: {c} ({d}%)' },
          data: expenseCategories.value.map((item) => ({
            name: item.category,
            value: item.amount,
          })),
        },
      ],
    });
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载图表数据失败');
  }
};

const fetchBudgetExecution = async () => {
  try {
    const res = await assetApi.getBudgetExecution();
    budgetExecution.value = res;
  } catch {
    // ignore
  }
};

const fetchRecentTransactions = async () => {
  try {
    const [incomeRes, expenseRes] = await Promise.allSettled([
      assetApi.getIncomes({ page: 1, pageSize: 5 }),
      assetApi.getExpenses({ page: 1, pageSize: 5 }),
    ]);

    const incomes = incomeRes.status === 'fulfilled'
      ? incomeRes.value.items.map((item: Income) => ({
          id: item.id,
          title: item.title,
          amount: item.amount,
          category: item.category,
          date: item.incomeDate,
          type: 'income' as const,
        }))
      : [];

    const expenses = expenseRes.status === 'fulfilled'
      ? expenseRes.value.items.map((item: Expense) => ({
          id: item.id,
          title: item.title,
          amount: item.amount,
          category: item.category,
          date: item.expenseDate,
          type: 'expense' as const,
        }))
      : [];

    recentTransactions.value = [...incomes, ...expenses]
      .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
      .slice(0, 10);
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载交易记录失败');
  }
};

const transactionColumns = [
  { title: '标题', dataIndex: 'title', key: 'title' },
  { title: '分类', dataIndex: 'category', key: 'category' },
  { title: '日期', dataIndex: 'date', key: 'date' },
  { title: '金额', key: 'amount' },
];

onMounted(() => {
  fetchOverview();
  fetchChartData();
  fetchBudgetExecution();
  fetchRecentTransactions();
});
</script>

<template>
  <Page description="财务资产总览" title="资产看板">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="净资产" :value="overview.netAsset" prefix="¥" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="本月收入" :value="overview.monthlyIncome" prefix="¥" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="本月支出" :value="overview.monthlyExpense" prefix="¥" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="储蓄率" :value="overview.savingsRate" suffix="%" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="12" :xs="24">
        <Card title="收支趋势（近6个月）">
          <div class="h-72">
            <EchartsUI ref="trendChartRef" />
          </div>
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="支出分类占比">
          <div class="h-72">
            <EchartsUI ref="pieChartRef" />
          </div>
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mb-4">
      <Col :span="24">
        <Card title="本月预算执行">
          <div v-if="budgetExecution.length > 0" class="space-y-4">
            <div v-for="item in budgetExecution" :key="item.category" class="flex items-center gap-4">
              <span class="w-16 text-right">{{ item.category }}</span>
              <div class="flex-1">
                <Progress
                  :percent="Math.min(item.executionRate, 100)"
                  :status="item.executionRate > 100 ? 'exception' : item.executionRate > 80 ? 'active' : 'success'"
                  :format="() => `${item.executionRate}%`"
                />
              </div>
              <span class="w-32 text-right text-sm text-gray-500">
                ¥{{ item.actualAmount.toLocaleString() }} / ¥{{ item.plannedAmount.toLocaleString() }}
              </span>
            </div>
          </div>
          <div v-else class="flex items-center justify-center h-20 text-gray-400">
            暂无预算数据
          </div>
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :lg="12" :xs="24">
        <Card title="资产概况">
          <div class="space-y-4">
            <div class="flex justify-between items-center">
              <span>总收入</span>
              <span class="font-bold text-green-500">¥{{ overview.totalIncome.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between items-center">
              <span>总支出</span>
              <span class="font-bold text-red-500">¥{{ overview.totalExpense.toLocaleString() }}</span>
            </div>
            <div class="flex justify-between items-center">
              <span>总投资</span>
              <span class="font-bold text-blue-500">¥{{ overview.totalInvestment.toLocaleString() }}</span>
            </div>
          </div>
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="近期交易">
          <Table
            :columns="transactionColumns"
            :data-source="recentTransactions"
            :pagination="false"
            :loading="loading"
            row-key="id"
            size="small"
          >
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'amount'">
                <span :class="record.type === 'income' ? 'text-green-500' : 'text-red-500'">
                  {{ record.type === 'income' ? '+' : '-' }}¥{{ record.amount.toLocaleString() }}
                </span>
              </template>
              <template v-else-if="column.key === 'category'">
                <Tag>{{ record.category }}</Tag>
              </template>
            </template>
          </Table>
        </Card>
      </Col>
    </Row>
  </Page>
</template>
