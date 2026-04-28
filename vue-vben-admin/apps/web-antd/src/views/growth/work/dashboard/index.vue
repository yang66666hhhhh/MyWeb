<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, Row, Statistic } from 'ant-design-vue';

import type { WorkStatisticsOverview } from '#/api/growth/work';
import { getWorkStatisticsOverviewApi } from '#/api/growth/work';

const loading = ref(false);
const overview = ref<WorkStatisticsOverview>({
  missingDataCount: 0,
  pendingSupplementCount: 0,
  todayHours: 0,
  todayLogs: 0,
  totalDevices: 0,
  totalHours: 0,
  totalLogs: 0,
  totalProjects: 0,
});

async function load() {
  loading.value = true;
  try {
    overview.value = await getWorkStatisticsOverviewApi();
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="工作模块总览，快速入口和关键指标" title="工作看板">
    <div class="space-y-4">
      <Row :gutter="[16, 16]">
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="条" title="工作日志总数" :value="overview.totalLogs" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="小时" title="总工作时长" :value="overview.totalHours" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="条" title="今日工作日志" :value="overview.todayLogs" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="小时" title="今日工作时长" :value="overview.todayHours" />
          </Card>
        </Col>
      </Row>

      <Row :gutter="[16, 16]">
        <Col :lg="8" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="个" title="项目总数" :value="overview.totalProjects" />
          </Card>
        </Col>
        <Col :lg="8" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="台" title="设备总数" :value="overview.totalDevices" />
          </Card>
        </Col>
        <Col :lg="8" :md="24" :xs="24">
          <Card hoverable>
            <Row :gutter="16">
              <Col :span="12">
                <Statistic :loading="loading" title="缺失数据" :value="overview.missingDataCount" />
              </Col>
              <Col :span="12">
                <Statistic :loading="loading" title="待补充" :value="overview.pendingSupplementCount" />
              </Col>
            </Row>
          </Card>
        </Col>
      </Row>

      <Card title="快捷入口">
        <Row :gutter="[12, 12]">
          <Col :lg="6" :md="12" :xs="24">
            <Card hoverable class="cursor-pointer transition-all hover:shadow-lg">
              <div class="flex items-center gap-4">
                <div class="w-12 h-12 rounded-lg flex items-center justify-center text-xl bg-blue-100 text-blue-600">
                  日
                </div>
                <div>
                  <div class="font-medium">每日计划</div>
                  <div class="text-xs text-text-secondary">管理工作计划</div>
                </div>
              </div>
            </Card>
          </Col>
          <Col :lg="6" :md="12" :xs="24">
            <Card hoverable class="cursor-pointer transition-all hover:shadow-lg">
              <div class="flex items-center gap-4">
                <div class="w-12 h-12 rounded-lg flex items-center justify-center text-xl bg-green-100 text-green-600">
                  志
                </div>
                <div>
                  <div class="font-medium">工作日志</div>
                  <div class="text-xs text-text-secondary">记录日常工作</div>
                </div>
              </div>
            </Card>
          </Col>
          <Col :lg="6" :md="12" :xs="24">
            <Card hoverable class="cursor-pointer transition-all hover:shadow-lg">
              <div class="flex items-center gap-4">
                <div class="w-12 h-12 rounded-lg flex items-center justify-center text-xl bg-orange-100 text-orange-600">
                  导
                </div>
                <div>
                  <div class="font-medium">工作导入</div>
                  <div class="text-xs text-text-secondary">Excel批量导入</div>
                </div>
              </div>
            </Card>
          </Col>
          <Col :lg="6" :md="12" :xs="24">
            <Card hoverable class="cursor-pointer transition-all hover:shadow-lg">
              <div class="flex items-center gap-4">
                <div class="w-12 h-12 rounded-lg flex items-center justify-center text-xl bg-purple-100 text-purple-600">
                  项
                </div>
                <div>
                  <div class="font-medium">项目管理</div>
                  <div class="text-xs text-text-secondary">管理工作项目</div>
                </div>
              </div>
            </Card>
          </Col>
        </Row>
      </Card>
    </div>
  </Page>
</template>
