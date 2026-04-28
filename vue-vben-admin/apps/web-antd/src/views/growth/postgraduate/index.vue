<script lang="ts" setup>
import { onMounted, ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  List,
  Progress,
  Row,
  Space,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

import type { ExamDashboard } from '#/api/growth';

import { getExamDashboardApi } from '#/api/growth';

const loading = ref(false);
const dashboard = ref<ExamDashboard>({
  materialCount: 0,
  mistakeCount: 0,
  recentRecords: [],
  reviewTaskCount: 0,
  subjects: [],
  todayTasks: [],
  weeklyHours: 0,
});

const taskColumns = [
  { dataIndex: 'title', key: 'title', title: '今日学习任务' },
  { dataIndex: 'course', key: 'course', title: '科目', width: 120 },
  { dataIndex: 'progress', key: 'progress', title: '进度', width: 160 },
  { dataIndex: 'dueDate', key: 'dueDate', title: '截止日期', width: 120 },
];

const shortcuts = [
  { color: 'blue', description: '安排各科节奏', title: '学习计划' },
  { color: 'green', description: '记录每日投入', title: '学习记录' },
  { color: 'red', description: '整理易错题', title: '错题本' },
  { color: 'purple', description: '归档讲义资料', title: '资料库' },
];

async function load() {
  loading.value = true;
  try {
    dashboard.value = await getExamDashboardApi();
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  void load();
});
</script>

<template>
  <Page
    description="面向 408、数学、英语、政治、专业课和在职备考的综合首页，当前使用 mock/预留接口。"
    title="备考中心"
  >
    <div class="space-y-4">
      <Row :gutter="[16, 16]">
        <Col :lg="6" :md="12" :xs="24">
          <Card>
            <Statistic :loading="loading" title="今日学习任务" :value="dashboard.todayTasks.length" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card>
            <Statistic :loading="loading" suffix="小时" title="本周学习时长" :value="dashboard.weeklyHours" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card>
            <Statistic :loading="loading" title="错题数量" :value="dashboard.mistakeCount" />
          </Card>
        </Col>
        <Col :lg="6" :md="12" :xs="24">
          <Card>
            <Statistic :loading="loading" title="资料数量" :value="dashboard.materialCount" />
          </Card>
        </Col>
      </Row>

      <Row :gutter="[16, 16]">
        <Col :lg="14" :xs="24">
          <Card title="各科学习进度">
            <Space class="w-full" direction="vertical" size="middle">
              <div v-for="subject in dashboard.subjects" :key="subject.id">
                <div class="mb-1 flex items-center justify-between">
                  <Space>
                    <Tag :color="subject.color">{{ subject.name }}</Tag>
                    <span>本周 {{ subject.weeklyHours }}h / 目标 {{ subject.targetHours }}h</span>
                  </Space>
                  <span>{{ subject.progress }}%</span>
                </div>
                <Progress :percent="subject.progress" />
              </div>
            </Space>
          </Card>
        </Col>
        <Col :lg="10" :xs="24">
          <Card title="快捷入口">
            <Row :gutter="[12, 12]">
              <Col v-for="item in shortcuts" :key="item.title" :span="12">
                <Button block class="h-20 !text-left">
                  <div class="w-full">
                    <Tag :color="item.color">{{ item.title }}</Tag>
                    <div class="mt-2 text-xs text-slate-500">{{ item.description }}</div>
                  </div>
                </Button>
              </Col>
            </Row>
          </Card>
        </Col>
      </Row>

      <Row :gutter="[16, 16]">
        <Col :lg="14" :xs="24">
          <Card title="今日学习任务">
            <Table
              :columns="taskColumns"
              :data-source="dashboard.todayTasks"
              :loading="loading"
              :pagination="false"
              row-key="id"
            >
              <template #bodyCell="{ column, text }">
                <template v-if="column.key === 'course'">
                  <Tag color="blue">{{ text }}</Tag>
                </template>
                <template v-else-if="column.key === 'progress'">
                  <Progress :percent="text" size="small" />
                </template>
              </template>
            </Table>
          </Card>
        </Col>
        <Col :lg="10" :xs="24">
          <div class="space-y-4">
            <Card title="最近学习记录">
              <List :data-source="dashboard.recentRecords" :loading="loading">
                <template #renderItem="{ item }">
                  <List.Item>
                    <List.Item.Meta
                      :description="item.summary"
                      :title="`${item.subject} · ${item.durationMinutes} 分钟`"
                    />
                    <Tag>{{ item.recordDate }}</Tag>
                  </List.Item>
                </template>
              </List>
            </Card>
            <Card title="复习提醒">
              <Space direction="vertical">
                <div>待复习任务：{{ dashboard.reviewTaskCount }} 个</div>
                <div>错题本和资料库入口已预留，后续可继续补列表与表单页。</div>
              </Space>
            </Card>
          </div>
        </Col>
      </Row>
    </div>
  </Page>
</template>
