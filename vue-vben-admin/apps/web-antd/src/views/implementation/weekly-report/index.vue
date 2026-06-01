<script lang="ts" setup>
import type { Dayjs } from 'dayjs';

import { computed, onMounted, ref } from 'vue';
import { useRoute } from 'vue-router';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  DatePicker,
  Empty,
  Form,
  List,
  Popconfirm,
  Row,
  Select,
  Space,
  Statistic,
  Table,
  Tag,
  message,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import { aiApi, type AiReport } from '#/api/ai';
import { projectApi, type WorkProject } from '#/api/work/project';
import {
  getWorkStatisticsOverviewApi,
  getWorkStatisticsProjectHoursApi,
  type WorkStatisticsOverview,
  type WorkStatisticsProjectHours,
} from '#/api/work/statistics';

const route = useRoute();
const loading = ref(false);
const generating = ref(false);
const projects = ref<WorkProject[]>([]);
const reports = ref<AiReport[]>([]);
const selectedReport = ref<AiReport | null>(null);
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
const projectHours = ref<WorkStatisticsProjectHours[]>([]);

const query = ref({
  endDate: dayjs().format('YYYY-MM-DD'),
  projectId: undefined as string | undefined,
  startDate: dayjs().subtract(6, 'day').format('YYYY-MM-DD'),
});

const projectOptions = computed(() => [
  { label: '全部项目', value: undefined },
  ...projects.value.map((item) => ({
    label: item.projectName,
    value: item.id,
  })),
]);

const reportColumns: any[] = [
  { title: '项目', dataIndex: 'projectName', key: 'projectName', minWidth: 180 },
  { title: '工时', dataIndex: 'totalHours', key: 'totalHours', width: 100 },
  { title: '日志数', dataIndex: 'logCount', key: 'logCount', width: 100 },
  { title: '占比', dataIndex: 'percentage', key: 'percentage', width: 100 },
];

async function loadProjects() {
  const result = await projectApi.getPage({ page: 1, pageSize: 100 });
  projects.value = result.items;
}

async function loadReports() {
  const result = await aiApi.getReports({
    endDate: query.value.endDate,
    page: 1,
    pageSize: 10,
    relatedProjectId: query.value.projectId,
    startDate: query.value.startDate,
    type: 'Weekly',
  });
  reports.value = result.items;
  if (selectedReport.value) {
    const matched = reports.value.find((item) => item.id === selectedReport.value?.id);
    selectedReport.value = matched ?? reports.value[0] ?? null;
    return;
  }
  if (reports.value.length > 0) {
    selectedReport.value = reports.value[0] ?? null;
  }
}

async function loadDashboard() {
  loading.value = true;
  try {
    const params = {
      endDate: query.value.endDate,
      projectId: query.value.projectId,
      startDate: query.value.startDate,
    };

    const [overviewRes, projectHourRes] = await Promise.allSettled([
      getWorkStatisticsOverviewApi(params),
      getWorkStatisticsProjectHoursApi(params),
    ]);
    overview.value = overviewRes.status === 'fulfilled' ? overviewRes.value : overview.value;
    projectHours.value = projectHourRes.status === 'fulfilled' ? projectHourRes.value : [];
  } catch (e: any) {
    message.error(e?.message || '加载实施周报数据失败');
  } finally {
    loading.value = false;
  }
}

async function refreshAll() {
  loading.value = true;
  try {
    await Promise.allSettled([loadProjects(), loadReports(), loadDashboard()]);
  } finally {
    loading.value = false;
  }
}

async function generateWeeklyReport() {
  generating.value = true;
  try {
    const report = await aiApi.generateReport({
      endDate: query.value.endDate,
      includeStatistics: true,
      relatedProjectId: query.value.projectId,
      startDate: query.value.startDate,
      type: 'Weekly',
    });
    selectedReport.value = report;
    message.success('实施周报已生成');
    await loadReports();
  } catch (error: any) {
    message.error(error?.message || '生成周报失败');
  } finally {
    generating.value = false;
  }
}

function handleDateRangeChange(values: [Dayjs, Dayjs] | [string, string] | null) {
  const start = values?.[0];
  const end = values?.[1];
  query.value.startDate =
    start && typeof start !== 'string' ? start.format('YYYY-MM-DD') : '';
  query.value.endDate =
    end && typeof end !== 'string' ? end.format('YYYY-MM-DD') : '';
}

async function selectReport(report: AiReport) {
  loading.value = true;
  try {
    selectedReport.value = await aiApi.getReportById(report.id);
  } catch (e: any) {
    message.error(e?.message || '加载周报详情失败');
  } finally {
    loading.value = false;
  }
}

async function deleteReport(reportId: string) {
  try {
    await aiApi.deleteReport(reportId);
    if (selectedReport.value?.id === reportId) {
      selectedReport.value = null;
    }
    message.success('周报已删除');
    await loadReports();
  } catch (error: any) {
    message.error(error?.message || '删除周报失败');
  }
}

// 统一显示报告类型标签，避免列表里的英文枚举直接暴露给用户。
function getReportTypeLabel(type: string) {
  if (type === 'Weekly') return '周报';
  if (type === 'Monthly') return '月报';
  if (type === 'Project') return '项目报告';
  if (type === 'Custom') return '自定义';
  return '日报';
}

onMounted(() => {
  const startDate = route.query.startDate;
  const endDate = route.query.endDate;
  const projectId = route.query.projectId;

  if (typeof startDate === 'string' && startDate) {
    query.value.startDate = startDate;
  }
  if (typeof endDate === 'string' && endDate) {
    query.value.endDate = endDate;
  }
  if (typeof projectId === 'string' && projectId) {
    query.value.projectId = projectId;
  }

  void refreshAll();
});
</script>

<template>
  <Page description="汇总实施项目数据，并生成 AI 周报总结" title="实施周报">
    <div class="space-y-4">
      <Card>
        <div class="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <Form :model="query" layout="inline">
            <Form.Item label="日期范围">
              <DatePicker.RangePicker
                :value="[dayjs(query.startDate), dayjs(query.endDate)]"
                format="YYYY-MM-DD"
                style="width: 260px"
                @change="handleDateRangeChange"
              />
            </Form.Item>
            <Form.Item label="项目">
              <Select
                v-model:value="query.projectId"
                :options="projectOptions"
                allow-clear
                class="w-52"
              />
            </Form.Item>
            <Form.Item>
              <Space>
                <Button type="primary" @click="loadDashboard">刷新统计</Button>
                <Button :loading="generating" @click="generateWeeklyReport">
                  生成 AI 周报
                </Button>
              </Space>
            </Form.Item>
          </Form>
        </div>
      </Card>

      <Row :gutter="[16, 16]">
        <Col :lg="6" :md="12" :xs="24">
          <Card>
            <Statistic title="日志总数" :value="overview.totalLogs" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card>
            <Statistic title="总工时" :value="overview.totalHours" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card>
            <Statistic title="项目数" :value="overview.totalProjects" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card>
            <Statistic title="今日工时" :value="overview.todayHours" />
          </Card>
        </Col>
      </Row>

      <Row :gutter="[16, 16]">
        <Col :lg="12" :xs="24">
          <Card title="项目工时分布">
            <Table
              :columns="reportColumns"
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

        <Col :lg="12" :xs="24">
          <Card title="历史周报">
            <List :data-source="reports" :loading="loading">
              <template #renderItem="{ item }">
                <List.Item class="cursor-pointer">
                  <List.Item.Meta
                    @click="selectReport(item)"
                  >
                    <template #title>
                      <div class="flex items-center gap-2">
                        <span class="font-medium">{{ item.title }}</span>
                        <Tag v-if="item.relatedProjectName" color="cyan">
                          {{ item.relatedProjectName }}
                        </Tag>
                      </div>
                    </template>
                    <template #description>
                      <div>
                        {{ item.startDate || '-' }} ~ {{ item.endDate || '-' }} |
                        {{ dayjs(item.createdAt).format('YYYY-MM-DD HH:mm') }}
                      </div>
                    </template>
                  </List.Item.Meta>
                  <div class="flex items-center gap-2">
                    <Tag color="blue">{{ getReportTypeLabel(item.type) }}</Tag>
                    <Popconfirm title="确认删除这份周报？" @confirm="deleteReport(item.id)">
                      <Button danger size="small" type="text">删除</Button>
                    </Popconfirm>
                  </div>
                </List.Item>
              </template>
            </List>
          </Card>
        </Col>
      </Row>

      <Card title="AI 周报内容">
        <template v-if="selectedReport?.content">
          <div class="mb-3 flex items-center justify-between">
            <div>
              <div class="font-medium">{{ selectedReport.title }}</div>
              <div class="text-text-secondary text-xs">
                {{ selectedReport.startDate || '-' }} ~ {{ selectedReport.endDate || '-' }}
              </div>
            </div>
            <Space>
              <Tag v-if="selectedReport.relatedProjectName" color="cyan">
                {{ selectedReport.relatedProjectName }}
              </Tag>
              <Tag color="processing">{{ getReportTypeLabel(selectedReport.type) }}</Tag>
            </Space>
          </div>
          <pre class="max-h-[520px] overflow-auto whitespace-pre-wrap rounded bg-neutral-50 p-4 text-sm leading-6">{{ selectedReport.content }}</pre>
        </template>
        <Empty v-else description="先生成一份实施周报，或者从右侧历史周报中选择一份" />
      </Card>
    </div>
  </Page>
</template>
