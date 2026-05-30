import type { PageQuery, PageResult } from '#/types/api';

import { requestClient } from '#/api/request';

// Skill types
export interface Skill {
  id: string;
  userId?: string;
  name: string;
  category: string;
  level: number;
  targetLevel: number;
  experiencePoints: number;
  description?: string;
  tags?: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateSkillInput {
  name: string;
  category: string;
  level?: number;
  targetLevel?: number;
  description?: string;
  tags?: string;
}

export interface UpdateSkillInput {
  name?: string;
  category?: string;
  level?: number;
  targetLevel?: number;
  description?: string;
  tags?: string;
  isActive?: boolean;
}

// Goal types
export interface Goal {
  id: string;
  userId?: string;
  title: string;
  description?: string;
  category: string;
  priority: number;
  status: number;
  startDate?: string;
  dueDate?: string;
  completedAt?: string;
  progress: number;
  tags?: string;
  createdAt: string;
}

export interface CreateGoalInput {
  title: string;
  description?: string;
  category: string;
  priority?: number;
  startDate?: string;
  dueDate?: string;
  tags?: string;
}

export interface UpdateGoalInput {
  title?: string;
  description?: string;
  category?: string;
  priority?: number;
  status?: number;
  startDate?: string;
  dueDate?: string;
  progress?: number;
  tags?: string;
}

// YearPlan types
export interface YearPlan {
  id: string;
  userId?: string;
  year: number;
  title: string;
  description?: string;
  category: string;
  status: number;
  progress: number;
  tags?: string;
  createdAt: string;
}

export interface CreateYearPlanInput {
  year: number;
  title: string;
  description?: string;
  category: string;
  tags?: string;
}

export interface UpdateYearPlanInput {
  title?: string;
  description?: string;
  category?: string;
  status?: number;
  progress?: number;
  tags?: string;
}

// MonthlyReview types
export interface MonthlyReview {
  id: string;
  userId?: string;
  year: number;
  month: number;
  title: string;
  achievements?: string;
  challenges?: string;
  lessonsLearned?: string;
  nextMonthGoals?: string;
  rating: number;
  tags?: string;
  createdAt: string;
}

export interface CreateMonthlyReviewInput {
  year: number;
  month: number;
  title: string;
  achievements?: string;
  challenges?: string;
  lessonsLearned?: string;
  nextMonthGoals?: string;
  rating?: number;
  tags?: string;
}

export interface UpdateMonthlyReviewInput {
  title?: string;
  achievements?: string;
  challenges?: string;
  lessonsLearned?: string;
  nextMonthGoals?: string;
  rating?: number;
  tags?: string;
}

// LearningPath types
export interface LearningPath {
  id: string;
  userId?: string;
  title: string;
  description?: string;
  category: string;
  status: number;
  progress: number;
  estimatedHours: number;
  actualHours: number;
  tags?: string;
  createdAt: string;
}

export interface CreateLearningPathInput {
  title: string;
  description?: string;
  category: string;
  estimatedHours?: number;
  tags?: string;
}

export interface UpdateLearningPathInput {
  title?: string;
  description?: string;
  category?: string;
  status?: number;
  progress?: number;
  estimatedHours?: number;
  actualHours?: number;
  tags?: string;
}

// Course types
export interface Course {
  id: string;
  userId?: string;
  title: string;
  description?: string;
  platform: string;
  category: string;
  status: number;
  progress: number;
  totalLessons: number;
  completedLessons: number;
  instructor?: string;
  url?: string;
  tags?: string;
  createdAt: string;
}

export interface CreateCourseInput {
  title: string;
  description?: string;
  platform: string;
  category: string;
  totalLessons?: number;
  instructor?: string;
  url?: string;
  tags?: string;
}

export interface UpdateCourseInput {
  title?: string;
  description?: string;
  platform?: string;
  category?: string;
  status?: number;
  progress?: number;
  totalLessons?: number;
  completedLessons?: number;
  instructor?: string;
  url?: string;
  tags?: string;
}

// Fitness types
export interface FitnessRecord {
  id: string;
  userId?: string;
  exerciseType: string;
  durationMinutes: number;
  caloriesBurned: number;
  notes?: string;
  exerciseDate: string;
  tags?: string;
  createdAt: string;
}

export interface CreateFitnessRecordInput {
  exerciseType: string;
  durationMinutes: number;
  caloriesBurned?: number;
  notes?: string;
  exerciseDate: string;
  tags?: string;
}

export interface UpdateFitnessRecordInput {
  exerciseType?: string;
  durationMinutes?: number;
  caloriesBurned?: number;
  notes?: string;
  exerciseDate?: string;
  tags?: string;
}

// Sleep types
export interface SleepRecord {
  id: string;
  userId?: string;
  bedTime: string;
  wakeTime: string;
  durationMinutes: number;
  quality: number;
  notes?: string;
  tags?: string;
  createdAt: string;
}

export interface CreateSleepRecordInput {
  bedTime: string;
  wakeTime: string;
  quality?: number;
  notes?: string;
  tags?: string;
}

export interface UpdateSleepRecordInput {
  bedTime?: string;
  wakeTime?: string;
  quality?: number;
  notes?: string;
  tags?: string;
}

// Mood types
export interface MoodRecord {
  id: string;
  userId?: string;
  moodLevel: number;
  moodType?: string;
  notes?: string;
  recordDate: string;
  tags?: string;
  createdAt: string;
}

export interface CreateMoodRecordInput {
  moodLevel: number;
  moodType?: string;
  notes?: string;
  recordDate: string;
  tags?: string;
}

export interface UpdateMoodRecordInput {
  moodLevel?: number;
  moodType?: string;
  notes?: string;
  recordDate?: string;
  tags?: string;
}

// Reading types
export interface ReadingBook {
  id: string;
  userId?: string;
  title: string;
  author?: string;
  category?: string;
  status: number;
  progress: number;
  totalPages: number;
  currentPage: number;
  startDate?: string;
  finishDate?: string;
  notes?: string;
  tags?: string;
  createdAt: string;
}

export interface CreateReadingBookInput {
  title: string;
  author?: string;
  category?: string;
  totalPages?: number;
  notes?: string;
  tags?: string;
}

export interface UpdateReadingBookInput {
  title?: string;
  author?: string;
  category?: string;
  status?: number;
  progress?: number;
  totalPages?: number;
  currentPage?: number;
  startDate?: string;
  finishDate?: string;
  notes?: string;
  tags?: string;
}

// Focus types
export interface FocusSession {
  id: string;
  userId?: string;
  taskTitle?: string;
  durationMinutes: number;
  plannedMinutes: number;
  status: number;
  startTime: string;
  endTime?: string;
  notes?: string;
  tags?: string;
  createdAt: string;
}

export interface CreateFocusSessionInput {
  taskTitle?: string;
  plannedMinutes?: number;
  notes?: string;
  tags?: string;
}

export interface UpdateFocusSessionInput {
  taskTitle?: string;
  plannedMinutes?: number;
  durationMinutes?: number;
  status?: number;
  endTime?: string;
  notes?: string;
  tags?: string;
}

// Query types
export interface GrowthQuery extends PageQuery {
  category?: string;
  status?: number;
  keyword?: string;
  startDate?: string;
  endDate?: string;
}

// API methods
export const growthExtendedApi = {
  // Skills
  getSkills: (params: GrowthQuery) =>
    requestClient.get<PageResult<Skill>>('/growth/skills', { params }),
  createSkill: (data: CreateSkillInput) =>
    requestClient.post<Skill>('/growth/skills', data),
  updateSkill: (id: string, data: UpdateSkillInput) =>
    requestClient.put<Skill>(`/growth/skills/${id}`, data),
  deleteSkill: (id: string) =>
    requestClient.delete(`/growth/skills/${id}`),

  // Goals
  getGoals: (params: GrowthQuery) =>
    requestClient.get<PageResult<Goal>>('/growth/goals', { params }),
  createGoal: (data: CreateGoalInput) =>
    requestClient.post<Goal>('/growth/goals', data),
  updateGoal: (id: string, data: UpdateGoalInput) =>
    requestClient.put<Goal>(`/growth/goals/${id}`, data),
  deleteGoal: (id: string) =>
    requestClient.delete(`/growth/goals/${id}`),

  // Year Plans
  getYearPlans: (params: GrowthQuery) =>
    requestClient.get<PageResult<YearPlan>>('/growth/year-plans', { params }),
  createYearPlan: (data: CreateYearPlanInput) =>
    requestClient.post<YearPlan>('/growth/year-plans', data),
  updateYearPlan: (id: string, data: UpdateYearPlanInput) =>
    requestClient.put<YearPlan>(`/growth/year-plans/${id}`, data),
  deleteYearPlan: (id: string) =>
    requestClient.delete(`/growth/year-plans/${id}`),

  // Monthly Reviews
  getMonthlyReviews: (params: GrowthQuery) =>
    requestClient.get<PageResult<MonthlyReview>>('/growth/monthly-reviews', { params }),
  createMonthlyReview: (data: CreateMonthlyReviewInput) =>
    requestClient.post<MonthlyReview>('/growth/monthly-reviews', data),
  updateMonthlyReview: (id: string, data: UpdateMonthlyReviewInput) =>
    requestClient.put<MonthlyReview>(`/growth/monthly-reviews/${id}`, data),
  deleteMonthlyReview: (id: string) =>
    requestClient.delete(`/growth/monthly-reviews/${id}`),

  // Learning Paths
  getLearningPaths: (params: GrowthQuery) =>
    requestClient.get<PageResult<LearningPath>>('/growth/learning-paths', { params }),
  createLearningPath: (data: CreateLearningPathInput) =>
    requestClient.post<LearningPath>('/growth/learning-paths', data),
  updateLearningPath: (id: string, data: UpdateLearningPathInput) =>
    requestClient.put<LearningPath>(`/growth/learning-paths/${id}`, data),
  deleteLearningPath: (id: string) =>
    requestClient.delete(`/growth/learning-paths/${id}`),

  // Courses
  getCourses: (params: GrowthQuery) =>
    requestClient.get<PageResult<Course>>('/growth/courses', { params }),
  createCourse: (data: CreateCourseInput) =>
    requestClient.post<Course>('/growth/courses', data),
  updateCourse: (id: string, data: UpdateCourseInput) =>
    requestClient.put<Course>(`/growth/courses/${id}`, data),
  deleteCourse: (id: string) =>
    requestClient.delete(`/growth/courses/${id}`),

  // Fitness
  getFitnessRecords: (params: GrowthQuery) =>
    requestClient.get<PageResult<FitnessRecord>>('/growth/fitness', { params }),
  createFitnessRecord: (data: CreateFitnessRecordInput) =>
    requestClient.post<FitnessRecord>('/growth/fitness', data),
  updateFitnessRecord: (id: string, data: UpdateFitnessRecordInput) =>
    requestClient.put<FitnessRecord>(`/growth/fitness/${id}`, data),
  deleteFitnessRecord: (id: string) =>
    requestClient.delete(`/growth/fitness/${id}`),

  // Sleep
  getSleepRecords: (params: GrowthQuery) =>
    requestClient.get<PageResult<SleepRecord>>('/growth/sleep', { params }),
  createSleepRecord: (data: CreateSleepRecordInput) =>
    requestClient.post<SleepRecord>('/growth/sleep', data),
  updateSleepRecord: (id: string, data: UpdateSleepRecordInput) =>
    requestClient.put<SleepRecord>(`/growth/sleep/${id}`, data),
  deleteSleepRecord: (id: string) =>
    requestClient.delete(`/growth/sleep/${id}`),

  // Mood
  getMoodRecords: (params: GrowthQuery) =>
    requestClient.get<PageResult<MoodRecord>>('/growth/mood', { params }),
  createMoodRecord: (data: CreateMoodRecordInput) =>
    requestClient.post<MoodRecord>('/growth/mood', data),
  updateMoodRecord: (id: string, data: UpdateMoodRecordInput) =>
    requestClient.put<MoodRecord>(`/growth/mood/${id}`, data),
  deleteMoodRecord: (id: string) =>
    requestClient.delete(`/growth/mood/${id}`),

  // Reading
  getReadingBooks: (params: GrowthQuery) =>
    requestClient.get<PageResult<ReadingBook>>('/growth/reading', { params }),
  createReadingBook: (data: CreateReadingBookInput) =>
    requestClient.post<ReadingBook>('/growth/reading', data),
  updateReadingBook: (id: string, data: UpdateReadingBookInput) =>
    requestClient.put<ReadingBook>(`/growth/reading/${id}`, data),
  deleteReadingBook: (id: string) =>
    requestClient.delete(`/growth/reading/${id}`),

  // Focus
  getFocusSessions: (params: GrowthQuery) =>
    requestClient.get<PageResult<FocusSession>>('/growth/focus', { params }),
  createFocusSession: (data: CreateFocusSessionInput) =>
    requestClient.post<FocusSession>('/growth/focus', data),
  updateFocusSession: (id: string, data: UpdateFocusSessionInput) =>
    requestClient.put<FocusSession>(`/growth/focus/${id}`, data),
  deleteFocusSession: (id: string) =>
    requestClient.delete(`/growth/focus/${id}`),
};

// Export individual API methods
export const getSkillsApi = growthExtendedApi.getSkills;
export const createSkillApi = growthExtendedApi.createSkill;
export const updateSkillApi = growthExtendedApi.updateSkill;
export const deleteSkillApi = growthExtendedApi.deleteSkill;

export const getGoalsApi = growthExtendedApi.getGoals;
export const createGoalApi = growthExtendedApi.createGoal;
export const updateGoalApi = growthExtendedApi.updateGoal;
export const deleteGoalApi = growthExtendedApi.deleteGoal;

export const getYearPlansApi = growthExtendedApi.getYearPlans;
export const createYearPlanApi = growthExtendedApi.createYearPlan;
export const updateYearPlanApi = growthExtendedApi.updateYearPlan;
export const deleteYearPlanApi = growthExtendedApi.deleteYearPlan;

export const getMonthlyReviewsApi = growthExtendedApi.getMonthlyReviews;
export const createMonthlyReviewApi = growthExtendedApi.createMonthlyReview;
export const updateMonthlyReviewApi = growthExtendedApi.updateMonthlyReview;
export const deleteMonthlyReviewApi = growthExtendedApi.deleteMonthlyReview;

export const getLearningPathsApi = growthExtendedApi.getLearningPaths;
export const createLearningPathApi = growthExtendedApi.createLearningPath;
export const updateLearningPathApi = growthExtendedApi.updateLearningPath;
export const deleteLearningPathApi = growthExtendedApi.deleteLearningPath;

export const getCoursesApi = growthExtendedApi.getCourses;
export const createCourseApi = growthExtendedApi.createCourse;
export const updateCourseApi = growthExtendedApi.updateCourse;
export const deleteCourseApi = growthExtendedApi.deleteCourse;

export const getFitnessRecordsApi = growthExtendedApi.getFitnessRecords;
export const createFitnessRecordApi = growthExtendedApi.createFitnessRecord;
export const updateFitnessRecordApi = growthExtendedApi.updateFitnessRecord;
export const deleteFitnessRecordApi = growthExtendedApi.deleteFitnessRecord;

export const getSleepRecordsApi = growthExtendedApi.getSleepRecords;
export const createSleepRecordApi = growthExtendedApi.createSleepRecord;
export const updateSleepRecordApi = growthExtendedApi.updateSleepRecord;
export const deleteSleepRecordApi = growthExtendedApi.deleteSleepRecord;

export const getMoodRecordsApi = growthExtendedApi.getMoodRecords;
export const createMoodRecordApi = growthExtendedApi.createMoodRecord;
export const updateMoodRecordApi = growthExtendedApi.updateMoodRecord;
export const deleteMoodRecordApi = growthExtendedApi.deleteMoodRecord;

export const getReadingBooksApi = growthExtendedApi.getReadingBooks;
export const createReadingBookApi = growthExtendedApi.createReadingBook;
export const updateReadingBookApi = growthExtendedApi.updateReadingBook;
export const deleteReadingBookApi = growthExtendedApi.deleteReadingBook;

export const getFocusSessionsApi = growthExtendedApi.getFocusSessions;
export const createFocusSessionApi = growthExtendedApi.createFocusSession;
export const updateFocusSessionApi = growthExtendedApi.updateFocusSession;
export const deleteFocusSessionApi = growthExtendedApi.deleteFocusSession;
