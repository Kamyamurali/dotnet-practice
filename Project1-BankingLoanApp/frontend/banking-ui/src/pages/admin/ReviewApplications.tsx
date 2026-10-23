import { useState } from 'react';
import { ageOn } from '../../utils/eligibility';
import { Link } from 'react-router-dom';
import { applicationApi } from '../../api/applicationApi';
import { adminApi } from '../../api/adminApi';
import { getErrorMessage } from '../../api/client';
import { useFetch } from '../../hooks/useFetch';
import PageHeader from '../../components/PageHeader';
import StatusBadge from '../../components/StatusBadge';
import { Alert, EmptyState, Loader } from '../../components/Feedback';
import { formatDate, formatMoney } from '../../utils/format';
import type { ApplicationStatus, LoanApplication } from '../../types';

const FILTERS: (ApplicationStatus | 'All')[] = ['Pending', 'Approved', 'Disbursed', 'Rejected', 'Closed', 'All'];

export default function ReviewApplications() {
  const [filter, setFilter] = useState<ApplicationStatus | 'All'>('Pending');
  const { data, loading, error, reload } = useFetch(
    () => applicationApi.getAll(filter === 'All' ? undefined : filter), [filter]);

  const [reviewing, setReviewing] = useState<LoanApplication | null>(null);
  const [remarks, setRemarks] = useState('');
  const [busy, setBusy] = useState(false);
  const [message, setMessage] = useState<{ type: 'error' | 'success'; text: string } | null>(null);

  const run = async (action: () => Promise<unknown>, success: string) => {
    setBusy(true);
    setMessage(null);
    try {
      await action();
      setMessage({ type: 'success', text: success });
      setReviewing(null);
      setRemarks('');
      await reload();
    } catch (err) {
      setMessage({ type: 'error', text: getErrorMessage(err) });
    } finally {
      setBusy(false);
    }
  };

  const review = (status: 'Approved' | 'Rejected') =>
    reviewing && run(() => applicationApi.review(reviewing.applicationId, status, remarks.trim()),
      `Application #${reviewing.applicationId} ${status.toLowerCase()}.`);

  const disburse = (a: LoanApplication) => {
    if (!window.confirm(`Release ${formatMoney(a.amountRequested)} to ${a.customerName}?`)) return;
    void run(() => adminApi.disburse(a.applicationId),
      `Funds released for application #${a.applicationId}. Repayment schedule created.`);
  };

  return (
    <div className="page">
      <PageHeader title="Loan applications" subtitle="Review, approve or reject, then release funds." />

      <div className="chips">
        {FILTERS.map((f) => (
          <button key={f} className={`chip ${filter === f ? 'is-active' : ''}`} onClick={() => setFilter(f)}>{f}</button>
        ))}
      </div>

      <Alert message={message?.text ?? null} type={message?.type} />
      {loading && <Loader />}
      <Alert message={error} />
      {data && data.length === 0 && <EmptyState title={`No ${filter === 'All' ? '' : filter.toLowerCase()} applications.`} />}

      {data && data.length > 0 && (
        <div className="table-wrap">
          <table className="table">
            <thead>
              <tr>
                <th>#</th><th>Customer</th><th>Product</th><th>Amount</th><th>Tenure</th>
                <th>Purpose</th><th>Applied</th><th>Status</th><th>Action</th>
              </tr>
            </thead>
            <tbody>
              {data.map((a) => (
                <tr key={a.applicationId}>
                  <td>{a.applicationId}</td>
                  <td>{a.customerName}<div className="muted small">{a.customerEmail}</div></td>
                  <td>{a.productName}</td>
                  <td>{formatMoney(a.amountRequested)}</td>
                  <td>{a.tenureMonths} mo</td>
                  <td className="cell-wrap">{a.purpose}</td>
                  <td>{formatDate(a.appliedAt)}</td>
                  <td><StatusBadge status={a.status} /></td>
                  <td className="actions">
                    {a.status === 'Pending' && (
                      <button className="btn btn-primary btn-sm" onClick={() => { setReviewing(a); setRemarks(''); }}>Review</button>
                    )}
                    {a.status === 'Approved' && (
                      <button className="btn btn-gold btn-sm" disabled={busy} onClick={() => disburse(a)}>Disburse</button>
                    )}
                    <Link className="btn btn-ghost btn-sm" to={`/admin/applications/${a.applicationId}`}>Open</Link>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {reviewing && (
        <div className="modal" role="dialog" aria-modal="true" aria-labelledby="review-title">
          <div className="modal__box">
            <h2 id="review-title">Review application #{reviewing.applicationId}</h2>
            <dl className="facts">
              <div><dt>Customer</dt><dd>{reviewing.customerName}</dd></div>
              <div><dt>Product</dt><dd>{reviewing.productName}</dd></div>
              <div><dt>Amount</dt><dd>{formatMoney(reviewing.amountRequested)} over {reviewing.tenureMonths} months</dd></div>
              <div><dt>Purpose</dt><dd>{reviewing.purpose}</dd></div>
              <div><dt>Monthly income</dt><dd>{reviewing.monthlyIncome != null ? formatMoney(reviewing.monthlyIncome) : '—'}</dd></div>
              <div><dt>Age</dt><dd>{reviewing.applicantDateOfBirth ? `${ageOn(reviewing.applicantDateOfBirth.slice(0, 10))} years` : '—'}</dd></div>
            </dl>
            <label className="field">
              <span>Remarks for the customer</span>
              <textarea rows={3} maxLength={300} value={remarks} onChange={(e) => setRemarks(e.target.value)} />
            </label>
            <div className="modal__actions">
              <button className="btn btn-ghost" onClick={() => setReviewing(null)} disabled={busy}>Cancel</button>
              <button className="btn btn-danger" onClick={() => review('Rejected')} disabled={busy}>Reject</button>
              <button className="btn btn-primary" onClick={() => review('Approved')} disabled={busy}>Approve</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}