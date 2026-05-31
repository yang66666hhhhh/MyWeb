<script lang="ts" setup>
import { computed, nextTick, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { useAccessStore } from '@vben/stores';

import {
  Button,
  Card,
  Input,
  List,
  ListItem,
  ListItemMeta,
  message,
  Popconfirm,
  Space,
  Spin,
} from 'ant-design-vue';

import type { AiChatMessage, AiChatSession } from '#/api/ai';

import { aiApi } from '#/api/ai';

const loading = ref(false);
const sending = ref(false);
const inputMessage = ref('');
const messages = ref<AiChatMessage[]>([]);
const sessions = ref<AiChatSession[]>([]);
const currentSessionId = ref<string | null>(null);
const sessionsLoading = ref(false);

const accessStore = useAccessStore();
const canUseAssistant = computed(() => accessStore.accessCodes.includes('AI_ASSISTANT'));
const canDeleteSession = computed(() => accessStore.accessCodes.includes('AI_ASSISTANT'));

const messagesEndRef = ref<HTMLDivElement>();

const scrollToBottom = () => {
  nextTick(() => {
    messagesEndRef.value?.scrollIntoView({ behavior: 'smooth' });
  });
};

const loadSessions = async () => {
  sessionsLoading.value = true;
  try {
    const res = await aiApi.getChatSessions({ page: 1, pageSize: 50 });
    sessions.value = res.items;
  } catch {
    message.error('加载失败，请稍后重试');
  } finally {
    sessionsLoading.value = false;
  }
};

const loadMessages = async (sessionId: string) => {
  loading.value = true;
  try {
    messages.value = await aiApi.getChatMessages(sessionId);
    scrollToBottom();
  } catch {
    message.error('加载失败，请稍后重试');
  } finally {
    loading.value = false;
  }
};

const handleSelectSession = (session: AiChatSession) => {
  currentSessionId.value = session.id;
  loadMessages(session.id);
};

const handleNewSession = () => {
  currentSessionId.value = null;
  messages.value = [];
};

const handleDeleteSession = async (sessionId: string) => {
  try {
    await aiApi.deleteChatSession(sessionId);
    message.success('会话已删除');
    if (currentSessionId.value === sessionId) {
      currentSessionId.value = null;
      messages.value = [];
    }
    loadSessions();
  } catch {
    message.error('删除失败');
  }
};

const sendMessage = async () => {
  if (!inputMessage.value.trim() || sending.value) return;

  const userMessage = inputMessage.value;
  inputMessage.value = '';

  messages.value.push({
    id: Date.now().toString(),
    sessionId: currentSessionId.value || '',
    role: 'user',
    content: userMessage,
    createdAt: new Date().toISOString(),
  });

  scrollToBottom();
  sending.value = true;

  try {
    const res = await aiApi.chat({
      message: userMessage,
      sessionId: currentSessionId.value || undefined,
    });

    if (res.sessionId && !currentSessionId.value) {
      currentSessionId.value = res.sessionId;
      loadSessions();
    }

    messages.value.push({
      id: res.messageId || (Date.now() + 1).toString(),
      sessionId: currentSessionId.value || '',
      role: 'assistant',
      content: res.content,
      createdAt: new Date().toISOString(),
    });

    scrollToBottom();
  } catch {
    message.error('发送失败');
  } finally {
    sending.value = false;
  }
};

onMounted(() => {
  loadSessions();
});
</script>

<template>
  <Page description="与AI助手对话，获取智能建议" title="AI助手">
    <div class="flex gap-4 h-[calc(100vh-200px)]">
      <Card title="会话列表" class="w-[280px] flex-shrink-0" :loading="sessionsLoading">
        <template #extra>
          <Button v-if="canUseAssistant" type="link" size="small" @click="handleNewSession">新建</Button>
        </template>
        <div class="h-[calc(100%-40px)] overflow-auto">
          <List :data-source="sessions" :split="false">
            <template #renderItem="{ item }">
              <ListItem
                class="cursor-pointer px-2 py-2 rounded"
                :class="{ 'bg-blue-50': currentSessionId === item.id }"
                @click="handleSelectSession(item)"
              >
                <ListItemMeta :title="item.title || '新会话'" :description="item.lastMessage || '暂无消息'" />
                <template #actions>
                  <Popconfirm v-if="canDeleteSession" title="确认删除此会话?" @confirm="handleDeleteSession(item.id)">
                    <Button type="link" size="small" danger @click.stop>删除</Button>
                  </Popconfirm>
                </template>
              </ListItem>
            </template>
          </List>
        </div>
      </Card>

      <Card class="flex-1 flex flex-col">
        <div class="flex-1 overflow-auto mb-4 px-2">
          <Spin :spinning="loading">
            <div v-if="messages.length === 0" class="flex items-center justify-center h-full text-gray-400">
              选择一个会话或开始新对话
            </div>
            <div v-else class="space-y-4">
              <div
                v-for="msg in messages"
                :key="msg.id"
                class="flex"
                :class="msg.role === 'user' ? 'justify-end' : 'justify-start'"
              >
                <div
                  class="max-w-[70%] p-3 rounded-lg"
                  :class="msg.role === 'user' ? 'bg-blue-500 text-white' : 'bg-gray-100'"
                  style="white-space: pre-wrap;"
                >
                  {{ msg.content }}
                </div>
              </div>
              <div v-if="sending" class="flex justify-start">
                <div class="bg-gray-100 p-3 rounded-lg">
                  <Spin size="small" /> 思考中...
                </div>
              </div>
              <div ref="messagesEndRef" />
            </div>
          </Spin>
        </div>
        <Space class="w-full">
          <Input.TextArea
            v-model:value="inputMessage"
            placeholder="输入消息... (Enter 发送)"
            :rows="2"
            :disabled="sending"
            @press-enter.exact.prevent="sendMessage"
          />
          <Button type="primary" :loading="sending" @click="sendMessage">发送</Button>
        </Space>
      </Card>
    </div>
  </Page>
</template>
