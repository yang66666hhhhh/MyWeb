<script lang="ts" setup>
import { onMounted, ref } from 'vue';

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
  Space,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import {
  createImplLogApi,
  deleteImplLogApi,
  getImplLogsApi,
  getMyWorkLogTemplateApi,
  type ImplLog,
  type WorkLogTemplate,
} from '#/api/work/implLog';
import { updateImplLogApi } from '#/api/work/implLog';
import DynamicFieldForm from '#/components/DynamicForm/DynamicFieldForm.vue';

const loading = ref(false);
const template = ref<WorkLogTemplate | null>(null);
const formOpen = ref(false);
const editingId = ref<string | null>(null);
const logs = ref<ImplLog[]>([]);
const totalCount = ref(0);
const totalHours = ref(0);

const queryParams = ref({
  page: 1,
  pageSize: 20,
  startDate: dayjs().startOf('month').format('YYYY-MM-DD'),
  endDate: dayjs().endOf('month').format('YYYY-MM-DD'),
});

const formState = ref({
  workDate: dayjs().format('YYYY-MM-DD'),
  title: '',
  projectName: '',
  totalHours: 8,
  extraData: {} as Record<string, any>,
});

const columns: any[] = [
  { title: '日期', dataIndex: 'workDate', key: 'workDate', width: 120 },
  { title: '项目', dataIndex: 'title', key: 'title', minWidth: 200 },
  { title: '工时', dataIndex: 'totalHours', key: 'totalHours', width: 80 },
  { title: '项目地', dataIndex: ['extraData', 'location'], key: 'location', width: 100 },
  { title: '设备', dataIndex: ['extraData', 'equipment'], key: 'equipment', width: 150 },
  { title: '任务类型', dataIndex: ['extraData', 'taskTypes'], key: 'taskTypes', width: 150 },
  { key: 'action', title: '操作', width: 150, fixed: 'right' },
];

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

async function loadLogs() {
  loading.value = true;
  try {
    const res = await getImplLogsApi(queryParams.value);
    logs.value = res.items;
    totalCount.value = res.total;
    totalHours.value = res.items.reduce((sum, log) => sum + log.totalHours, 0);
  } catch {
    message.error('加载失败');
  } finally {
    loading.value = false;
  }
}

function openCreate() {
  editingId.value = null;
  formState.value = {
    workDate: dayjs().format('YYYY-MM-DD'),
    title: '',
    projectName: '',
    totalHours: 8,
    extraData: {},
  };
  formOpen.value = true;
}

function openEdit(record: Record<string, any>) {
  const log = record as ImplLog;
  editingId.value = log.id;
  formState.value = {
    workDate: log.workDate,
    title: log.title,
    projectName: log.projectName || '',
    totalHours: log.totalHours,
    extraData: typeof log.extraData === 'string' ? JSON.parse(log.extraData) : (log.extraData || {}),
  };
  formOpen.value = true;
}

async function handleSave() {
  if (!formState.value.title) {
    message.error('请填写项目名称');
    return;
  }

  try {
    if (editingId.value) {
      await updateImplLogApi(editingId.value, {
        workDate: formState.value.workDate,
        title: formState.value.title,
        projectName: formState.value.projectName,
        totalHours: formState.value.totalHours,
        extraData: formState.value.extraData,
      });
      message.success('更新成功');
    } else {
      await createImplLogApi({
        workDate: formState.value.workDate,
        title: formState.value.title,
        projectName: formState.value.projectName,
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
  loadLogs();
});
</script>

<template>
  <Page description="EAP设备自动化平台实施工程师专用日志" title="实施日志">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月日志" :value="totalCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月工时" :value="totalHours" suffix="小时" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="涉及设备" :value="0" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="任务类型" :value="0" /></Card>
      </Col>
    </Row>

    <Card>
      <div class="mb-4 flex items-center justify-between">
        <Space>
          <DatePicker.RangePicker
            v-model:value="[queryParams.startDate, queryParams.endDate]"
            format="YYYY-MM-DD"
            @change="loadLogs"
          />
        </Space>
        <Space>
          <Button type="primary" @click="openCreate">新增日志</Button>
        </Space>
      </div>

      <Table
        :columns="columns"
        :data-source="logs"
        :loading="loading"
        :scroll="{ x: 1200 }"
        row-key="id"
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
              <Button size="small" type="link" @click="openEdit(record)">编辑</Button>
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
      :title="editingId ? '编辑日志' : '新增日志'"
      width="800px"
      @ok="handleSave"
    >
      <Form :model="formState" layout="vertical">
        <Row :gutter="16">
          <Col :span="12">
            <Form.Item label="日期" required>
              <DatePicker
                v-model:value="formState.workDate"
                style="width: 100%"
                format="YYYY-MM-DD"
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

        <Form.Item label="项目地（选填）">
          <Input v-model:value="formState.projectName" placeholder="如：惠州" />
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
