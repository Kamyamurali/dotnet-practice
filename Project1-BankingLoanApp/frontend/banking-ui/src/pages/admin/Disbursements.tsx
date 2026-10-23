import { Link } from 'react-router-dom';
import { adminApi } from '../../api/adminApi';
import { useFetch } from '../../hooks/useFetch';
import PageHeader from '../../components/PageHeader';
import { Alert, EmptyState, Loader } from '../../components/Feedback';
import { formatDateTime, formatMoney } from '../../utils/format';

export default function Disbursements() {
  const { data, loading, error } = useFetch(() => adminApi.getDisbursements());
  const total = (data ?? []).reduce((sum, d) => sum + d.amount, 0);

  return (
    <div className="page">
      <PageHeader title="Disbursements" subtitle={data ? `${data.length} loans · ${formatMoney(total)} released` : undefined}
        actions={<Link to="/admin/applications" className="btn btn-ghost">Approved applications</Link>} />

      {loading && <Loader />}
      <Alert message={error} />
      {data && data.length === 0 && <EmptyState title="No funds have been released yet." />}

      {data && data.length > 0 && (
        <div className="table-wrap">
          <table className="table">
            <thead><tr><th>Reference</th><th>Application</th><th>Customer</th><th>Product</th><th>Amount</th><th>Date</th></tr></thead>
            <tbody>
              {data.map((d) => (
                <tr key={d.disbursementId}>
                  <td><code>{d.reference}</code></td>
                  <td><Link to={`/admin/applications/${d.applicationId}`}>#{d.applicationId}</Link></td>
                  <td>{d.customerName}</td>
                  <td>{d.productName}</td>
                  <td>{formatMoney(d.amount)}</td>
                  <td>{formatDateTime(d.disbursedAt)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
