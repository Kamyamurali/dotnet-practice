interface Props {
  label: string;
  value: string | number;
  tone?: 'default' | 'warn' | 'good';
}

export default function StatCard({ label, value, tone = 'default' }: Props) {
  return (
    <div className={`stat stat-${tone}`}>
      <span className="stat__label">{label}</span>
      <span className="stat__value">{value}</span>
    </div>
  );
}
