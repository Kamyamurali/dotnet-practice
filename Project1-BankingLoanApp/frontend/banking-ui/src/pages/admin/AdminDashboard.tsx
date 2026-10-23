import { Link } from 'react-router-dom';
import { adminApi } from '../../api/adminApi';
import { applicationApi } from '../../api/applicationApi';
import { useFetch } from '../../hooks/useFetch';
import PageHeader from '../../components/PageHeader';
import StatCard from '../../components/StatCard';
import { Alert, EmptyState, Loader } from '../../components/Feedback';
import { formatDate, formatMoney } from '../../utils/format';

export default function AdminDashboard() {
  const summary = useFetch(() => adminApi.getSummary());
  const pending = useFetch(() => applicationApi.getAll('Pending'));
  const s = summary.data;

  return (
    <div className="page">
      <PageHeader title="Admin overview" subtitle="Applications waiting on you, money out, and money back in." />

      {summary.loading && <Loader />}
      <Alert message={summary.error} />
      {s && (
        <div className="stats">
          <StatCard label="Customers" value={s.totalCustomers} />
          <StatCard label="Active products" value={s.activeProducts} />
          <StatCard label="Pending review" value={s.pendingApplications} tone={s.pendingApplications ? 'warn' : 'default'} />
          <StatCard label="Awaiting disbursement" value={s.awaitingDisbursement} />
          <StatCard label="Total disbursed" value={formatMoney(s.totalDisbursed)} />
          <StatCard label="Total repaid" value={formatMoney(s.totalRepaid)} tone="good" />
          <StatCard label="Overdue installments" value={s.overdueInstallments} tone={s.overdueInstallments ? 'warn' : 'default'} />
        </div>
      )}

      <section className="card">
        <div className="card__head">
          <h2 className="card__title">Needs review</h2>
          <Link to="/admin/applications">All applications</Link>
        </div>
        {pending.loading && <Loader />}
        <Alert message={pending.error} />
        {pending.data && pending.data.length === 0 && <EmptyState title="No pending applications. All caught up." />}
        {pending.data && pending.data.length > 0 && (
          <ul className="list">
            {pending.data.slice(0, 6).map((a) => (
              <li key={a.applicationId}>
                <Link to="/admin/applications" className="list__row">
                  <div>
                    <strong>{a.customerName}</strong>
                    <span className="muted"> · {a.productName} · {formatDate(a.appliedAt)}</span>
                  </div>
                  <span>{formatMoney(a.amountRequested)}</span>
                </Link>
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  );
}
