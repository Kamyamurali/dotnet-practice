import client from './client';
import type { CustomerSummary, Transaction } from '../types';

export const historyApi = {
  getMyTransactions: (type?: string) =>
    client.get<Transaction[]>('/api/transactions/my', { params: { type } }).then((r) => r.data),

  getMySummary: () => client.get<CustomerSummary>('/api/dashboard/customer').then((r) => r.data),
};
