<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, Row, Statistic } from 'ant-design-vue';
</script>

<template>
  <Page description="分析财务状况和趋势" title="财务分析">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月结余" :value="6500" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="储蓄率" :value="35" suffix="%" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="投资收益" :value="1550" prefix="¥" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="预算执行" :value="84" suffix="%" /></Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :lg="12" :xs="24">
        <Card title="收支趋势">
          <div class="h-64 flex items-center justify-center text-gray-400">
            图表区域 - 月度收支趋势
          </div>
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="支出分类">
          <div class="h-64 flex items-center justify-center text-gray-400">
            图表区域 - 支出分类饼图
          </div>
        </Card>
      </Col>
    </Row>
  </Page>
</template>
