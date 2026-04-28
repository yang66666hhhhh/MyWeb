<script lang="ts" setup>
import { computed, onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Card,
  Col,
  List,
  Progress,
  Row,
  Space,
  Statistic,
  Tag,
  Timeline,
} from 'ant-design-vue';

import type { DailyPlan } from '#/api/growth';

import { getDailyPlanPageApi } from '#/api/growth';

const loading = ref(false);
const todayPlans = ref<DailyPlan[]>([]);

const today = new Date().toISOString().slice(0, 10);

const completedCount = computed(
  () => todayPlans.value.filter((item) => item.status === 2).length,
);

const completionRate = computed(() => {
  if (todayPlans.value.length === 0) {
    return 0;
  }
  return Math.round((completedCount.value / todayPlans.value.length) * 100);
});

const focusBlocks = [
  {
    color: 'blue',
    label: '工作',
    text: '沉淀工作日志、问题复盘和项目产出',
  },
  {
    color: 'purple',
    label: '408',
    text: '数据结构、计组、操作系统、计网按周滚动推进',
  },
  {
    color: 'cyan',
    label: '暨南大学 AI',
    text: '复试方向、项目作品集、论文与英语表达同步准备',
  },
  {
    color: 'green',
    label: '生活',
    text: '睡眠、运动、习惯打卡，保证长期续航',
  },
];

const weeklyMilestones = [
  '工作日保持 2 个高质量学习块：午间复盘 20 分钟，晚间 90 分钟主攻 408',
  '周末完成 1 次 408 真题/专题训练，并沉淀错题到知识库',
  '每周至少输出 1 篇项目/技术复盘，服务工作成长和复试作品集',
  '每晚记录睡眠、运动和情绪能量，防止备考节奏失真',
];

async function loadDashboard() {
  loading.value = true;
  try {
    const result = await getDailyPlanPageApi({
      endDate: today,
      page: 1,
      pageSize: 5,
      startDate: today,
    });
    todayPlans.value = result.items;
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  void loadDashboard();
});
</script>

<template>
  <Page
    description="围绕在职工作、408 全日制备考、暨南大学非全日制人工智能目标和生活续航的个人驾驶舱"
    title="成长看板"
  >
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="8" :md="12" :xs="24">
        <Card>
          <Statistic :loading="loading" title="今日计划" :value="todayPlans.length" />
        </Card>
      </Col>
      <Col :lg="8" :md="12" :xs="24">
        <Card>
          <Statistic :loading="loading" title="已完成" :value="completedCount" />
          <Progress :percent="completionRate" size="small" />
        </Card>
      </Col>
      <Col :lg="8" :md="12" :xs="24">
        <Card>
          <Statistic title="本周主线" value="408 + 工作复盘" />
        </Card>
      </Col>
    </Row>

    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="14" :xs="24">
        <Card title="今日计划">
          <List :data-source="todayPlans" :loading="loading">
            <template #renderItem="{ item }">
              <List.Item>
                <List.Item.Meta
                  :description="item.description"
                  :title="item.title"
                />
                <Tag :color="item.status === 2 ? 'success' : 'processing'">
                  {{ item.status === 2 ? '已完成' : '进行中' }}
                </Tag>
              </List.Item>
            </template>
          </List>
        </Card>
      </Col>
      <Col :lg="10" :xs="24">
        <Card title="本周节奏">
          <Timeline>
            <Timeline.Item v-for="item in weeklyMilestones" :key="item">
              {{ item }}
            </Timeline.Item>
          </Timeline>
        </Card>
      </Col>
    </Row>

    <Card title="四条成长主线">
      <Row :gutter="[12, 12]">
        <Col
          v-for="block in focusBlocks"
          :key="block.label"
          :lg="6"
          :md="12"
          :xs="24"
        >
          <Card size="small">
            <Space direction="vertical">
              <Tag :color="block.color">{{ block.label }}</Tag>
              <span>{{ block.text }}</span>
            </Space>
          </Card>
        </Col>
      </Row>
    </Card>
  </Page>
</template>
