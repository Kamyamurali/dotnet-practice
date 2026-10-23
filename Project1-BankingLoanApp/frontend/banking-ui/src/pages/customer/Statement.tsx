import { useState } from 'react';
import { Link, useLocation, useParams } from 'react-router-dom';
import { applicationApi } from '../../api/applicationApi';
import { adminApi } from '../../api/adminApi';
import { getErrorMessage } from '../../api/client';
import { useFetch } from '../../hooks/useFetch';
import { useAppSelector } from '../../store/hooks';
import PageHeader from '../../components/PageHeader';
import StatusBadge from '../../components/StatusBadge';
import StatCard from '../../components/StatCard';
import { Alert, Loader } from '../../components/Feedback';
import { formatDate, formatDateTime, formatMoney } from '../../utils/format';
import type { PaymentMethod, PaymentReceipt, PayoffQuote, Repayment, RepaymentStatus } from '../../types';
import ReceiptModal from '../../components/ReceiptModal';

export default function Statement() {
  const { id } = useParams();
  const location = useLocation();
  const isAdmin = useAppSelector((state) => state.auth.user?.role === 'Admin');
  const { data, loading, error, reload } = useFetch(() => applicationApi.getStatement(Number(id)), [id]);

  const [actionError, setActionError] = useState<string | null>(null);
  const [busyId, setBusyId] = useState<number | null>(null);
  const justApplied = (location.state as { justApplied?: boolean } | null)?.justApplied;

  const [paying, setPaying] = useState<Repayment | null>(null);
  const [method, setMethod] = useState<PaymentMethod>('UPI');
  const [payBusy, setPayBusy] = useState(false);
  const [payMessage, setPayMessage] = useState<string | null>(null);
  const [receipt, setReceipt] = useState<PaymentReceipt | null>(null);      
  const [quote, setQuote] = useState<PayoffQuote | null>(null);            
  const [quoteBusy, setQuoteBusy] = useState(false);

  const methodLabel = (m: string) => (m === 'NetBanking' ? 'Net Banking' : m);

  const confirmPayment = async () => {
    if (!paying) return;
    setPayBusy(true);
    setActionError(null);
    try {
      const result = await applicationApi.payInstallment(paying.repaymentId, method);
      setPayMessage(`Payment successful: installment ${paying.installmentNo} paid via ${methodLabel(method)}. Transaction ID ${result.transactionId}.`);
      setPaying(null);
      setReceipt(result);         
      await reload();
    } catch (err) {
      setActionError(getErrorMessage(err));
      setPaying(null);
    } finally {
      setPayBusy(false);
    }
  };

  const updateStatus = async (repaymentId: number, status: RepaymentStatus) => {
    setBusyId(repaymentId);
    setActionError(null);
    try {
      await adminApi.updateRepaymentStatus(repaymentId, status);
      await reload();
    } catch (err) {
      setActionError(getErrorMessage(err));
    } finally {
      setBusyId(null);
    }
  };

  const openPayoff = async () => {
    setQuoteBusy(true);
    setActionError(null);
    setPayMessage(null);
    try {
      setQuote(await applicationApi.getPayoffQuote(Number(id)));
      setMethod('UPI');
    } catch (err) {
      setActionError(getErrorMessage(err));
    } finally {
      setQuoteBusy(false);
    }
  };

  const confirmPayoff = async () => {
    if (!quote) return;
    setPayBusy(true);
    setActionError(null);
    try {
      const result = await applicationApi.payOff(quote.applicationId, method);
      setPayMessage(`Loan repaid early. Transaction ID ${result.transactionId}.`);
      setQuote(null);
      setReceipt(result);
      await reload();
    } catch (err) {
      setActionError(getErrorMessage(err));
      setQuote(null);
    } finally {
      setPayBusy(false);
    }
  };

  if (loading && !data) return <div className="page"><Loader /></div>;
  if (error || !data) return <div className="page"><Alert message={error ?? 'Not found.'} /></div>;

  const { application: a, disbursement: d, repayments } = data;
  const backLink = isAdmin ? '/admin/applications' : '/applications';
  const canPay = !isAdmin && a.status === 'Disbursed' && !!data.nextDue;

  return (
    <div className="page">
      <PageHeader title={`${a.productName} · #${a.applicationId}`}
        subtitle={isAdmin ? `${a.customerName} (${a.customerEmail})` : `Applied on ${formatDate(a.appliedAt)}`}
        actions={<>
          <button className="btn btn-ghost" onClick={() => window.print()}>Print</button>
          <Link to={backLink} className="btn btn-ghost">Back</Link>
        </>} />

      {justApplied && <Alert type="success" message="Application submitted. We'll review it shortly." />}
      <Alert message={actionError} />
      <Alert type="success" message={payMessage} />

      <div className="split">
        <section className="card">
          <div className="card__head">
            <h2 className="card__title">Application</h2>
            <StatusBadge status={a.status} />
          </div>
          <dl className="facts">
            <div><dt>Amount requested</dt><dd>{formatMoney(a.amountRequested)}</dd></div>
            <div><dt>Tenure</dt><dd>{a.tenureMonths} months</dd></div>
            <div><dt>Interest rate</dt><dd>{a.interestRate.toFixed(2)}% p.a.</dd></div>
            <div><dt>Purpose</dt><dd>{a.purpose}</dd></div>
            <div><dt>Applied</dt><dd>{formatDateTime(a.appliedAt)}</dd></div>
            <div><dt>Reviewed</dt><dd>{formatDateTime(a.reviewedAt)}</dd></div>
            {a.adminRemarks && <div><dt>Bank remarks</dt><dd>{a.adminRemarks}</dd></div>}
          </dl>
        </section>

        <section className="card card-accent">
          <h2 className="card__title">Disbursement</h2>
          {d ? (
            <dl className="facts">
              <div><dt>Reference</dt><dd>{d.reference}</dd></div>
              <div><dt>Amount released</dt><dd>{formatMoney(d.amount)}</dd></div>
              <div><dt>Date</dt><dd>{formatDateTime(d.disbursedAt)}</dd></div>
            </dl>
          ) : (
            <p className="muted">
              {a.status === 'Pending' && 'Your application is waiting for review.'}
              {a.status === 'Approved' && 'Approved. The funds will be released soon.'}
              {a.status === 'Rejected' && 'This application was not approved.'}
            </p>
          )}
        </section>
      </div>

      {repayments.length > 0 && (
        <>
          <div className="stats">
            <StatCard label="Total payable" value={formatMoney(data.totalPayable)} />
            <StatCard label="Paid so far" value={formatMoney(data.totalPaid)} tone="good" />
            <StatCard label="Outstanding" value={formatMoney(data.outstanding)} />
            <StatCard label="Installments paid" value={`${data.installmentsPaid} / ${repayments.length}`} />
            <StatCard label="Next due" value={data.nextDue ? formatDate(data.nextDue.dueDate) : 'All paid'} />
          </div>

          {canPay && data.nextDue && (
            <section className="card card-accent pay-banner">
              <div>
                <h2 className="card__title">Next installment</h2>
                <p className="muted">
                  Installment {data.nextDue.installmentNo} · {formatMoney(data.nextDue.amountDue)} · due {formatDate(data.nextDue.dueDate)}
                </p>
              </div>
              <div className="pay-banner__actions">
                <button className="btn btn-gold" onClick={() => { setPaying(data.nextDue!); setMethod('UPI'); setPayMessage(null); }}>
                  Pay now
                </button>
                <button className="btn btn-ghost" onClick={openPayoff} disabled={quoteBusy}>
                  {quoteBusy ? 'Calculating…' : 'Repay loan early'}
                </button>
              </div>
            </section>
          )}

          <section className="card">
            <h2 className="card__title">Repayment schedule</h2>
            <div className="table-wrap">
              <table className="table">
                <thead>
                  <tr>
                    <th>No.</th><th>Due date</th><th>Amount due</th><th>Paid</th>
                    <th>Paid on</th><th>Status</th><th>Receipt</th>{isAdmin && <th>Update</th>}{canPay && <th>Pay</th>}
                  </tr>
                </thead>
                <tbody>
                  {repayments.map((r) => (
                    <tr key={r.repaymentId}>
                      <td>{r.installmentNo}</td>
                      <td>{formatDate(r.dueDate)}</td>
                      <td>{formatMoney(r.amountDue)}</td>
                      <td>{formatMoney(r.amountPaid)}</td>
                      <td>{formatDate(r.paidAt)}</td>
                      <td><StatusBadge status={r.status} /></td>
                      <td>
                        {(() => {
                          const p = r.paymentId ? data.payments.find((x) => x.paymentId === r.paymentId) : undefined;
                          return p
                            ? <button className="btn btn-ghost btn-sm" onClick={() => setReceipt(p)}>{p.transactionId}</button>
                            : <span className="muted">—</span>;
                        })()}
                      </td>
                      {isAdmin && (
                        <td>
                          <select aria-label={`Status for installment ${r.installmentNo}`}
                            value={r.status} disabled={busyId === r.repaymentId}
                            onChange={(e) => updateStatus(r.repaymentId, e.target.value as RepaymentStatus)}>
                            <option value="Due">Due</option>
                            <option value="Paid">Paid</option>
                            <option value="Overdue">Overdue</option>
                          </select>
                        </td>
                      )}
                      {canPay && (
                        <td>
                          {data.nextDue?.repaymentId === r.repaymentId && (
                            <button className="btn btn-primary btn-sm"
                              onClick={() => { setPaying(r); setMethod('UPI'); setPayMessage(null); }}>
                              Pay
                            </button>
                          )}
                        </td>
                      )}
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </section>
          {data.payments.length > 0 && (
            <section className="card">
              <h2 className="card__title">Payment receipts</h2>
              <div className="table-wrap">
                <table className="table">
                  <thead>
                    <tr><th>Transaction ID</th><th>Date</th><th>Type</th><th>Installments</th><th>Method</th><th>Amount</th><th>Status</th><th></th></tr>
                  </thead>
                  <tbody>
                    {data.payments.map((p) => (
                      <tr key={p.paymentId}>
                        <td><strong>{p.transactionId}</strong></td>
                        <td>{formatDateTime(p.paidAt)}</td>
                        <td>{p.type === 'EarlyPayoff' ? 'Early repayment' : 'Installment'}</td>
                        <td>{p.installmentNos.length > 1
                          ? `${p.installmentNos[0]}–${p.installmentNos[p.installmentNos.length - 1]}`
                          : p.installmentNos[0] ?? '—'}</td>
                        <td>{methodLabel(p.method)}</td>
                        <td>{formatMoney(p.amount)}</td>
                        <td><StatusBadge status={p.status} /></td>
                        <td><button className="btn btn-ghost btn-sm" onClick={() => setReceipt(p)}>View</button></td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </section>
          )}
        </>
      )}

      {quote && (
        <div className="modal" role="dialog" aria-modal="true" aria-labelledby="payoff-title">
          <div className="modal__box">
            <h2 id="payoff-title">Repay loan early</h2>
            <p className="muted">Pay the remaining principal today and close the loan. Future interest is not charged.</p>
            <dl className="facts">
              <div><dt>Installments remaining</dt><dd>{quote.installmentsRemaining}</dd></div>
              <div><dt>If you keep paying monthly</dt><dd>{formatMoney(quote.scheduledRemaining)}</dd></div>
              <div><dt>Pay today to close</dt><dd><strong>{formatMoney(quote.payoffAmount)}</strong></dd></div>
              <div><dt>You save</dt><dd className="text-good">{formatMoney(quote.interestSaved)}</dd></div>
            </dl>
            <label className="field">
              <span>Payment method</span>
              <select value={method} onChange={(e) => setMethod(e.target.value as PaymentMethod)}>
                <option value="UPI">UPI</option>
                <option value="Card">Debit / credit card</option>
                <option value="NetBanking">Net Banking</option>
              </select>
              <small>Demo payment: no real money is charged and no card details are stored.</small>
            </label>
            <div className="modal__actions">
              <button className="btn btn-ghost" onClick={() => setQuote(null)} disabled={payBusy}>Cancel</button>
              <button className="btn btn-gold" onClick={confirmPayoff} disabled={payBusy}>
                {payBusy ? 'Processing…' : `Pay ${formatMoney(quote.payoffAmount)} and close loan`}
              </button>
            </div>
          </div>
        </div>
      )}

      {receipt && <ReceiptModal receipt={receipt} onClose={() => setReceipt(null)} />}

      {paying && (
        <div className="modal" role="dialog" aria-modal="true" aria-labelledby="pay-title">
          <div className="modal__box">
            <h2 id="pay-title">Pay installment {paying.installmentNo}</h2>
            <dl className="facts">
              <div><dt>Loan</dt><dd>{a.productName} · #{a.applicationId}</dd></div>
              <div><dt>Amount</dt><dd>{formatMoney(paying.amountDue)}</dd></div>
              <div><dt>Due date</dt><dd>{formatDate(paying.dueDate)}</dd></div>
            </dl>
            <label className="field">
              <span>Payment method</span>
              <select value={method} onChange={(e) => setMethod(e.target.value as PaymentMethod)}>
                <option value="UPI">UPI</option>
                <option value="Card">Debit / credit card</option>
                <option value="NetBanking">Net Banking</option>
              </select>
              <small>Demo payment: no real money is charged and no card details are stored.</small>
            </label>
            <div className="modal__actions">
              <button className="btn btn-ghost" onClick={() => setPaying(null)} disabled={payBusy}>Cancel</button>
              <button className="btn btn-gold" onClick={confirmPayment} disabled={payBusy}>
                {payBusy ? 'Processing…' : `Pay ${formatMoney(paying.amountDue)}`}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}