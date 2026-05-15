<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Input,
  Row,
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
} from '#/enums/workEnum';

interface CustomerProjectSummary {
  activeProjects: number;
  completedProjects: number;
  customerName: string;
  latestProject?: WorkProject;
  locations: string[];
  projectCount: number;
  suspendedProjects: number;
}

const loading = ref(false);
const keyword = ref('');
const projects = ref<WorkProject[]>([]);

const columns: TableColumnsType<CustomerProjectSummary> = [
  { title: '客户名称', dataIndex: 'customerName', key: 'customerName', minWidth: 180 },
  { title: '项目地', dataIndex: 'locations', key: 'locations', width: 220 },
  { title: '项目数', dataIndex: 'projectCount', key: 'projectCount', width: 100 },
  { title: '进行中', dataIndex: 'activeProjects', key: 'activeProjects', width: 100 },
  { title: '已完成', dataIndex: 'completedProjects', key: 'completedProjects', width: 100 },
  { title: '已暂停', dataIndex: 'suspendedProjects', key: 'suspendedProjects', width: 100 },
  { title: '最近项目', key: 'latestProject', width: 260 },
];

const customers = computed<CustomerProjectSummary[]>(() => {
  const groups = new Map<string, WorkProject[]>();

  for (const project of projects.value) {
    const name = project.customerName?.trim();
    if (!name) continue;
    groups.set(name, [...(groups.get(name) ?? []), project]);
  }

  return [...groups.entries()]
    .map(([customerName, items]) => {
      const latestProject = [...items].sort(
        (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime(),
      )[0];

      return {
        activeProjects: items.filter((item) => item.status === WorkProjectStatus.Active).length,
        completedProjects: items.filter((item) => item.status === WorkProjectStatus.Completed).length,
        customerName,
        latestProject,
        locations: [...new Set(items.map((item) => item.location?.trim()).filter((item): item is string => !!item))],
        projectCount: items.length,
        suspendedProjects: items.filter((item) => item.status === WorkProjectStatus.Suspended).length,
      };
    })
    .filter((item) =>
      !keyword.value ||
      item.customerName.includes(keyword.value) ||
      item.locations.some((location) => location.includes(keyword.value)),
    )
    .sort((a, b) => b.projectCount - a.projectCount || a.customerName.localeCompare(b.customerName));
});

const activeCustomerCount = computed(
  () => customers.value.filter((item) => item.activeProjects > 0).length,
);
const completedProjectCount = computed(
  () => customers.value.reduce((sum, item) => sum + item.completedProjects, 0),
);
const totalLocationCount = computed(
  () => new Set(customers.value.flatMap((item) => item.locations)).size,
);

function formatLocations(locations: string[]) {
  if (locations.length === 0) {
    return '-';
  }

  if (locations.length <= 2) {
    return locations.join(' / ');
  }

  return `${locations.slice(0, 2).join(' / ')} 等 ${locations.length} 地`;
}

async function fetchProjects() {
  loading.value = true;
  try {
    const allProjects: WorkProject[] = [];
    let page = 1;
    const pageSize = 100;

    while (true) {
      const result = await projectApi.getPage({ page, pageSize });
      allProjects.push(...result.items);

      if (allProjects.length >= result.total || result.items.length < pageSize) {
        break;
      }

      page += 1;
    }

    projects.value = allProjects;
  } catch {
    message.error('加载客户项目失败');
  } finally {
    loading.value = false;
  }
}

function resetFilters() {
  keyword.value = '';
}

onMounted(() => {
  void fetchProjects();
});
</script>

<template>
  <Page description="按实施项目汇总客户合作情况" title="客户管理">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="客户总数" :value="customers.length" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="活跃客户" :value="activeCustomerCount" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="覆盖项目地" :value="totalLocationCount" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="完成项目" :value="completedProjectCount" />
        </Card>
      </Col>
    </Row>

    <Card title="客户项目概览">
      <template #extra>
        <Space>
          <Input
            v-model:value="keyword"
            allow-clear
            placeholder="客户名称/项目地"
            style="width: 180px"
          />
          <Button @click="resetFilters">重置</Button>
          <Button :loading="loading" type="primary" @click="fetchProjects">刷新</Button>
        </Space>
      </template>
      <Table
        :columns="columns"
        :data-source="customers"
        :loading="loading"
        :pagination="{ pageSize: 10, showSizeChanger: true }"
        :scroll="{ x: 900 }"
        row-key="customerName"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'locations'">
            <span>{{ formatLocations(record.locations) }}</span>
          </template>
          <template v-else-if="column.key === 'latestProject'">
            <div v-if="record.latestProject">
              <div class="font-medium">{{ record.latestProject.projectName }}</div>
              <div v-if="record.latestProject.location" class="text-text-secondary text-xs">
                {{ record.latestProject.location }}
              </div>
              <Tag :color="WorkProjectStatusColor[record.latestProject.status as WorkProjectStatus]">
                {{ WorkProjectStatusLabel[record.latestProject.status as WorkProjectStatus] }}
              </Tag>
            </div>
            <span v-else>-</span>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>
