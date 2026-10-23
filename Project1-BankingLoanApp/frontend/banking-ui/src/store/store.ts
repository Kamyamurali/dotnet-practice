import { configureStore } from '@reduxjs/toolkit';
import authReducer, { emptyAuthState, type AuthState } from './authSlice';
import { TOKEN_KEY } from '../api/client';

const AUTH_KEY = 'banking.auth';

function loadAuth(): AuthState {
  try {
    const saved = localStorage.getItem(AUTH_KEY);
    if (!saved) return emptyAuthState;
    const auth = JSON.parse(saved) as AuthState;
    if (!auth.expiresAt || new Date(auth.expiresAt) <= new Date()) return emptyAuthState;
    return auth;
  } catch {
    return emptyAuthState;
  }
}

export const store = configureStore({
  reducer: { auth: authReducer },
  preloadedState: { auth: loadAuth() },
});

store.subscribe(() => {
  const { auth } = store.getState();
  try {
    if (auth.token) {
      localStorage.setItem(AUTH_KEY, JSON.stringify(auth));
      localStorage.setItem(TOKEN_KEY, auth.token);
    } else {
      localStorage.removeItem(AUTH_KEY);
      localStorage.removeItem(TOKEN_KEY);
    }
  } catch {
  }
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
