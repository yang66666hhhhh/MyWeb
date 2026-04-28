<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  AreaChartOutlined,
  BarChartOutlined,
  CalendarOutlined,
  CheckCircleOutlined,
  CloseCircleOutlined,
  Col,
  ExclamationCircleOutlined,
  FileExcelOutlined,
  LineChartOutlined,
  PieChartOutlined,
  ProfileOutlined,
  ProjectOutlined,
  Row,
  TeamOutlined,
} from '@ant-design/icons-vue';
import { Card, Col, Row as AntRow, Statistic } from 'ant-design-vue';

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

const shortcuts = [
  { key: 'daily-plan', icon: CalendarOutlined, title: '每日计划', description: '管理工作计划', color: 'blue' },
  { key: 'log', icon: ProfileOutlined, title: '工作日志', description: '记录日常工作', color: 'green' },
  { key: 'import', icon: FileExcelOutlined, title: '工作导入', description: 'Excel批量导入', color: 'orange' },
  { key: 'project', icon: ProjectOutlined, title: '项目管理', description: '管理工作项目', color: 'purple' },
];

const statusCards = [
  { key: 'normal', title: '正常', icon: CheckCircleOutlined, color: 'success' },
  { key: 'missing', title: '缺失数据', icon: ExclamationCircleOutlined, color: 'warning' },
  { key: 'pending', title: '待补充', icon: CloseCircleOutlined, color: 'error' },
];

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
      <AntRow :gutter="[16, 16]">
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="条" title="工作日志总数" :value="overview.totalLogs">
              <template #prefix>
                <ProfileOutlined class="text-blue-500" />
              </template>
            </Statistic>
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="小时" title="总工作时长" :value="overview.totalHours">
              <template #prefix>
                <AreaChartOutlined class="text-green-500" />
              </template>
            </Statistic>
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="条" title="今日工作日志" :value="overview.todayLogs">
              <template #prefix>
                <CalendarOutlined class="text-orange-500" />
              </template>
            </Statistic>
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="小时" title="今日工作时长" :value="overview.todayHours">
              <template #prefix>
                <LineChartOutlined class="text-purple-500" />
              </template>
            </Statistic>
          </Card>
        </Col>
      </AntRow>

      <AntRow :gutter="[16, 16]">
        <Col :lg="8" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="个" title="项目总数" :value="overview.totalProjects">
              <template #prefix>
                <ProjectOutlined class="text-cyan-500" />
              </template>
            </Statistic>
          </Card>
        </Col>
        <Col :lg="8" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="台" title="设备总数" :value="overview.totalDevices">
              <template #prefix>
                <TeamOutlined class="text-blue-500" />
              </template>
            </Statistic>
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
      </AntRow>

      <Card title="快捷入口">
        <Row :gutter="[12, 12]">
          <Col v-for="item in shortcuts" :key="item.key" :lg="6" :md="12" :xs="24">
            <Card hoverable class="cursor-pointer transition-all hover:shadow-lg">
              <div class="flex items-center gap-4">
                <div
                  class="w-12 h-12 rounded-lg flex items-center justify-center text-xl"
                  :style="{ backgroundColor: `var(--ant-${item.color})` + '15' }"
                >
                  <component :is="item.icon" :style="{ color: `var(--ant-${item.color})` }" />
                </div>
                <div>
                  <div class="font-medium">{{ item.title }}</div>
                  <div class="text-xs text-text-secondary">{{ item.description }}</div>
                </div>
              </div>
            </Card>
          </Col>
        </Row>
      </Card>

      <Card title="工作统计">
        <Row :gutter="[16, 16]">
          <Col :lg="12" :md="24">
            <Card size="small" title="项目工时分布">
              <div class="h-40 flex items-center justify-center text-text-secondary">
                <BarChartOutlined class="mr-2" />
                图表占位 - 项目工时统计
              </div>
            </Card>
          </Col>
          <Col :lg="12" :md="24">
            <Card size="small" title="任务类型分布">
              <div class="h-40 flex items-center justify-center text-text-secondary">
                <PieChartOutlined class="mr-2" />
                图表占位 - 任务类型统计
              </div>
            </Card>
          </Col>
        </Row>
      </Card>
    </div>
  </Page>
</template>
