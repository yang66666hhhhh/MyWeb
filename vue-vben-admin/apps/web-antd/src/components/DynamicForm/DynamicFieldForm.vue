<script lang="ts" setup>
import { ref, watch } from 'vue';

import {
  Button,
  Input,
  InputNumber,
  Select,
} from 'ant-design-vue';

interface FieldDefinition {
  key: string;
  label: string;
  type: 'date' | 'multi-select' | 'number' | 'select' | 'task-list' | 'text' | 'textarea';
  options?: string[];
  required?: boolean;
  placeholder?: string;
  fields?: string[];
}

const props = defineProps<{
  fields: FieldDefinition[];
  modelValue: Record<string, any>;
}>();

const emit = defineEmits<{
  (e: 'update:modelValue', value: Record<string, any>): void;
}>();

const localData = ref<Record<string, any>>({ ...props.modelValue });

watch(
  () => props.modelValue,
  (val) => {
    localData.value = { ...val };
  },
  { deep: true }
);

watch(
  localData,
  (val) => {
    emit('update:modelValue', { ...val });
  },
  { deep: true }
);

function getFieldValue(key: string) {
  return localData.value[key];
}

function setFieldValue(key: string, value: any) {
  localData.value[key] = value;
}

function addTaskListItem(key: string) {
  if (!localData.value[key]) {
    localData.value[key] = [];
  }
  localData.value[key].push({ content: '', percent: 0 });
}

function removeTaskListItem(key: string, index: number) {
  if (localData.value[key]) {
    localData.value[key].splice(index, 1);
  }
}

function updateTaskListItem(key: string, index: number, field: string, value: any) {
  if (localData.value[key] && localData.value[key][index]) {
    localData.value[key][index][field] = value;
  }
}
</script>

<template>
  <div class="dynamic-form">
    <div v-for="field in fields" :key="field.key" class="mb-4">
      <label class="mb-1 block text-sm font-medium">
        {{ field.label }}
        <span v-if="field.required" class="text-red-500">*</span>
      </label>

      <!-- Text Input -->
      <Input
        v-if="field.type === 'text'"
        :value="getFieldValue(field.key)"
        :placeholder="field.placeholder"
        @update:value="setFieldValue(field.key, $event)"
      />

      <!-- Textarea -->
      <Input.TextArea
        v-else-if="field.type === 'textarea'"
        :value="getFieldValue(field.key)"
        :placeholder="field.placeholder"
        :rows="3"
        @update:value="setFieldValue(field.key, $event)"
      />

      <!-- Number -->
      <InputNumber
        v-else-if="field.type === 'number'"
        :value="getFieldValue(field.key)"
        :placeholder="field.placeholder"
        style="width: 100%"
        @update:value="setFieldValue(field.key, $event)"
      />

      <!-- Select -->
      <Select
        v-else-if="field.type === 'select'"
        :value="getFieldValue(field.key)"
        :placeholder="field.placeholder"
        style="width: 100%"
        @update:value="setFieldValue(field.key, $event)"
      >
        <Select.Option v-for="opt in field.options" :key="opt" :value="opt">
          {{ opt }}
        </Select.Option>
      </Select>

      <!-- Multi Select -->
      <Select
        v-else-if="field.type === 'multi-select'"
        :value="getFieldValue(field.key)"
        :placeholder="field.placeholder"
        mode="multiple"
        style="width: 100%"
        @update:value="setFieldValue(field.key, $event)"
      >
        <Select.Option v-for="opt in field.options" :key="opt" :value="opt">
          {{ opt }}
        </Select.Option>
      </Select>

      <!-- Task List -->
      <div v-else-if="field.type === 'task-list'">
        <div
          v-for="(item, index) in getFieldValue(field.key) || []"
          :key="index"
          class="mb-2 flex items-start gap-2"
        >
          <Input
            :value="item.content"
            placeholder="工作内容"
            style="flex: 1"
            @update:value="updateTaskListItem(field.key, Number(index), 'content', $event)"
          />
          <InputNumber
            :value="item.percent"
            placeholder="%"
            :min="0"
            :max="100"
            style="width: 80px"
            @update:value="updateTaskListItem(field.key, Number(index), 'percent', $event)"
          />
          <span class="mt-2">%</span>
          <Button
            type="text"
            danger
            @click="removeTaskListItem(field.key, Number(index))"
          >
            删除
          </Button>
        </div>
        <Button type="dashed" block @click="addTaskListItem(field.key)">
          {{ field.placeholder || '添加项目' }}
        </Button>
      </div>
    </div>
  </div>
</template>
