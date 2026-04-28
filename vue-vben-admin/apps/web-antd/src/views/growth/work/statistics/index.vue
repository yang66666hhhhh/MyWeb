<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Card, Col, DatePicker, Form, Row, Select, Space, Statistic, Table, Tag } from 'ant-design-vue';
import type { Dayjs } from 'dayjs';
import dayjs from 'dayjs';

import type {
  WorkStatisticsDailyHours,
  WorkStatisticsDeviceRanking,
  WorkStatisticsOverview,
  WorkStatisticsProjectHours,
  WorkStatisticsTaskTypeDistribution,
} from '#/api/growth/work';
import {
  getWorkStatisticsDeviceRankingApi,
  getWorkStatisticsDailyHoursApi,
  getWorkStatisticsOverviewApi,
  getWorkStatisticsProjectHoursApi,
  getWorkStatisticsTaskTypeDistributionApi,
} from '#/api/growth/work';

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
const dailyHours = ref<WorkStatisticsDailyHours[]>([]);
const projectHours = ref<WorkStatisticsProjectHours[]>([]);
const taskTypeDistribution = ref<WorkStatisticsTaskTypeDistribution[]>([]);
const deviceRanking = ref<WorkStatisticsDeviceRanking[]>([]);

const queryParams = ref({
  endDate: dayjs().format('YYYY-MM-DD'),
  projectId: undefined as string | undefined,
  startDate: dayjs().subtract(7, 'day').format('YYYY-MM-DD'),
});

const projectOptions = [
  { label: '全部项目', value: undefined },
  { label: '生产线升级项目', value: 'project-1' },
  { label: '质量改进项目', value: 'project-2' },
];

async function load() {
  loading.value = true;
  try {
    const [overviewData, dailyData, projectData, taskTypeData, deviceData] = await Promise.all([
      getWorkStatisticsOverviewApi(queryParams.value),
      getWorkStatisticsDailyHoursApi(queryParams.value),
      getWorkStatisticsProjectHoursApi(queryParams.value),
      getWorkStatisticsTaskTypeDistributionApi(queryParams.value),
      getWorkStatisticsDeviceRankingApi(queryParams.value),
    ]);
    overview.value = overviewData;
    dailyHours.value = dailyData;
    projectHours.value = projectData;
    taskTypeDistribution.value = taskTypeData;
    deviceRanking.value = deviceData;
  } finally {
    loading.value = false;
  }
}

function onDateRangeChange(values: [string, string] | [Dayjs, Dayjs] | null) {
  const start = values?.[0];
  const end = values?.[1];
  queryParams.value.startDate = start && typeof start !== 'string' ? start.format('YYYY-MM-DD') : undefined;
  queryParams.value.endDate = end && typeof end !== 'string' ? end.format('YYYY-MM-DD') : undefined;
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="工作数据统计分析" title="工作统计">
    <div class="space-y-4">
      <Card>
        <Form :model="queryParams" layout="inline">
          <Form.Item label="日期范围">
            <DatePicker.RangePicker
              :value="[dayjs(queryParams.startDate), dayjs(queryParams.endDate)]"
              format="YYYY-MM-DD"
              style="width: 260px"
              @change="onDateRangeChange"
            />
          </Form.Item>
          <Form.Item label="项目">
            <Select v-model:value="queryParams.projectId" :options="projectOptions" allow-clear class="w-40" />
          </Form.Item>
          <Form.Item>
            <Space>
              <button class="ant-btn ant-btn-primary" @click="load">查询</button>
            </Space>
          </Form.Item>
        </Form>
      </Card>

      <Row :gutter="[16, 16]">
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="条" title="工作日志总数" :value="overview.totalLogs">
              <template #prefix>
                <span class="text-blue-500">📋</span>
              </template>
            </Statistic>
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="小时" title="总工作时长" :value="overview.totalHours">
              <template #prefix>
                <span class="text-green-500">⏱️</span>
              </template>
            </Statistic>
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="条" title="缺失数据" :value="overview.missingDataCount">
              <template #prefix>
                <span class="text-orange-500">⚠️</span>
              </template>
            </Statistic>
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card hoverable>
            <Statistic :loading="loading" suffix="条" title="待补充" :value="overview.pendingSupplementCount">
              <template #prefix>
                <span class="text-red-500">❌</span>
              </template>
            </Statistic>
          </Card>
        </Col>
      </Row>

      <Row :gutter="[16, 16]">
        <Col :lg="12" :xs="24">
          <Card title="每日工时趋势">
            <Table
              :columns="[
                { dataIndex: 'date', key: 'date', title: '日期' },
                { dataIndex: 'hours', key: 'hours', title: '工时' },
                { dataIndex: 'logCount', key: 'logCount', title: '日志数' },
              ]"
              :data-source="dailyHours"
              :loading="loading"
              :pagination="false"
              row-key="date"
              size="small"
            />
          </Card>
        </Col>
        <Col :lg="12" :xs="24">
          <Card title="项目工时分布">
            <Table
              :columns="[
                { dataIndex: 'projectName', key: 'projectName', title: '项目' },
                { dataIndex: 'totalHours', key: 'totalHours', title: '工时' },
                { dataIndex: 'logCount', key: 'logCount', title: '日志数' },
                { dataIndex: 'percentage', key: 'percentage', title: '占比' },
              ]"
              :data-source="projectHours"
              :loading="loading"
              :pagination="false"
              row-key="projectId"
              size="small"
            >
              <template #bodyCell="{ column, text }">
                <template v-if="column.key === 'percentage'">
                  <span class="text-blue-500">{{ text }}%</span>
                </template>
              </template>
            </Table>
          </Card>
        </Col>
      </Row>

      <Row :gutter="[16, 16]">
        <Col :lg="12" :xs="24">
          <Card title="任务类型分布">
            <Table
              :columns="[
                { dataIndex: 'taskTypeName', key: 'taskTypeName', title: '类型' },
                { dataIndex: 'totalHours', key: 'totalHours', title: '工时' },
                { dataIndex: 'logCount', key: 'logCount', title: '日志数' },
                { dataIndex: 'percentage', key: 'percentage', title: '占比' },
              ]"
              :data-source="taskTypeDistribution"
              :loading="loading"
              :pagination="false"
              row-key="taskTypeId"
              size="small"
            >
              <template #bodyCell="{ column, text }">
                <template v-if="column.key === 'percentage'">
                  <span class="text-purple-500">{{ text }}%</span>
                </template>
              </template>
            </Table>
          </Card>
        </Col>
        <Col :lg="12" :xs="24">
          <Card title="设备使用排行">
            <Table
              :columns="[
                { dataIndex: 'ranking', key: 'ranking', title: '排名', width: 60 },
                { dataIndex: 'deviceName', key: 'deviceName', title: '设备' },
                { dataIndex: 'totalHours', key: 'totalHours', title: '工时' },
                { dataIndex: 'logCount', key: 'logCount', title: '日志数' },
              ]"
              :data-source="deviceRanking"
              :loading="loading"
              :pagination="false"
              row-key="deviceId"
              size="small"
            >
              <template #bodyCell="{ column, text }">
                <template v-if="column.key === 'ranking'">
                  <Tag v-if="text === 1" color="gold">{{ text }}</Tag>
                  <Tag v-else-if="text === 2" color="silver">{{ text }}</Tag>
                  <Tag v-else-if="text === 3" color="bronze">{{ text }}</Tag>
                  <Tag v-else>{{ text }}</Tag>
                </template>
              </template>
            </Table>
          </Card>
        </Col>
      </Row>
    </div>
  </Page>
</template>
