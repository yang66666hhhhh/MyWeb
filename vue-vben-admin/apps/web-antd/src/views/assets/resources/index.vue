<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, Empty, Row, Statistic } from 'ant-design-vue';

const loading = ref(false);
</script>

<template>
  <Page description="管理订阅和云服务资源" title="资源中心">
    <div class="space-y-4">
      <Row :gutter="[16, 16]">
        <Col :lg="6" :md="12" :xs="24">
          <Card :loading="loading">
            <Statistic title="资源总数" :value="0" suffix="个" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card :loading="loading">
            <Statistic title="月费用" prefix="¥" :value="0" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card :loading="loading">
            <Statistic title="年费用" prefix="¥" :value="0" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card :loading="loading">
            <Statistic title="即将到期" :value="0" suffix="个" />
          </Card>
        </Col>
      </Row>

      <Card title="资源列表">
        <Empty description="资源管理功能正在开发中，敬请期待！" />
      </Card>
    </div>
  </Page>
</template>
