<script lang="ts" setup>
import { computed, h, nextTick, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';
import { useAccessStore } from '@vben/stores';

import {
  Avatar,
  Button,
  Input,
  List,
  message,
  Popconfirm,
  Spin,
} from 'ant-design-vue';
import {
  DeleteOutlined,
  PlusOutlined,
  RobotOutlined,
  SendOutlined,
  UserOutlined,
} from '@ant-design/icons-vue';

import type { KnowledgeChatMessage } from '#/api/ai/extended';

import {
  deleteKnowledgeChatSessionApi,
  getKnowledgeChatMessagesApi,
  getKnowledgeChatSessionsApi,
  sendKnowledgeChatMessageApi,
} from '#/api/ai/extended';
import { usePagedQuery } from '#/composables/usePagedQuery';

const accessStore = useAccessStore();
const canUseChat = computed(() => accessStore.accessCodes.includes('AI_KNOWLEDGE'));
const canDeleteChat = computed(() => accessStore.accessCodes.includes('AI_KNOWLEDGE'));

const chatInput = ref('');
const sending = ref(false);
const loadingMessages = ref(false);
const selectedSessionId = ref<null | string>(null);
const messages = ref<KnowledgeChatMessage[]>([]);
const messageListRef = ref<HTMLElement>();

const { items, load, loading, query, total, changePage } = usePagedQuery<
  any,
  { keyword?: string; page: number; pageSize: number; type?: string }
>({
  defaultQuery: {
    page: 1,
    pageSize: 10,
  },
  fetcher: getKnowledgeChatSessionsApi,
});

const selectedSession = computed(() =>
  items.value.find((i: any) => i.id === selectedSessionId.value),
);

async function loadMessages(sessionId: string) {
  loadingMessages.value = true;
  try {
    messages.value = await getKnowledgeChatMessagesApi(sessionId);
    await nextTick();
    scrollToBottom();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '加载消息失败');
  } finally {
    loadingMessages.value = false;
  }
}

async function handleSelectSession(id: string) {
  selectedSessionId.value = id;
  await loadMessages(id);
}

async function handleSendMessage() {
  if (!chatInput.value.trim()) {
    message.warning('请输入消息');
    return;
  }

  const userContent = chatInput.value;
  chatInput.value = '';

  const tempUserMsg: KnowledgeChatMessage = {
    id: `temp-${Date.now()}`,
    sessionId: selectedSessionId.value || '',
    role: 'user',
    content: userContent,
    createdAt: new Date().toISOString(),
  };
  messages.value.push(tempUserMsg);
  await nextTick();
  scrollToBottom();

  sending.value = true;
  try {
    const res = await sendKnowledgeChatMessageApi({
      message: userContent,
      sessionId: selectedSessionId.value || undefined,
    });

    tempUserMsg.sessionId = res.sessionId;

    const aiMsg: KnowledgeChatMessage = {
      id: `ai-${Date.now()}`,
      sessionId: res.sessionId,
      role: 'assistant',
      content: res.content,
      createdAt: new Date().toISOString(),
    };
    messages.value.push(aiMsg);

    if (!selectedSessionId.value) {
      selectedSessionId.value = res.sessionId;
      await load();
    }
    await nextTick();
    scrollToBottom();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '发送失败');
    messages.value = messages.value.filter((m) => m.id !== tempUserMsg.id);
  } finally {
    sending.value = false;
  }
}

async function handleDeleteSession(id: string) {
  try {
    await deleteKnowledgeChatSessionApi(id);
    message.success('删除成功');
    if (selectedSessionId.value === id) {
      selectedSessionId.value = null;
      messages.value = [];
    }
    await load();
  } catch (e: unknown) {
    message.error((e instanceof Error ? e.message : null) || '删除失败');
  }
}

function handleNewChat() {
  selectedSessionId.value = null;
  messages.value = [];
}

function handlePageChange(page: number, pageSize: number) {
  void changePage(page, pageSize);
}

function scrollToBottom() {
  if (messageListRef.value) {
    messageListRef.value.scrollTop = messageListRef.value.scrollHeight;
  }
}

function formatTime(dateStr: string) {
  const d = new Date(dateStr);
  return d.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' });
}

function handleInputPressEnter(e: KeyboardEvent) {
  if (!e.shiftKey) {
    e.preventDefault();
    handleSendMessage();
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page description="基于知识库的智能问答" title="知识问答">
    <div class="chat-container">
      <!-- 左侧会话列表 -->
      <div class="chat-sidebar">
        <div class="sidebar-header">
          <span class="font-medium">会话列表</span>
          <Button size="small" type="primary" @click="handleNewChat">
            <PlusOutlined /> 新对话
          </Button>
        </div>

        <div class="sidebar-content">
          <List
            :data-source="items"
            :loading="loading"
            size="small"
          >
            <template #renderItem="{ item }">
              <div
                :class="{ 'session-active': selectedSessionId === item.id }"
                class="session-item"
                @click="handleSelectSession(item.id)"
              >
                <div class="session-info">
                  <div class="session-title">{{ item.title || '新对话' }}</div>
                  <div v-if="item.lastMessage" class="session-preview">
                    {{ item.lastMessage }}
                  </div>
                  <div class="session-meta">
                    <span>{{ item.messageCount }} 条消息</span>
                    <span>{{ formatTime(item.createdAt) }}</span>
                  </div>
                </div>
                <Popconfirm
                  title="确认删除？"
                  @confirm.stop="handleDeleteSession(item.id)"
                >
                  <Button
                    v-if="canDeleteChat"
                    class="delete-btn"
                    danger
                    size="small"
                    type="text"
                    @click.stop
                  >
                    <DeleteOutlined />
                  </Button>
                </Popconfirm>
              </div>
            </template>
          </List>
          <div v-if="total > query.pageSize" class="load-more">
            <Button type="link" @click="handlePageChange(query.page + 1, query.pageSize)">
              加载更多
            </Button>
          </div>
        </div>
      </div>

      <!-- 右侧对话区域 -->
      <div class="chat-main">
        <!-- 未选择会话的空状态 -->
        <div v-if="!selectedSessionId && messages.length === 0" class="empty-state">
          <RobotOutlined class="empty-icon" />
          <div class="empty-text">选择一个会话或开始新对话</div>
        </div>

        <!-- 对话界面 -->
        <template v-else>
          <!-- 会话标题 -->
          <div class="chat-header">
            <span class="font-medium">
              {{ selectedSession?.title || '新对话' }}
            </span>
          </div>

          <!-- 消息列表 -->
          <div ref="messageListRef" class="message-list">
            <Spin :spinning="loadingMessages">
              <div class="message-inner">
                <div
                  v-for="msg in messages"
                  :key="msg.id"
                  :class="msg.role === 'user' ? 'message-user' : 'message-assistant'"
                  class="message-wrapper"
                >
                  <div class="message-avatar">
                    <Avatar
                      v-if="msg.role === 'user'"
                      :icon="h(UserOutlined)"
                      style="background-color: #87d068"
                    />
                    <Avatar
                      v-else
                      :icon="h(RobotOutlined)"
                      style="background-color: #1890ff"
                    />
                  </div>
                  <div class="message-body">
                    <div
                      :class="msg.role === 'user' ? 'bubble-user' : 'bubble-assistant'"
                      class="message-bubble"
                    >
                      {{ msg.content }}
                    </div>
                    <div
                      :class="msg.role === 'user' ? 'time-right' : 'time-left'"
                      class="message-time"
                    >
                      {{ formatTime(msg.createdAt) }}
                    </div>
                  </div>
                </div>

                <!-- 发送中状态 -->
                <div v-if="sending" class="message-wrapper message-assistant">
                  <div class="message-avatar">
                    <Avatar :icon="h(RobotOutlined)" style="background-color: #1890ff" />
                  </div>
                  <div class="message-body">
                    <div class="bubble-assistant message-bubble">
                      <span class="typing-indicator">思考中...</span>
                    </div>
                  </div>
                </div>
              </div>
            </Spin>
          </div>

          <!-- 输入区域 -->
          <div class="chat-input-area">
            <div class="input-wrapper">
              <Input.TextArea
                v-model:value="chatInput"
                :auto-size="{ minRows: 1, maxRows: 4 }"
                :disabled="!canUseChat"
                placeholder="输入消息，按 Enter 发送，Shift+Enter 换行..."
                @keydown="handleInputPressEnter"
              />
              <Button
                :disabled="!canUseChat || !chatInput.trim()"
                :loading="sending"
                class="send-btn"
                type="primary"
                @click="handleSendMessage"
              >
                <SendOutlined />
              </Button>
            </div>
          </div>
        </template>
      </div>
    </div>
  </Page>
</template>

<style scoped>
.chat-container {
  display: flex;
  height: calc(100vh - 200px);
  min-height: 500px;
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  overflow: hidden;
  background: #fff;
}

.chat-sidebar {
  width: 300px;
  min-width: 300px;
  border-right: 1px solid #e8e8e8;
  display: flex;
  flex-direction: column;
}

.sidebar-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 16px;
  border-bottom: 1px solid #e8e8e8;
}

.sidebar-content {
  flex: 1;
  overflow-y: auto;
}

.session-item {
  display: flex;
  align-items: flex-start;
  padding: 12px 16px;
  cursor: pointer;
  transition: background-color 0.2s;
  border-bottom: 1px solid #f0f0f0;
}

.session-item:hover {
  background-color: #f5f5f5;
}

.session-item:hover .delete-btn {
  opacity: 1;
}

.session-active {
  background-color: #e6f7ff;
}

.session-info {
  flex: 1;
  min-width: 0;
}

.session-title {
  font-weight: 500;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.session-preview {
  font-size: 12px;
  color: #999;
  margin-top: 4px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.session-meta {
  font-size: 12px;
  color: #bbb;
  margin-top: 4px;
  display: flex;
  gap: 8px;
}

.delete-btn {
  opacity: 0;
  transition: opacity 0.2s;
  flex-shrink: 0;
  margin-left: 4px;
}

.load-more {
  text-align: center;
  padding: 8px;
}

.chat-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.empty-state {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: #bbb;
}

.empty-icon {
  font-size: 64px;
  margin-bottom: 16px;
}

.empty-text {
  font-size: 16px;
}

.chat-header {
  padding: 12px 16px;
  border-bottom: 1px solid #e8e8e8;
  font-size: 15px;
}

.message-list {
  flex: 1;
  overflow-y: auto;
  background: #fafafa;
}

.message-inner {
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  min-height: 100%;
}

.message-wrapper {
  display: flex;
  gap: 12px;
  max-width: 80%;
}

.message-user {
  align-self: flex-end;
  flex-direction: row-reverse;
}

.message-assistant {
  align-self: flex-start;
}

.message-avatar {
  flex-shrink: 0;
}

.message-body {
  display: flex;
  flex-direction: column;
}

.message-user .message-body {
  align-items: flex-end;
}

.message-assistant .message-body {
  align-items: flex-start;
}

.message-bubble {
  padding: 10px 14px;
  border-radius: 12px;
  word-break: break-word;
  line-height: 1.6;
  white-space: pre-wrap;
}

.bubble-user {
  background: #1890ff;
  color: #fff;
  border-bottom-right-radius: 4px;
}

.bubble-assistant {
  background: #fff;
  color: #333;
  border: 1px solid #e8e8e8;
  border-bottom-left-radius: 4px;
}

.message-time {
  font-size: 11px;
  color: #bbb;
  margin-top: 4px;
}

.time-right {
  text-align: right;
}

.time-left {
  text-align: left;
}

.typing-indicator {
  display: inline-block;
  animation: pulse 1.5s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.4; }
}

.chat-input-area {
  padding: 12px 16px;
  border-top: 1px solid #e8e8e8;
  background: #fff;
}

.input-wrapper {
  display: flex;
  gap: 8px;
  align-items: flex-end;
}

.input-wrapper :deep(.ant-input) {
  border-radius: 8px;
}

.send-btn {
  height: 40px;
  width: 40px;
  border-radius: 8px;
  flex-shrink: 0;
}
</style>
