import { render, screen } from '@testing-library/react';
import StatusBadge from '../components/StatusBadge';

describe('StatusBadge', () => {
  it('shows the status text', () => {
    render(<StatusBadge status="Approved" />);
    expect(screen.getByText('Approved')).toBeInTheDocument();
  });

  it('uses green for Paid and red for Overdue', () => {
    const { rerender } = render(<StatusBadge status="Paid" />);
    expect(screen.getByText('Paid')).toHaveClass('badge-green');

    rerender(<StatusBadge status="Overdue" />);
    expect(screen.getByText('Overdue')).toHaveClass('badge-red');
  });

  it('falls back to grey for an unknown status', () => {
    render(<StatusBadge status="Something" />);
    expect(screen.getByText('Something')).toHaveClass('badge-grey');
  });
});
