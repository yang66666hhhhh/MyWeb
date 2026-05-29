import type { PageQuery, PageResult } from '#/types/api';

import { requestClient } from '#/api/request';

export interface Contact {
  id: string;
  userId?: string;
  name: string;
  company?: string;
  position?: string;
  phone?: string;
  email?: string;
  weChat?: string;
  tags?: string;
  remark?: string;
  interactionCount: number;
  lastInteractionAt?: string;
  createdAt: string;
}

export interface Interaction {
  id: string;
  userId?: string;
  contactId: string;
  contactName: string;
  type: string;
  content: string;
  interactionDate: string;
  nextFollowUpDate?: string;
  remark?: string;
  createdAt: string;
}

export interface Tag {
  id: string;
  name: string;
  color: string;
  usageCount: number;
}

export interface NetworkQuery extends PageQuery {
  tag?: string;
  keyword?: string;
  startDate?: string;
  endDate?: string;
}

export interface CreateContactInput {
  name: string;
  company?: string;
  position?: string;
  phone?: string;
  email?: string;
  weChat?: string;
  tags?: string;
  remark?: string;
}

export interface UpdateContactInput {
  name?: string;
  company?: string;
  position?: string;
  phone?: string;
  email?: string;
  weChat?: string;
  tags?: string;
  remark?: string;
}

export interface CreateInteractionInput {
  contactId: string;
  type: string;
  content: string;
  interactionDate: string;
  nextFollowUpDate?: string;
  remark?: string;
}

export interface UpdateInteractionInput {
  type?: string;
  content?: string;
  interactionDate?: string;
  nextFollowUpDate?: string;
  remark?: string;
}

export const networkApi = {
  getContacts: (params: NetworkQuery) =>
    requestClient.get<PageResult<Contact>>('/network/contacts', { params }),

  getContactById: (id: string) =>
    requestClient.get<Contact>(`/network/contacts/${id}`),

  createContact: (data: CreateContactInput) =>
    requestClient.post<Contact>('/network/contacts', data),

  updateContact: (id: string, data: UpdateContactInput) =>
    requestClient.put<Contact>(`/network/contacts/${id}`, data),

  deleteContact: (id: string) =>
    requestClient.delete(`/network/contacts/${id}`),

  getInteractions: (contactId: string, params: NetworkQuery) =>
    requestClient.get<PageResult<Interaction>>(`/network/contacts/${contactId}/interactions`, { params }),

  getInteractionById: (id: string) =>
    requestClient.get<Interaction>(`/network/interactions/${id}`),

  createInteraction: (data: CreateInteractionInput) =>
    requestClient.post<Interaction>('/network/interactions', data),

  updateInteraction: (id: string, data: UpdateInteractionInput) =>
    requestClient.put<Interaction>(`/network/interactions/${id}`, data),

  deleteInteraction: (id: string) =>
    requestClient.delete(`/network/interactions/${id}`),

  getTags: () =>
    requestClient.get<Tag[]>('/network/tags'),
};

export const getContactPageApi = networkApi.getContacts;
export const createContactApi = networkApi.createContact;
export const updateContactApi = networkApi.updateContact;
export const deleteContactApi = networkApi.deleteContact;
export const getInteractionPageApi = networkApi.getInteractions;
export const createInteractionApi = networkApi.createInteraction;
export const updateInteractionApi = networkApi.updateInteraction;
export const deleteInteractionApi = networkApi.deleteInteraction;
export const getNetworkTagsApi = networkApi.getTags;
