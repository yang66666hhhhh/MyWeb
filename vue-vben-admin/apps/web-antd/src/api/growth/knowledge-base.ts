import type { BaseEntity, PageQuery, PageResult } from './types';

import {
  createId,
  createPageResult,
  includesKeyword,
  mockDelay,
  today,
} from './mock-utils';

export interface KnowledgeArticle extends BaseEntity {
  category: string;
  favorite: boolean;
  markdownContent: string;
  sourceUrl?: string;
  summary: string;
  tags?: string[];
  title: string;
}

export interface KnowledgeBaseQuery extends PageQuery {
  category?: string;
  keyword?: string;
  tag?: string;
}

export type SaveKnowledgeArticleInput = Omit<
  KnowledgeArticle,
  'createdAt' | 'id' | 'updatedAt'
>;

const articles: KnowledgeArticle[] = [
  {
    category: '学习/数据结构',
    createdAt: `${today()}T10:00:00`,
    favorite: true,
    id: 'knowledge-1',
    markdownContent: '## 栈和队列\n\n整理基本操作、复杂度和典型题型。',
    sourceUrl: '',
    summary: '栈、队列、循环队列和表达式求值的核心知识点。',
    tags: ['学习', '数据结构'],
    title: '栈与队列专题笔记',
  },
  {
    category: '工作/工程实践',
    createdAt: `${today()}T15:30:00`,
    favorite: false,
    id: 'knowledge-2',
    markdownContent: '## 接口联调清单\n\n- 字段对齐\n- 异常处理\n- 权限校验',
    sourceUrl: 'https://example.com/api-checklist',
    summary: '接口联调时常用的一份自检清单。',
    tags: ['工作', '接口'],
    title: '接口联调清单',
  },
];

function filterArticles(query: KnowledgeBaseQuery) {
  return articles.filter((item) => {
    const inKeyword =
      includesKeyword(item.title, query.keyword) ||
      includesKeyword(item.summary, query.keyword) ||
      includesKeyword(item.markdownContent, query.keyword);
    const inCategory = !query.category || item.category === query.category;
    const inTag = !query.tag || item.tags?.includes(query.tag);
    return inKeyword && inCategory && inTag;
  });
}

export async function getKnowledgeArticlePageApi(params: KnowledgeBaseQuery) {
  return mockDelay<PageResult<KnowledgeArticle>>(
    createPageResult(filterArticles(params), params),
  );
}

export async function getKnowledgeArticleApi(id: string) {
  return mockDelay(articles.find((item) => item.id === id));
}

export async function createKnowledgeArticleApi(
  data: SaveKnowledgeArticleInput,
) {
  const item: KnowledgeArticle = {
    ...data,
    createdAt: new Date().toISOString(),
    id: createId('knowledge'),
  };
  articles.unshift(item);
  return mockDelay(item);
}

export async function updateKnowledgeArticleApi(
  id: string,
  data: SaveKnowledgeArticleInput,
) {
  const index = articles.findIndex((item) => item.id === id);
  const current = articles[index];
  if (current) {
    articles[index] = {
      ...current,
      ...data,
      updatedAt: new Date().toISOString(),
    };
  }
  return mockDelay(articles[index]);
}

export async function deleteKnowledgeArticleApi(id: string) {
  const index = articles.findIndex((item) => item.id === id);
  if (index >= 0) {
    articles.splice(index, 1);
  }
  return mockDelay(true);
}
