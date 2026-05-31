<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  DatePicker,
  Form,
  Input,
  InputNumber,
  message,
  Modal,
  Popconfirm,
  Row,
  Select,
  Space,
  Statistic,
  type TableColumnsType,
  Table,
  Tag,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import {
  createImplLogApi,
  deleteImplLogApi,
  getImplLogsApi,
  getImplLogSummaryApi,
  getMyWorkLogTemplateApi,
  type ImplLog,
  type ImplLogExtraData,
  type WorkLogTemplate,
} from '#/api/work/implLog';
import { projectApi, type WorkProject } from '#/api/work/project';
import { updateImplLogApi } from '#/api/work/implLog';
import DynamicFieldForm from '#/components/DynamicForm/DynamicFieldForm.vue';

import type { Dayjs } from 'dayjs';

interface ImplLogTableRow extends ImplLog {
  extraData: ImplLogExtraData;
}

interface ImplLogFormState {
  extraData: ImplLogExtraData;
  selectedProjectId?: string;
  title: string;
  totalHours: number;
  workDate: string;
}

const props = withDefaults(
  defineProps<{
    context?: 'default' | 'implementation';
  }>(),
  {
    context: 'default',
  },
);

const formRef = ref();
const formRules = {
  title: [{ required: true, message: '请输入项目名称', type: 'string' as const, trigger: 'blur' as const }],
  workDate: [{ required: true, message: '请选择日期', type: 'string' as const, trigger: 'change' as const }],
  totalHours: [{ required: true, message: '请输入工时', type: 'number' as const, trigger: 'blur' as const }],
};

const loading = ref(false);
const template = ref<WorkLogTemplate | null>(null);
const projects = ref<WorkProject[]>([]);
const formOpen = ref(false);
const editingId = ref<string | null>(null);
const logs = ref<ImplLog[]>([]);
const totalCount = ref(0);
const totalHours = ref(0);
const totalDeviceCount = ref(0);
const totalTaskTypeCount = ref(0);
const pageLogs = computed<ImplLogTableRow[]>(() =>
  logs.value.map((log) => ({
    ...log,
    extraData: normalizeExtraData(log.extraData),
  })),
);

const queryParams = ref({
  page: 1,
  pageSize: 20,
  keyword: '',
  startDate: dayjs().startOf('month').format('YYYY-MM-DD'),
  endDate: dayjs().endOf('month').format('YYYY-MM-DD'),
});
const dateRange = ref<[Dayjs, Dayjs]>([
  dayjs(queryParams.value.startDate),
  dayjs(queryParams.value.endDate),
]);
const workDateValue = computed(() =>
  formState.value.workDate ? dayjs(formState.value.workDate) : undefined,
);

const formState = ref<ImplLogFormState>({
  workDate: dayjs().format('YYYY-MM-DD'),
  title: '',
  selectedProjectId: undefined as string | undefined,
  totalHours: 8,
  extraData: {
    location: '',
  } as ImplLogExtraData,
});

const columns: TableColumnsType<ImplLogTableRow> = [
  { title: '日期', dataIndex: 'workDate', key: 'workDate', width: 120 },
  { title: '项目', dataIndex: 'title', key: 'title', minWidth: 200 },
  { title: '工时', dataIndex: 'totalHours', key: 'totalHours', width: 80 },
  { title: '项目地', dataIndex: ['extraData', 'location'], key: 'location', width: 100 },
  { title: '设备', dataIndex: ['extraData', 'equipment'], key: 'equipment', width: 150 },
  { title: '任务类型', dataIndex: ['extraData', 'taskTypes'], key: 'taskTypes', width: 150 },
  { key: 'action', title: '操作', width: 150, fixed: 'right' },
];

const pageTitle = computed(() =>
  props.context === 'implementation' ? '实施日志' : '工作日志',
);

const pageDescription = computed(() =>
  props.context === 'implementation'
    ? '记录实施项目现场工作、设备调试与交付情况'
    : '记录日常工作日志和项目执行情况',
);

const createButtonText = computed(() =>
  props.context === 'implementation' ? '新增实施日志' : '新增日志',
);
const summaryText = computed(() => `共 ${totalCount.value} 条日志`);

const projectOptions = computed(() =>
  projects.value.map((item) => ({
    label: item.projectName,
    value: item.id,
  })),
);

async function loadTemplate() {
  try {
    const res = await getMyWorkLogTemplateApi();
    if (res) {
      template.value = res;
    }
  } catch {
    // 静默处理
  }
}

async function loadProjects() {
  try {
    const result = await projectApi.getPage({ page: 1, pageSize: 100 });
    projects.value = result.items;
  } catch {
    // 静默处理
  }
}

async function loadLogs() {
  loading.value = true;
  try {
    const [pageResult, summaryResult] = await Promise.all([
      getImplLogsApi(queryParams.value),
      getImplLogSummaryApi(queryParams.value),
    ]);
    logs.value = pageResult.items;
    totalCount.value = summaryResult.totalCount;
    totalHours.value = summaryResult.totalHours;
    totalDeviceCount.value = summaryResult.uniqueEquipmentCount;
    totalTaskTypeCount.value = summaryResult.uniqueTaskTypeCount;
  } catch {
    message.error('加载失败');
  } finally {
    loading.value = false;
  }
}

async function handleDateRangeChange(
  value: [string, string] | [Dayjs, Dayjs] | null,
  dateStrings: [string, string],
) {
  if (dateStrings[0] && dateStrings[1]) {
    dateRange.value = [
      typeof value?.[0] === 'string' ? dayjs(dateStrings[0]) : (value?.[0] ?? dayjs(dateStrings[0])),
      typeof value?.[1] === 'string' ? dayjs(dateStrings[1]) : (value?.[1] ?? dayjs(dateStrings[1])),
    ];
    queryParams.value.startDate = dateStrings[0];
    queryParams.value.endDate = dateStrings[1];
  } else {
    queryParams.value.startDate = '';
    queryParams.value.endDate = '';
  }

  queryParams.value.page = 1;
  await loadLogs();
}

function handleWorkDateChange(value: Dayjs | string, dateString: string) {
  formState.value.workDate = dateString || (typeof value === 'string' ? value : '');
}

function handleTableChange(pagination: { current?: number; pageSize?: number }) {
  queryParams.value.page = pagination.current ?? 1;
  queryParams.value.pageSize = pagination.pageSize ?? 20;
  void loadLogs();
}

function normalizeExtraData(extraData?: ImplLogExtraData | null): ImplLogExtraData {
  return {
    location: '',
    ...(extraData ?? {}),
  };
}

function search() {
  queryParams.value.page = 1;
  void loadLogs();
}

function resetFilters() {
  queryParams.value.keyword = '';
  queryParams.value.startDate = dayjs().startOf('month').format('YYYY-MM-DD');
  queryParams.value.endDate = dayjs().endOf('month').format('YYYY-MM-DD');
  queryParams.value.page = 1;
  dateRange.value = [
    dayjs(queryParams.value.startDate),
    dayjs(queryParams.value.endDate),
  ];
  void loadLogs();
}

function openCreate() {
  editingId.value = null;
  formState.value = {
    workDate: dayjs().format('YYYY-MM-DD'),
    title: '',
    selectedProjectId: undefined,
    totalHours: 8,
    extraData: {
      location: '',
    },
  };
  formOpen.value = true;
}

function openEdit(log: ImplLogTableRow) {
  editingId.value = log.id;
  formState.value = {
    workDate: log.workDate,
    title: log.title,
    selectedProjectId: log.projectId,
    totalHours: log.totalHours,
    extraData: normalizeExtraData(log.extraData),
  };
  formOpen.value = true;
}

function toTableRow(record: Record<string, any>): ImplLogTableRow {
  const log = record as ImplLog;
  return {
    ...log,
    extraData: normalizeExtraData(log.extraData),
  };
}

function handleProjectChange(projectId?: unknown) {
  if (typeof projectId !== 'string') {
    return;
  }
  const project = projects.value.find((item) => item.id === projectId);
  if (!project) {
    return;
  }
  formState.value.title = project.projectName;
  if (!formState.value.extraData.location && project.location) {
    formState.value.extraData.location = project.location;
  }
}

async function handleSave() {
  try { await formRef.value?.validate(); } catch { return; }

  try {
    if (editingId.value) {
      await updateImplLogApi(editingId.value, {
        workDate: formState.value.workDate,
        title: formState.value.title,
        projectId: formState.value.selectedProjectId,
        totalHours: formState.value.totalHours,
        extraData: formState.value.extraData,
      });
      message.success('更新成功');
    } else {
      await createImplLogApi({
        workDate: formState.value.workDate,
        title: formState.value.title,
        projectId: formState.value.selectedProjectId,
        totalHours: formState.value.totalHours,
        templateId: template.value?.id,
        extraData: formState.value.extraData,
      });
      message.success('创建成功');
    }
    formOpen.value = false;
    await loadLogs();
  } catch {
    message.error('操作失败');
  }
}

async function handleDelete(id: string) {
  try {
    await deleteImplLogApi(id);
    message.success('删除成功');
    await loadLogs();
  } catch {
    message.error('删除失败');
  }
}

onMounted(() => {
  loadTemplate();
  loadProjects();
  loadLogs();
});
</script>

<template>
  <Page :description="pageDescription" :title="pageTitle">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月日志" :value="totalCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月工时" :value="totalHours" suffix="小时" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="涉及设备" :value="totalDeviceCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="任务类型" :value="totalTaskTypeCount" /></Card>
      </Col>
    </Row>

    <Card>
      <div class="mb-4 flex items-center justify-between">
        <Form layout="inline">
          <Form.Item label="关键词">
            <Input
              v-model:value="queryParams.keyword"
              placeholder="搜索项目、项目地..."
              style="width: 200px"
              @pressEnter="search"
            />
          </Form.Item>
          <Form.Item label="日期范围">
            <DatePicker.RangePicker
              :value="dateRange"
              format="YYYY-MM-DD"
              @change="handleDateRangeChange"
            />
          </Form.Item>
          <Form.Item>
            <Space>
              <Button type="primary" @click="search">查询</Button>
              <Button @click="resetFilters">重置</Button>
            </Space>
          </Form.Item>
        </Form>
        <Space>
          <span class="text-gray-500 text-sm">{{ summaryText }}</span>
          <Button type="primary" @click="openCreate">{{ createButtonText }}</Button>
        </Space>
      </div>

      <Table
        :columns="columns"
        :data-source="pageLogs"
        :loading="loading"
        :pagination="{ current: queryParams.page, pageSize: queryParams.pageSize, showSizeChanger: true, showTotal: (value: number) => `共 ${value} 条`, total: totalCount }"
        :scroll="{ x: 1200 }"
        row-key="id"
        @change="handleTableChange"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'equipment'">
            <Space>
              <Tag v-for="eq in record.extraData?.equipment || []" :key="eq">{{ eq }}</Tag>
            </Space>
          </template>
          <template v-else-if="column.key === 'taskTypes'">
            <Space>
              <Tag v-for="tt in record.extraData?.taskTypes || []" :key="tt" color="blue">{{ tt }}</Tag>
            </Space>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button size="small" type="link" @click="openEdit(toTableRow(record))">编辑</Button>
              <Popconfirm title="确认删除？" @confirm="handleDelete(record.id)">
                <Button danger size="small" type="link">删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="formOpen"
      :title="editingId ? '编辑日志' : createButtonText"
      width="800px"
      @ok="handleSave"
    >
      <Form ref="formRef" :model="formState" :rules="formRules" layout="vertical">
        <Row :gutter="16">
          <Col :span="12">
            <Form.Item label="日期" required>
              <DatePicker
                :value="workDateValue"
                style="width: 100%"
                format="YYYY-MM-DD"
                @change="handleWorkDateChange"
              />
            </Form.Item>
          </Col>
          <Col :span="12">
            <Form.Item label="工时" required>
              <InputNumber v-model:value="formState.totalHours" :min="0" :max="24" style="width: 100%" />
            </Form.Item>
          </Col>
        </Row>

        <Form.Item label="项目名称" required>
          <Input v-model:value="formState.title" placeholder="如：胜宏科技HDI二处工业物联网平台实施项目" />
        </Form.Item>

        <Form.Item v-if="props.context === 'implementation'" label="关联项目">
          <Select
            v-model:value="formState.selectedProjectId"
            :options="projectOptions"
            allow-clear
            placeholder="选择实施项目后自动带出项目名"
            @change="handleProjectChange"
          />
        </Form.Item>

        <Form.Item label="项目地（选填）">
          <Input v-model:value="formState.extraData.location" placeholder="如：惠州" />
        </Form.Item>

        <Card v-if="template" title="扩展字段" class="mb-4">
          <DynamicFieldForm
            :fields="template.fieldDefinitions.fields"
            v-model="formState.extraData"
          />
        </Card>
      </Form>
    </Modal>
  </Page>
</template>
