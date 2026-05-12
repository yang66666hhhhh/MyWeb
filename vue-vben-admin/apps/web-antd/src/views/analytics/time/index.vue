<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Alert, Card, Col, Row, Statistic } from 'ant-design-vue';
</script>

<template>
  <Page description="分析时间分配和使用效率" title="时间分析">
    <Alert
      class="mb-4"
      message="功能开发中"
      description="后端API正在开发中，当前为模拟数据"
      show-icon
      type="warning"
    />
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="日均工时" :value="8.5" suffix="小时" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="日均学习" :value="2.5" suffix="小时" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="日均休息" :value="7" suffix="小时" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="时间利用率" :value="68" suffix="%" /></Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :lg="12" :xs="24">
        <Card title="时间分配">
          <div class="h-64 flex items-center justify-center text-gray-400">
            图表区域 - 24小时时间分配
          </div>
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="时间趋势">
          <div class="h-64 flex items-center justify-center text-gray-400">
            图表区域 - 每日时间分配趋势
          </div>
        </Card>
      </Col>
    </Row>
  </Page>
</template>