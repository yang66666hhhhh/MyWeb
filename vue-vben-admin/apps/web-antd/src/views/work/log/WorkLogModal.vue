<script lang="ts" setup>
import { computed, reactive, ref, watch } from 'vue';

import {
  DatePicker,
  Form,
  Input,
  InputNumber,
  message,
  Modal,
  Select,
  Space,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import type { CreateWorkLogInput, WorkLog } from '#/api/growth/work';

import { createWorkLogApi, getWorkLogApi, updateWorkLogApi } from '#/api/growth/work';
import { WorkLogSourceType, WorkLogStatus, WorkLogStatusLabel } from '#/enums/workEnum';

const props = defineProps<{
  id?: null | string;
  open: boolean;
}>();

const emit = defineEmits<{
  success: [];
  'update:open': [value: boolean];
}>();

const loading = ref(false);
const Textarea = Input.TextArea;

const formState = reactive<CreateWorkLogInput>({
  deviceIds: [],
  originalContent: '',
  projectId: '',
  remark: '',
  sourceType: 0,
  status: 0,
  summary: '',
  taskTypeIds: [],
  title: '',
  totalHours: 0,
  weekDay: '',
  workDate: dayjs().format('YYYY-MM-DD'),
});

const projectOptions = [
  { label: '生产线升级项目', value: 'project-1' },
  { label: '质量改进项目', value: 'project-2' },
  { label: '设备维护项目', value: 'project-3' },
];

const deviceOptions = [
  { label: 'A线体', value: 'device-1' },
  { label: 'B线体', value: 'device-2' },
  { label: 'C线体', value: 'device-3' },
  { label: 'D线体', value: 'device-4' },
];

const taskTypeOptions = [
  { label: '调试', value: 'tasktype-1' },
  { label: '问题处理', value: 'tasktype-2' },
  { label: '维护', value: 'tasktype-3' },
  { label: '生产', value: 'tasktype-4' },
  { label: '检测', value: 'tasktype-5' },
];

const statusOptions = [
  { label: '正常', value: WorkLogStatus.Normal },
  { label: '缺失数据', value: WorkLogStatus.MissingData },
  { label: '待补充', value: WorkLogStatus.PendingSupplement },
];

const sourceTypeOptions = [
  { label: '手动', value: WorkLogSourceType.Manual },
  { label: 'Excel导入', value: WorkLogSourceType.ExcelImport },
  { label: '计划转换', value: WorkLogSourceType.PlanConversion },
];

const modalTitle = computed(() => (props.id ? '编辑工作日志' : '新增工作日志'));

function fillForm(item?: WorkLog) {
  Object.assign(formState, {
    deviceIds: item?.deviceIds || [],
    originalContent: item?.originalContent || '',
    projectId: item?.projectId || '',
    remark: item?.remark || '',
    sourceType: item?.sourceType ?? WorkLogSourceType.Manual,
    status: item?.status ?? WorkLogStatus.Normal,
    summary: item?.summary || '',
    taskTypeIds: item?.taskTypeIds || [],
    title: item?.title || '',
    totalHours: item?.totalHours || 0,
    weekDay: item?.weekDay || '',
    workDate: item?.workDate || dayjs().format('YYYY-MM-DD'),
  });
}

async function loadDetail() {
  if (!props.id) {
    fillForm();
    return;
  }
  loading.value = true;
  try {
    const result = await getWorkLogApi(props.id);
    if (result) {
      fillForm(result);
    }
  } finally {
    loading.value = false;
  }
}

function onWorkDateChange(value: null | string | dayjs.Dayjs) {
  if (value && typeof value !== 'string') {
    formState.workDate = value.format('YYYY-MM-DD');
    formState.weekDay = ['周日', '周一', '周二', '周三', '周四', '周五', '周六'][value.day()];
  }
}

async function submit() {
  if (!formState.workDate || !formState.title.trim()) {
    message.warning('请填写日期和标题');
    return;
  }
  if (!formState.projectId) {
    message.warning('请选择项目');
    return;
  }

  loading.value = true;
  try {
    if (props.id) {
      await updateWorkLogApi(props.id, { ...formState });
      message.success('工作日志已更新');
    } else {
      await createWorkLogApi({ ...formState });
      message.success('工作日志已创建');
    }
    emit('update:open', false);
    emit('success');
  } finally {
    loading.value = false;
  }
}

watch(
  () => props.open,
  (open) => {
    if (open) {
      void loadDetail();
    }
  },
);
</script>

<template>
  <Modal
    :confirm-loading="loading"
    :open="open"
    :title="modalTitle"
    width="720px"
    @cancel="emit('update:open', false)"
    @ok="submit"
  >
    <Form :model="formState" layout="vertical">
      <div class="grid grid-cols-2 gap-4">
        <Form.Item label="工作日期" required>
          <DatePicker
            style="width: 100%"
            :value="formState.workDate ? dayjs(formState.workDate) : undefined"
            format="YYYY-MM-DD"
            @change="onWorkDateChange"
          />
        </Form.Item>
        <Form.Item label="星期">
          <Input :value="formState.weekDay" disabled />
        </Form.Item>
      </div>
      <div class="grid grid-cols-2 gap-4">
        <Form.Item label="项目" required>
          <Select v-model:value="formState.projectId" :options="projectOptions" placeholder="请选择项目" />
        </Form.Item>
        <Form.Item label="状态">
          <Select v-model:value="formState.status" :options="statusOptions" />
        </Form.Item>
      </div>
      <Form.Item label="标题" required>
        <Input v-model:value="formState.title" placeholder="工作标题" />
      </Form.Item>
      <div class="grid grid-cols-2 gap-4">
        <Form.Item label="设备/线体">
          <Select v-model:value="formState.deviceIds" :options="deviceOptions" mode="multiple" placeholder="可多选" />
        </Form.Item>
        <Form.Item label="任务类型">
          <Select v-model:value="formState.taskTypeIds" :options="taskTypeOptions" mode="multiple" placeholder="可多选" />
        </Form.Item>
      </div>
      <div class="grid grid-cols-2 gap-4">
        <Form.Item label="耗时(小时)">
          <InputNumber v-model:value="formState.totalHours" :min="0" :max="24" addon-after="h" class="w-full" />
        </Form.Item>
        <Form.Item label="来源">
          <Select v-model:value="formState.sourceType" :options="sourceTypeOptions" />
        </Form.Item>
      </div>
      <Form.Item label="原始工作内容">
        <Textarea
          v-model:value="formState.originalContent"
          :auto-size="{ minRows: 3, maxRows: 6 }"
          placeholder="详细描述工作内容"
        />
      </Form.Item>
      <Form.Item label="工作总结">
        <Textarea
          v-model:value="formState.summary"
          :auto-size="{ minRows: 2, maxRows: 4 }"
          placeholder="一句话总结"
        />
      </Form.Item>
      <Form.Item label="备注">
        <Textarea
          v-model:value="formState.remark"
          :auto-size="{ minRows: 2, maxRows: 4 }"
          placeholder="补充说明"
        />
      </Form.Item>
    </Form>
  </Modal>
</template>
