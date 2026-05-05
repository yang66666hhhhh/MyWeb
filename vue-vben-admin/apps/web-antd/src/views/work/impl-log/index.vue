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
  Table,
  Tag,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import { getMyWorkLogTemplateApi, type WorkLogTemplate } from '#/api/work/logTemplate';
import DynamicFieldForm from '#/components/DynamicForm/DynamicFieldForm.vue';

const loading = ref(false);
const template = ref<WorkLogTemplate | null>(null);
const formOpen = ref(false);
const editingId = ref<string | null>(null);

const formState = ref({
  workDate: dayjs().format('YYYY-MM-DD'),
  title: '',
  totalHours: 8,
  extraData: {} as Record<string, any>,
});

const logs = ref([
  {
    id: '1',
    workDate: '2026-03-04',
    title: '胜宏科技HDI二处工业物联网平台实施项目',
    totalHours: 8,
    extraData: {
      location: '惠州',
      equipment: ['镍钯金'],
      taskTypes: ['学习', '梳理', '调试/协助'],
      msapItems: [
        { content: '学习熟悉现场架构', percent: 50 },
        { content: '镍钯金现场调试学习', percent: 50 },
      ],
    },
  },
  {
    id: '2',
    workDate: '2026-03-05',
    title: '胜宏科技HDI二处工业物联网平台实施项目',
    totalHours: 8,
    extraData: {
      location: '惠州',
      equipment: ['垂直电镀TKC VCP1'],
      taskTypes: ['学习', '梳理', '调试/协助'],
      msapItems: [
        { content: '学习熟悉现场架构', percent: 70 },
        { content: '垂直电镀TKC单机测试对接学习', percent: 10 },
      ],
    },
  },
]);

const columns = [
  { title: '日期', dataIndex: 'workDate', key: 'workDate', width: 120 },
  { title: '项目', dataIndex: 'title', key: 'title', minWidth: 200 },
  { title: '工时', dataIndex: 'totalHours', key: 'totalHours', width: 80 },
  { title: '项目地', dataIndex: ['extraData', 'location'], key: 'location', width: 100 },
  { title: '设备', dataIndex: ['extraData', 'equipment'], key: 'equipment', width: 150 },
  { title: '任务类型', dataIndex: ['extraData', 'taskTypes'], key: 'taskTypes', width: 150 },
  { key: 'action', title: '操作', width: 150, fixed: 'right' },
];

const summaryText = computed(() => {
  const totalHours = logs.value.reduce((sum, log) => sum + log.totalHours, 0);
  return `共 ${logs.value.length} 条记录，总工时 ${totalHours} 小时`;
});

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

function openCreate() {
  editingId.value = null;
  formState.value = {
    workDate: dayjs().format('YYYY-MM-DD'),
    title: '',
    totalHours: 8,
    extraData: {},
  };
  formOpen.value = true;
}

function openEdit(record: any) {
  editingId.value = record.id;
  formState.value = {
    workDate: record.workDate,
    title: record.title,
    totalHours: record.totalHours,
    extraData: { ...record.extraData },
  };
  formOpen.value = true;
}

async function handleSave() {
  if (!formState.value.title) {
    message.error('请填写项目名称');
    return;
  }

  if (editingId.value) {
    const index = logs.value.findIndex((l) => l.id === editingId.value);
    if (index !== -1) {
      logs.value[index] = { ...logs.value[index], ...formState.value };
    }
    message.success('更新成功');
  } else {
    logs.value.unshift({
      id: Date.now().toString(),
      ...formState.value,
    });
    message.success('创建成功');
  }
  formOpen.value = false;
}

async function handleDelete(id: string) {
  logs.value = logs.value.filter((l) => l.id !== id);
  message.success('删除成功');
}

onMounted(() => {
  loadTemplate();
});
</script>

<template>
  <Page description="EAP设备自动化平台实施工程师专用日志" title="实施日志">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月日志" :value="logs.length" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="本月工时" :value="logs.reduce((s, l) => s + l.totalHours, 0)" suffix="小时" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="涉及设备" :value="5" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="任务类型" :value="4" /></Card>
      </Col>
    </Row>

    <Card>
      <div class="mb-4 flex items-center justify-between">
        <span class="text-gray-500">{{ summaryText }}</span>
        <Button type="primary" @click="openCreate">新增日志</Button>
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
