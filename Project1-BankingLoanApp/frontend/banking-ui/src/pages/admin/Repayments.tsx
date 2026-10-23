import { useState } from 'react';
import { Link } from 'react-router-dom';
import { applicationApi } from '../../api/applicationApi';
import { useFetch } from '../../hooks/useFetch';
import PageHeader from '../../components/PageHeader';
import StatusBadge from '../../components/StatusBadge';
import { Alert, EmptyState, Loader } from '../../components/Feedback';
import { formatMoney } from '../../utils/format';

export default function Repayments() {
  const [showClosed, setShowClosed] = useState(false);
  const { data, loading, error } = useFetch(
    () => applicationApi.getAll(showClosed ? 'Closed' : 'Disbursed'), [showClosed]);

  return (
    <div className="page">
      <PageHeader title="Repayments" subtitle="Open a loan to mark installments as Paid, Due or Overdue." />

      <div className="chips">
        <button className={`chip ${!showClosed ? 'is-active' : ''}`} onClick={() => setShowClosed(false)}>Active loans</button>
        <button className={`chip ${showClosed ? 'is-active' : ''}`} onClick={() => setShowClosed(true)}>Closed loans</button>
      </div>

      {loading && <Loader />}
      <Alert message={error} />
      {data && data.length === 0 && <EmptyState title={showClosed ? 'No fully repaid loans yet.' : 'No active loans.'} />}

      {data && data.length > 0 && (
        <div className="table-wrap">
          <table className="table">
            <thead>
              <tr><th>#</th><th>Customer</th><th>Product</th><th>Principal</th><th>Tenure</th><th>Status</th><th></th></tr>
            </thead>
            <tbody>
              {data.map((a) => (
                <tr key={a.applicationId}>
                  <td>{a.applicationId}</td>
                  <td>{a.customerName}</td>
                  <td>{a.productName}</td>
                  <td>{formatMoney(a.amountRequested)}</td>
                  <td>{a.tenureMonths} mo</td>
                  <td><StatusBadge status={a.status} /></td>
                  <td><Link className="btn btn-primary btn-sm" to={`/admin/applications/${a.applicationId}`}>Schedule</Link></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
