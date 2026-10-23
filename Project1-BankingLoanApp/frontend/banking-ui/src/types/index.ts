// TypeScript shapes of the JSON the Web API sends back

export type Role = 'Customer' | 'Admin';
export type ApplicationStatus = 'Pending' | 'Approved' | 'Rejected' | 'Disbursed' | 'Closed';
export type RepaymentStatus = 'Due' | 'Paid' | 'Overdue';

export interface User {
  userId: number;
  fullName: string;
  email: string;
  phone?: string | null;
  role: Role;
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
  user: User;
}

export interface LoanProduct {
  productId: number;
  name: string;
  category: string;
  description?: string | null;
  interestRate: number;
  minAmount: number;
  maxAmount: number;
  maxTenureMonths: number;
  isActive: boolean;
  createdAt: string;
}

export type ProductRequest = Omit<LoanProduct, 'productId' | 'createdAt'>;

export interface LoanApplication {
  applicationId: number;
  userId: number;
  customerName: string;
  customerEmail: string;
  productId: number;
  productName: string;
  category: string;
  amountRequested: number;
  tenureMonths: number;
  interestRate: number;
  purpose: string;
  applicantDateOfBirth?: string | null;
  monthlyIncome?: number | null;
  status: ApplicationStatus;
  adminRemarks?: string | null;
  appliedAt: string;
  reviewedAt?: string | null;
}

export interface ApplyLoanRequest {
  productId: number;
  amountRequested: number;
  tenureMonths: number;
  purpose: string;
  dateOfBirth: string;      
  monthlyIncome: number;
}

export interface Disbursement {
  disbursementId: number;
  applicationId: number;
  amount: number;
  reference: string;
  disbursedAt: string;
  customerName: string;
  productName: string;
}

export interface Repayment {
  repaymentId: number;
  applicationId: number;
  installmentNo: number;
  dueDate: string;
  amountDue: number;
  amountPaid: number;
  paidAt?: string | null;
  status: RepaymentStatus;
  paymentId?: number | null;   
}

export interface PaymentReceipt {
  paymentId: number;
  transactionId: string;
  applicationId: number;
  productName: string;
  customerName: string;
  amount: number;
  method: string;              
  type: 'Installment' | 'EarlyPayoff';
  status: 'Success' | 'Reversed';
  description: string;
  paidAt: string;
  installmentNos: number[];
}

export interface PayoffQuote {
  applicationId: number;
  installmentsRemaining: number;
  scheduledRemaining: number;
  payoffAmount: number;
  interestSaved: number;
}

export type PaymentMethod = 'Card' | 'UPI' | 'NetBanking';

export interface LoanStatement {
  application: LoanApplication;
  disbursement?: Disbursement | null;
  repayments: Repayment[];
  totalPayable: number;
  totalPaid: number;
  outstanding: number;
  installmentsPaid: number;
  nextDue?: Repayment | null;
  payments: PaymentReceipt[];
}

export interface Transaction {
  transactionId: number;
  userId: number;
  applicationId?: number | null;
  type: string;
  amount?: number | null;
  description: string;
  createdAt: string;
}

export interface CustomerSummary {
  totalApplications: number;
  pendingApplications: number;
  activeLoans: number;
  outstandingAmount: number;
  overdueInstallments: number;
}

export interface AdminSummary {
  totalCustomers: number;
  activeProducts: number;
  pendingApplications: number;
  awaitingDisbursement: number;
  totalDisbursed: number;
  totalRepaid: number;
  overdueInstallments: number;
}

export interface CustomerInfo {
  userId: number;
  fullName: string;
  email: string;
  phone?: string | null;
  createdAt: string;
  totalApplications: number;
}