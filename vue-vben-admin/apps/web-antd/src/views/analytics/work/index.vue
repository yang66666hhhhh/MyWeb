<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Alert, Card, Col, Row, Statistic } from 'ant-design-vue';
</script>

<template>
  <Page description="分析工作效率和产出" title="工作分析">
    <Alert
      class="mb-4"
      message="功能开发中"
      description="后端API正在开发中，当前为模拟数据"
      show-icon
      type="warning"
    />
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月工时" :value="168" suffix="小时" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="完成任务" :value="42" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="项目数" :value="3" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="效率指数" :value="85" suffix="%" /></Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :lg="12" :xs="24">
        <Card title="工时分布">
          <div class="h-64 flex items-center justify-center text-gray-400">
            图表区域 - 项目工时分布
          </div>
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="任务完成趋势">
          <div class="h-64 flex items-center justify-center text-gray-400">
            图表区域 - 每日任务完成趋势
          </div>
        </Card>
      </Col>
    </Row>
  </Page>
</template>