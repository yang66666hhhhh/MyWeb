import type { PageQuery, PageResult } from '#/types/api';

import { requestClient } from '#/api/request';

// Automation types
export interface AutomationWorkflow {
  id: string;
  name: string;
  description?: string;
  triggerType?: string;
  actions?: string;
  isActive: boolean;
  lastRunAt?: string;
  createdAt: string;
}

export interface CreateAutomationWorkflowInput {
  name: string;
  description?: string;
  triggerType?: string;
  actions?: string;
}

export interface UpdateAutomationWorkflowInput {
  name?: string;
  description?: string;
  triggerType?: string;
  actions?: string;
  isActive?: boolean;
}

// Knowledge Chat types
export interface KnowledgeChatSession {
  id: string;
  title?: string;
  lastMessage?: string;
  messageCount: number;
  createdAt: string;
}

export interface KnowledgeChatResponse {
  sessionId: string;
  content: string;
  sources?: string;
}

export interface KnowledgeChatRequest {
  message: string;
  sessionId?: string;
}

// AI Insight types
export interface AiInsightItem {
  id: string;
  title: string;
  content?: string;
  category?: string;
  source?: string;
  createdAt: string;
}

export interface GenerateInsightInput {
  title?: string;
  category?: string;
  source?: string;
}

// Query types
export interface AiExtendedQuery extends PageQuery {
  type?: string;
  keyword?: string;
}

// API methods
export const aiExtendedApi = {
  // Automation
  getWorkflows: (params: AiExtendedQuery) =>
    requestClient.get<PageResult<AutomationWorkflow>>('/ai/automation', { params }),
  createWorkflow: (data: CreateAutomationWorkflowInput) =>
    requestClient.post<AutomationWorkflow>('/ai/automation', data),
  updateWorkflow: (id: string, data: UpdateAutomationWorkflowInput) =>
    requestClient.put<AutomationWorkflow>(`/ai/automation/${id}`, data),
  deleteWorkflow: (id: string) =>
    requestClient.delete(`/ai/automation/${id}`),

  // Knowledge Chat
  getKnowledgeChatSessions: (params: AiExtendedQuery) =>
    requestClient.get<PageResult<KnowledgeChatSession>>('/ai/knowledge-chat/sessions', { params }),
  sendKnowledgeChatMessage: (data: KnowledgeChatRequest) =>
    requestClient.post<KnowledgeChatResponse>('/ai/knowledge-chat', data),
  deleteKnowledgeChatSession: (id: string) =>
    requestClient.delete(`/ai/knowledge-chat/sessions/${id}`),

  // Insights
  getInsights: (params: AiExtendedQuery) =>
    requestClient.get<PageResult<AiInsightItem>>('/ai/insights', { params }),
  generateInsight: (data: GenerateInsightInput) =>
    requestClient.post<AiInsightItem>('/ai/insights/generate', data),
  deleteInsight: (id: string) =>
    requestClient.delete(`/ai/insights/${id}`),
};

// Export individual API methods
export const getWorkflowsApi = aiExtendedApi.getWorkflows;
export const createWorkflowApi = aiExtendedApi.createWorkflow;
export const updateWorkflowApi = aiExtendedApi.updateWorkflow;
export const deleteWorkflowApi = aiExtendedApi.deleteWorkflow;

export const getKnowledgeChatSessionsApi = aiExtendedApi.getKnowledgeChatSessions;
export const sendKnowledgeChatMessageApi = aiExtendedApi.sendKnowledgeChatMessage;
export const deleteKnowledgeChatSessionApi = aiExtendedApi.deleteKnowledgeChatSession;

export const getInsightsApi = aiExtendedApi.getInsights;
export const generateInsightApi = aiExtendedApi.generateInsight;
export const deleteInsightApi = aiExtendedApi.deleteInsight;
