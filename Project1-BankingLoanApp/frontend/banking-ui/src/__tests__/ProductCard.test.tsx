import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import ProductCard from '../components/ProductCard';
import type { LoanProduct } from '../types';

const product: LoanProduct = {
  productId: 1,
  name: 'Education Loan',
  category: 'Education',
  description: 'Tuition and living costs',
  interestRate: 6.75,
  minAmount: 5000,
  maxAmount: 150000,
  maxTenureMonths: 120,
  isActive: true,
  createdAt: '2026-10-01T00:00:00Z',
};

describe('ProductCard', () => {
  it('shows the product details', () => {
    render(<ProductCard product={product} />);
    expect(screen.getByText('Education Loan')).toBeInTheDocument();
    expect(screen.getByText('6.75%')).toBeInTheDocument();
    expect(screen.getByText('Up to 120 months')).toBeInTheDocument();
  });

  it('calls onApply with the product when the button is clicked', async () => {
    const onApply = jest.fn();
    render(<ProductCard product={product} onApply={onApply} />);

    await userEvent.click(screen.getByRole('button', { name: 'Apply now' }));

    expect(onApply).toHaveBeenCalledWith(product);
  });

  it('hides the button when no onApply is given', () => {
    render(<ProductCard product={product} />);
    expect(screen.queryByRole('button')).not.toBeInTheDocument();
  });
});
