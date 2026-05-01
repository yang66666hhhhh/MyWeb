<script setup lang="ts">
import { ref, watch } from 'vue';

import { DatePicker, Input, InputNumber, Select, Upload } from 'ant-design-vue';

export interface DynamicField {
  fieldName: string;
  fieldLabel: string;
  fieldType: number;
  options?: string;
  isRequired?: boolean;
  defaultValue?: string;
}

export interface DynamicValue {
  fieldName: string;
  stringValue?: string;
  numberValue?: number;
  dateValue?: string;
}

const props = defineProps<{
  fields: DynamicField[];
  modelValue: DynamicValue[];
}>();

const emit = defineEmits<{
  'update:modelValue': [value: DynamicValue[]];
}>();

const localValues = ref<DynamicValue[]>([]);

watch(
  () => props.modelValue,
  (val) => {
    localValues.value = val || [];
  },
  { immediate: true, deep: true },
);

function updateValue(fieldName: string, value: Partial<DynamicValue>) {
  const idx = localValues.value.findIndex((v) => v.fieldName === fieldName);
  if (idx >= 0) {
    localValues.value[idx] = { ...localValues.value[idx], ...value };
  } else {
    localValues.value.push({ fieldName, ...value });
  }
  emit('update:modelValue', [...localValues.value]);
}

function getValue(fieldName: string): DynamicValue | undefined {
  return localValues.value.find((v) => v.fieldName === fieldName);
}

function getOptions(field: DynamicField): Array<{ label: string; value: string }> {
  if (!field.options) return [];
  return field.options.split(',').map((o) => ({
    label: o.trim(),
    value: o.trim(),
  }));
}

function handleTextInput(fieldName: string, e: Event) {
  const val = (e.target as HTMLInputElement).value;
  updateValue(fieldName, { stringValue: val });
}

function handleNumberChange(fieldName: string, val: number | null) {
  updateValue(fieldName, { numberValue: val ?? undefined });
}

function handleDateChange(fieldName: string, _date: unknown, dateString: string) {
  updateValue(fieldName, { dateValue: dateString });
}

function handleSelectChange(fieldName: string, val: string) {
  updateValue(fieldName, { stringValue: val });
}
</script>

<template>
  <div class="dynamic-form">
    <div v-for="field in fields" :key="field.fieldName" class="mb-4">
      <label class="block text-sm font-medium mb-1">
        {{ field.fieldLabel }}
        <span v-if="field.isRequired" class="text-red-500">*</span>
      </label>

      <!-- Text -->
      <Input
        v-if="field.fieldType === 0"
        :value="getValue(field.fieldName)?.stringValue ?? ''"
        :placeholder="'请输入' + field.fieldLabel"
        @input="handleTextInput(field.fieldName, $event)"
      />

      <!-- Number -->
      <InputNumber
        v-else-if="field.fieldType === 1"
        class="w-full"
        :value="getValue(field.fieldName)?.numberValue"
        :placeholder="'请输入' + field.fieldLabel"
        @change="handleNumberChange(field.fieldName, $event)"
      />

      <!-- Date -->
      <DatePicker
        v-else-if="field.fieldType === 2"
        class="w-full"
        :value="getValue(field.fieldName)?.dateValue"
        @change="handleDateChange(field.fieldName, $event, $event as unknown as string)"
      />

      <!-- Select -->
      <Select
        v-else-if="field.fieldType === 3"
        class="w-full"
        :value="getValue(field.fieldName)?.stringValue"
        :options="getOptions(field)"
        :placeholder="'请选择' + field.fieldLabel"
        @change="handleSelectChange(field.fieldName, $event as unknown as string)"
      />

      <!-- MultiSelect -->
      <Select
        v-else-if="field.fieldType === 4"
        class="w-full"
        mode="multiple"
        :value="(getValue(field.fieldName)?.stringValue ?? '').split(',').filter(Boolean)"
        :options="getOptions(field)"
        :placeholder="'请选择' + field.fieldLabel"
        @change="(val: string[]) => updateValue(field.fieldName, { stringValue: val.join(',') })"
      />

      <!-- Textarea -->
      <Input.TextArea
        v-else-if="field.fieldType === 5"
        :value="getValue(field.fieldName)?.stringValue ?? ''"
        :rows="3"
        :placeholder="'请输入' + field.fieldLabel"
        @input="handleTextInput(field.fieldName, $event)"
      />

      <!-- File -->
      <Upload
        v-else-if="field.fieldType === 6"
        :max-count="1"
        :before-upload="() => false"
      >
        <Button>上传文件</Button>
      </Upload>
    </div>
  </div>
</template>