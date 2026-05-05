<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, Row, Statistic } from 'ant-design-vue';
</script>

<template>
  <Page description="分析个人成长数据和趋势" title="成长分析">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="学习天数" :value="45" suffix="天" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="完成任务" :value="128" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="技能提升" :value="12" suffix="项" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="成长指数" :value="78" suffix="%" /></Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :lg="12" :xs="24">
        <Card title="学习趋势">
          <div class="h-64 flex items-center justify-center text-gray-400">
            图表区域 - 学习时间趋势
          </div>
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="技能分布">
          <div class="h-64 flex items-center justify-center text-gray-400">
            图表区域 - 技能雷达图
          </div>
        </Card>
      </Col>
    </Row>
  </Page>
</template>
