import { Link } from 'react-router-dom';
import { historyApi } from '../../api/historyApi';
import { applicationApi } from '../../api/applicationApi';
import { useFetch } from '../../hooks/useFetch';
import { useAppSelector } from '../../store/hooks';
import PageHeader from '../../components/PageHeader';
import StatCard from '../../components/StatCard';
import StatusBadge from '../../components/StatusBadge';
import { Alert, EmptyState, Loader } from '../../components/Feedback';
import { formatDate, formatMoney } from '../../utils/format';

export default function Dashboard() {
  const user = useAppSelector((state) => state.auth.user);
  const summary = useFetch(() => historyApi.getMySummary());
  const applications = useFetch(() => applicationApi.getMine());

  const recent = (applications.data ?? []).slice(0, 4);
  const s = summary.data;

  return (
    <div className="page">
      <PageHeader title={`Hello, ${user?.fullName.split(' ')[0]}`}
        subtitle="Here's where your loans stand today."
        actions={<Link to="/products" className="btn btn-primary">Apply for a loan</Link>} />

      {summary.loading && <Loader />}
      <Alert message={summary.error} />
      {s && (
        <div className="stats">
          <StatCard label="Applications" value={s.totalApplications} />
          <StatCard label="Pending review" value={s.pendingApplications} />
          <StatCard label="Active loans" value={s.activeLoans} tone="good" />
          <StatCard label="Outstanding" value={formatMoney(s.outstandingAmount)} />
          <StatCard label="Overdue installments" value={s.overdueInstallments}
            tone={s.overdueInstallments > 0 ? 'warn' : 'default'} />
        </div>
      )}

      <section className="card">
        <div className="card__head">
          <h2 className="card__title">Recent applications</h2>
          <Link to="/applications">View all</Link>
        </div>
        {applications.loading && <Loader />}
        <Alert message={applications.error} />
        {!applications.loading && recent.length === 0 && (
          <EmptyState title="You haven't applied for a loan yet.">
            <Link to="/products" className="btn btn-primary">Browse loan products</Link>
          </EmptyState>
        )}
        {recent.length > 0 && (
          <ul className="list">
            {recent.map((a) => (
              <li key={a.applicationId}>
                <Link to={`/applications/${a.applicationId}`} className="list__row">
                  <div>
                    <strong>{a.productName}</strong>
                    <span className="muted"> · {formatDate(a.appliedAt)}</span>
                  </div>
                  <div className="list__end">
                    <span>{formatMoney(a.amountRequested)}</span>
                    <StatusBadge status={a.status} />
                  </div>
                </Link>
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  );
}
