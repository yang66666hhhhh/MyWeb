import { requestClient } from '#/api/request';

export interface AiPlan {
  id: string;
  userId?: string;
  title: string;
  description?: string;
  type: string;
  status: string;
  generatedContent?: string;
  remark?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface AiReport {
  id: string;
  userId?: string;
  title: string;
  type: string;
  content?: string;
  remark?: string;
  startDate?: string;
  endDate?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface AiChatMessage {
  id: string;
  sessionId: string;
  role: string;
  content: string;
  createdAt: string;
}

export interface AiChatSession {
  id: string;
  userId?: string;
  title: string;
  lastMessage?: string;
  messageCount: number;
  createdAt: string;
}

export interface ChatResponse {
  sessionId?: string;
  messageId?: string;
  content: string;
  success: boolean;
  errorMessage?: string;
}

export interface PageResult<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

export interface GeneratePlanRequest {
  type: string;
  description?: string;
  targetDate?: string;
  relatedProjectId?: string;
  includeCategories?: string[];
}

export interface GenerateReportRequest {
  type: string;
  startDate?: string;
  endDate?: string;
  relatedProjectId?: string;
  includeStatistics?: boolean;
}

export interface ChatRequest {
  message: string;
  sessionId?: string;
  history?: { content: string; role: string; }[];
}

export const aiApi = {
  getPlans: (params?: { keyword?: string; page?: number; pageSize?: number; type?: string; }) =>
    requestClient.get<PageResult<AiPlan>>('/ai/plans', { params }),

  getPlanById: (id: string) =>
    requestClient.get<AiPlan>(`/ai/plans/${id}`),

  generatePlan: (data: GeneratePlanRequest) =>
    requestClient.post<AiPlan>('/ai/generate-plan', data),

  deletePlan: (id: string) =>
    requestClient.delete(`/ai/plans/${id}`),

  getReports: (params?: { keyword?: string; page?: number; pageSize?: number; type?: string; }) =>
    requestClient.get<PageResult<AiReport>>('/ai/reports', { params }),

  getReportById: (id: string) =>
    requestClient.get<AiReport>(`/ai/reports/${id}`),

  generateReport: (data: GenerateReportRequest) =>
    requestClient.post<AiReport>('/ai/generate-report', data),

  deleteReport: (id: string) =>
    requestClient.delete(`/ai/reports/${id}`),

  getChatSessions: (params?: { page?: number; pageSize?: number }) =>
    requestClient.get<PageResult<AiChatSession>>('/ai/chat/sessions', { params }),

  getChatMessages: (sessionId: string) =>
    requestClient.get<AiChatMessage[]>(`/ai/chat/sessions/${sessionId}/messages`),

  chat: (data: ChatRequest) =>
    requestClient.post<ChatResponse>('/ai/chat', data),

  deleteChatSession: (id: string) =>
    requestClient.delete(`/ai/chat/sessions/${id}`),
};
