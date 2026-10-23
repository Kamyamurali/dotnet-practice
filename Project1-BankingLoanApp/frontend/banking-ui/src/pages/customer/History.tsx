import { useState } from 'react';
import { historyApi } from '../../api/historyApi';
import { useFetch } from '../../hooks/useFetch';
import PageHeader from '../../components/PageHeader';
import { Alert, EmptyState, Loader } from '../../components/Feedback';
import { formatDateTime, formatMoney } from '../../utils/format';

const TYPES = ['All', 'Registration', 'Application', 'Approval', 'Rejection', 'Disbursement', 'Repayment'];

export default function History() {
  const [type, setType] = useState('All');
  const { data, loading, error } = useFetch(
    () => historyApi.getMyTransactions(type === 'All' ? undefined : type), [type]);

  return (
    <div className="page">
      <PageHeader title="Transaction history" subtitle="Everything that has happened on your account since you registered." />

      <div className="chips">
        {TYPES.map((t) => (
          <button key={t} className={`chip ${type === t ? 'is-active' : ''}`} onClick={() => setType(t)}>{t}</button>
        ))}
      </div>

      {loading && <Loader />}
      <Alert message={error} />
      {data && data.length === 0 && <EmptyState title="Nothing to show for this filter." />}

      {data && data.length > 0 && (
        <ol className="timeline">
          {data.map((t) => (
            <li key={t.transactionId} className={`timeline__item type-${t.type.toLowerCase()}`}>
              <div className="timeline__main">
                <strong>{t.type}</strong>
                <span>{t.description}</span>
              </div>
              <div className="timeline__end">
                {t.amount != null && <strong>{formatMoney(t.amount)}</strong>}
                <span className="muted">{formatDateTime(t.createdAt)}</span>
              </div>
            </li>
          ))}
        </ol>
      )}
    </div>
  );
}
