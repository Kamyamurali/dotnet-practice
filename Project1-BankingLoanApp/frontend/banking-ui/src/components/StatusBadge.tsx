const STYLES: Record<string, string> = {
  Pending: 'badge-amber',
  Approved: 'badge-blue',
  Rejected: 'badge-red',
  Disbursed: 'badge-green',
  Closed: 'badge-grey',
  Due: 'badge-amber',
  Paid: 'badge-green',
  Overdue: 'badge-red',
  Success: 'badge-green',
  Reversed: 'badge-red',
};

interface Props {
  status: string;
}

export default function StatusBadge({ status }: Props) {
  return <span className={`badge ${STYLES[status] ?? 'badge-grey'}`}>{status}</span>;
}