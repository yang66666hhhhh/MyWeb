import type { PageQuery, PageResult } from '#/types/api';

import { requestClient } from '#/api/request';

// Dev types
export interface CodeRepository {
  id: string;
  name: string;
  description?: string;
  url: string;
  language: string;
  isPublic: boolean;
  stars: number;
  tags?: string;
  createdAt: string;
}

export interface CreateCodeRepositoryInput {
  name: string;
  description?: string;
  url: string;
  language: string;
  isPublic?: boolean;
  tags?: string;
}

export interface Issue {
  id: string;
  title: string;
  description?: string;
  repository: string;
  status: number;
  priority: number;
  assignee?: string;
  labels?: string;
  createdAt: string;
}

export interface CreateIssueInput {
  title: string;
  description?: string;
  repository: string;
  priority?: number;
  assignee?: string;
  labels?: string;
}

export interface Pipeline {
  id: string;
  name: string;
  description?: string;
  repository: string;
  branch: string;
  status: number;
  triggerType?: string;
  steps?: string;
  lastRunAt?: string;
  createdAt: string;
}

export interface CreatePipelineInput {
  name: string;
  description?: string;
  repository: string;
  branch: string;
  triggerType?: string;
  steps?: string;
}

// Design types
export interface DesignAsset {
  id: string;
  name: string;
  description?: string;
  category: string;
  fileUrl: string;
  fileType: string;
  fileSize: number;
  tags?: string;
  createdAt: string;
}

export interface CreateDesignAssetInput {
  name: string;
  description?: string;
  category: string;
  fileUrl: string;
  fileType: string;
  fileSize: number;
  tags?: string;
}

export interface Prototype {
  id: string;
  title: string;
  description?: string;
  project: string;
  previewUrl?: string;
  status: number;
  tags?: string;
  createdAt: string;
}

export interface CreatePrototypeInput {
  title: string;
  description?: string;
  project: string;
  previewUrl?: string;
  tags?: string;
}

// Teacher types
export interface TeacherCourse {
  id: string;
  name: string;
  description?: string;
  code: string;
  semester: number;
  year: number;
  studentCount: number;
  status: number;
  tags?: string;
  createdAt: string;
}

export interface CreateTeacherCourseInput {
  name: string;
  description?: string;
  code: string;
  semester: number;
  year: number;
  tags?: string;
}

export interface TeacherStudent {
  id: string;
  name: string;
  studentId?: string;
  email?: string;
  phone?: string;
  course?: string;
  grade: number;
  tags?: string;
  createdAt: string;
}

export interface CreateTeacherStudentInput {
  name: string;
  studentId?: string;
  email?: string;
  phone?: string;
  course?: string;
  tags?: string;
}

// Query types
export interface PersonaQuery extends PageQuery {
  category?: string;
  status?: number;
  keyword?: string;
}

// API methods
export const personaApi = {
  // Dev - Repositories
  getRepositories: (params: PersonaQuery) =>
    requestClient.get<PageResult<CodeRepository>>('/persona/dev/repositories', { params }),
  createRepository: (data: CreateCodeRepositoryInput) =>
    requestClient.post<CodeRepository>('/persona/dev/repositories', data),
  updateRepository: (id: string, data: Partial<CreateCodeRepositoryInput>) =>
    requestClient.put<CodeRepository>(`/persona/dev/repositories/${id}`, data),
  deleteRepository: (id: string) =>
    requestClient.delete(`/persona/dev/repositories/${id}`),

  // Dev - Issues
  getIssues: (params: PersonaQuery) =>
    requestClient.get<PageResult<Issue>>('/persona/dev/issues', { params }),
  createIssue: (data: CreateIssueInput) =>
    requestClient.post<Issue>('/persona/dev/issues', data),
  updateIssue: (id: string, data: Partial<CreateIssueInput>) =>
    requestClient.put<Issue>(`/persona/dev/issues/${id}`, data),
  deleteIssue: (id: string) =>
    requestClient.delete(`/persona/dev/issues/${id}`),

  // Dev - Pipelines
  getPipelines: (params: PersonaQuery) =>
    requestClient.get<PageResult<Pipeline>>('/persona/dev/pipelines', { params }),
  createPipeline: (data: CreatePipelineInput) =>
    requestClient.post<Pipeline>('/persona/dev/pipelines', data),
  updatePipeline: (id: string, data: Partial<CreatePipelineInput>) =>
    requestClient.put<Pipeline>(`/persona/dev/pipelines/${id}`, data),
  deletePipeline: (id: string) =>
    requestClient.delete(`/persona/dev/pipelines/${id}`),

  // Design - Assets
  getDesignAssets: (params: PersonaQuery) =>
    requestClient.get<PageResult<DesignAsset>>('/persona/design/assets', { params }),
  createDesignAsset: (data: CreateDesignAssetInput) =>
    requestClient.post<DesignAsset>('/persona/design/assets', data),
  updateDesignAsset: (id: string, data: Partial<CreateDesignAssetInput>) =>
    requestClient.put<DesignAsset>(`/persona/design/assets/${id}`, data),
  deleteDesignAsset: (id: string) =>
    requestClient.delete(`/persona/design/assets/${id}`),

  // Design - Prototypes
  getPrototypes: (params: PersonaQuery) =>
    requestClient.get<PageResult<Prototype>>('/persona/design/prototypes', { params }),
  createPrototype: (data: CreatePrototypeInput) =>
    requestClient.post<Prototype>('/persona/design/prototypes', data),
  updatePrototype: (id: string, data: Partial<CreatePrototypeInput>) =>
    requestClient.put<Prototype>(`/persona/design/prototypes/${id}`, data),
  deletePrototype: (id: string) =>
    requestClient.delete(`/persona/design/prototypes/${id}`),

  // Teacher - Courses
  getTeacherCourses: (params: PersonaQuery) =>
    requestClient.get<PageResult<TeacherCourse>>('/persona/teacher/courses', { params }),
  createTeacherCourse: (data: CreateTeacherCourseInput) =>
    requestClient.post<TeacherCourse>('/persona/teacher/courses', data),
  updateTeacherCourse: (id: string, data: Partial<CreateTeacherCourseInput>) =>
    requestClient.put<TeacherCourse>(`/persona/teacher/courses/${id}`, data),
  deleteTeacherCourse: (id: string) =>
    requestClient.delete(`/persona/teacher/courses/${id}`),

  // Teacher - Students
  getTeacherStudents: (params: PersonaQuery) =>
    requestClient.get<PageResult<TeacherStudent>>('/persona/teacher/students', { params }),
  createTeacherStudent: (data: CreateTeacherStudentInput) =>
    requestClient.post<TeacherStudent>('/persona/teacher/students', data),
  updateTeacherStudent: (id: string, data: Partial<CreateTeacherStudentInput>) =>
    requestClient.put<TeacherStudent>(`/persona/teacher/students/${id}`, data),
  deleteTeacherStudent: (id: string) =>
    requestClient.delete(`/persona/teacher/students/${id}`),
};

// Export individual API methods
export const getRepositoriesApi = personaApi.getRepositories;
export const createRepositoryApi = personaApi.createRepository;
export const updateRepositoryApi = personaApi.updateRepository;
export const deleteRepositoryApi = personaApi.deleteRepository;

export const getIssuesApi = personaApi.getIssues;
export const createIssueApi = personaApi.createIssue;
export const updateIssueApi = personaApi.updateIssue;
export const deleteIssueApi = personaApi.deleteIssue;

export const getPipelinesApi = personaApi.getPipelines;
export const createPipelineApi = personaApi.createPipeline;
export const updatePipelineApi = personaApi.updatePipeline;
export const deletePipelineApi = personaApi.deletePipeline;

export const getDesignAssetsApi = personaApi.getDesignAssets;
export const createDesignAssetApi = personaApi.createDesignAsset;
export const updateDesignAssetApi = personaApi.updateDesignAsset;
export const deleteDesignAssetApi = personaApi.deleteDesignAsset;

export const getPrototypesApi = personaApi.getPrototypes;
export const createPrototypeApi = personaApi.createPrototype;
export const updatePrototypeApi = personaApi.updatePrototype;
export const deletePrototypeApi = personaApi.deletePrototype;

export const getTeacherCoursesApi = personaApi.getTeacherCourses;
export const createTeacherCourseApi = personaApi.createTeacherCourse;
export const updateTeacherCourseApi = personaApi.updateTeacherCourse;
export const deleteTeacherCourseApi = personaApi.deleteTeacherCourse;

export const getTeacherStudentsApi = personaApi.getTeacherStudents;
export const createTeacherStudentApi = personaApi.createTeacherStudent;
export const updateTeacherStudentApi = personaApi.updateTeacherStudent;
export const deleteTeacherStudentApi = personaApi.deleteTeacherStudent;
