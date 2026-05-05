<script lang="ts" setup>
import { computed, onUnmounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import { Button, Card, Col, Progress, Row, Space, Statistic } from 'ant-design-vue';

const mode = ref<'break' | 'focus'>('focus');
const timeLeft = ref(25 * 60);
const isRunning = ref(false);
const completedSessions = ref(3);
let timer: ReturnType<typeof setInterval> | null = null;

const focusDuration = 25 * 60;
const breakDuration = 5 * 60;

const formattedTime = computed(() => {
  const minutes = Math.floor(timeLeft.value / 60);
  const seconds = timeLeft.value % 60;
  return `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
});

const progressPercent = computed(() => {
  const total = mode.value === 'focus' ? focusDuration : breakDuration;
  return Math.round(((total - timeLeft.value) / total) * 100);
});

function startTimer() {
  if (isRunning.value) return;
  isRunning.value = true;
  timer = setInterval(() => {
    if (timeLeft.value > 0) {
      timeLeft.value--;
    } else {
      stopTimer();
      if (mode.value === 'focus') {
        completedSessions.value++;
        mode.value = 'break';
        timeLeft.value = breakDuration;
      } else {
        mode.value = 'focus';
        timeLeft.value = focusDuration;
      }
    }
  }, 1000);
}

function stopTimer() {
  isRunning.value = false;
  if (timer) {
    clearInterval(timer);
    timer = null;
  }
}

function resetTimer() {
  stopTimer();
  mode.value = 'focus';
  timeLeft.value = focusDuration;
}

onUnmounted(() => {
  stopTimer();
});
</script>

<template>
  <Page description="使用番茄工作法提升专注力" title="专注计时">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="8" :md="12" :xs="24">
        <Card><Statistic title="今日完成" :value="completedSessions" suffix="个番茄" /></Card>
      </Col>
      <Col :lg="8" :md="12" :xs="24">
        <Card><Statistic title="专注时长" :value="completedSessions * 25" suffix="分钟" /></Card>
      </Col>
      <Col :lg="8" :md="12" :xs="24">
        <Card><Statistic title="当前模式" :value="mode === 'focus' ? '专注' : '休息'" /></Card>
      </Col>
    </Row>

    <Card class="text-center">
      <div class="mb-8">
        <Progress type="circle" :percent="progressPercent" :size="200">
          <template #format>
            <span class="text-4xl font-bold">{{ formattedTime }}</span>
          </template>
        </Progress>
      </div>
      <div class="mb-4 text-xl font-bold">
        {{ mode === 'focus' ? '专注时间' : '休息时间' }}
      </div>
      <Space>
        <Button v-if="!isRunning" type="primary" size="large" @click="startTimer">开始</Button>
        <Button v-else size="large" @click="stopTimer">暂停</Button>
        <Button size="large" @click="resetTimer">重置</Button>
      </Space>
    </Card>
  </Page>
</template>
