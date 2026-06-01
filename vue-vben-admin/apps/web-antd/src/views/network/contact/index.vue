<script lang="ts" setup>
import { computed, onMounted, reactive, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { useAccessStore } from '@vben/stores';

import {
  Button,
  Card,
  Col,
  Form,
  FormItem,
  Input,
  message,
  Modal,
  Popconfirm,
  Row,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { Contact, CreateContactInput } from '#/api/network';

import { createContactApi, deleteContactApi, getContactPageApi, updateContactApi } from '#/api/network';

const loading = ref(false);
const dataList = ref<Contact[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);

const accessStore = useAccessStore();
const canCreateContact = computed(() => accessStore.accessCodes.includes('GROWTH_KNOWLEDGE'));
const canEditContact = computed(() => accessStore.accessCodes.includes('GROWTH_KNOWLEDGE'));
const canDeleteContact = computed(() => accessStore.accessCodes.includes('GROWTH_KNOWLEDGE'));

const formState = reactive<CreateContactInput>({
  name: '',
  company: '',
  position: '',
  phone: '',
  email: '',
  weChat: '',
  tags: '',
  remark: '',
});

const formRef = ref();
const formRules = {
  name: [{ required: true, message: '请输入姓名', type: 'string' as const }],};

const columns = [
  { title: '姓名', dataIndex: 'name', key: 'name', width: 100 },
  { title: '公司', dataIndex: 'company', key: 'company', width: 150 },
  { title: '职位', dataIndex: 'position', key: 'position', width: 120 },
  { title: '电话', dataIndex: 'phone', key: 'phone', width: 120 },
  { title: '邮箱', dataIndex: 'email', key: 'email', width: 180 },
  { title: '标签', dataIndex: 'tags', key: 'tags', width: 150 },
  { title: '互动次数', dataIndex: 'interactionCount', key: 'interactionCount', width: 100 },
  { title: '操作', key: 'action', width: 150 },
];

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getContactPageApi({ page: currentPage.value, pageSize: pageSize.value });
    dataList.value = res.items;
    total.value = res.total;
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载失败，请稍后重试');
  } finally {
    loading.value = false;
  }
};

const handleAdd = () => {
  editingId.value = null;
  Object.assign(formState, {
    name: '',
    company: '',
    position: '',
    phone: '',
    email: '',
    weChat: '',
    tags: '',
    remark: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: Contact) => {
  editingId.value = record.id;
  Object.assign(formState, {
    name: record.name,
    company: record.company || '',
    position: record.position || '',
    phone: record.phone || '',
    email: record.email || '',
    weChat: record.weChat || '',
    tags: record.tags || '',
    remark: record.remark || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deleteContactApi(id);
    message.success('删除成功');
    fetchData();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '删除失败');
  }
};

const handleSubmit = async () => {
    try { await formRef.value?.validate(); } catch { return; }
  if (!formState.name) {
    message.warning('请填写姓名');
    return;
  }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateContactApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createContactApi(formState);
      message.success('创建成功');
    }
    modalVisible.value = false;
    fetchData();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '操作失败');
  } finally {
    submitting.value = false;
  }
};

const handlePageChange = (page: number, size: number) => {
  currentPage.value = page;
  pageSize.value = size;
  fetchData();
};

onMounted(() => {
  fetchData();
});
</script>

<template>
  <Page description="管理联系人信息" title="联系人">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="联系人总数" :value="total" suffix="人" />
        </Card>
      </Col>
    </Row>

    <Card title="联系人列表">
      <template #extra>
        <Button v-if="canCreateContact" type="primary" @click="handleAdd">添加联系人</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="dataList"
        :loading="loading"
        :locale="{ emptyText: '暂无数据' }"
        :pagination="{
          current: currentPage,
          pageSize,
          total,
          showSizeChanger: true,
          showTotal: (t: number) => `共 ${t} 条`,
          onChange: handlePageChange,
        }"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'tags'">
            <Tag v-for="tag in (record.tags || '').split(',').filter(Boolean)" :key="tag" color="blue">{{ tag }}</Tag>
          </template>
          <template v-else-if="column.key === 'interactionCount'">
            <Tag :color="record.interactionCount > 0 ? 'green' : 'default'">{{ record.interactionCount }}</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button v-if="canEditContact" type="link" size="small" @click="handleEdit(record as Contact)">编辑</Button>
              <Popconfirm v-if="canDeleteContact" title="确认删除?" @confirm="handleDelete(record.id)">
                <Button type="link" size="small" danger>删除</Button>
              </Popconfirm>
            </div>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="modalVisible"
      :title="editingId ? '编辑联系人' : '添加联系人'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form ref="formRef" :model="formState" layout="vertical" :rules="formRules">
        <FormItem label="姓名" required>
          <Input v-model:value="formState.name" placeholder="联系人姓名" />
        </FormItem>
        <Row :gutter="16">
          <Col :span="12">
            <FormItem label="公司">
              <Input v-model:value="formState.company" placeholder="公司名称" />
            </FormItem>
          </Col>
          <Col :span="12">
            <FormItem label="职位">
              <Input v-model:value="formState.position" placeholder="职位" />
            </FormItem>
          </Col>
        </Row>
        <Row :gutter="16">
          <Col :span="12">
            <FormItem label="电话">
              <Input v-model:value="formState.phone" placeholder="电话号码" />
            </FormItem>
          </Col>
          <Col :span="12">
            <FormItem label="邮箱">
              <Input v-model:value="formState.email" placeholder="邮箱地址" />
            </FormItem>
          </Col>
        </Row>
        <FormItem label="微信">
          <Input v-model:value="formState.weChat" placeholder="微信号" />
        </FormItem>
        <FormItem label="标签">
          <Input v-model:value="formState.tags" placeholder="多个标签用逗号分隔" />
        </FormItem>
        <FormItem label="备注">
          <Input.TextArea v-model:value="formState.remark" placeholder="备注" :rows="2" />
        </FormItem>
      </Form>
    </Modal>
  </Page>
</template>
