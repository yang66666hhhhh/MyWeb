<script lang="ts" setup>

import { Page } from '@vben/common-ui';

import { Alert, Card, Col, Row, Statistic } from 'ant-design-vue';
</script>

<template>
  <Page description="分析习惯养成和坚持情况" title="习惯分析">
    <Alert
      class="mb-4"
      message="功能开发中"
      description="后端API正在开发中，当前为模拟数据"
      show-icon
      type="warning"
    />
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="活跃习惯" :value="5" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月打卡" :value="85" suffix="次" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="最长连续" :value="21" suffix="天" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="完成率" :value="78" suffix="%" /></Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :lg="12" :xs="24">
        <Card title="习惯打卡趋势">
          <div class="h-64 flex items-center justify-center text-gray-400">
            图表区域 - 每日打卡趋势
          </div>
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="习惯完成率">
          <div class="h-64 flex items-center justify-center text-gray-400">
            图表区域 - 各习惯完成率
          </div>
        </Card>
      </Col>
    </Row>
  </Page>
</template>