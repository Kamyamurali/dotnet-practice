import client from './client';
import type { AuthResponse, User } from '../types';

export interface RegisterPayload {
  fullName: string;
  email: string;
  phone?: string;
  password: string;
}

export const authApi = {
  register: (data: RegisterPayload) =>
    client.post<AuthResponse>('/api/auth/register', data).then((r) => r.data),

  login: (email: string, password: string) =>
    client.post<AuthResponse>('/api/auth/login', { email, password }).then((r) => r.data),

  me: () => client.get<User>('/api/auth/me').then((r) => r.data),
};
