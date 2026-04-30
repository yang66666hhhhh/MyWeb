<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Card,
  Col,
  Descriptions,
  Form,
  Input,
  Row,
  Button,
  message,
  Modal,
} from 'ant-design-vue';

import {
  changePasswordApi,
  getUserProfileApi,
  updateUserProfileApi,
  type ChangePasswordDto,
  type UpdateProfileDto,
  type UserProfileDto,
} from '#/api/system/profile';

const profile = ref<UserProfileDto | null>(null);
const loading = ref(false);
const editOpen = ref(false);
const passwordOpen = ref(false);

const editForm = ref({
  realName: '',
  avatar: '',
  email: '',
  phone: '',
});

const passwordForm = ref({
  oldPassword: '',
  newPassword: '',
  confirmPassword: '',
});

async function loadProfile() {
  loading.value = true;
  try {
    profile.value = await getUserProfileApi();
    if (profile.value) {
      editForm.value = {
        realName: profile.value.realName,
        avatar: profile.value.avatar || '',
        email: profile.value.email || '',
        phone: profile.value.phone || '',
      };
    }
  } finally {
    loading.value = false;
  }
}

function openEdit() {
  editOpen.value = true;
}

async function handleEditSubmit() {
  const data: UpdateProfileDto = {
    realName: editForm.value.realName || undefined,
    avatar: editForm.value.avatar || undefined,
    email: editForm.value.email || undefined,
    phone: editForm.value.phone || undefined,
  };
  await updateUserProfileApi(data);
  message.success('更新成功');
  editOpen.value = false;
  await loadProfile();
}

function openPasswordModal() {
  passwordForm.value = {
    oldPassword: '',
    newPassword: '',
    confirmPassword: '',
  };
  passwordOpen.value = true;
}

async function handlePasswordSubmit() {
  if (passwordForm.value.newPassword !== passwordForm.value.confirmPassword) {
    message.error('两次输入的密码不一致');
    return;
  }
  if (passwordForm.value.newPassword.length < 6) {
    message.error('密码长度不能少于6位');
    return;
  }
  const data: ChangePasswordDto = {
    oldPassword: passwordForm.value.oldPassword,
    newPassword: passwordForm.value.newPassword,
  };
  await changePasswordApi(data);
  message.success('密码修改成功');
  passwordOpen.value = false;
}

function getRoleLabel(roles: string) {
  if (roles.includes('super')) return '超级管理员';
  if (roles.includes('admin')) return '管理员';
  return '普通用户';
}

onMounted(() => {
  loadProfile();
});
</script>

<template>
  <Page>
    <Row :gutter="16">
      <Col :span="16">
        <Card title="个人信息" :loading="loading">
          <Descriptions v-if="profile" :column="2" bordered>
            <Descriptions.Item label="用户名">
              {{ profile.username }}
            </Descriptions.Item>
            <Descriptions.Item label="角色">
              {{ getRoleLabel(profile.roles) }}
            </Descriptions.Item>
            <Descriptions.Item label="姓名">
              {{ profile.realName }}
            </Descriptions.Item>
            <Descriptions.Item label="邮箱">
              {{ profile.email || '-' }}
            </Descriptions.Item>
            <Descriptions.Item label="电话">
              {{ profile.phone || '-' }}
            </Descriptions.Item>
            <Descriptions.Item label="头像">
              <img v-if="profile.avatar" :src="profile.avatar" style="width: 50px; height: 50px; border-radius: 50%;" />
              <span v-else>-</span>
            </Descriptions.Item>
          </Descriptions>

          <div class="mt-4">
            <Button type="primary" @click="openEdit">
              编辑资料
            </Button>
            <Button class="ml-2" @click="openPasswordModal">
              修改密码
            </Button>
          </div>
        </Card>
      </Col>

      <Col :span="8">
        <Card title="账号安全" :loading="loading">
          <div class="flex flex-col gap-3">
            <div>
              <div class="text-gray-500">用户名</div>
              <div class="font-medium">{{ profile?.username }}</div>
            </div>
            <div>
              <div class="text-gray-500">角色</div>
              <div class="font-medium">{{ getRoleLabel(profile?.roles || '') }}</div>
            </div>
            <div>
              <div class="text-gray-500">账号状态</div>
              <div class="font-medium">{{ profile?.status === 0 ? '正常' : '异常' }}</div>
            </div>
          </div>
        </Card>
      </Col>
    </Row>

    <Modal v-model:open="editOpen" title="编辑资料" @ok="handleEditSubmit">
      <Form :model="editForm" layout="vertical">
        <Form.Item label="姓名" name="realName">
          <Input v-model:value="editForm.realName" placeholder="请输入姓名" />
        </Form.Item>
        <Form.Item label="头像URL" name="avatar">
          <Input v-model:value="editForm.avatar" placeholder="请输入头像URL" />
        </Form.Item>
        <Form.Item label="邮箱" name="email">
          <Input v-model:value="editForm.email" placeholder="请输入邮箱" />
        </Form.Item>
        <Form.Item label="电话" name="phone">
          <Input v-model:value="editForm.phone" placeholder="请输入电话" />
        </Form.Item>
      </Form>
    </Modal>

    <Modal v-model:open="passwordOpen" title="修改密码" @ok="handlePasswordSubmit">
      <Form :model="passwordForm" layout="vertical">
        <Form.Item label="原密码" name="oldPassword" required>
          <Input v-model:value="passwordForm.oldPassword" type="password" placeholder="请输入原密码" />
        </Form.Item>
        <Form.Item label="新密码" name="newPassword" required>
          <Input v-model:value="passwordForm.newPassword" type="password" placeholder="请输入新密码" />
        </Form.Item>
        <Form.Item label="确认密码" name="confirmPassword" required>
          <Input v-model:value="passwordForm.confirmPassword" type="password" placeholder="请再次输入新密码" />
        </Form.Item>
      </Form>
    </Modal>
  </Page>
</template>