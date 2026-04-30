<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Descriptions,
  Drawer,
  Form,
  Input,
  Modal,
  Popconfirm,
  Select,
  Space,
  Table,
  Tag,
  message,
} from 'ant-design-vue';

import {
  createUserApi,
  deleteUserApi,
  getUserApi,
  getUserPageApi,
  updateUserApi,
  type CreateUserDto,
  type UpdateUserDto,
  type UserDto,
} from '#/api/system/user';
import { usePagedQuery } from '#/composables/usePagedQuery';

const formOpen = ref(false);
const detailOpen = ref(false);
const editingId = ref<null | string>(null);
const selectedItem = ref<UserDto | null>(null);

const formState = ref({
  email: '',
  phone: '',
  realName: '',
  roles: 'user',
  username: '',
  password: '',
});

const statusOptions = [
  { label: '正常', value: 0 },
  { label: '停用', value: 1 },
  { label: '锁定', value: 2 },
  { label: '已删除', value: 3 },
];

const roleOptions = [
  { label: '超级管理员', value: 'super' },
  { label: '管理员', value: 'admin' },
  { label: '普通用户', value: 'user' },
];

const columns = [
  { dataIndex: 'username', key: 'username', title: '用户名', width: 120 },
  { dataIndex: 'realName', key: 'realName', title: '姓名', width: 120 },
  { dataIndex: 'roles', key: 'roles', title: '角色', width: 100 },
  { dataIndex: 'email', key: 'email', title: '邮箱', width: 180 },
  { dataIndex: 'phone', key: 'phone', title: '电话', width: 140 },
  { dataIndex: 'status', key: 'status', title: '状态', width: 90 },
  { key: 'action', title: '操作', width: 180, fixed: 'right' },
];

const { changePage, items, load, loading, query, resetQuery, search, total } = usePagedQuery<
  UserDto,
  { keyword?: string; page: number; pageSize: number; status?: number }
>({
  defaultQuery: { page: 1, pageSize: 10 },
  fetcher: getUserPageApi,
});

onMounted(() => {
  load();
});

function openCreate() {
  editingId.value = null;
  formState.value = {
    email: '',
    phone: '',
    realName: '',
    roles: 'user',
    username: '',
    password: '',
  };
  formOpen.value = true;
}

async function openEdit(record: UserDto) {
  editingId.value = record.id;
  const detail = await getUserApi(record.id);
  if (detail) {
    formState.value = {
      email: detail.email || '',
      phone: detail.phone || '',
      realName: detail.realName,
      roles: detail.roles || 'user',
      username: detail.username,
      password: '',
    };
  }
  formOpen.value = true;
}

async function showDetail(record: UserDto) {
  const detail = await getUserApi(record.id);
  if (detail) {
    selectedItem.value = detail;
    detailOpen.value = true;
  }
}

async function handleSubmit() {
  if (editingId.value) {
    const data: UpdateUserDto = {
      realName: formState.value.realName,
      email: formState.value.email || undefined,
      phone: formState.value.phone || undefined,
      roles: formState.value.roles,
    };
    await updateUserApi(editingId.value, data);
    message.success('更新成功');
  }
  else {
    const data: CreateUserDto = {
      username: formState.value.username,
      password: formState.value.password,
      realName: formState.value.realName,
      email: formState.value.email || undefined,
      phone: formState.value.phone || undefined,
      roles: formState.value.roles,
    };
    await createUserApi(data);
    message.success('创建成功');
  }
  formOpen.value = false;
  await load();
}

async function handleRemove(id: string) {
  await deleteUserApi(id);
  message.success('删除成功');
  await load();
}

function getRoleLabel(role: string) {
  return roleOptions.find(r => r.value === role)?.label || role;
}
</script>

<template>
  <Page>
    <div class="flex flex-col gap-4">
      <Card>
        <div class="flex items-center justify-between">
          <Form layout="inline" @submit.prevent="search">
            <Form.Item name="keyword">
              <Input v-model:value="query.keyword" placeholder="搜索用户名/姓名" />
            </Form.Item>
            <Form.Item>
              <Button type="primary" html-type="submit">
                搜索
              </Button>
            </Form.Item>
          </Form>
          <Button type="primary" @click="openCreate">
            新增用户
          </Button>
        </div>
      </Card>

      <Card>
        <Table
          :columns="columns"
          :data-source="items"
          :loading="loading"
          :pagination="{
            current: query.page,
            pageSize: query.pageSize,
            showSizeChanger: true,
            showTotal: (value: number) => `共 ${value} 条`,
            total,
          }"
          :scroll="{ x: 1100 }"
          row-key="id"
          @change="changePage($event.current ?? 1, $event.pageSize ?? 10)"
        >
          <template #bodyCell="{ column, record, text }">
            <template v-if="column.key === 'username'">
              <div class="font-medium">{{ record.username }}</div>
            </template>
            <template v-else-if="column.key === 'roles'">
              <Tag :color="text === 'super' ? 'purple' : text === 'admin' ? 'blue' : 'green'">
                {{ getRoleLabel(text) }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'status'">
              <Tag :color="text === 0 ? 'success' : text === 1 ? 'default' : text === 2 ? 'warning' : 'error'">
                {{ statusOptions[text]?.label }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'action'">
              <Space>
                <Button size="small" type="link" @click="showDetail(record)">
                  详情
                </Button>
                <Button size="small" type="link" @click="openEdit(record)">
                  编辑
                </Button>
                <Popconfirm title="确认删除？" @confirm="handleRemove(record.id)">
                  <Button danger size="small" type="link">
                    删除
                  </Button>
                </Popconfirm>
              </Space>
            </template>
          </template>
        </Table>
      </Card>
    </div>

    <Modal
      :open="formOpen"
      :title="editingId ? '编辑用户' : '新增用户'"
      width="560px"
      @cancel="formOpen = false"
      @ok="handleSubmit"
    >
      <Form :model="formState" layout="vertical">
        <Form.Item label="用户名" required>
          <Input v-model:value="formState.username" :disabled="!!editingId" placeholder="用户名" />
        </Form.Item>
        <Form.Item v-if="!editingId" label="密码" required>
          <Input v-model:value="formState.password" type="password" placeholder="密码" />
        </Form.Item>
        <Form.Item label="姓名" required>
          <Input v-model:value="formState.realName" placeholder="姓名" />
        </Form.Item>
        <Form.Item label="角色" required>
          <Select v-model:value="formState.roles" style="width: 200px">
            <Select.Option v-for="opt in roleOptions" :key="opt.value" :value="opt.value">
              {{ opt.label }}
            </Select.Option>
          </Select>
        </Form.Item>
        <Form.Item label="邮箱">
          <Input v-model:value="formState.email" placeholder="邮箱" />
        </Form.Item>
        <Form.Item label="电话">
          <Input v-model:value="formState.phone" placeholder="电话" />
        </Form.Item>
      </Form>
    </Modal>

    <Drawer v-model:open="detailOpen" title="用户详情" width="640px" :footer="null">
      <Descriptions v-if="selectedItem" :column="2" bordered>
        <Descriptions.Item label="用户名">
          {{ selectedItem.username }}
        </Descriptions.Item>
        <Descriptions.Item label="姓名">
          {{ selectedItem.realName }}
        </Descriptions.Item>
        <Descriptions.Item label="角色">
          <Tag :color="selectedItem.roles === 'super' ? 'purple' : selectedItem.roles === 'admin' ? 'blue' : 'green'">
            {{ getRoleLabel(selectedItem.roles) }}
          </Tag>
        </Descriptions.Item>
        <Descriptions.Item label="邮箱">
          {{ selectedItem.email || '-' }}
        </Descriptions.Item>
        <Descriptions.Item label="电话">
          {{ selectedItem.phone || '-' }}
        </Descriptions.Item>
        <Descriptions.Item label="状态">
          <Tag :color="selectedItem.status === 0 ? 'success' : selectedItem.status === 1 ? 'default' : selectedItem.status === 2 ? 'warning' : 'error'">
            {{ statusOptions[selectedItem.status]?.label }}
          </Tag>
        </Descriptions.Item>
        <Descriptions.Item label="最后登录IP">
          {{ selectedItem.lastLoginIp || '-' }}
        </Descriptions.Item>
        <Descriptions.Item label="创建时间" :span="2">
          {{ selectedItem.createdAt }}
        </Descriptions.Item>
      </Descriptions>
    </Drawer>
  </Page>
</template>