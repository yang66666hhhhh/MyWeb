<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Input,
  List,
  Select,
  Space,
  Tag,
  message,
} from 'ant-design-vue';
import {
  DeleteOutlined,
  RobotOutlined,
  SendOutlined,
  UserOutlined,
} from '@ant-design/icons-vue';

import { sendKnowledgeChatMessageApi } from '#/api/ai/extended';

interface ChatMessage {
  role: 'assistant' | 'user';
  content: string;
  time: string;
}

const prompt = ref('');
const loading = ref(false);
const messages = ref<ChatMessage[]>([]);
const selectedModel = ref('gpt-4');
const temperature = ref(0.7);

const modelOptions = [
  { label: 'GPT-4', value: 'gpt-4' },
  { label: 'GPT-3.5 Turbo', value: 'gpt-3.5-turbo' },
  { label: 'Claude 3', value: 'claude-3' },
];

async function handleSend() {
  if (!prompt.value.trim()) return;

  const userMessage: ChatMessage = {
    role: 'user',
    content: prompt.value,
    time: new Date().toLocaleTimeString(),
  };
  messages.value.push(userMessage);

  const currentPrompt = prompt.value;
  prompt.value = '';
  loading.value = true;

  try {
    const res = await sendKnowledgeChatMessageApi({ message: currentPrompt });
    const aiMessage: ChatMessage = {
      role: 'assistant',
      content: res.content,
      time: new Date().toLocaleTimeString(),
    };
    messages.value.push(aiMessage);
  } catch (e: unknown) {
    message.error(e instanceof Error ? e.message : '请求失败');
  } finally {
    loading.value = false;
  }
}

function handleClear() {
  messages.value = [];
}
</script>

<template>
  <Page title="AI 实验室" description="交互式 AI 测试环境">
    <div class="space-y-4">
      <Card>
        <div class="flex items-center justify-between">
          <Space>
            <span>模型:</span>
            <Select
              v-model:value="selectedModel"
              :options="modelOptions"
              style="width: 150px"
            />
            <span>温度:</span>
            <Select v-model:value="temperature" style="width: 80px">
              <Select.Option :value="0">0</Select.Option>
              <Select.Option :value="0.3">0.3</Select.Option>
              <Select.Option :value="0.5">0.5</Select.Option>
              <Select.Option :value="0.7">0.7</Select.Option>
              <Select.Option :value="1">1</Select.Option>
            </Select>
          </Space>
          <Button @click="handleClear">
            <DeleteOutlined /> 清空对话
          </Button>
        </div>
      </Card>

      <Card>
        <div class="mb-4 max-h-[500px] min-h-[300px] overflow-y-auto">
          <List
            :data-source="messages"
            :locale="{ emptyText: '暂无对话，请输入消息开始' }"
          >
            <template #renderItem="{ item }">
              <List.Item>
                <div
                  class="w-full"
                  :class="item.role === 'user' ? 'text-right' : 'text-left'"
                >
                  <div class="mb-1 inline-flex items-center gap-2">
                    <Tag :color="item.role === 'user' ? 'blue' : 'green'">
                      <UserOutlined v-if="item.role === 'user'" />
                      <RobotOutlined v-else />
                      {{ item.role === 'user' ? '用户' : 'AI' }}
                    </Tag>
                    <span class="text-xs text-gray-400">{{ item.time }}</span>
                  </div>
                  <div
                    class="mt-1 inline-block max-w-[80%] rounded-lg p-3"
                    :class="
                      item.role === 'user'
                        ? 'bg-blue-50 text-right'
                        : 'bg-gray-50 text-left'
                    "
                  >
                    {{ item.content }}
                  </div>
                </div>
              </List.Item>
            </template>
          </List>

          <div v-if="loading" class="flex items-center gap-2 p-3 text-gray-400">
            <RobotOutlined />
            <span class="animate-pulse">AI 正在思考...</span>
          </div>
        </div>

        <div class="flex gap-2">
          <Input.TextArea
            v-model:value="prompt"
            :rows="2"
            placeholder="输入消息... (Enter 发送)"
            @pressEnter="handleSend"
          />
          <Button type="primary" :loading="loading" @click="handleSend">
            <SendOutlined />
          </Button>
        </div>
      </Card>
    </div>
  </Page>
</template>
