import { useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { productApi } from '../../api/productApi';
import { useFetch } from '../../hooks/useFetch';
import { useAppSelector } from '../../store/hooks';
import ProductCard from '../../components/ProductCard';
import PageHeader from '../../components/PageHeader';
import { Alert, EmptyState, Loader } from '../../components/Feedback';
import type { LoanProduct } from '../../types';

export default function Products() {
  const { data: products, loading, error } = useFetch(() => productApi.getAll());
  const [category, setCategory] = useState('All');
  const user = useAppSelector((state) => state.auth.user);
  const navigate = useNavigate();

  const categories = useMemo(
    () => ['All', ...new Set((products ?? []).map((p) => p.category))],
    [products]
  );

  const visible = (products ?? []).filter((p) => category === 'All' || p.category === category);

  const handleApply = (product: LoanProduct) =>
    navigate(user ? `/apply/${product.productId}` : '/login', {
      state: user ? undefined : { from: `/apply/${product.productId}` },
    });

  return (
    <div className="page">
      <PageHeader title="Loan products" subtitle="Compare rates and terms, then apply online in a few minutes." />

      <div className="chips" role="tablist">
        {categories.map((c) => (
          <button key={c} role="tab" aria-selected={category === c}
            className={`chip ${category === c ? 'is-active' : ''}`} onClick={() => setCategory(c)}>
            {c}
          </button>
        ))}
      </div>

      {loading && <Loader />}
      <Alert message={error} />
      {!loading && !error && visible.length === 0 && <EmptyState title="No products in this category yet." />}

      <div className="grid-cards">
        {visible.map((p) => (
          <ProductCard key={p.productId} product={p}
            onApply={user?.role === 'Admin' ? undefined : handleApply}
            applyLabel={user ? 'Apply now' : 'Log in to apply'} />
        ))}
      </div>
    </div>
  );
}
