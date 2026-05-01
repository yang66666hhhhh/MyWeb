<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Descriptions,
  Form,
  Input,
  message,
  Modal,
  Popconfirm,
  Row,
  Select,
  Space,
  Table,
  Tag,
} from 'ant-design-vue';

import type { IndustryTemplate, CreateTemplateInput, CreateTemplateFieldInput } from '#/api/work/template';
import {
  createTemplateApi,
  deleteTemplateApi,
  getTemplatePageApi,
  updateTemplateApi,
  setDefaultTemplateApi,
  FieldTypeLabels,
} from '#/api/work/template';

const templates = ref<IndustryTemplate[]>([]);
const loading = ref(false);
const formOpen = ref(false);
const editingId = ref<string | null>(null);

const formState = ref<CreateTemplateInput>({
  name: '',
  description: '',
  industry: 'IT',
  isDefault: false,
  fields: [],
});

const fieldFormState = ref<CreateTemplateFieldInput>({
  fieldName: '',
  fieldLabel: '',
  fieldType: 0,
  options: '',
  isRequired: false,
  sort: 0,
  defaultValue: '',
});

const industryOptions = [
  { label: 'IT/开发', value: 'IT' },
  { label: '制造业', value: 'Manufacturing' },
  { label: '销售', value: 'Sales' },
  { label: '教育', value: 'Education' },
  { label: '医疗', value: 'Healthcare' },
  { label: '金融', value: 'Finance' },
  { label: '其他', value: 'Other' },
];

const fieldTypeOptions = Object.entries(FieldTypeLabels).map(([value, label]) => ({
  label,
  value: Number(value),
}));

const isEditing = computed(() => !!editingId.value);

async function load() {
  loading.value = true;
  try {
    const result = await getTemplatePageApi({ page: 1, pageSize: 100 });
    templates.value = result.items;
  } finally {
    loading.value = false;
  }
}

function openCreate() {
  editingId.value = null;
  formState.value = {
    name: '',
    description: '',
    industry: 'IT',
    isDefault: false,
    fields: [],
  };
  formOpen.value = true;
}

function openEdit(record: IndustryTemplate) {
  editingId.value = record.id;
  formState.value = {
    name: record.name,
    description: record.description || '',
    industry: record.industry,
    isDefault: record.isDefault,
    fields: record.fields.map((f) => ({
      fieldName: f.fieldName,
      fieldLabel: f.fieldLabel,
      fieldType: f.fieldType,
      options: f.options || '',
      isRequired: f.isRequired,
      sort: f.sort,
      defaultValue: f.defaultValue || '',
    })),
  };
  formOpen.value = true;
}

function addField() {
  formState.value.fields.push({
    fieldName: fieldFormState.value.fieldName,
    fieldLabel: fieldFormState.value.fieldLabel,
    fieldType: fieldFormState.value.fieldType,
    options: fieldFormState.value.options,
    isRequired: fieldFormState.value.isRequired,
    sort: formState.value.fields.length + 1,
    defaultValue: fieldFormState.value.defaultValue,
  });
  fieldFormState.value = {
    fieldName: '',
    fieldLabel: '',
    fieldType: 0,
    options: '',
    isRequired: false,
    sort: 0,
    defaultValue: '',
  };
}

function removeField(index: number) {
  formState.value.fields.splice(index, 1);
}

async function handleSubmit() {
  if (!formState.value.name) {
    message.error('请输入模板名称');
    return;
  }
  if (!formState.value.industry) {
    message.error('请选择行业');
    return;
  }

  try {
    if (isEditing.value) {
      await updateTemplateApi(editingId.value!, formState.value);
      message.success('模板已更新');
    } else {
      await createTemplateApi(formState.value);
      message.success('模板已创建');
    }
    formOpen.value = false;
    await load();
  } catch {
    message.error('操作失败');
  }
}

async function handleDelete(id: string) {
  try {
    await deleteTemplateApi(id);
    message.success('模板已删除');
    await load();
  } catch {
    message.error('删除失败');
  }
}

async function handleSetDefault(id: string) {
  try {
    await setDefaultTemplateApi(id);
    message.success('已设为默认模板');
    await load();
  } catch {
    message.error('操作失败');
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page title="模板中心" description="配置行业模板和自定义字段">
    <Card class="mb-4">
      <div class="mb-4 flex items-center justify-between">
        <span>管理日志模板，支持自定义字段配置</span>
        <Button type="primary" @click="openCreate">新建模板</Button>
      </div>

      <Table :data-source="templates" :loading="loading" :pagination="false" row-key="id">
        <Table.Column title="模板名称" data-index="name" />
        <Table.Column title="行业" data-index="industry">
          <template #default="{ text }">
            <Tag>{{ text }}</Tag>
          </template>
        </Table.Column>
        <Table.Column title="字段数">
          <template #default="{ record }">
            {{ record.fields?.length || 0 }}
          </template>
        </Table.Column>
        <Table.Column title="默认">
          <template #default="{ text }">
            <Tag :color="text ? 'gold' : 'default'">{{ text ? '是' : '否' }}</Tag>
          </template>
        </Table.Column>
        <Table.Column title="操作" width="200">
          <template #default="{ record }">
            <Space>
              <Button size="small" type="link" @click="openEdit(record)">编辑</Button>
              <Button v-if="!record.isDefault" size="small" type="link" @click="handleSetDefault(record.id)">设为默认</Button>
              <Popconfirm title="确定删除?" @confirm="handleDelete(record.id)">
                <Button size="small" type="link" danger>删除</Button>
              </Popconfirm>
            </Space>
          </template>
        </Table.Column>
      </Table>
    </Card>

    <Modal
      v-model:open="formOpen"
      :title="isEditing ? '编辑模板' : '新建模板'"
      width="800px"
      :footer="null"
    >
      <Form layout="vertical">
        <Row :gutter="16">
          <Col :span="12">
            <Form.Item label="模板名称" required>
              <Input v-model:value="formState.name" placeholder="请输入模板名称" />
            </Form.Item>
          </Col>
          <Col :span="12">
            <Form.Item label="行业" required>
              <Select v-model:value="formState.industry" :options="industryOptions" placeholder="请选择行业" />
            </Form.Item>
          </Col>
        </Row>
        <Form.Item label="描述">
          <Input.TextArea v-model:value="formState.description" :rows="2" placeholder="请输入描述" />
        </Form.Item>

        <div class="border-t border-gray-200 pt-4 mt-4">
          <div class="flex items-center justify-between mb-2">
            <span class="font-medium">字段配置</span>
            <Button size="small" type="primary" @click="addField">添加字段</Button>
          </div>

          <div class="mb-4 p-4 bg-gray-50 rounded">
            <Row :gutter="12" class="mb-2">
              <Col :span="4">
                <Input v-model:value="fieldFormState.fieldName" placeholder="字段名(英文)" />
              </Col>
              <Col :span="4">
                <Input v-model:value="fieldFormState.fieldLabel" placeholder="显示标签" />
              </Col>
              <Col :span="4">
                <Select v-model:value="fieldFormState.fieldType" :options="fieldTypeOptions" placeholder="类型" />
              </Col>
              <Col :span="4">
                <Input v-model:value="fieldFormState.options" placeholder="选项(逗号分隔)" />
              </Col>
              <Col :span="4">
                <Select v-model:value="fieldFormState.isRequired" :options="[{ label: '必填', value: true }, { label: '选填', value: false }]" />
              </Col>
              <Col :span="4">
                <Button type="primary" size="small" @click="addField">+</Button>
              </Col>
            </Row>
          </div>

          <Descriptions v-if="formState.fields.length > 0" bordered :column="1" size="small">
            <Descriptions.Item
              v-for="(field, index) in formState.fields"
              :key="index"
              :label="`${field.fieldLabel} (${field.fieldName})`"
            >
              <Space>
                <Tag>{{ FieldTypeLabels[field.fieldType] }}</Tag>
                <span v-if="field.options" class="text-gray-500">选项: {{ field.options }}</span>
                <Tag v-if="field.isRequired" color="red">必填</Tag>
                <Button size="small" type="link" danger @click="removeField(index)">删除</Button>
              </Space>
            </Descriptions.Item>
          </Descriptions>
        </div>
      </Form>

      <div class="mt-4 flex justify-end gap-2">
        <Button @click="formOpen = false">取消</Button>
        <Button type="primary" @click="handleSubmit">保存</Button>
      </div>
    </Modal>
  </Page>
</template>