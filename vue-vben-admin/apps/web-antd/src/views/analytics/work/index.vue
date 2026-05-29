<script lang="ts" setup>
import type { WorkStatisticsOverview } from '#/api/work/statistics';

import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, Row, Statistic, Table } from 'ant-design-vue';

import { statisticsApi } from '#/api/work/statistics';

const loading = ref(false);
const overview = ref<WorkStatisticsOverview | null>(null);
const projectHours = ref<Array<{ logCount: number; percentage: number; projectId: string; projectName: string; totalHours: number }>>([]);
const dailyHours = ref<Array<{ date: string; hours: number; logCount: number }>>([]);
const taskTypeDistribution = ref<Array<{ logCount: number; percentage: number; taskTypeId: string; taskTypeName: string; totalHours: number }>>([]);

const projectColumns = [
  { title: '项目', dataIndex: 'projectName', key: 'projectName' },
  { title: '工时', key: 'totalHours' },
  { title: '占比', key: 'percentage' },
  { title: '日志数', dataIndex: 'logCount', key: 'logCount' },
];

const taskTypeColumns = [
  { title: '任务类型', dataIndex: 'taskTypeName', key: 'taskTypeName' },
  { title: '工时', key: 'totalHours' },
  { title: '占比', key: 'percentage' },
  { title: '日志数', dataIndex: 'logCount', key: 'logCount' },
];

const dailyColumns = [
  { title: '日期', dataIndex: 'date', key: 'date' },
  { title: '工时', key: 'hours' },
  { title: '日志数', dataIndex: 'logCount', key: 'logCount' },
];

async function fetchData() {
  loading.value = true;
  try {
    const [overviewData, projectData, dailyData, taskTypeData] = await Promise.all([
      statisticsApi.getOverview(),
      statisticsApi.getProjectHours(),
      statisticsApi.getDailyHours(),
      statisticsApi.getTaskTypeDistribution(),
    ]);
    overview.value = overviewData;
    projectHours.value = projectData;
    dailyHours.value = dailyData;
    taskTypeDistribution.value = taskTypeData;
  } catch {
    // ignore
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  void fetchData();
});
</script>

<template>
  <Page description="分析工作效率和产出" title="工作分析">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="总工时" :value="overview?.totalHours ?? 0" suffix="小时" :precision="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="日志总数" :value="overview?.totalLogs ?? 0" suffix="条" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="项目数" :value="overview?.totalProjects ?? 0" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="设备数" :value="overview?.totalDevices ?? 0" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="今日工时" :value="overview?.todayHours ?? 0" suffix="小时" :precision="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="今日日志" :value="overview?.todayLogs ?? 0" suffix="条" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="缺失数据" :value="overview?.missingDataCount ?? 0" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="待补充" :value="overview?.pendingSupplementCount ?? 0" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="12" :xs="24">
        <Card title="项目工时分布">
          <Table
            :columns="projectColumns"
            :data-source="projectHours"
            :loading="loading"
            :pagination="false"
            row-key="projectId"
            size="small"
          >
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'totalHours'">
                {{ record.totalHours.toFixed(1) }} 小时
              </template>
              <template v-else-if="column.key === 'percentage'">
                {{ record.percentage.toFixed(1) }}%
              </template>
            </template>
          </Table>
        </Card>
      </Col>
      <Col :lg="12" :xs="24">
        <Card title="任务类型分布">
          <Table
            :columns="taskTypeColumns"
            :data-source="taskTypeDistribution"
            :loading="loading"
            :pagination="false"
            row-key="taskTypeId"
            size="small"
          >
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'totalHours'">
                {{ record.totalHours.toFixed(1) }} 小时
              </template>
              <template v-else-if="column.key === 'percentage'">
                {{ record.percentage.toFixed(1) }}%
              </template>
            </template>
          </Table>
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]">
      <Col :span="24">
        <Card title="每日工时趋势">
          <Table
            :columns="dailyColumns"
            :data-source="dailyHours"
            :loading="loading"
            :pagination="{ pageSize: 10 }"
            row-key="date"
            size="small"
          >
            <template #bodyCell="{ column, record }">
              <template v-if="column.key === 'hours'">
                {{ record.hours.toFixed(1) }} 小时
              </template>
            </template>
          </Table>
        </Card>
      </Col>
    </Row>
  </Page>
</template>
