import { requestClient } from '#/api/request';

export interface UserProfileDto {
  id: string;
  username: string;
  realName: string;
  avatar?: string;
  email?: string;
  phone?: string;
  roles: string;
  status: number;
}

export interface UpdateProfileDto {
  realName?: string;
  avatar?: string;
  email?: string;
  phone?: string;
}

export interface ChangePasswordDto {
  oldPassword: string;
  newPassword: string;
}

export const getUserProfileApi = () =>
  requestClient.get<UserProfileDto>('/user/profile');

export const updateUserProfileApi = (data: UpdateProfileDto) =>
  requestClient.put<UserProfileDto>('/user/profile', data);

export const changePasswordApi = (data: ChangePasswordDto) =>
  requestClient.post('/user/profile/change-password', data);