import { useState } from 'react';
import { adminApi } from '../../api/adminApi';
import { getErrorMessage } from '../../api/client';
import { useFetch } from '../../hooks/useFetch';
import PageHeader from '../../components/PageHeader';
import { Alert, EmptyState, Loader } from '../../components/Feedback';
import { formatDate, formatDateTime, formatMoney } from '../../utils/format';
import type { Transaction } from '../../types';

export default function Customers() {
  const { data, loading, error } = useFetch(() => adminApi.getCustomers());
  const [search, setSearch] = useState('');
  const [openId, setOpenId] = useState<number | null>(null);
  const [history, setHistory] = useState<Transaction[]>([]);
  const [historyError, setHistoryError] = useState<string | null>(null);

  const toggle = async (userId: number) => {
    if (openId === userId) { setOpenId(null); return; }
    setOpenId(userId);
    setHistory([]);
    setHistoryError(null);
    try {
      setHistory(await adminApi.getCustomerHistory(userId));
    } catch (err) {
      setHistoryError(getErrorMessage(err));
    }
  };

  const term = search.trim().toLowerCase();
  const visible = (data ?? []).filter(
    (c) => !term || c.fullName.toLowerCase().includes(term) || c.email.toLowerCase().includes(term));

  return (
    <div className="page">
      <PageHeader title="Customers" subtitle="Everyone registered as a member." />

      <input className="search" placeholder="Search by name or email" value={search}
        onChange={(e) => setSearch(e.target.value)} aria-label="Search customers" />

      {loading && <Loader />}
      <Alert message={error} />
      {data && visible.length === 0 && <EmptyState title="No customers match." />}

      <ul className="list list-cards">
        {visible.map((c) => (
          <li key={c.userId} className="card">
            <button className="list__row list__button" onClick={() => toggle(c.userId)} aria-expanded={openId === c.userId}>
              <div>
                <strong>{c.fullName}</strong>
                <span className="muted"> · {c.email}{c.phone ? ` · ${c.phone}` : ''}</span>
              </div>
              <div className="list__end">
                <span className="muted">Joined {formatDate(c.createdAt)}</span>
                <span className="badge badge-blue">{c.totalApplications} applications</span>
              </div>
            </button>
            {openId === c.userId && (
              <div className="list__detail">
                <Alert message={historyError} />
                {history.map((t) => (
                  <div key={t.transactionId} className="mini-row">
                    <span>{formatDateTime(t.createdAt)}</span>
                    <span><strong>{t.type}</strong> · {t.description}</span>
                    <span>{t.amount != null ? formatMoney(t.amount) : ''}</span>
                  </div>
                ))}
              </div>
            )}
          </li>
        ))}
      </ul>
    </div>
  );
}
