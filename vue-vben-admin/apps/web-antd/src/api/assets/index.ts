import type { PageQuery, PageResult } from '#/types/api';

import { requestClient } from '#/api/request';

export interface AssetSummary {
  totalIncome: number;
  totalExpense: number;
  totalInvestment: number;
  netAsset: number;
  incomeCount: number;
  expenseCount: number;
  investmentCount: number;
}

export interface Income {
  id: string;
  userId: string;
  incomeDate: string;
  category: string;
  title: string;
  amount: number;
  description?: string;
  remark?: string;
  createdAt: string;
}

export interface Expense {
  id: string;
  userId: string;
  expenseDate: string;
  category: string;
  title: string;
  amount: number;
  description?: string;
  remark?: string;
  createdAt: string;
}

export interface Budget {
  id: string;
  userId: string;
  year: number;
  month: number;
  category: string;
  plannedAmount: number;
  actualAmount: number;
  remainingAmount: number;
  remark?: string;
  createdAt: string;
}

export interface Investment {
  id: string;
  userId: string;
  investmentDate: string;
  type: string;
  name: string;
  amount: number;
  currentValue?: number;
  returnRate?: number;
  description?: string;
  remark?: string;
  createdAt: string;
}

export interface AssetQuery extends PageQuery {
  startDate?: string;
  endDate?: string;
  category?: string;
  keyword?: string;
}

export interface CreateIncomeInput {
  incomeDate: string;
  category: string;
  title: string;
  amount: number;
  description?: string;
  remark?: string;
}

export interface UpdateIncomeInput {
  incomeDate?: string;
  category?: string;
  title?: string;
  amount?: number;
  description?: string;
  remark?: string;
}

export interface CreateExpenseInput {
  expenseDate: string;
  category: string;
  title: string;
  amount: number;
  description?: string;
  remark?: string;
}

export interface UpdateExpenseInput {
  expenseDate?: string;
  category?: string;
  title?: string;
  amount?: number;
  description?: string;
  remark?: string;
}

export interface CreateBudgetInput {
  year: number;
  month: number;
  category: string;
  plannedAmount: number;
  actualAmount?: number;
  remark?: string;
}

export interface UpdateBudgetInput {
  year?: number;
  month?: number;
  category?: string;
  plannedAmount?: number;
  actualAmount?: number;
  remark?: string;
}

export interface CreateInvestmentInput {
  investmentDate: string;
  type: string;
  name: string;
  amount: number;
  currentValue?: number;
  returnRate?: number;
  description?: string;
  remark?: string;
}

export interface UpdateInvestmentInput {
  investmentDate?: string;
  type?: string;
  name?: string;
  amount?: number;
  currentValue?: number;
  returnRate?: number;
  description?: string;
  remark?: string;
}

export const assetApi = {
  getSummary: () =>
    requestClient.get<AssetSummary>('/assets/summary'),

  getIncomes: (params: AssetQuery) =>
    requestClient.get<PageResult<Income>>('/assets/incomes', { params }),

  getIncomeById: (id: string) =>
    requestClient.get<Income>(`/assets/incomes/${id}`),

  createIncome: (data: CreateIncomeInput) =>
    requestClient.post<Income>('/assets/incomes', data),

  updateIncome: (id: string, data: UpdateIncomeInput) =>
    requestClient.put<Income>(`/assets/incomes/${id}`, data),

  deleteIncome: (id: string) =>
    requestClient.delete(`/assets/incomes/${id}`),

  getExpenses: (params: AssetQuery) =>
    requestClient.get<PageResult<Expense>>('/assets/expenses', { params }),

  getExpenseById: (id: string) =>
    requestClient.get<Expense>(`/assets/expenses/${id}`),

  createExpense: (data: CreateExpenseInput) =>
    requestClient.post<Expense>('/assets/expenses', data),

  updateExpense: (id: string, data: UpdateExpenseInput) =>
    requestClient.put<Expense>(`/assets/expenses/${id}`, data),

  deleteExpense: (id: string) =>
    requestClient.delete(`/assets/expenses/${id}`),

  getBudgets: (params: AssetQuery) =>
    requestClient.get<PageResult<Budget>>('/assets/budgets', { params }),

  getBudgetById: (id: string) =>
    requestClient.get<Budget>(`/assets/budgets/${id}`),

  createBudget: (data: CreateBudgetInput) =>
    requestClient.post<Budget>('/assets/budgets', data),

  updateBudget: (id: string, data: UpdateBudgetInput) =>
    requestClient.put<Budget>(`/assets/budgets/${id}`, data),

  deleteBudget: (id: string) =>
    requestClient.delete(`/assets/budgets/${id}`),

  getInvestments: (params: AssetQuery) =>
    requestClient.get<PageResult<Investment>>('/assets/investments', { params }),

  getInvestmentById: (id: string) =>
    requestClient.get<Investment>(`/assets/investments/${id}`),

  createInvestment: (data: CreateInvestmentInput) =>
    requestClient.post<Investment>('/assets/investments', data),

  updateInvestment: (id: string, data: UpdateInvestmentInput) =>
    requestClient.put<Investment>(`/assets/investments/${id}`, data),

  deleteInvestment: (id: string) =>
    requestClient.delete(`/assets/investments/${id}`),
};

export const getAssetSummaryApi = assetApi.getSummary;
export const getIncomePageApi = assetApi.getIncomes;
export const createIncomeApi = assetApi.createIncome;
export const updateIncomeApi = assetApi.updateIncome;
export const deleteIncomeApi = assetApi.deleteIncome;
export const getExpensePageApi = assetApi.getExpenses;
export const createExpenseApi = assetApi.createExpense;
export const updateExpenseApi = assetApi.updateExpense;
export const deleteExpenseApi = assetApi.deleteExpense;
export const getBudgetPageApi = assetApi.getBudgets;
export const createBudgetApi = assetApi.createBudget;
export const updateBudgetApi = assetApi.updateBudget;
export const deleteBudgetApi = assetApi.deleteBudget;
export const getInvestmentPageApi = assetApi.getInvestments;
export const createInvestmentApi = assetApi.createInvestment;
export const updateInvestmentApi = assetApi.updateInvestment;
export const deleteInvestmentApi = assetApi.deleteInvestment;
