import type { BaseEntity, PageQuery, PageResult } from './types';

import { createId, createPageResult, mockDelay, today } from './mock-utils';

export interface ExamSubject extends BaseEntity {
  color: string;
  name: string;
  progress: number;
  targetHours: number;
  weeklyHours: number;
}

export interface PostgraduateTask extends BaseEntity {
  course: string;
  dueDate?: null | string;
  progress: number;
  status?: 'completed' | 'pending' | 'reviewing';
  title: string;
}

export interface StudyRecord extends BaseEntity {
  durationMinutes: number;
  recordDate: string;
  subject: string;
  summary: string;
}

export interface ExamMistake extends BaseEntity {
  course: string;
  errorReason: string;
  correctAnswer: string;
  question: string;
  recordDate: string;
  reviewCount: number;
  reviewStatus: 'pending' | 'reviewed' | 'mastered';
  tags?: string[];
}

export interface ExamMaterial extends BaseEntity {
  category: string;
  fileName: string;
  fileUrl?: string;
  subject: string;
  summary: string;
  tags?: string[];
  type: string;
}

export interface ExamDashboard {
  materialCount?: number;
  mistakeCount: number;
  recentRecords: StudyRecord[];
  reviewTaskCount?: number;
  subjects: ExamSubject[];
  todayTasks: PostgraduateTask[];
  weeklyHours: number;
}

export interface PostgraduateQuery extends PageQuery {
  course?: string;
  keyword?: string;
}

export type SavePostgraduateTaskInput = Omit<
  PostgraduateTask,
  'createdAt' | 'id' | 'updatedAt'
>;

const subjects: ExamSubject[] = [
  {
    color: 'blue',
    createdAt: `${today()}T08:00:00`,
    id: 'subject-408-ds',
    name: '数据结构',
    progress: 24,
    targetHours: 120,
    weeklyHours: 5,
  },
  {
    color: 'cyan',
    createdAt: `${today()}T08:00:00`,
    id: 'subject-math',
    name: '数学',
    progress: 12,
    targetHours: 180,
    weeklyHours: 4,
  },
  {
    color: 'green',
    createdAt: `${today()}T08:00:00`,
    id: 'subject-english',
    name: '英语',
    progress: 18,
    targetHours: 90,
    weeklyHours: 3,
  },
  {
    color: 'purple',
    createdAt: `${today()}T08:00:00`,
    id: 'subject-politics',
    name: '政治',
    progress: 5,
    targetHours: 80,
    weeklyHours: 1.5,
  },
];

const tasks: PostgraduateTask[] = [
  {
    course: '数据结构',
    createdAt: `${today()}T08:00:00`,
    dueDate: today(),
    id: 'exam-task-1',
    progress: 40,
    status: 'pending',
    title: '线性表与链表专题复习',
  },
  {
    course: '英语',
    createdAt: `${today()}T08:00:00`,
    dueDate: today(),
    id: 'exam-task-2',
    progress: 60,
    status: 'reviewing',
    title: '阅读理解精读 2 篇',
  },
];

const records: StudyRecord[] = [
  {
    createdAt: `${today()}T21:30:00`,
    durationMinutes: 90,
    id: 'study-record-1',
    recordDate: today(),
    subject: '数据结构',
    summary: '完成线性表基础题，整理 3 个边界条件错题。',
  },
];

export async function getExamDashboardApi() {
  return mockDelay<ExamDashboard>({
    materialCount: 26,
    mistakeCount: 18,
    recentRecords: records,
    reviewTaskCount: 9,
    subjects,
    todayTasks: tasks,
    weeklyHours: subjects.reduce((sum, item) => sum + item.weeklyHours, 0),
  });
}

export async function getPostgraduateTaskPageApi(params: PostgraduateQuery) {
  const filtered = tasks.filter((item) => {
    const inCourse = !params.course || item.course === params.course;
    const inKeyword = !params.keyword || item.title.includes(params.keyword);
    return inCourse && inKeyword;
  });
  return mockDelay<PageResult<PostgraduateTask>>(
    createPageResult(filtered, params),
  );
}

export async function createPostgraduateTaskApi(
  data: SavePostgraduateTaskInput,
) {
  const item: PostgraduateTask = {
    ...data,
    createdAt: new Date().toISOString(),
    id: `exam-task-${Date.now()}`,
  };
  tasks.unshift(item);
  return mockDelay(item);
}

const mistakes: ExamMistake[] = [
  {
    course: '数据结构',
    correctAnswer: 'O(1)',
    createdAt: `${today()}T10:00:00`,
    errorReason: '未考虑最坏情况下的查找',
    id: 'mistake-1',
    question: '顺序表按值查找的平均时间复杂度是多少？',
    recordDate: today(),
    reviewCount: 2,
    reviewStatus: 'reviewed',
    tags: ['查找', '复杂度'],
  },
  {
    course: '数据结构',
    correctAnswer: '栈',
    createdAt: `${today()}T14:00:00`,
    errorReason: '队列也可以用链表实现',
    id: 'mistake-2',
    question: '用数组实现队列时，哪个结构最合适？',
    recordDate: today(),
    reviewCount: 1,
    reviewStatus: 'pending',
    tags: ['队列', '数组'],
  },
  {
    course: '英语',
    correctAnswer: 'despite',
    createdAt: `${today()}T21:00:00`,
    errorReason: '混淆了 despite 和 although 的用法',
    id: 'mistake-3',
    question: 'Although he was tired, he continued walking. 用其他词替换 Although',
    recordDate: today(),
    reviewCount: 0,
    reviewStatus: 'pending',
    tags: ['词汇', '语法'],
  },
];

export interface MistakeQuery extends PageQuery {
  course?: string;
  keyword?: string;
  reviewStatus?: 'pending' | 'reviewed' | 'mastered';
}

export async function getMistakePageApi(params: MistakeQuery) {
  const filtered = mistakes.filter((item) => {
    const inCourse = !params.course || item.course === params.course;
    const inKeyword =
      !params.keyword ||
      item.question.includes(params.keyword) ||
      item.errorReason.includes(params.keyword);
    const inStatus = !params.reviewStatus || item.reviewStatus === params.reviewStatus;
    return inCourse && inKeyword && inStatus;
  });
  return mockDelay<PageResult<ExamMistake>>(createPageResult(filtered, params));
}

export async function createMistakeApi(data: Omit<ExamMistake, 'id' | 'createdAt'>) {
  const item: ExamMistake = {
    ...data,
    createdAt: new Date().toISOString(),
    id: createId('mistake'),
    reviewCount: 0,
  };
  mistakes.unshift(item);
  return mockDelay(item);
}

export async function updateMistakeReviewStatusApi(id: string, status: ExamMistake['reviewStatus']) {
  const mistake = mistakes.find((item) => item.id === id);
  if (mistake) {
    mistake.reviewStatus = status;
    mistake.reviewCount += 1;
    mistake.updatedAt = new Date().toISOString();
  }
  return mockDelay(mistake);
}

export async function deleteMistakeApi(id: string) {
  const index = mistakes.findIndex((item) => item.id === id);
  if (index >= 0) mistakes.splice(index, 1);
  return mockDelay(true);
}

const materials: ExamMaterial[] = [
  {
    category: '讲义',
    createdAt: `${today()}T08:00:00`,
    fileName: '数据结构重点知识整理.pdf',
    id: 'material-1',
    subject: '数据结构',
    summary: '包含栈、队列、树、图的核心知识点归纳',
    tags: ['408', '重点'],
    type: 'PDF',
  },
  {
    category: '真题',
    createdAt: `${today()}T09:00:00`,
    fileName: '2024年考研英语真题.pdf',
    id: 'material-2',
    subject: '英语',
    summary: '2024年英语一真题及答案解析',
    tags: ['真题', '2024'],
    type: 'PDF',
  },
  {
    category: '笔记',
    createdAt: `${today()}T10:00:00`,
    fileName: '高等数学公式手册.md',
    id: 'material-3',
    subject: '数学',
    summary: '常用积分、微分、级数公式汇总',
    tags: ['公式', '高数'],
    type: 'Markdown',
  },
];

export interface MaterialQuery extends PageQuery {
  category?: string;
  keyword?: string;
  subject?: string;
}

export async function getMaterialPageApi(params: MaterialQuery) {
  const filtered = materials.filter((item) => {
    const inSubject = !params.subject || item.subject === params.subject;
    const inCategory = !params.category || item.category === params.category;
    const inKeyword =
      !params.keyword ||
      item.fileName.includes(params.keyword) ||
      item.summary.includes(params.keyword);
    return inSubject && inCategory && inKeyword;
  });
  return mockDelay<PageResult<ExamMaterial>>(createPageResult(filtered, params));
}

export async function createMaterialApi(data: Omit<ExamMaterial, 'id' | 'createdAt'>) {
  const item: ExamMaterial = {
    ...data,
    createdAt: new Date().toISOString(),
    id: createId('material'),
  };
  materials.unshift(item);
  return mockDelay(item);
}

export async function deleteMaterialApi(id: string) {
  const index = materials.findIndex((item) => item.id === id);
  if (index >= 0) materials.splice(index, 1);
  return mockDelay(true);
}
