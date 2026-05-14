<script setup lang="ts">
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Form,
  FormItem,
  Input,
  InputNumber,
  message,
  Modal,
  Popconfirm,
  Select,
  Space,
  Switch,
  Table,
  Tag,
} from 'ant-design-vue';
import {
  createRoleMenuApi,
  deleteRoleMenuApi,
  getMenuListApi,
  type RoleMenuItem,
  updateRoleMenuApi,
} from '#/api/core/menu';

const loading = ref(false);
const menus = ref<RoleMenuItem[]>([]);
const modalVisible = ref(false);
const editingId = ref<string | undefined>(undefined);
const submitting = ref(false);

const formState = ref<Partial<RoleMenuItem>>({
  bindingType: 'Role',
  bindingValue: 'user',
  name: '',
  path: '',
  icon: '',
  component: '',
  sort: 0,
  isVisible: true,
  isEnabled: true,
  parentId: undefined,
});

const bindingTypeOptions = [
  { label: '角色权限', value: 'Role' },
  { label: '领域身份', value: 'Persona' },
  { label: '标签', value: 'Tag' },
];

const roleValueOptions = [
  { label: 'Member', value: 'member' },
  { label: 'Pro', value: 'pro' },
  { label: 'Owner', value: 'owner' },
];

const personaValueOptions = [
  { label: '开发者', value: 'Developer' },
  { label: '实施工程师', value: 'Implementation' },
  { label: '销售', value: 'Sales' },
  { label: '教师', value: 'Teacher' },
  { label: '设计师', value: 'Designer' },
  { label: '学生', value: 'Student' },
  { label: '自由职业', value: 'Freelancer' },
  { label: '通用', value: 'General' },
];

const filterType = ref<string | undefined>(undefined);
const filterValue = ref<string | undefined>(undefined);

const columns: any[] = [
  { title: '绑定类型', dataIndex: 'bindingType', key: 'bindingType', width: 100 },
  { title: '绑定值', dataIndex: 'bindingValue', key: 'bindingValue', width: 120 },
  { title: '名称', dataIndex: 'name', key: 'name', width: 160 },
  { title: '路径', dataIndex: 'path', key: 'path', width: 200 },
  { title: '组件', dataIndex: 'component', key: 'component', ellipsis: true },
  { title: '排序', dataIndex: 'sort', key: 'sort', width: 60 },
  { title: '可见', dataIndex: 'isVisible', key: 'isVisible', width: 60 },
  { title: '操作', key: 'action', width: 140 },
];

const bindingTypeColor: Record<string, string> = {
  Role: 'blue',
  Persona: 'green',
  Tag: 'orange',
};

async function fetchMenus() {
  loading.value = true;
  try {
    menus.value = await getMenuListApi(filterType.value, filterValue.value);
  } catch (e) {
    console.error(e);
  } finally {
    loading.value = false;
  }
}

function getBindingValueOptions() {
  if (formState.value.bindingType === 'Role') return roleValueOptions;
  if (formState.value.bindingType === 'Persona') return personaValueOptions;
  return [];
}

function openCreate(parentId?: string) {
  editingId.value = undefined;
  formState.value = {
    bindingType: 'Role',
    bindingValue: 'user',
    name: '',
    path: '',
    icon: '',
    component: '',
    sort: 0,
    isVisible: true,
    isEnabled: true,
    parentId: parentId,
  };
  modalVisible.value = true;
}

function openEdit(record: Record<string, any>) {
  const menu = record as RoleMenuItem;
  editingId.value = menu.id;
  formState.value = { ...menu };
  modalVisible.value = true;
}

async function handleSubmit() {
  if (!formState.value.name || !formState.value.path) {
    message.warning('名称和路径不能为空');
    return;
  }
  submitting.value = true;
  try {
    if (editingId.value) {
      await updateRoleMenuApi(editingId.value, formState.value);
      message.success('更新成功');
    } else {
      await createRoleMenuApi(formState.value);
      message.success('创建成功');
    }
    modalVisible.value = false;
    await fetchMenus();
  } catch (e) {
    message.error('操作失败');
  } finally {
    submitting.value = false;
  }
}

async function handleDelete(id: string) {
  try {
    await deleteRoleMenuApi(id);
    message.success('删除成功');
    await fetchMenus();
  } catch {
    message.error('删除失败');
  }
}

onMounted(() => {
  fetchMenus();
});
</script>

<template>
  <Page description="按角色、领域身份、标签配置动态菜单，不同用户登录后看到不同菜单" title="菜单管理">
    <div class="space-y-4">
      <Card>
        <Space>
          <span>绑定类型：</span>
          <Select
            v-model:value="filterType"
            :options="bindingTypeOptions"
            allow-clear
            placeholder="全部"
            class="w-32"
            @change="fetchMenus"
          />
          <span>绑定值：</span>
          <Select
            v-model:value="filterValue"
            :options="filterType === 'Role' ? roleValueOptions : filterType === 'Persona' ? personaValueOptions : []"
            allow-clear
            placeholder="全部"
            class="w-40"
            @change="fetchMenus"
          />
          <Button type="primary" @click="openCreate()"> 新增菜单 </Button>
        </Space>
      </Card>

      <Card>
      <Table
        :columns="columns"
        :data-source="menus"
        :loading="loading"
        :pagination="false"
        :default-expand-all-rows="true"
        row-key="id"
        children-column-name="children"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'bindingType'">
            <Tag :color="bindingTypeColor[record.bindingType] || 'default'">
              {{ record.bindingType === 'Role' ? '角色' : record.bindingType === 'Persona' ? '领域' : '标签' }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'bindingValue'">
            <Tag>{{ record.bindingValue }}</Tag>
          </template>
          <template v-else-if="column.key === 'name'">
            <Tag v-if="record.parentId" color="blue">{{ record.name }}</Tag>
            <strong v-else>{{ record.name }}</strong>
          </template>
          <template v-else-if="column.key === 'isVisible'">
            <Tag :color="record.isVisible ? 'green' : 'red'">
              {{ record.isVisible ? '是' : '否' }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'action'">
            <Space>
              <Button size="small" type="link" @click="openCreate(record.id)"> 添加子项 </Button>
              <Button size="small" type="link" @click="openEdit(record)"> 编辑 </Button>
              <Popconfirm title="确认删除？" @confirm="handleDelete(record.id)">
                <Button danger size="small" type="link"> 删除 </Button>
              </Popconfirm>
            </Space>
          </template>
        </template>
      </Table>
    </Card>

    <Modal
      v-model:open="modalVisible"
      :title="editingId ? '编辑菜单' : '新增菜单'"
      @ok="handleSubmit"
      :confirm-loading="submitting"
      width="600px"
    >
      <Form :label-col="{ span: 5 }">
        <FormItem label="绑定类型" required>
          <Select v-model:value="formState.bindingType" :options="bindingTypeOptions" />
        </FormItem>
        <FormItem label="绑定值" required>
          <Select v-model:value="formState.bindingValue" :options="getBindingValueOptions()" />
        </FormItem>
        <FormItem label="上级菜单">
          <Input v-model:value="formState.parentId" placeholder="留空为顶级菜单" allow-clear />
        </FormItem>
        <FormItem label="名称" required>
          <Input v-model:value="formState.name" placeholder="如: 工作日志" />
        </FormItem>
        <FormItem label="路径" required>
          <Input v-model:value="formState.path" placeholder="如: /work/log" />
        </FormItem>
        <FormItem label="图标">
          <Input v-model:value="formState.icon" placeholder="如: lucide:clipboard-list" />
        </FormItem>
        <FormItem label="组件">
          <Input v-model:value="formState.component" placeholder="如: /views/work/log/index.vue" />
        </FormItem>
        <FormItem label="排序">
          <InputNumber v-model:value="formState.sort" :min="0" />
        </FormItem>
        <FormItem label="可见">
          <Switch v-model:checked="formState.isVisible" />
        </FormItem>
        <FormItem label="启用">
          <Switch v-model:checked="formState.isEnabled" />
        </FormItem>
      </Form>
    </Modal>
    </div>
  </Page>
</template>
