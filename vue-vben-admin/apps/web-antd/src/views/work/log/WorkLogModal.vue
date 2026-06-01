<script setup lang="ts">
import type { DynamicValue } from '#/components/DynamicForm';

import { computed, reactive, ref, watch } from 'vue';

import { DatePicker, Form, Input, InputNumber, message, Modal } from 'ant-design-vue';
import dayjs from 'dayjs';

import { workLogApi } from '#/api/work/workLog';

const props = defineProps<{
  id?: string;
  open: boolean;
}>();

const emit = defineEmits<{
  success: [];
  'update:open': [value: boolean];
}>();

const formRef = ref();
const formRules = {
  title: [{ required: true, message: '请输入标题', type: 'string' as const, trigger: 'blur' as const }],
};

const loading = ref(false);
const dynamicValues = ref<DynamicValue[]>([]);

const formState = reactive({
  workDate: dayjs(),
  projectId: '',
  title: '',
  totalHours: 0,
  originalContent: '',
  summary: '',
  remark: '',
});

const modalTitle = computed(() => (props.id ? '编辑工作日志' : '新增工作日志'));

watch(
  () => props.open,
  async (open) => {
    if (open) {
      dynamicValues.value = [];
      if (props.id) {
        await loadDetail();
      }
    }
  },
);

async function loadDetail() {
  if (!props.id) return;
  loading.value = true;
  try {
    const result = await workLogApi.getById(props.id);
    if (result) {
      Object.assign(formState, {
        workDate: result.workDate ? dayjs(result.workDate) : dayjs(),
        projectId: result.projectId || '',
        title: result.title || '',
        totalHours: result.totalHours || 0,
        originalContent: result.originalContent || '',
        summary: result.summary || '',
        remark: result.remark || '',
      });
      if (result.dynamicValues) {
        dynamicValues.value = result.dynamicValues;
      }
    }
  } catch (e: any) {
    message.error(e?.message || '加载详情失败');
  } finally {
    loading.value = false;
  }
}

async function submit() {
  try { await formRef.value?.validate(); } catch { return; }
  loading.value = true;
  try {
    const data: any = {
      workDate: formState.workDate.format('YYYY-MM-DD'),
      projectId: formState.projectId,
      title: formState.title,
      totalHours: formState.totalHours,
      originalContent: formState.originalContent,
      summary: formState.summary,
      remark: formState.remark,
      dynamicValues: dynamicValues.value,
    };
    await (props.id ? workLogApi.update(props.id, data) : workLogApi.create(data));
    emit('update:open', false);
    emit('success');
  } catch (e: any) {
    message.error(e?.message || '保存失败，请重试');
  } finally {
    loading.value = false;
  }
}
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
    <Form ref="formRef" :model="formState" :rules="formRules" layout="vertical">
      <Form.Item label="工作日期">
        <DatePicker v-model:value="formState.workDate" format="YYYY-MM-DD" class="w-full" />
      </Form.Item>

      <Form.Item label="项目ID">
        <Input v-model:value="formState.projectId" placeholder="请输入项目ID" />
      </Form.Item>

      <Form.Item label="标题">
        <Input v-model:value="formState.title" placeholder="工作标题" />
      </Form.Item>

      <Form.Item label="耗时(小时)">
        <InputNumber
          v-model:value="formState.totalHours"
          :min="0"
          :max="24"
          addon-after="h"
          class="w-full"
        />
      </Form.Item>

      <Form.Item label="原始工作内容">
        <Input.TextArea
          v-model:value="formState.originalContent"
          :auto-size="{ minRows: 3, maxRows: 6 }"
          placeholder="详细描述"
        />
      </Form.Item>

      <Form.Item label="工作总结">
        <Input.TextArea
          v-model:value="formState.summary"
          :auto-size="{ minRows: 2, maxRows: 4 }"
          placeholder="一句话总结"
        />
      </Form.Item>

      <Form.Item label="备注">
        <Input.TextArea
          v-model:value="formState.remark"
          :auto-size="{ minRows: 2, maxRows: 4 }"
          placeholder="补充说明"
        />
      </Form.Item>
    </Form>
  </Modal>
</template>