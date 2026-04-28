<script lang="ts" setup>
import { reactive, ref, watch } from 'vue';

import { DatePicker, Form, Input, InputNumber, message, Modal, Select } from 'ant-design-vue';
import type { Dayjs } from 'dayjs';
import dayjs from 'dayjs';

import type { Project, SaveProjectInput } from '#/api/growth';

import { createProjectApi, getProjectApi, updateProjectApi } from '#/api/growth';

const props = defineProps<{ id?: null | string; open: boolean }>();
const emit = defineEmits<{ success: []; 'update:open': [value: boolean] }>();

const loading = ref(false);
const Textarea = Input.TextArea;

const formState = reactive<SaveProjectInput>({
  description: '',
  endDate: '',
  name: '',
  progress: 0,
  startDate: new Date().toISOString().slice(0, 10),
  status: 0,
  taskCount: 0,
  type: 2,
});

const typeOptions = [
  { label: '工作', value: 0 },
  { label: '学习', value: 1 },
  { label: '个人', value: 2 },
  { label: '其他', value: 3 },
];

const statusOptions = [
  { label: '未开始', value: 0 },
  { label: '进行中', value: 1 },
  { label: '已完成', value: 2 },
  { label: '暂停', value: 3 },
];

function fillForm(item?: Project) {
  Object.assign(formState, {
    description: item?.description ?? '',
    endDate: item?.endDate ?? '',
    name: item?.name ?? '',
    progress: item?.progress ?? 0,
    startDate: item?.startDate ?? new Date().toISOString().slice(0, 10),
    status: item?.status ?? 0,
    taskCount: item?.taskCount ?? 0,
    type: item?.type ?? 2,
  });
}

function onStartDateChange(value: null | string | Dayjs) {
  formState.startDate =
    value && typeof value !== 'string' ? value.format('YYYY-MM-DD') : '';
}

function onEndDateChange(value: null | string | Dayjs) {
  formState.endDate =
    value && typeof value !== 'string' ? value.format('YYYY-MM-DD') : '';
}

async function loadDetail() {
  if (!props.id) {
    fillForm();
    return;
  }
  loading.value = true;
  try {
    fillForm((await getProjectApi(props.id)) as Project);
  } finally {
    loading.value = false;
  }
}

async function submit() {
  if (!formState.name.trim()) {
    message.warning('请填写项目名称');
    return;
  }
  loading.value = true;
  try {
    if (props.id) {
      await updateProjectApi(props.id, { ...formState });
      message.success('项目已更新');
    } else {
      await createProjectApi({ ...formState });
      message.success('项目已创建');
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
    :title="id ? '编辑项目' : '新增项目'"
    width="720px"
    @cancel="emit('update:open', false)"
    @ok="submit"
  >
    <Form :model="formState" layout="vertical">
      <Form.Item label="项目名称" required>
        <Input v-model:value="formState.name" placeholder="例如：个人管理系统" />
      </Form.Item>
      <div class="grid grid-cols-1 gap-4 md:grid-cols-2">
        <Form.Item label="项目类型">
          <Select v-model:value="formState.type" :options="typeOptions" />
        </Form.Item>
        <Form.Item label="项目状态">
          <Select v-model:value="formState.status" :options="statusOptions" />
        </Form.Item>
        <Form.Item label="开始日期">
          <DatePicker
            style="width: 100%"
            :value="formState.startDate ? dayjs(formState.startDate) : undefined"
            format="YYYY-MM-DD"
            @change="onStartDateChange"
          />
        </Form.Item>
        <Form.Item label="结束日期">
          <DatePicker
            style="width: 100%"
            :value="formState.endDate ? dayjs(formState.endDate) : undefined"
            format="YYYY-MM-DD"
            @change="onEndDateChange"
          />
        </Form.Item>
        <Form.Item label="项目进度">
          <InputNumber
            v-model:value="formState.progress"
            :max="100"
            :min="0"
            addon-after="%"
            class="w-full"
          />
        </Form.Item>
        <Form.Item label="关联任务数量">
          <InputNumber v-model:value="formState.taskCount" :min="0" class="w-full" />
        </Form.Item>
      </div>
      <Form.Item label="项目描述">
        <Textarea
          v-model:value="formState.description"
          :auto-size="{ minRows: 4, maxRows: 8 }"
          placeholder="简要描述目标、范围和当前阶段"
        />
      </Form.Item>
    </Form>
  </Modal>
</template>
