import axios, { AxiosError } from 'axios';
import { API_BASE_URL } from '../apiConfig';

const TOKEN_KEY = 'banking.token';

const client = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
});

client.interceptors.request.use((config) => {
  const token = readToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

client.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    const isLoginCall = error.config?.url?.includes('/auth/login');
    if (error.response?.status === 401 && !isLoginCall && readToken()) {
      window.dispatchEvent(new Event('auth:expired'));
    }
    return Promise.reject(error);
  }
);

function readToken(): string | null {
  try {
    return localStorage.getItem(TOKEN_KEY);
  } catch {
    return null;
  }
}

export function getErrorMessage(error: unknown): string {
  if (axios.isAxiosError(error)) {
    if (!error.response) return 'Cannot reach the server. Please check your connection.';
    const data = error.response.data as { message?: string; title?: string } | undefined;
    return data?.message ?? data?.title ?? `Request failed (${error.response.status}).`;
  }
  return error instanceof Error ? error.message : 'Something went wrong.';
}

export { TOKEN_KEY };
export default client;
