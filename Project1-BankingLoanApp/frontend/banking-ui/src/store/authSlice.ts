import { createSlice, type PayloadAction } from '@reduxjs/toolkit';
import type { AuthResponse, User } from '../types';

export interface AuthState {
  user: User | null;
  token: string | null;
  expiresAt: string | null;
}

export const emptyAuthState: AuthState = { user: null, token: null, expiresAt: null };

const authSlice = createSlice({
  name: 'auth',
  initialState: emptyAuthState,
  reducers: {
    loginSuccess(_state, action: PayloadAction<AuthResponse>) {
      return {
        user: action.payload.user,
        token: action.payload.token,
        expiresAt: action.payload.expiresAt,
      };
    },
    logout() {
      return emptyAuthState;
    },
  },
});

export const { loginSuccess, logout } = authSlice.actions;
export default authSlice.reducer;
