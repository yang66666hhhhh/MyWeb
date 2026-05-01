<script lang="ts" setup>
import { onMounted, ref, watch } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Form,
  Input,
  InputNumber,
  Modal,
  Popconfirm,
  Select,
  Space,
  Table,
  Tag as AntTag,
  Tabs,
  Transfer,
  message,
} from 'ant-design-vue';

import {
  menuAdminApi,
  tagApi,
  type CreateMenuItemDto,
  type CreateTagDto,
  type CreateUserTypeDto,
  type MenuPathDto,
  type MenuTreeAdminDto,
  type TagDto,
  type UserTagDto,
  type UserTypeDto,
  userTagApi,
  userTypeApi,
} from '#/api/system/menu-tag';

const activeTab = ref('user-types');

const tags = ref<TagDto[]>([]);
const menuTree = ref<MenuTreeAdminDto[]>([]);
const menuPaths = ref<MenuPathDto[]>([]);
const users = ref<UserTagDto[]>([]);
const userTypes = ref<UserTypeDto[]>([]);

async function loadTags() {
  const res = await tagApi.list();
  if (res) tags.value = res;
}

async function loadMenus() {
  const res = await menuAdminApi.getAll();
  if (res) {
    menuTree.value = res;
  }
}

async function loadMenuPaths() {
  const res = await menuAdminApi.getPaths();
  if (res) menuPaths.value = res;
}

async function loadUsers() {
  const res = await userTagApi.getUsers();
  if (res) users.value = res;
}

async function loadUserTypes() {
  const res = await userTypeApi.list();
  if (res) userTypes.value = res;
}

onMounted(() => {
  loadAll();
});

watch(activeTab, () => {
  loadAll();
});

function loadAll() {
  loadTags();
  loadMenus();
  loadMenuPaths();
  loadUsers();
  loadUserTypes();
}

// Tag form
const tagFormOpen = ref(false);
const tagFormState = ref<CreateTagDto>({ name: '', sort: 0, color: '#1890ff' });
const editingTagId = ref<null | string>(null);

function openTagForm(tag?: TagDto) {
  if (tag) {
    editingTagId.value = tag.id;
    tagFormState.value = { name: tag.name, sort: tag.sort, color: tag.color, description: tag.description };
  } else {
    editingTagId.value = null;
    tagFormState.value = { name: '', sort: 0, color: '#1890ff' };
  }
  tagFormOpen.value = true;
}

async function saveTag() {
  if (!tagFormState.value.name) {
    message.error('请输入标签名称');
    return;
  }
  if (editingTagId.value) {
    await tagApi.update(editingTagId.value, tagFormState.value);
    message.success('更新成功');
  } else {
    await tagApi.create(tagFormState.value);
    message.success('创建成功');
  }
  tagFormOpen.value = false;
  loadTags();
}

async function deleteTag(id: string) {
  await tagApi.delete(id);
  message.success('删除成功');
  loadTags();
}

// Menu form
const menuFormOpen = ref(false);
const menuFormState = ref<CreateMenuItemDto>({ name: '', path: '', sort: 0 });
const editingMenuId = ref<null | string>(null);

function openMenuForm(parent?: MenuTreeAdminDto) {
  editingMenuId.value = null;
  menuFormState.value = { name: '', path: '', sort: 0, parentId: parent?.id, tagIds: [] };
  menuFormOpen.value = true;
}

function editMenuForm(menu: MenuTreeAdminDto) {
  editingMenuId.value = menu.id;
  menuFormState.value = {
    name: menu.name,
    path: menu.path,
    icon: menu.icon,
    sort: menu.sort,
    parentId: menu.parentId,
    tagIds: menu.tagIds,
  };
  menuFormOpen.value = true;
}

watch(() => menuFormState.value.path, (newPath) => {
  if (!editingMenuId.value && newPath) {
    const found = menuPaths.value.find((p) => p.path === newPath);
    if (found) {
      menuFormState.value.name = found.name;
      if (found.icon) {
        menuFormState.value.icon = found.icon;
      }
    }
  }
});

async function saveMenu() {
  if (!menuFormState.value.name || !menuFormState.value.path) {
    message.error('请输入名称和路径');
    return;
  }
  if (editingMenuId.value) {
    await menuAdminApi.update(editingMenuId.value, menuFormState.value);
    message.success('更新成功');
  } else {
    await menuAdminApi.create(menuFormState.value);
    message.success('创建成功');
  }
  menuFormOpen.value = false;
  loadMenus();
}

async function deleteMenu(id: string) {
  await menuAdminApi.delete(id);
  message.success('删除成功');
  loadMenus();
}

// UserType form
const userTypeFormOpen = ref(false);
const userTypeFormState = ref<CreateUserTypeDto>({ name: '', code: '', sort: 0, tagIds: [] });
const editingUserTypeId = ref<null | string>(null);

function openUserTypeForm(type?: UserTypeDto) {
  if (type) {
    editingUserTypeId.value = type.id;
    userTypeFormState.value = {
      name: type.name,
      code: type.code,
      description: type.description,
      color: type.color,
      sort: type.sort,
      tagIds: type.tagIds,
    };
  } else {
    editingUserTypeId.value = null;
    userTypeFormState.value = { name: '', code: '', sort: 0, tagIds: [] };
  }
  userTypeFormOpen.value = true;
}

async function saveUserType() {
  if (!userTypeFormState.value.name || !userTypeFormState.value.code) {
    message.error('请输入名称和代码');
    return;
  }
  if (editingUserTypeId.value) {
    await userTypeApi.update(editingUserTypeId.value, userTypeFormState.value);
    message.success('更新成功');
  } else {
    await userTypeApi.create(userTypeFormState.value);
    message.success('创建成功');
  }
  userTypeFormOpen.value = false;
  loadUserTypes();
}

async function deleteUserType(id: string) {
  await userTypeApi.delete(id);
  message.success('删除成功');
  loadUserTypes();
}

// User assign
const userAssignModalOpen = ref(false);
const editingAssignUserId = ref<string | null>(null);
const assignUserTypeId = ref<string | undefined>(undefined);

function openAssignModal(user: UserTagDto) {
  editingAssignUserId.value = user.userId;
  assignUserTypeId.value = user.userTypeId;
  userAssignModalOpen.value = true;
}

async function saveUserAssign() {
  if (!editingAssignUserId.value) return;
  await userTypeApi.assign(editingAssignUserId.value, assignUserTypeId.value);
  message.success('分配成功');
  userAssignModalOpen.value = false;
  loadUsers();
}
</script>

<template>
  <Page>
    <Card>
      <Tabs v-model:activeKey="activeTab">
        <Tabs.TabPane key="user-types" tab="用户类型">
          <div class="mb-4">
            <Button type="primary" @click="openUserTypeForm()">新增用户类型</Button>
          </div>
          <Table :dataSource="userTypes" :rowKey="(r) => r.id" :pagination="false">
            <Table.Column title="名称" dataIndex="name">
              <template #default="{ record }">
                <AntTag :color="record.color">{{ record.name }}</AntTag>
              </template>
            </Table.Column>
            <Table.Column title="代码" dataIndex="code" />
            <Table.Column title="描述" dataIndex="description" />
            <Table.Column title="标签">
              <template #default="{ record }">
                <AntTag v-for="t in record.tagNames" :key="t" :color="tags.find((x) => x.name === t)?.color || '#ccc'">
                  {{ t }}
                </AntTag>
              </template>
            </Table.Column>
            <Table.Column title="操作">
              <template #default="{ record }">
                <Space>
                  <Button size="small" @click="openUserTypeForm(record)">编辑</Button>
                  <Popconfirm title="确定删除?" @confirm="deleteUserType(record.id)">
                    <Button size="small" danger>删除</Button>
                  </Popconfirm>
                </Space>
              </template>
            </Table.Column>
          </Table>
        </Tabs.TabPane>

        <Tabs.TabPane key="tags" tab="标签管理">
          <div class="mb-4">
            <Button type="primary" @click="openTagForm()">新增标签</Button>
          </div>
          <Table :dataSource="tags" :rowKey="(r) => r.id" :pagination="false">
            <Table.Column title="名称" dataIndex="name">
              <template #default="{ record }">
                <AntTag :color="record.color">{{ record.name }}</AntTag>
              </template>
            </Table.Column>
            <Table.Column title="描述" dataIndex="description" />
            <Table.Column title="排序" dataIndex="sort" />
            <Table.Column title="操作">
              <template #default="{ record }">
                <Space>
                  <Button size="small" @click="openTagForm(record)">编辑</Button>
                  <Popconfirm title="确定删除?" @confirm="deleteTag(record.id)">
                    <Button size="small" danger>删除</Button>
                  </Popconfirm>
                </Space>
              </template>
            </Table.Column>
          </Table>
        </Tabs.TabPane>

        <Tabs.TabPane key="menus" tab="菜单管理">
          <div class="mb-4">
            <Button type="primary" @click="openMenuForm()">新增菜单</Button>
          </div>
          <Table :dataSource="menuTree" :rowKey="(r) => r.id" :pagination="false" :key="menuTree.length">
            <Table.Column title="名称" dataIndex="name" />
            <Table.Column title="路径" dataIndex="path" />
            <Table.Column title="标签">
              <template #default="{ record }">
                <AntTag v-for="t in record.tagNames" :key="t" :color="tags.find((x) => x.name === t)?.color || '#ccc'" class="mr-1">
                  {{ t }}
                </AntTag>
              </template>
            </Table.Column>
            <Table.Column title="排序" dataIndex="sort" width="80" />
            <Table.Column title="操作" width="200">
              <template #default="{ record }">
                <Space>
                  <Button size="small" @click="editMenuForm(record)">编辑</Button>
                  <Popconfirm title="确定删除?" @confirm="deleteMenu(record.id)">
                    <Button size="small" danger>删除</Button>
                  </Popconfirm>
                </Space>
              </template>
            </Table.Column>
          </Table>
        </Tabs.TabPane>

        <Tabs.TabPane key="users" tab="用户分配">
          <Table :dataSource="users" :rowKey="(r) => r.userId" :pagination="{ pageSize: 20 }">
            <Table.Column title="用户名" dataIndex="username" />
            <Table.Column title="用户类型">
              <template #default="{ record }">
                <AntTag v-if="record.userTypeName" :color="userTypes.find((x) => x.name === record.userTypeName)?.color || '#ccc'">
                  {{ record.userTypeName }}
                </AntTag>
                <span v-else class="text-gray-400">未分配</span>
              </template>
            </Table.Column>
            <Table.Column title="操作">
              <template #default="{ record }">
                <Button size="small" @click="openAssignModal(record)">分配类型</Button>
              </template>
            </Table.Column>
          </Table>
        </Tabs.TabPane>
      </Tabs>
    </Card>
  </Page>

  <Modal v-model:open="userTypeFormOpen" :title="editingUserTypeId ? '编辑用户类型' : '新增用户类型'" width="600px" @ok="saveUserType">
    <Form layout="vertical">
      <Form.Item label="名称" required>
        <Input v-model:value="userTypeFormState.name" placeholder="如：考研学生" />
      </Form.Item>
      <Form.Item label="代码" required>
        <Input v-model:value="userTypeFormState.code" placeholder="如：exam_student" />
      </Form.Item>
      <Form.Item label="描述">
        <Input v-model:value="userTypeFormState.description" placeholder="描述" />
      </Form.Item>
      <Form.Item label="颜色">
        <Input type="color" v-model:value="userTypeFormState.color" style="width: 100px" />
      </Form.Item>
      <Form.Item label="排序">
        <InputNumber v-model:value="userTypeFormState.sort" :min="0" />
      </Form.Item>
      <Form.Item label="关联标签">
        <Transfer
          v-model:targetKeys="userTypeFormState.tagIds"
          :dataSource="tags.map((t) => ({ key: t.id, title: t.name }))"
          :titles="['可选标签', '已选标签']"
          :render="(item) => item.title || ''"
        />
      </Form.Item>
    </Form>
  </Modal>

  <Modal v-model:open="tagFormOpen" :title="editingTagId ? '编辑标签' : '新增标签'" @ok="saveTag">
    <Form layout="vertical">
      <Form.Item label="名称" required>
        <Input v-model:value="tagFormState.name" placeholder="标签名称" />
      </Form.Item>
      <Form.Item label="描述">
        <Input v-model:value="tagFormState.description" placeholder="描述" />
      </Form.Item>
      <Form.Item label="颜色">
        <Input type="color" v-model:value="tagFormState.color" style="width: 100px" />
      </Form.Item>
      <Form.Item label="排序">
        <InputNumber v-model:value="tagFormState.sort" :min="0" />
      </Form.Item>
    </Form>
  </Modal>

  <Modal v-model:open="menuFormOpen" :title="editingMenuId ? '编辑菜单' : '新增菜单'" width="600px" @ok="saveMenu">
    <Form layout="vertical">
      <Form.Item label="路径" required>
        <Select v-model:value="menuFormState.path" placeholder="选择路径" style="width: 100%">
          <Select.Option v-for="p in menuPaths" :key="p.path" :value="p.path">
            <div class="flex items-center gap-2">
              <span>{{ p.name }}</span>
              <span class="text-gray-400 text-xs">{{ p.path }}</span>
              <span v-if="p.exists" class="text-red-400 text-xs">(已存在)</span>
            </div>
          </Select.Option>
        </Select>
      </Form.Item>
      <Form.Item label="名称" required>
        <Input v-model:value="menuFormState.name" placeholder="自动填充或手动输入" />
      </Form.Item>
      <Form.Item label="图标">
        <Input v-model:value="menuFormState.icon" placeholder="自动填充或手动输入" />
      </Form.Item>
      <Form.Item label="排序">
        <InputNumber v-model:value="menuFormState.sort" :min="0" />
      </Form.Item>
      <Form.Item label="关联标签">
        <Transfer
          v-model:targetKeys="menuFormState.tagIds"
          :dataSource="tags.map((t) => ({ key: t.id, title: t.name }))"
          :titles="['可选标签', '已选标签']"
          :render="(item) => item.title || ''"
        />
      </Form.Item>
    </Form>
  </Modal>

  <Modal v-model:open="userAssignModalOpen" title="分配用户类型" @ok="saveUserAssign">
    <Form layout="vertical">
      <Form.Item label="选择用户类型">
        <Select v-model:value="assignUserTypeId" placeholder="请选择用户类型" style="width: 200px" allowClear>
          <Select.Option v-for="ut in userTypes" :key="ut.id" :value="ut.id">
            <AntTag :color="ut.color">{{ ut.name }}</AntTag>
          </Select.Option>
        </Select>
      </Form.Item>
    </Form>
  </Modal>
</template>