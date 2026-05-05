<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Input, List, Space } from 'ant-design-vue';

const loading = ref(false);
const inputMessage = ref('');

const messages = ref([
  { id: '1', role: 'assistant', content: '你好！我是AI助手，有什么可以帮助你的吗？' },
  { id: '2', role: 'user', content: '帮我分析一下最近的工作效率' },
  { id: '3', role: 'assistant', content: '根据你的工作日志分析，最近一周你平均每天完成3.5个任务，工时约7.5小时。建议：\n1. 上午专注处理高优先级任务\n2. 下午安排会议和协作工作\n3. 预留1小时处理突发事务' },
]);

function sendMessage() {
  if (!inputMessage.value.trim()) return;
  
  messages.value.push({
    id: Date.now().toString(),
    role: 'user',
    content: inputMessage.value,
  });

  const userMsg = inputMessage.value;
  inputMessage.value = '';

  setTimeout(() => {
    messages.value.push({
      id: (Date.now() + 1).toString(),
      role: 'assistant',
      content: `收到你的问题："${userMsg}"。我正在思考中...`,
    });
  }, 500);
}
</script>

<template>
  <Page description="与AI助手对话，获取智能建议" title="AI助手">
    <Card class="h-[600px] flex flex-col">
      <div class="flex-1 overflow-auto mb-4">
        <List :data-source="messages" :loading="loading">
          <template #renderItem="{ item }">
            <List.Item :class="item.role === 'user' ? 'text-right' : 'text-left'">
              <div :class="[
                'inline-block p-3 rounded-lg max-w-[70%]',
                item.role === 'user' ? 'bg-blue-500 text-white' : 'bg-gray-100'
              ]">
                {{ item.content }}
              </div>
            </List.Item>
          </template>
        </List>
      </div>
      <div class="flex gap-2">
        <Input.TextArea v-model:value="inputMessage" placeholder="输入消息..." :rows="2" @press-enter="sendMessage" />
        <Button type="primary" @click="sendMessage">发送</Button>
      </div>
    </Card>
  </Page>
</template>
