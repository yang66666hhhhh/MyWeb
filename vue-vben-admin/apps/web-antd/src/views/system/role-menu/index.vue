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

import type { TableColumnsType } from 'ant-design-vue';

import {
  createRoleMenuApi,
  deleteRoleMenuApi,
  getMenuListApi,
  type RoleMenuItem,
  type UpsertRoleMenuInput,
  updateRoleMenuApi,
} from '#/api/core/menu';

type FormState = UpsertRoleMenuInput;

const loading = ref(false);
const menus = ref<RoleMenuItem[]>([]);
const modalVisible = ref(false);
const editingId = ref<string | undefined>(undefined);
const submitting = ref(false);

const roleLevelOptions = [
  { label: 'Member', value: 1 },
  { label: 'Pro', value: 2 },
  { label: 'Owner', value: 3 },
];

const personaOptions = [
  { label: '通用', value: 'General' },
  { label: '开发者', value: 'Developer' },
  { label: '实施工程师', value: 'Implementation' },
  { label: '销售', value: 'Sales' },
  { label: '教师', value: 'Teacher' },
  { label: '设计师', value: 'Designer' },
  { label: '学生', value: 'Student' },
  { label: '自由职业', value: 'Freelancer' },
];

const categoryOptions = [
  { label: 'Dashboard', value: 'Dashboard' },
  { label: 'Growth', value: 'Growth' },
  { label: 'Work', value: 'Work' },
  { label: 'AI', value: 'AI' },
  { label: 'Assets', value: 'Assets' },
  { label: 'Analytics', value: 'Analytics' },
  { label: 'System', value: 'System' },
  { label: 'Persona', value: 'Persona' },
  { label: 'General', value: 'General' },
];

const columns: TableColumnsType = [
  { title: '名称', dataIndex: 'name', key: 'name', width: 190 },
  { title: '路径', dataIndex: 'path', key: 'path', width: 220 },
  { title: '组件', dataIndex: 'component', key: 'component', width: 180, ellipsis: true },
  { title: '角色等级', dataIndex: 'minRoleLevel', key: 'minRoleLevel', width: 100 },
  { title: 'Persona', dataIndex: 'personaTag', key: 'personaTag', width: 120 },
  { title: '功能码', dataIndex: 'featureCode', key: 'featureCode', width: 160 },
  { title: '分类', dataIndex: 'menuCategory', key: 'menuCategory', width: 110 },
  { title: '排序', dataIndex: 'sort', key: 'sort', width: 70 },
  { title: '可见', dataIndex: 'isVisible', key: 'isVisible', width: 70 },
  { title: '启用', dataIndex: 'isEnabled', key: 'isEnabled', width: 70 },
  { title: '操作', key: 'action', width: 220, fixed: 'right' },
];

const roleLevelColor: Record<number, string> = {
  1: 'green',
  2: 'blue',
  3: 'purple',
};

function createDefaultFormState(parentId?: string): FormState {
  return {
    parentId,
    name: '',
    path: '',
    icon: '',
    component: '',
    sort: 0,
    isVisible: true,
    isEnabled: true,
    permission: '',
    redirect: '',
    isExternal: false,
    badge: '',
    tag: '',
    minRoleLevel: 1,
    personaTag: undefined,
    isBaseMenu: true,
    menuCategory: 'General',
    featureCode: '',
  };
}

const formState = ref<FormState>(createDefaultFormState());

async function fetchMenus() {
  loading.value = true;
  try {
    menus.value = await getMenuListApi();
  } catch (error) {
    console.error(error);
    message.error('菜单加载失败');
  } finally {
    loading.value = false;
  }
}

function getRoleLevelLabel(level: number) {
  return roleLevelOptions.find((item) => item.value === level)?.label ?? `Level ${level}`;
}

function normalizeOptionalValue(value?: string) {
  const normalized = value?.trim();
  return normalized ? normalized : undefined;
}

function buildPayload(): UpsertRoleMenuInput {
  return {
    ...formState.value,
    parentId: normalizeOptionalValue(formState.value.parentId),
    name: formState.value.name.trim(),
    path: formState.value.path.trim(),
    icon: normalizeOptionalValue(formState.value.icon),
    component: normalizeOptionalValue(formState.value.component),
    permission: normalizeOptionalValue(formState.value.permission),
    redirect: normalizeOptionalValue(formState.value.redirect),
    badge: normalizeOptionalValue(formState.value.badge),
    tag: normalizeOptionalValue(formState.value.tag),
    personaTag: normalizeOptionalValue(formState.value.personaTag),
    menuCategory: formState.value.menuCategory || 'General',
    featureCode: normalizeOptionalValue(formState.value.featureCode),
  };
}

function openCreate(parentId?: string) {
  editingId.value = undefined;
  formState.value = createDefaultFormState(parentId);
  modalVisible.value = true;
}

function openEdit(record: Record<string, any>) {
  const menu = record as RoleMenuItem;
  editingId.value = menu.id;
  formState.value = {
    parentId: menu.parentId,
    name: menu.name,
    path: menu.path,
    icon: menu.icon ?? '',
    component: menu.component ?? '',
    sort: menu.sort,
    isVisible: menu.isVisible,
    isEnabled: menu.isEnabled,
    permission: menu.permission ?? '',
    redirect: menu.redirect ?? '',
    isExternal: menu.isExternal,
    badge: menu.badge ?? '',
    tag: menu.tag ?? '',
    minRoleLevel: menu.minRoleLevel,
    personaTag: menu.personaTag,
    isBaseMenu: menu.isBaseMenu,
    menuCategory: menu.menuCategory,
    featureCode: menu.featureCode ?? '',
  };
  modalVisible.value = true;
}

async function handleSubmit() {
  const payload = buildPayload();
  if (!payload.name || !payload.path) {
    message.warning('名称和路径不能为空');
    return;
  }

  submitting.value = true;
  try {
    if (editingId.value) {
      await updateRoleMenuApi(editingId.value, payload);
      message.success('更新成功');
    } else {
      await createRoleMenuApi(payload);
      message.success('创建成功');
    }
    modalVisible.value = false;
    await fetchMenus();
  } catch (error) {
    console.error(error);
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
  } catch (error) {
    console.error(error);
    message.error('删除失败');
  }
}

onMounted(() => {
  fetchMenus();
});
</script>

<template>
  <Page description="按角色等级、Persona、订阅功能统一配置动态菜单" title="菜单管理">
    <div class="space-y-4">
      <Card>
        <Space>
          <Button type="primary" @click="openCreate()"> 新增菜单 </Button>
        </Space>
      </Card>

      <Card>
        <Table
          :columns="columns"
          :data-source="menus"
          :default-expand-all-rows="true"
          :loading="loading"
          :pagination="false"
          :scroll="{ x: 1580 }"
          children-column-name="children"
          row-key="id"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.key === 'name'">
              <Tag v-if="record.parentId" color="blue">{{ record.name }}</Tag>
              <strong v-else>{{ record.name }}</strong>
            </template>
            <template v-else-if="column.key === 'minRoleLevel'">
              <Tag :color="roleLevelColor[record.minRoleLevel] || 'default'">
                {{ getRoleLevelLabel(record.minRoleLevel) }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'personaTag'">
              <Tag v-if="record.personaTag">{{ record.personaTag }}</Tag>
              <span v-else>-</span>
            </template>
            <template v-else-if="column.key === 'featureCode'">
              <Tag v-if="record.featureCode" color="cyan">{{ record.featureCode }}</Tag>
              <span v-else>-</span>
            </template>
            <template v-else-if="column.key === 'menuCategory'">
              <Tag>{{ record.menuCategory }}</Tag>
            </template>
            <template v-else-if="column.key === 'isVisible'">
              <Tag :color="record.isVisible ? 'green' : 'red'">
                {{ record.isVisible ? '是' : '否' }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'isEnabled'">
              <Tag :color="record.isEnabled ? 'green' : 'red'">
                {{ record.isEnabled ? '是' : '否' }}
              </Tag>
            </template>
            <template v-else-if="column.key === 'action'">
              <Space class="whitespace-nowrap">
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
        :confirm-loading="submitting"
        :title="editingId ? '编辑菜单' : '新增菜单'"
        width="720px"
        @ok="handleSubmit"
      >
        <Form :label-col="{ span: 5 }">
          <FormItem label="上级菜单">
            <Input v-model:value="formState.parentId" allow-clear placeholder="留空为顶级菜单" />
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
            <InputNumber v-model:value="formState.sort" :min="0" class="w-full" />
          </FormItem>
          <FormItem label="最低角色" required>
            <Select v-model:value="formState.minRoleLevel" :options="roleLevelOptions" />
          </FormItem>
          <FormItem label="Persona">
            <Select
              v-model:value="formState.personaTag"
              :options="personaOptions"
              allow-clear
              placeholder="不限制 Persona"
            />
          </FormItem>
          <FormItem label="功能码">
            <Input v-model:value="formState.featureCode" placeholder="如: work.logs" />
          </FormItem>
          <FormItem label="菜单分类" required>
            <Select v-model:value="formState.menuCategory" :options="categoryOptions" />
          </FormItem>
          <FormItem label="权限码">
            <Input v-model:value="formState.permission" placeholder="可选" />
          </FormItem>
          <FormItem label="重定向">
            <Input v-model:value="formState.redirect" placeholder="可选" />
          </FormItem>
          <FormItem label="徽标">
            <Input v-model:value="formState.badge" placeholder="可选" />
          </FormItem>
          <FormItem label="标签">
            <Input v-model:value="formState.tag" placeholder="可选" />
          </FormItem>
          <FormItem label="基础菜单">
            <Switch v-model:checked="formState.isBaseMenu" />
          </FormItem>
          <FormItem label="外链">
            <Switch v-model:checked="formState.isExternal" />
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
