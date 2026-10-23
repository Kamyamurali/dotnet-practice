import type { LoanProduct } from '../types';
import { formatMoney } from '../utils/format';

interface Props {
  product: LoanProduct;
  onApply?: (product: LoanProduct) => void;   // optional: hidden when not provided
  applyLabel?: string;
}

export default function ProductCard({ product, onApply, applyLabel = 'Apply now' }: Props) {
  return (
    <article className="product-card">
      <div className="product-card__top">
        <span className="product-card__category">{product.category}</span>
        {!product.isActive && <span className="badge badge-grey">Inactive</span>}
      </div>
      <h3 className="product-card__name">{product.name}</h3>
      {product.description && <p className="product-card__desc">{product.description}</p>}

      <div className="product-card__rate">
        <strong>{product.interestRate.toFixed(2)}%</strong>
        <span>per year</span>
      </div>

      <dl className="product-card__facts">
        <div>
          <dt>Amount</dt>
          <dd>{formatMoney(product.minAmount)} – {formatMoney(product.maxAmount)}</dd>
        </div>
        <div>
          <dt>Tenure</dt>
          <dd>Up to {product.maxTenureMonths} months</dd>
        </div>
      </dl>

      {onApply && (
        <button className="btn btn-primary btn-block" onClick={() => onApply(product)}>
          {applyLabel}
        </button>
      )}
    </article>
  );
}
