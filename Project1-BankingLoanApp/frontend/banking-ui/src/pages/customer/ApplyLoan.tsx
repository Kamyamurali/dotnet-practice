import { useEffect, useState, type FormEvent } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { productApi } from '../../api/productApi';
import { applicationApi } from '../../api/applicationApi';
import { getErrorMessage } from '../../api/client';
import { useFetch } from '../../hooks/useFetch';
import PageHeader from '../../components/PageHeader';
import { Alert, Loader } from '../../components/Feedback';
import { calculateEmi } from '../../utils/emi';
import { formatMoney } from '../../utils/format';
import { checkEligibility, MAX_INCOME_SHARE } from '../../utils/eligibility';

export default function ApplyLoan() {
  const { productId } = useParams();
  const navigate = useNavigate();
  const { data: product, loading, error: loadError } = useFetch(
    () => productApi.getById(Number(productId)), [productId]);

  const [amount, setAmount] = useState('');
  const [tenure, setTenure] = useState('');
  const [purpose, setPurpose] = useState('');
  const [dateOfBirth, setDateOfBirth] = useState('');
  const [income, setIncome] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (product) {
      setAmount(String(product.minAmount));
      setTenure(String(Math.min(12, product.maxTenureMonths)));
    }
  }, [product]);

  if (loading) return <div className="page"><Loader /></div>;
  if (loadError || !product) return <div className="page"><Alert message={loadError ?? 'Product not found.'} /></div>;

  const amountValue = Number(amount);
  const tenureValue = Number(tenure);
  const emi = calculateEmi(amountValue, product.interestRate, tenureValue);
  const incomeValue = Number(income);
  const incomeShare = incomeValue > 0 ? emi / incomeValue : null;

  const validate = (): string | null => {
    if (!amountValue || amountValue < product.minAmount || amountValue > product.maxAmount)
      return `Amount must be between ${formatMoney(product.minAmount)} and ${formatMoney(product.maxAmount)}.`;
    if (!Number.isInteger(tenureValue) || tenureValue < 1 || tenureValue > product.maxTenureMonths)
      return `Tenure must be between 1 and ${product.maxTenureMonths} months.`;
    if (purpose.trim().length < 3) return 'Please tell us what the loan is for.';
    return checkEligibility(dateOfBirth, incomeValue, emi);
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    const problem = validate();
    if (problem) {
      setError(problem);
      return;
    }
    setSubmitting(true);
    setError(null);
    try {
      const application = await applicationApi.apply({
        productId: product.productId,
        amountRequested: amountValue,
        tenureMonths: tenureValue,
        purpose: purpose.trim(),
        dateOfBirth,
        monthlyIncome: incomeValue,
      });
      navigate(`/applications/${application.applicationId}`, { state: { justApplied: true } });
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="page">
      <PageHeader title={`Apply for ${product.name}`} subtitle={product.description ?? undefined}
        actions={<Link to="/products" className="btn btn-ghost">Back to products</Link>} />

      <div className="split">
        <form className="card" onSubmit={handleSubmit} noValidate>
          <Alert message={error} />
          <label className="field">
            <span>Loan amount (USD)</span>
            <input type="number" min={product.minAmount} max={product.maxAmount} step="100"
              value={amount} onChange={(e) => setAmount(e.target.value)} />
            <small>{formatMoney(product.minAmount)} – {formatMoney(product.maxAmount)}</small>
          </label>
          <label className="field">
            <span>Tenure (months)</span>
            <input type="number" min={1} max={product.maxTenureMonths}
              value={tenure} onChange={(e) => setTenure(e.target.value)} />
            <small>Up to {product.maxTenureMonths} months</small>
          </label>
          <div className="field-row">
            <label className="field">
              <span>Date of birth</span>
              <input type="date" value={dateOfBirth} onChange={(e) => setDateOfBirth(e.target.value)} />
              <small>You must be 18 or older</small>
            </label>
            <label className="field">
              <span>Monthly income (USD)</span>
              <input type="number" min={1} step="100" value={income}
                placeholder="e.g. 5000" onChange={(e) => setIncome(e.target.value)} />
              <small>Used to check affordability</small>
            </label>
          </div>
          <label className="field">
            <span>Purpose</span>
            <textarea rows={3} maxLength={300} value={purpose}
              placeholder="e.g. Tuition for a master's degree" onChange={(e) => setPurpose(e.target.value)} />
          </label>
          <button type="submit" className="btn btn-primary btn-block" disabled={submitting}>
            {submitting ? 'Submitting…' : 'Submit application'}
          </button>
        </form>

        <aside className="card card-accent">
          <h2 className="card__title">Estimate</h2>
          <div className="estimate">
            <span>Monthly installment</span>
            <strong>{formatMoney(emi)}</strong>
          </div>
          <dl className="facts">
            <div><dt>Interest rate</dt><dd>{product.interestRate.toFixed(2)}% p.a.</dd></div>
            <div><dt>Total payable</dt><dd>{formatMoney(emi * (tenureValue || 0))}</dd></div>
            <div><dt>Share of income</dt><dd className={incomeShare !== null && incomeShare > MAX_INCOME_SHARE ? 'text-danger' : undefined}>
              {incomeShare === null ? '—' : `${Math.round(incomeShare * 100)}% (max 50%)`}
            </dd></div>
            <div><dt>Total interest</dt><dd>{formatMoney(Math.max(0, emi * (tenureValue || 0) - (amountValue || 0)))}</dd></div>
          </dl>
          <p className="hint">The final schedule is created when the bank disburses your loan.</p>
        </aside>
      </div>
    </div>
  );
}