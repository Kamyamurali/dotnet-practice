import client from './client';
import type { LoanProduct, ProductRequest } from '../types';

export const productApi = {
  getAll: (includeInactive = false) =>
    client.get<LoanProduct[]>('/api/products', { params: { includeInactive } }).then((r) => r.data),

  getById: (id: number) => client.get<LoanProduct>(`/api/products/${id}`).then((r) => r.data),

  add: (data: ProductRequest) => client.post<LoanProduct>('/api/products', data).then((r) => r.data),

  update: (id: number, data: ProductRequest) =>
    client.put<LoanProduct>(`/api/products/${id}`, data).then((r) => r.data),

  remove: (id: number) =>
    client.delete<{ productId: number; result: string }>(`/api/products/${id}`).then((r) => r.data),
};
