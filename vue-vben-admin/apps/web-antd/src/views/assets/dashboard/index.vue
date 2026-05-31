<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, message, Row, Statistic, Table, Tag } from 'ant-design-vue';

import type { AssetSummary, Income } from '#/api/assets';

import { assetApi } from '#/api/assets';

const loading = ref(false);
const summary = ref<AssetSummary>({
  totalIncome: 0,
  totalExpense: 0,
  totalInvestment: 0,
  netAsset: 0,
  incomeCount: 0,
  expenseCount: 0,
  investmentCount: 0,
});

const recentTransactions = ref<Array<{
  amount: number;
  category: string;
  date: string;
  id: string;
  title: string;
  type: 'expense' | 'income';
}>>([]);

const fetchSummary = async () => {
  loading.value = true;
  try {
    const data = await assetApi.getSummary();
    summary.value = data;
  } catch {
    message.error('加载资产数据失败');
  } finally {
    loading.value = false;
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
      ? expenseRes.value.items.map((item: any) => ({
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
  } catch {
    message.error('加载交易记录失败');
  }
};

const transactionColumns = [
  { title: '标题', dataIndex: 'title', key: 'title' },
  { title: '分类', dataIndex: 'category', key: 'category' },
  { title: '日期', dataIndex: 'date', key: 'date' },
  { title: '金额', key: 'amount' },
];

onMounted(() => {
  fetchSummary();
  fetchRecentTransactions();
});
</script>

<template>
  <Page description="财务资产总览" title="资产看板">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="净资产" :value="summary.netAsset" prefix="¥" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="总收入" :value="summary.totalIncome" prefix="¥" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="总支出" :value="summary.totalExpense" prefix="¥" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="总投资" :value="summary.totalInvestment" prefix="¥" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :lg="12" :xs="24">
        <Card title="资产概况">
          <div class="space-y-4">
            <div class="flex justify-between items-center">
              <span>收入记录数</span>
              <span class="font-bold">{{ summary.incomeCount }} 条</span>
            </div>
            <div class="flex justify-between items-center">
              <span>支出记录数</span>
              <span class="font-bold">{{ summary.expenseCount }} 条</span>
            </div>
            <div class="flex justify-between items-center">
              <span>投资记录数</span>
              <span class="font-bold">{{ summary.investmentCount }} 条</span>
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
