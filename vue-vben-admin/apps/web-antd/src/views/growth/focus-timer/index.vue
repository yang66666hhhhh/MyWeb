<script lang="ts" setup>
import { computed, onMounted, onUnmounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Input,
  List,
  Progress,
  Row,
  Space,
  Statistic,
  Tag,
  message,
} from 'ant-design-vue';
import {
  PauseOutlined,
  PlayCircleOutlined,
  ReloadOutlined,
} from '@ant-design/icons-vue';

import {
  createFocusSessionApi,
  getFocusSessionsApi,
  updateFocusSessionApi,
} from '#/api/growth/extended';

const duration = ref(25);
const timeLeft = ref(duration.value * 60);
const isRunning = ref(false);
const taskName = ref('');
const completedSessions = ref(0);
const history = ref<
  Array<{ id?: string; task: string; duration: number; completedAt: string }>
>([]);
const currentSessionId = ref<string | null>(null);

let timer: ReturnType<typeof setInterval> | null = null;

const progress = computed(() => {
  const total = duration.value * 60;
  return Math.round(((total - timeLeft.value) / total) * 100);
});

const formattedTime = computed(() => {
  const minutes = Math.floor(timeLeft.value / 60);
  const seconds = timeLeft.value % 60;
  return `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
});

const presetDurations = [15, 25, 30, 45, 60];

async function startTimer() {
  if (timeLeft.value <= 0) {
    resetTimer();
  }
  isRunning.value = true;

  try {
    const res = await createFocusSessionApi({
      taskTitle: taskName.value || '专注',
      plannedMinutes: duration.value,
    });
    currentSessionId.value = res.id;
  } catch {
    // Continue even if API fails
  }

  timer = setInterval(() => {
    if (timeLeft.value > 0) {
      timeLeft.value--;
    } else {
      completeSession();
    }
  }, 1000);
}

function pauseTimer() {
  isRunning.value = false;
  if (timer) {
    clearInterval(timer);
    timer = null;
  }
}

function resetTimer() {
  pauseTimer();
  timeLeft.value = duration.value * 60;
  currentSessionId.value = null;
}

async function completeSession() {
  pauseTimer();
  completedSessions.value++;

  const record = {
    id: currentSessionId.value || undefined,
    task: taskName.value || '专注',
    duration: duration.value,
    completedAt: new Date().toLocaleTimeString(),
  };
  history.value.unshift(record);

  if (currentSessionId.value) {
    try {
      await updateFocusSessionApi(currentSessionId.value, {
        durationMinutes: duration.value,
        status: 1,
        endTime: new Date().toISOString(),
      });
    } catch {
      // Continue even if API fails
    }
  }

  message.success(`完成！已专注 ${duration.value} 分钟`);
  timeLeft.value = duration.value * 60;
  currentSessionId.value = null;
}

function setDuration(minutes: number) {
  duration.value = minutes;
  resetTimer();
}

async function loadHistory() {
  try {
    const res = await getFocusSessionsApi({ page: 1, pageSize: 10 });
    completedSessions.value = res.total || 0;
  } catch {
    // Continue with local state
  }
}

onMounted(() => {
  loadHistory();
});

onUnmounted(() => {
  if (timer) {
    clearInterval(timer);
  }
});
</script>

<template>
  <Page title="专注计时" description="使用番茄工作法提升专注力">
    <div class="mx-auto max-w-2xl space-y-6">
      <Row :gutter="[16, 16]">
        <Col :span="8">
          <Card>
            <Statistic title="今日完成" :value="completedSessions" suffix="次" />
          </Card>
        </Col>
        <Col :span="8">
          <Card>
            <Statistic
              title="总专注时长"
              :value="completedSessions * duration"
              suffix="分钟"
            />
          </Card>
        </Col>
        <Col :span="8">
          <Card>
            <Statistic title="当前模式" :value="isRunning ? '专注中' : '待开始'" />
          </Card>
        </Col>
      </Row>

      <Card>
        <div class="text-center">
          <div class="mb-4">
            <Input
              v-model:value="taskName"
              placeholder="输入正在专注的任务..."
              style="max-width: 300px"
            />
          </div>

          <div class="mb-6">
            <span
              class="font-mono text-6xl font-bold"
              :class="{ 'text-blue-500': isRunning }"
            >
              {{ formattedTime }}
            </span>
          </div>

          <Progress
            :percent="progress"
            :status="isRunning ? 'active' : 'normal'"
            class="mb-6"
          />

          <Space size="large">
            <Button
              v-if="!isRunning"
              type="primary"
              size="large"
              @click="startTimer"
            >
              <PlayCircleOutlined /> 开始
            </Button>
            <Button v-else size="large" @click="pauseTimer">
              <PauseOutlined /> 暂停
            </Button>
            <Button size="large" @click="resetTimer">
              <ReloadOutlined /> 重置
            </Button>
          </Space>

          <div class="mt-6">
            <Space>
              <span>预设时长:</span>
              <Button
                v-for="min in presetDurations"
                :key="min"
                :type="duration === min ? 'primary' : 'default'"
                size="small"
                @click="setDuration(min)"
              >
                {{ min }}分钟
              </Button>
            </Space>
          </div>
        </div>
      </Card>

      <Card v-if="history.length > 0" title="历史记录">
        <List :data-source="history" :locale="{ emptyText: '暂无记录' }">
          <template #renderItem="{ item }">
            <List.Item>
              <List.Item.Meta
                :title="item.task"
                :description="`${item.duration} 分钟 · ${item.completedAt}`"
              />
              <Tag color="success">完成</Tag>
            </List.Item>
          </template>
        </List>
      </Card>
    </div>
  </Page>
</template>
