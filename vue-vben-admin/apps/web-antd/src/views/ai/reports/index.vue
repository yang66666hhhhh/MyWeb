<script lang="ts" setup>
import { computed, onMounted, reactive, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  DatePicker,
  Form,
  FormItem,
  List,
  message,
  Modal,
  Popconfirm,
  Row,
  Select,
  SelectOption,
  Space,
  Statistic,
  Tag,
} from 'ant-design-vue';

import type { AiReport, GenerateReportRequest } from '#/api/ai';

import { aiApi } from '#/api/ai';

const loading = ref(false);
const generating = ref(false);
const reports = ref<AiReport[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const detailModalVisible = ref(false);
const currentReport = ref<AiReport | null>(null);

const formState = reactive<GenerateReportRequest>({
  type: 'Weekly',
  startDate: '',
  endDate: '',
  relatedProjectId: '',
  includeStatistics: true,
});

const includeStatsValue = ref('true');

const reportTypeOptions = [
  { value: 'Daily', label: '日报' },
  { value: 'Weekly', label: '周报' },
  { value: 'Monthly', label: '月报' },
  { value: 'Project', label: '项目报告' },
  { value: 'Custom', label: '自定义' },
];

const typeColors: Record<string, string> = {
  Daily: 'blue',
  Weekly: 'green',
  Monthly: 'orange',
  Project: 'purple',
  Custom: 'default',
};

const typeLabels: Record<string, string> = {
  Daily: '日报',
  Weekly: '周报',
  Monthly: '月报',
  Project: '项目报告',
  Custom: '自定义',
};

const reportCountByType = computed(() => {
  const counts: Record<string, number> = {};
  for (const report of reports.value) {
    counts[report.type] = (counts[report.type] || 0) + 1;
  }
  return counts;
});

const fetchReports = async () => {
  loading.value = true;
  try {
    const res = await aiApi.getReports({ page: currentPage.value, pageSize: pageSize.value });
    reports.value = res.items;
    total.value = res.total;
  } catch {
    message.error('加载失败，请稍后重试');
  } finally {
    loading.value = false;
  }
};

const handleGenerate = () => {
  Object.assign(formState, {
    type: 'Weekly',
    startDate: '',
    endDate: '',
    relatedProjectId: '',
    includeStatistics: true,
  });
  includeStatsValue.value = 'true';
  modalVisible.value = true;
};

const handleGenerateSubmit = async () => {
  generating.value = true;
  try {
    formState.includeStatistics = includeStatsValue.value === 'true';
    await aiApi.generateReport(formState);
    message.success('报告生成成功');
    modalVisible.value = false;
    fetchReports();
  } catch {
    message.error('生成失败');
  } finally {
    generating.value = false;
  }
};

const handleView = async (id: string) => {
  try {
    currentReport.value = await aiApi.getReportById(id);
    detailModalVisible.value = true;
  } catch {
    message.error('获取报告详情失败');
  }
};

const handleDelete = async (id: string) => {
  try {
    await aiApi.deleteReport(id);
    message.success('删除成功');
    fetchReports();
  } catch {
    message.error('删除失败');
  }
};

const handlePageChange = (page: number) => {
  currentPage.value = page;
  fetchReports();
};

onMounted(() => {
  fetchReports();
});
</script>

<template>
  <Page description="AI生成的工作和成长报告" title="AI报告">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="报告总数" :value="total" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="周报" :value="reportCountByType.Weekly || 0" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="月报" :value="reportCountByType.Monthly || 0" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="项目报告" :value="reportCountByType.Project || 0" />
        </Card>
      </Col>
    </Row>

    <Card title="AI报告列表">
      <template #extra>
        <Button type="primary" @click="handleGenerate">生成报告</Button>
      </template>
      <List
        :data-source="reports"
        :loading="loading"
        :pagination="{
          current: currentPage,
          pageSize,
          total,
          onChange: handlePageChange,
        }"
      >
        <template #renderItem="{ item }">
          <List.Item>
            <List.Item.Meta>
              <template #title>
                {{ item.title }}
              </template>
              <template #description>
                <Space>
                  <span v-if="item.startDate && item.endDate">{{ item.startDate }} ~ {{ item.endDate }}</span>
                  <span v-if="item.relatedProjectName">项目: {{ item.relatedProjectName }}</span>
                  <span>{{ item.createdAt }}</span>
                </Space>
              </template>
            </List.Item.Meta>
            <Space>
              <Tag :color="typeColors[item.type]">{{ typeLabels[item.type] || item.type }}</Tag>
              <Button type="link" @click="handleView(item.id)">查看</Button>
              <Popconfirm title="确认删除?" @confirm="handleDelete(item.id)">
                <Button type="link" danger>删除</Button>
              </Popconfirm>
            </Space>
          </List.Item>
        </template>
      </List>
    </Card>

    <Modal
      v-model:open="modalVisible"
      title="生成AI报告"
      :confirm-loading="generating"
      @ok="handleGenerateSubmit"
    >
      <Form layout="vertical">
        <FormItem label="报告类型" required>
          <Select v-model:value="formState.type">
            <SelectOption v-for="opt in reportTypeOptions" :key="opt.value" :value="opt.value">
              {{ opt.label }}
            </SelectOption>
          </Select>
        </FormItem>
        <FormItem label="开始日期">
          <DatePicker v-model:value="formState.startDate" style="width: 100%" />
        </FormItem>
        <FormItem label="结束日期">
          <DatePicker v-model:value="formState.endDate" style="width: 100%" />
        </FormItem>
        <FormItem label="包含统计">
          <Select v-model:value="includeStatsValue" :options="[{ value: 'true', label: '是' }, { value: 'false', label: '否' }]" />
        </FormItem>
      </Form>
    </Modal>

    <Modal
      v-model:open="detailModalVisible"
      title="报告详情"
      :footer="null"
      width="800px"
    >
      <div v-if="currentReport">
        <h3>{{ currentReport.title }}</h3>
        <Space class="mb-4">
          <Tag :color="typeColors[currentReport.type]">{{ typeLabels[currentReport.type] || currentReport.type }}</Tag>
          <span class="text-gray-500">{{ currentReport.createdAt }}</span>
        </Space>
        <div class="whitespace-pre-wrap bg-gray-50 p-4 rounded">{{ currentReport.content }}</div>
      </div>
    </Modal>
  </Page>
</template>
