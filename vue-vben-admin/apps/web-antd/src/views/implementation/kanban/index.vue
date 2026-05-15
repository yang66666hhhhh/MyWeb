<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Input,
  Row,
  Select,
  Space,
  Statistic,
  type TableColumnsType,
  Table,
  Tag,
  message,
} from 'ant-design-vue';

import { projectApi, type WorkProject } from '#/api/work/project';
import {
  WorkProjectStatus,
  WorkProjectStatusColor,
  WorkProjectStatusLabel,
  WorkProjectType,
  WorkProjectTypeLabel,
} from '#/enums/workEnum';

const loading = ref(false);
const projects = ref<WorkProject[]>([]);
const keyword = ref('');
const status = ref<number | undefined>();

const statusOptions = [
  { label: '全部状态', value: undefined },
  { label: '进行中', value: WorkProjectStatus.Active },
  { label: '已完成', value: WorkProjectStatus.Completed },
  { label: '已暂停', value: WorkProjectStatus.Suspended },
  { label: '已归档', value: WorkProjectStatus.Archived },
];

const columns: TableColumnsType<WorkProject> = [
  { title: '项目名称', dataIndex: 'projectName', key: 'projectName', minWidth: 220 },
  { title: '客户', dataIndex: 'customerName', key: 'customerName', width: 150 },
  { title: '项目地', dataIndex: 'location', key: 'location', width: 120 },
  { title: '项目类型', dataIndex: 'projectType', key: 'projectType', width: 110 },
  { title: '状态', dataIndex: 'status', key: 'status', width: 100 },
  { title: '进度', key: 'progress', width: 160 },
  { title: '开始日期', dataIndex: 'startDate', key: 'startDate', width: 120 },
  { title: '结束日期', dataIndex: 'endDate', key: 'endDate', width: 120 },
];

const activeCount = computed(
  () => projects.value.filter((item) => item.status === WorkProjectStatus.Active).length,
);
const completedCount = computed(
  () => projects.value.filter((item) => item.status === WorkProjectStatus.Completed).length,
);
const locationCount = computed(
  () => new Set(projects.value.map((item) => item.location?.trim()).filter((item): item is string => !!item)).size,
);

async function fetchProjects() {
  loading.value = true;
  try {
    const allProjects: WorkProject[] = [];
    let page = 1;
    const pageSize = 100;

    while (true) {
      const result = await projectApi.getPage({
        keyword: keyword.value || undefined,
        page,
        pageSize,
        status: status.value,
      });
      allProjects.push(...result.items);

      if (allProjects.length >= result.total || result.items.length < pageSize) {
        break;
      }

      page += 1;
    }

    projects.value = allProjects;
  } catch {
    message.error('加载项目失败');
  } finally {
    loading.value = false;
  }
}

function resetFilters() {
  keyword.value = '';
  status.value = undefined;
  void fetchProjects();
}

function getProgress(project: WorkProject) {
  if (project.status === WorkProjectStatus.Completed) return 100;
  if (project.status === WorkProjectStatus.Archived) return 100;
  if (project.status === WorkProjectStatus.Suspended) return 0;
  if (!project.startDate || !project.endDate) return 0;

  const start = new Date(project.startDate).getTime();
  const end = new Date(project.endDate).getTime();
  const now = Date.now();
  if (Number.isNaN(start) || Number.isNaN(end) || end <= start) return 0;
  return Math.min(99, Math.max(0, Math.round(((now - start) / (end - start)) * 100)));
}

function toProject(record: Record<string, any>): WorkProject {
  return record as WorkProject;
}

onMounted(() => {
  void fetchProjects();
});
</script>

<template>
  <Page description="管理实施项目、跟踪进度和协调资源" title="项目看板">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="项目总数" :value="projects.length" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="进行中" :value="activeCount" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="覆盖项目地" :value="locationCount" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="已完成" :value="completedCount" />
        </Card>
      </Col>
    </Row>

    <Card title="项目列表">
      <template #extra>
        <Space>
          <Input
            v-model:value="keyword"
            allow-clear
            placeholder="项目名称/编号/项目地"
            style="width: 180px"
            @press-enter="fetchProjects"
          />
          <Select
            v-model:value="status"
            :options="statusOptions"
            allow-clear
            placeholder="状态"
            style="width: 120px"
          />
          <Button type="primary" @click="fetchProjects">查询</Button>
          <Button @click="resetFilters">重置</Button>
        </Space>
      </template>
      <Table
        :columns="columns"
        :data-source="projects"
        :loading="loading"
        :pagination="{ pageSize: 10, showSizeChanger: true }"
        :scroll="{ x: 980 }"
        row-key="id"
      >
        <template #bodyCell="{ column, record, text }">
          <template v-if="column.key === 'projectName'">
            <div class="font-medium">{{ record.projectName }}</div>
            <div v-if="record.description" class="text-text-secondary line-clamp-1 text-xs">
              {{ record.description }}
            </div>
          </template>
          <template v-else-if="column.key === 'customerName'">
            {{ text || '-' }}
          </template>
          <template v-else-if="column.key === 'location'">
            {{ text || '-' }}
          </template>
          <template v-else-if="column.key === 'projectType'">
            {{ WorkProjectTypeLabel[text as WorkProjectType] }}
          </template>
          <template v-else-if="column.key === 'status'">
            <Tag :color="WorkProjectStatusColor[text as WorkProjectStatus]">
              {{ WorkProjectStatusLabel[text as WorkProjectStatus] }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'progress'">
            <div class="flex items-center gap-2">
              <div class="h-2 w-20 rounded-full bg-gray-200">
                <div
                  class="h-full rounded-full bg-blue-500"
                  :style="{ width: `${getProgress(toProject(record))}%` }"
                ></div>
              </div>
              <span>{{ getProgress(toProject(record)) }}%</span>
            </div>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>
