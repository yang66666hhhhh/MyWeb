import type { PageQuery, PageResult } from '#/types/api';

import { requestClient } from '#/api/request';

// Time Analytics types
export interface TimeAnalyticsOverview {
  dailyWorkHours: number;
  dailyLearningHours: number;
  dailyRestHours: number;
  timeUtilizationRate: number;
}

export interface HourlyDistribution {
  hour: number;
  value: number;
  category: string;
}

export interface WeeklyTrend {
  date: string;
  workHours: number;
  learningHours: number;
}

// Habits Analytics types
export interface HabitsAnalyticsOverview {
  activeHabits: number;
  monthlyCheckIns: number;
  longestStreak: number;
  completionRate: number;
}

export interface HabitTrend {
  date: string;
  checkIns: number;
  completionRate: number;
}

// Finance Analytics types
export interface FinanceAnalyticsOverview {
  monthlyBalance: number;
  savingsRate: number;
  investmentReturn: number;
  budgetExecution: number;
}

export interface MonthlyFinanceTrend {
  month: string;
  income: number;
  expense: number;
}

export interface ExpenseBreakdown {
  category: string;
  amount: number;
  percentage: number;
}

// Custom Report types
export interface CustomReport {
  id: string;
  title: string;
  description?: string;
  type?: string;
  createdAt: string;
}

export interface CreateCustomReportInput {
  title: string;
  description?: string;
  type?: string;
}

// AI Insight types
export interface AiInsight {
  id: string;
  title: string;
  content?: string;
  category?: string;
  createdAt: string;
}

// Query types
export interface AnalyticsQuery extends PageQuery {
  type?: string;
  keyword?: string;
  startDate?: string;
  endDate?: string;
}

// API methods
export const analyticsExtendedApi = {
  // Time Analytics
  getTimeOverview: () =>
    requestClient.get<TimeAnalyticsOverview>('/analytics/time/overview'),
  getHourlyDistribution: () =>
    requestClient.get<HourlyDistribution[]>('/analytics/time/hourly-distribution'),
  getWeeklyTrend: () =>
    requestClient.get<WeeklyTrend[]>('/analytics/time/weekly-trend'),

  // Habits Analytics
  getHabitsOverview: () =>
    requestClient.get<HabitsAnalyticsOverview>('/analytics/habits/overview'),
  getHabitTrends: () =>
    requestClient.get<HabitTrend[]>('/analytics/habits/trends'),

  // Finance Analytics
  getFinanceOverview: () =>
    requestClient.get<FinanceAnalyticsOverview>('/analytics/finance/overview'),
  getMonthlyFinanceTrend: () =>
    requestClient.get<MonthlyFinanceTrend[]>('/analytics/finance/monthly-trend'),
  getExpenseBreakdown: () =>
    requestClient.get<ExpenseBreakdown[]>('/analytics/finance/expense-breakdown'),

  // Custom Reports
  getCustomReports: (params: AnalyticsQuery) =>
    requestClient.get<PageResult<CustomReport>>('/analytics/reports', { params }),
  createCustomReport: (data: CreateCustomReportInput) =>
    requestClient.post<CustomReport>('/analytics/reports', data),
  deleteCustomReport: (id: string) =>
    requestClient.delete(`/analytics/reports/${id}`),

  // AI Insights
  getAiInsights: (params: AnalyticsQuery) =>
    requestClient.get<PageResult<AiInsight>>('/analytics/ai-insights', { params }),
  generateAiInsight: () =>
    requestClient.post<AiInsight>('/analytics/ai-insights/generate'),
};

// Export individual API methods
export const getTimeOverviewApi = analyticsExtendedApi.getTimeOverview;
export const getHourlyDistributionApi = analyticsExtendedApi.getHourlyDistribution;
export const getWeeklyTrendApi = analyticsExtendedApi.getWeeklyTrend;

export const getHabitsOverviewApi = analyticsExtendedApi.getHabitsOverview;
export const getHabitTrendsApi = analyticsExtendedApi.getHabitTrends;

export const getFinanceOverviewApi = analyticsExtendedApi.getFinanceOverview;
export const getMonthlyFinanceTrendApi = analyticsExtendedApi.getMonthlyFinanceTrend;
export const getExpenseBreakdownApi = analyticsExtendedApi.getExpenseBreakdown;

export const getCustomReportsApi = analyticsExtendedApi.getCustomReports;
export const createCustomReportApi = analyticsExtendedApi.createCustomReport;
export const deleteCustomReportApi = analyticsExtendedApi.deleteCustomReport;

export const getAiInsightsApi = analyticsExtendedApi.getAiInsights;
export const generateAiInsightApi = analyticsExtendedApi.generateAiInsight;
