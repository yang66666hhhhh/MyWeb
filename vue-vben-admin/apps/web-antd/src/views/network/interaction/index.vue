<script lang="ts" setup>
import { onMounted, reactive, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  DatePicker,
  Form,
  FormItem,
  Input,
  message,
  Modal,
  Popconfirm,
  Row,
  Select,
  SelectOption,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { Contact, CreateInteractionInput, Interaction } from '#/api/network';

import {
  createInteractionApi,
  deleteInteractionApi,
  getContactPageApi,
  getInteractionPageApi,
  updateInteractionApi,
} from '#/api/network';

const loading = ref(false);
const dataList = ref<Interaction[]>([]);
const contacts = ref<Contact[]>([]);
const total = ref(0);
const currentPage = ref(1);
const pageSize = ref(10);
const modalVisible = ref(false);
const editingId = ref<string | null>(null);
const submitting = ref(false);
const selectedContactId = ref<string | undefined>(undefined);

const formState = reactive<CreateInteractionInput>({
  contactId: '',
  type: '',
  content: '',
  interactionDate: '',
  nextFollowUpDate: '',
  remark: '',
});

const typeOptions = ['电话', '会议', '邮件', '微信', '面谈', '其他'];

const columns = [
  { title: '联系人', dataIndex: 'contactName', key: 'contactName', width: 100 },
  { title: '类型', dataIndex: 'type', key: 'type', width: 80 },
  { title: '内容', dataIndex: 'content', key: 'content', ellipsis: true },
  { title: '互动日期', dataIndex: 'interactionDate', key: 'interactionDate', width: 120 },
  { title: '下次跟进', dataIndex: 'nextFollowUpDate', key: 'nextFollowUpDate', width: 120 },
  { title: '操作', key: 'action', width: 150 },
];

const typeColors: Record<string, string> = {
  '电话': 'blue',
  '会议': 'purple',
  '邮件': 'green',
  '微信': 'cyan',
  '面谈': 'orange',
  '其他': 'default',
};

const fetchContacts = async () => {
  try {
    const res = await getContactPageApi({ page: 1, pageSize: 100 });
    contacts.value = res.items;
  } catch {
    // ignore
  }
};

const fetchData = async () => {
  loading.value = true;
  try {
    const res = await getInteractionPageApi(selectedContactId.value || '', {
      page: currentPage.value,
      pageSize: pageSize.value,
    });
    dataList.value = res.items;
    total.value = res.total;
  } catch {
    // ignore
  } finally {
    loading.value = false;
  }
};

const handleAdd = () => {
  editingId.value = null;
  Object.assign(formState, {
    contactId: selectedContactId.value || '',
    type: '',
    content: '',
    interactionDate: '',
    nextFollowUpDate: '',
    remark: '',
  });
  modalVisible.value = true;
};

const handleEdit = (record: Interaction) => {
  editingId.value = record.id;
  Object.assign(formState, {
    contactId: record.contactId,
    type: record.type,
    content: record.content,
    interactionDate: record.interactionDate,
    nextFollowUpDate: record.nextFollowUpDate || '',
    remark: record.remark || '',
  });
  modalVisible.value = true;
};

const handleDelete = async (id: string) => {
  try {
    await deleteInteractionApi(id);
    message.success('删除成功');
    fetchData();
  } catch {
    message.error('删除失败');
  }
};

const handleSubmit = async () => {
  if (!formState.contactId || !formState.type || !formState.content || !formState.interactionDate) {
    message.warning('请填写必填项');
    return;
  }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateInteractionApi(editingId.value, formState);
      message.success('更新成功');
    } else {
      await createInteractionApi(formState);
      message.success('创建成功');
    }
    modalVisible.value = false;
    fetchData();
  } catch {
    message.error('操作失败');
  } finally {
    submitting.value = false;
  }
};

const handleContactFilter = (value: any) => {
  selectedContactId.value = value as string | undefined;
  currentPage.value = 1;
  fetchData();
};

const handlePageChange = (page: number, size: number) => {
  currentPage.value = page;
  pageSize.value = size;
  fetchData();
};

onMounted(() => {
  fetchContacts();
  fetchData();
});
</script>

<template>
  <Page description="记录与联系人的互动" title="互动记录">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card :loading="loading">
          <Statistic title="互动总数" :value="total" suffix="条" />
        </Card>
      </Col>
    </Row>

    <Card title="互动记录">
      <template #extra>
        <div class="flex gap-4">
          <Select
            v-model:value="selectedContactId"
            placeholder="筛选联系人"
            allow-clear
            style="width: 200px"
            @change="handleContactFilter"
          >
            <SelectOption v-for="c in contacts" :key="c.id" :value="c.id">{{ c.name }}</SelectOption>
          </Select>
          <Button type="primary" @click="handleAdd">记录互动</Button>
        </div>
      </template>
      <Table
        :columns="columns"
        :data-source="dataList"
        :loading="loading"
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
          <template v-if="column.key === 'type'">
            <Tag :color="typeColors[record.type]">{{ record.type }}</Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <div class="flex gap-2">
              <Button type="link" size="small" @click="handleEdit(record as Interaction)">编辑</Button>
              <Popconfirm title="确认删除?" @confirm="handleDelete(record.id)">
                <Button type="link" size="small" danger>删除</Button>
              </Popconfirm>
            </div>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="modalVisible"
      :title="editingId ? '编辑互动' : '记录互动'"
      :confirm-loading="submitting"
      @ok="handleSubmit"
    >
      <Form layout="vertical">
        <FormItem label="联系人" required>
          <Select v-model:value="formState.contactId" placeholder="选择联系人">
            <SelectOption v-for="c in contacts" :key="c.id" :value="c.id">{{ c.name }}</SelectOption>
          </Select>
        </FormItem>
        <Row :gutter="16">
          <Col :span="12">
            <FormItem label="互动类型" required>
              <Select v-model:value="formState.type" placeholder="选择类型">
                <SelectOption v-for="t in typeOptions" :key="t" :value="t">{{ t }}</SelectOption>
              </Select>
            </FormItem>
          </Col>
          <Col :span="12">
            <FormItem label="互动日期" required>
              <DatePicker v-model:value="formState.interactionDate" style="width: 100%" />
            </FormItem>
          </Col>
        </Row>
        <FormItem label="互动内容" required>
          <Input.TextArea v-model:value="formState.content" placeholder="互动内容" :rows="3" />
        </FormItem>
        <FormItem label="下次跟进日期">
          <DatePicker v-model:value="formState.nextFollowUpDate" style="width: 100%" />
        </FormItem>
        <FormItem label="备注">
          <Input.TextArea v-model:value="formState.remark" placeholder="备注" :rows="2" />
        </FormItem>
      </Form>
    </Modal>
  </Page>
</template>
