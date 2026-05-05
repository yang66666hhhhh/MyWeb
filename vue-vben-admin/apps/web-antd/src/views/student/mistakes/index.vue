<script lang="ts" setup>
import { ref } from 'vue';

import { Page } from '@vben/common-ui';

import {
  Button,
  Card,
  Col,
  Row,
  Space,
  Statistic,
  Table,
  Tag,
} from 'ant-design-vue';

const loading = ref(false);

const mistakes = ref([
  {
    id: '1',
    subject: '数据结构',
    question: '二叉树的中序遍历结果是什么？',
    wrongAnswer: 'A. 根-左-右',
    correctAnswer: 'B. 左-根-右',
    difficulty: '中',
    reviewCount: 2,
    lastReviewed: '2024-01-15',
  },
  {
    id: '2',
    subject: '操作系统',
    question: '什么是死锁的必要条件？',
    wrongAnswer: 'A. 互斥、占有、等待、循环',
    correctAnswer: 'B. 互斥、占有并等待、非抢占、循环等待',
    difficulty: '难',
    reviewCount: 1,
    lastReviewed: '2024-01-12',
  },
  {
    id: '3',
    subject: '计算机网络',
    question: 'TCP 三次握手的顺序是？',
    wrongAnswer: 'A. SYN, ACK, FIN',
    correctAnswer: 'B. SYN, SYN-ACK, ACK',
    difficulty: '易',
    reviewCount: 3,
    lastReviewed: '2024-01-10',
  },
]);

const columns = [
  { title: '科目', dataIndex: 'subject', key: 'subject' },
  { title: '题目', dataIndex: 'question', key: 'question' },
  { title: '错误答案', dataIndex: 'wrongAnswer', key: 'wrongAnswer' },
  { title: '正确答案', dataIndex: 'correctAnswer', key: 'correctAnswer' },
  { title: '难度', dataIndex: 'difficulty', key: 'difficulty' },
  { title: '复习次数', dataIndex: 'reviewCount', key: 'reviewCount' },
  { title: '最后复习', dataIndex: 'lastReviewed', key: 'lastReviewed' },
];

const subjectColors: Record<string, string> = {
  '数据结构': 'blue',
  '操作系统': 'purple',
  '计算机网络': 'green',
  '计算机组成原理': 'orange',
};

const difficultyColors: Record<string, string> = {
  '易': 'green',
  '中': 'orange',
  '难': 'red',
};
</script>

<template>
  <Page description="记录错题、分析薄弱环节、针对性复习" title="错题本">
    <Row :gutter="[16, 16]" class="mb-4">
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="错题总数" :value="mistakes.length" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="数据结构" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="操作系统" :value="1" />
        </Card>
      </Col>
      <Col :lg="6" :md="12" :xs="24">
        <Card>
          <Statistic title="计算机网络" :value="1" />
        </Card>
      </Col>
    </Row>

    <Card title="错题列表">
      <template #extra>
        <Button type="primary">添加错题</Button>
      </template>
      <Table
        :columns="columns"
        :data-source="mistakes"
        :loading="loading"
        row-key="id"
      >
        <template #bodyCell="{ column, record }">
          <template v-if="column.key === 'subject'">
            <Tag :color="subjectColors[record.subject] || 'default'">
              {{ record.subject }}
            </Tag>
          </template>
          <template v-else-if="column.key === 'difficulty'">
            <Tag :color="difficultyColors[record.difficulty] || 'default'">
              {{ record.difficulty }}
            </Tag>
          </template>
        </template>
      </Table>
    </Card>
  </Page>
</template>
