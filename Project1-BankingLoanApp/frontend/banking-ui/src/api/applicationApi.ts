import client from './client';
import type { ApplicationStatus, ApplyLoanRequest, LoanApplication, LoanStatement, PaymentMethod, PaymentReceipt, PayoffQuote } from '../types';

export const applicationApi = {
  apply: (data: ApplyLoanRequest) =>
    client.post<LoanApplication>('/api/applications', data).then((r) => r.data),

  getMine: () => client.get<LoanApplication[]>('/api/applications/my').then((r) => r.data),

  getAll: (status?: ApplicationStatus) =>
    client.get<LoanApplication[]>('/api/applications', { params: { status } }).then((r) => r.data),

  review: (id: number, status: 'Approved' | 'Rejected', adminRemarks: string) =>
    client.put<LoanApplication>(`/api/applications/${id}/status`, { status, adminRemarks }).then((r) => r.data),

  getStatement: (id: number) =>
    client.get<LoanStatement>(`/api/applications/${id}/statement`).then((r) => r.data),

  payInstallment: (repaymentId: number, paymentMethod: PaymentMethod) =>
    client.post<PaymentReceipt>(`/api/repayments/${repaymentId}/pay`, { paymentMethod }).then((r) => r.data),

  getPayoffQuote: (applicationId: number) =>
    client.get<PayoffQuote>(`/api/repayments/application/${applicationId}/payoff-quote`).then((r) => r.data),

  payOff: (applicationId: number, paymentMethod: PaymentMethod) =>
    client.post<PaymentReceipt>(`/api/repayments/application/${applicationId}/payoff`, { paymentMethod }).then((r) => r.data),
};