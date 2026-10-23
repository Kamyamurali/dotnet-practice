import client from './client';
import type { AdminSummary, CustomerInfo, Disbursement, Repayment, RepaymentStatus, Transaction } from '../types';

export const adminApi = {
  getSummary: () => client.get<AdminSummary>('/api/dashboard/admin').then((r) => r.data),

  getCustomers: () => client.get<CustomerInfo[]>('/api/customers').then((r) => r.data),

  getCustomerHistory: (userId: number) =>
    client.get<Transaction[]>(`/api/customers/${userId}/transactions`).then((r) => r.data),

  getDisbursements: () => client.get<Disbursement[]>('/api/disbursements').then((r) => r.data),

  disburse: (applicationId: number) =>
    client.post<Disbursement>(`/api/disbursements/${applicationId}`).then((r) => r.data),

  updateRepaymentStatus: (repaymentId: number, status: RepaymentStatus) =>
    client.put<Repayment>(`/api/repayments/${repaymentId}/status`, { status }).then((r) => r.data),
};
