<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, Row, Statistic } from 'ant-design-vue';
</script>

<template>
  <Page description="财务资产总览" title="资产看板">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="总资产" :value="125000" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月收入" :value="15000" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月支出" :value="8500" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月结余" :value="6500" prefix="¥" /></Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :lg="12" :xs="24">
        <Card title="资产分布">
          <div class="space-y-4">
            <div v-for="item in [{name: '银行存款', amount: 80000}, {name: '投资理财', amount: 35000}, {name: '现金', amount: 10000}]" :key="item.name" class="flex justify-between items-center">
              <span>{{ item.name }}</span>
              <span class="font-bold">¥{{ item.amount.toLocaleString() }}</span>
            </div>
          </div>
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="近期交易">
          <div class="space-y-4">
            <div v-for="item in [{name: '工资', amount: 15000, type: 'income'}, {name: '房租', amount: -3000, type: 'expense'}, {name: '餐饮', amount: -1500, type: 'expense'}]" :key="item.name" class="flex justify-between items-center">
              <span>{{ item.name }}</span>
              <span :class="item.type === 'income' ? 'text-green-500' : 'text-red-500'">
                {{ item.type === 'income' ? '+' : '' }}¥{{ item.amount.toLocaleString() }}
              </span>
            </div>
          </div>
        </Card>
      </Col>
    </Row>
  </Page>
</template>
