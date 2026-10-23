import type { ReactNode } from 'react';


export function Loader({ text = 'Loading…' }: { text?: string }) {
  return (
    <div className="loader" role="status">
      <span className="loader__spinner" aria-hidden="true" />
      {text}
    </div>
  );
}

export function Alert({ message, type = 'error' }: { message: string | null; type?: 'error' | 'success' }) {
  if (!message) return null;
  return (
    <div className={`alert alert-${type}`} role={type === 'error' ? 'alert' : 'status'}>
      {message}
    </div>
  );
}

export function EmptyState({ title, children }: { title: string; children?: ReactNode }) {
  return (
    <div className="empty">
      <p className="empty__title">{title}</p>
      {children}
    </div>
  );
}
