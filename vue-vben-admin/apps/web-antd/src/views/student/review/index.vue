<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  DatePicker,
  Empty,
  Row,
  Select,
  Space,
  Statistic,
  Timeline,
  Tag,
  message,
} from 'ant-design-vue';
import dayjs from 'dayjs';

import {
  getMistakePageApi,
  type ExamMistake,
  updateMistakeApi,
  updateMistakeReviewStatusApi,
} from '#/api/student';

import type { Dayjs } from 'dayjs';

const loading = ref(false);
const reviewDate = ref(dayjs().format('YYYY-MM-DD'));
const subject = ref<string | undefined>();
const mistakes = ref<ExamMistake[]>([]);

const defaultSubjects = ['数据结构', '操作系统', '计算机网络', '计算机组成原理', '数学', '英语', '政治'];

const subjectOptions = computed(() => {
  const values = new Set(defaultSubjects);
  for (const item of mistakes.value) {
    if (item.subject?.trim()) {
      values.add(item.subject.trim());
    }
  }
  return [...values].map((item) => ({ label: item, value: item }));
});

const dueMistakes = computed(() => {
  const target = dayjs(reviewDate.value);
  return mistakes.value
    .filter((item) => item.status !== 2)
    .filter((item) => !subject.value || item.subject === subject.value)
    .filter((item) => {
      if (!item.nextReviewDate) {
        return item.status === 0;
      }
      return dayjs(item.nextReviewDate).isSame(target, 'day') || dayjs(item.nextReviewDate).isBefore(target, 'day');
    })
    .sort((a, b) => {
      const aDate = a.nextReviewDate || '9999-12-31';
      const bDate = b.nextReviewDate || '9999-12-31';
      return aDate.localeCompare(bDate);
    });
});

const overdueCount = computed(() =>
  mistakes.value.filter((item) =>
    item.status !== 2 &&
    item.nextReviewDate &&
    dayjs(item.nextReviewDate).isBefore(dayjs(), 'day'),
  ).length,
);
const todayCount = computed(() =>
  mistakes.value.filter((item) =>
    item.status !== 2 &&
    item.nextReviewDate &&
    dayjs(item.nextReviewDate).isSame(dayjs(), 'day'),
  ).length,
);
const unscheduledCount = computed(() =>
  mistakes.value.filter((item) => item.status === 0 && !item.nextReviewDate).length,
);
const masteredCount = computed(() => mistakes.value.filter((item) => item.status === 2).length);

async function fetchMistakes() {
  loading.value = true;
  try {
    const allMistakes: ExamMistake[] = [];
    let page = 1;
    const pageSize = 100;

    while (true) {
      const result = await getMistakePageApi({
        page,
        pageSize,
        subject: subject.value,
      });
      allMistakes.push(...result.items);
      if (allMistakes.length >= result.total || result.items.length < pageSize) {
        break;
      }
      page += 1;
    }

    mistakes.value = allMistakes;
  } catch (e: any) {
    message.error(e?.message || '加载复习清单失败');
  } finally {
    loading.value = false;
  }
}

function handleDateChange(_: Dayjs | string, dateString: string) {
  reviewDate.value = dateString || dayjs().format('YYYY-MM-DD');
}

function scheduleNextDate(mistake: ExamMistake) {
  const intervals = [1, 2, 4, 7, 15, 30];
  const interval = intervals[Math.min(mistake.reviewCount, intervals.length - 1)] || 30;
  return dayjs().add(interval, 'day').format('YYYY-MM-DD');
}

async function markReviewed(mistake: ExamMistake) {
  try {
    await updateMistakeApi(mistake.id, {
      answer: mistake.answer,
      explanation: mistake.explanation,
      lastReviewDate: dayjs().format('YYYY-MM-DD'),
      nextReviewDate: scheduleNextDate(mistake),
      question: mistake.question,
      reviewCount: mistake.reviewCount + 1,
      status: 1,
      subject: mistake.subject,
      tags: mistake.tags,
    });
    message.success('已安排下一次复习');
    await fetchMistakes();
  } catch (e: any) {
    message.error(e?.message || '更新复习进度失败');
  }
}

async function markMastered(mistake: ExamMistake) {
  try {
    await updateMistakeReviewStatusApi(mistake.id, 'mastered');
    message.success('已标记为掌握');
    await fetchMistakes();
  } catch (e: any) {
    message.error(e?.message || '标记掌握失败');
  }
}

onMounted(() => {
  void fetchMistakes();
});
</script>

<template>
  <Page description="集中处理到期错题，按间隔自动安排下一轮复习" title="复习日程">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已逾期" :value="overdueCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="今日复习" :value="todayCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="未排期错题" :value="unscheduledCount" /></Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card><Statistic title="已掌握" :value="masteredCount" /></Card>
      </Col>
    </Row>

    <Card title="复习清单">
      <template #extra>
        <Space>
          <DatePicker
            :value="dayjs(reviewDate)"
            format="YYYY-MM-DD"
            style="width: 140px"
            @change="handleDateChange"
          />
          <Select
            v-model:value="subject"
            :options="subjectOptions"
            allow-clear
            placeholder="科目"
            style="width: 140px"
            @change="fetchMistakes"
          />
          <Button :loading="loading" type="primary" @click="fetchMistakes">刷新</Button>
        </Space>
      </template>

      <div v-if="dueMistakes.length === 0 && !loading" class="py-8">
        <Empty description="当前没有到期复习项" />
      </div>

      <Timeline v-else>
        <Timeline.Item v-for="mistake in dueMistakes" :key="mistake.id">
          <div class="rounded border border-gray-200 p-3">
            <div class="mb-2 flex flex-wrap items-center gap-2">
              <Tag color="blue">{{ mistake.subject }}</Tag>
              <Tag v-if="mistake.nextReviewDate" color="orange">
                {{ mistake.nextReviewDate }}
              </Tag>
              <Tag v-else>未排期</Tag>
              <span class="text-text-secondary text-xs">已复习 {{ mistake.reviewCount }} 次</span>
            </div>
            <div class="mb-2 font-medium">{{ mistake.question }}</div>
            <div v-if="mistake.explanation" class="text-text-secondary mb-3 line-clamp-2 text-sm">
              {{ mistake.explanation }}
            </div>
            <Space>
              <Button size="small" type="primary" @click="markReviewed(mistake)">完成本轮</Button>
              <Button size="small" @click="markMastered(mistake)">标记掌握</Button>
            </Space>
          </div>
        </Timeline.Item>
      </Timeline>
    </Card>
  </Page>
</template>
