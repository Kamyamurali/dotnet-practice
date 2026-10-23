
const currency = new Intl.NumberFormat('en-US', { style: 'currency', currency: 'USD', maximumFractionDigits: 2 });

export const formatMoney = (value: number | null | undefined): string =>
  value === null || value === undefined ? '—' : currency.format(value);

export const formatDate = (value: string | null | undefined): string =>
  value ? new Date(value).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' }) : '—';

export const formatDateTime = (value: string | null | undefined): string =>
  value
    ? new Date(value).toLocaleString('en-US', { year: 'numeric', month: 'short', day: 'numeric', hour: 'numeric', minute: '2-digit' })
    : '—';
