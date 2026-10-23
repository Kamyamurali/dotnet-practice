import { Link } from 'react-router-dom';
import { applicationApi } from '../../api/applicationApi';
import { useFetch } from '../../hooks/useFetch';
import PageHeader from '../../components/PageHeader';
import StatusBadge from '../../components/StatusBadge';
import { Alert, EmptyState, Loader } from '../../components/Feedback';
import { formatDate, formatMoney } from '../../utils/format';

export default function MyApplications() {
  const { data, loading, error } = useFetch(() => applicationApi.getMine());

  return (
    <div className="page">
      <PageHeader title="My applications" subtitle="Track the status of every loan you've applied for."
        actions={<Link to="/products" className="btn btn-primary">New application</Link>} />

      {loading && <Loader />}
      <Alert message={error} />
      {data && data.length === 0 && (
        <EmptyState title="No applications yet.">
          <Link to="/products" className="btn btn-primary">Browse loan products</Link>
        </EmptyState>
      )}

      {data && data.length > 0 && (
        <div className="table-wrap">
          <table className="table">
            <thead>
              <tr>
                <th>#</th><th>Product</th><th>Amount</th><th>Tenure</th>
                <th>Applied</th><th>Status</th><th></th>
              </tr>
            </thead>
            <tbody>
              {data.map((a) => (
                <tr key={a.applicationId}>
                  <td>{a.applicationId}</td>
                  <td>{a.productName}</td>
                  <td>{formatMoney(a.amountRequested)}</td>
                  <td>{a.tenureMonths} mo</td>
                  <td>{formatDate(a.appliedAt)}</td>
                  <td><StatusBadge status={a.status} /></td>
                  <td><Link to={`/applications/${a.applicationId}`}>
                    {a.status === 'Disbursed' || a.status === 'Closed' ? 'Statement' : 'Details'}
                  </Link></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
