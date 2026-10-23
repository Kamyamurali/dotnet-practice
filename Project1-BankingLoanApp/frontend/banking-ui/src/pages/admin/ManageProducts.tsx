import { useState, type FormEvent } from 'react';
import { productApi } from '../../api/productApi';
import { getErrorMessage } from '../../api/client';
import { useFetch } from '../../hooks/useFetch';
import PageHeader from '../../components/PageHeader';
import { Alert, Loader } from '../../components/Feedback';
import { formatMoney } from '../../utils/format';
import type { LoanProduct, ProductRequest } from '../../types';

const blank: ProductRequest = {
  name: '', category: '', description: '', interestRate: 0,
  minAmount: 0, maxAmount: 0, maxTenureMonths: 12, isActive: true,
};

export default function ManageProducts() {
  const { data, loading, error, reload } = useFetch(() => productApi.getAll(true));
  const [form, setForm] = useState<ProductRequest>(blank);
  const [editingId, setEditingId] = useState<number | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [message, setMessage] = useState<{ type: 'error' | 'success'; text: string } | null>(null);
  const [busy, setBusy] = useState(false);

  const set = <K extends keyof ProductRequest>(key: K, value: ProductRequest[K]) =>
    setForm({ ...form, [key]: value });

  const startAdd = () => { setForm(blank); setEditingId(null); setShowForm(true); setMessage(null); };

  const startEdit = (p: LoanProduct) => {
    setForm({
      name: p.name, category: p.category, description: p.description ?? '',
      interestRate: p.interestRate, minAmount: p.minAmount, maxAmount: p.maxAmount,
      maxTenureMonths: p.maxTenureMonths, isActive: p.isActive,
    });
    setEditingId(p.productId);
    setShowForm(true);
    setMessage(null);
  };

  const validate = (): string | null => {
    if (form.name.trim().length < 2) return 'Name is required.';
    if (!form.category.trim()) return 'Category is required.';
    if (form.interestRate < 0 || form.interestRate > 50) return 'Interest rate must be between 0 and 50.';
    if (form.minAmount <= 0) return 'Minimum amount must be greater than 0.';
    if (form.maxAmount < form.minAmount) return 'Maximum amount must be at least the minimum amount.';
    if (form.maxTenureMonths < 1 || form.maxTenureMonths > 480) return 'Tenure must be between 1 and 480 months.';
    return null;
  };

  const save = async (e: FormEvent) => {
    e.preventDefault();
    const problem = validate();
    if (problem) { setMessage({ type: 'error', text: problem }); return; }

    setBusy(true);
    try {
      const payload = { ...form, name: form.name.trim(), category: form.category.trim() };
      if (editingId) await productApi.update(editingId, payload);
      else await productApi.add(payload);
      setMessage({ type: 'success', text: editingId ? 'Product updated.' : 'Product added.' });
      setShowForm(false);
      await reload();
    } catch (err) {
      setMessage({ type: 'error', text: getErrorMessage(err) });
    } finally {
      setBusy(false);
    }
  };

  const remove = async (p: LoanProduct) => {
    if (!window.confirm(`Delete "${p.name}"? If customers have applied for it, it will be deactivated instead.`)) return;
    try {
      const { result } = await productApi.remove(p.productId);
      setMessage({ type: 'success', text: `${p.name}: ${result.toLowerCase()}.` });
      await reload();
    } catch (err) {
      setMessage({ type: 'error', text: getErrorMessage(err) });
    }
  };

  return (
    <div className="page">
      <PageHeader title="Loan products" subtitle="What customers can apply for."
        actions={<button className="btn btn-primary" onClick={startAdd}>Add product</button>} />

      <Alert message={message?.text ?? null} type={message?.type} />

      {showForm && (
        <form className="card" onSubmit={save} noValidate>
          <h2 className="card__title">{editingId ? 'Edit product' : 'New product'}</h2>
          <div className="form-grid">
            <label className="field"><span>Name</span>
              <input value={form.name} onChange={(e) => set('name', e.target.value)} /></label>
            <label className="field"><span>Category</span>
              <input value={form.category} list="categories" onChange={(e) => set('category', e.target.value)} />
              <datalist id="categories">
                {['Home', 'Education', 'Personal', 'Vehicle', 'Business'].map((c) => <option key={c} value={c} />)}
              </datalist></label>
            <label className="field"><span>Interest rate (% per year)</span>
              <input type="number" step="0.01" value={form.interestRate} onChange={(e) => set('interestRate', Number(e.target.value))} /></label>
            <label className="field"><span>Max tenure (months)</span>
              <input type="number" value={form.maxTenureMonths} onChange={(e) => set('maxTenureMonths', Number(e.target.value))} /></label>
            <label className="field"><span>Minimum amount</span>
              <input type="number" value={form.minAmount} onChange={(e) => set('minAmount', Number(e.target.value))} /></label>
            <label className="field"><span>Maximum amount</span>
              <input type="number" value={form.maxAmount} onChange={(e) => set('maxAmount', Number(e.target.value))} /></label>
            <label className="field field-wide"><span>Description</span>
              <textarea rows={2} maxLength={500} value={form.description ?? ''} onChange={(e) => set('description', e.target.value)} /></label>
            <label className="check"><input type="checkbox" checked={form.isActive}
              onChange={(e) => set('isActive', e.target.checked)} /> Available to customers</label>
          </div>
          <div className="modal__actions">
            <button type="button" className="btn btn-ghost" onClick={() => setShowForm(false)}>Cancel</button>
            <button type="submit" className="btn btn-primary" disabled={busy}>{busy ? 'Saving…' : 'Save product'}</button>
          </div>
        </form>
      )}

      {loading && <Loader />}
      <Alert message={error} />
      {data && (
        <div className="table-wrap">
          <table className="table">
            <thead>
              <tr><th>Name</th><th>Category</th><th>Rate</th><th>Amount range</th><th>Max tenure</th><th>Status</th><th>Action</th></tr>
            </thead>
            <tbody>
              {data.map((p) => (
                <tr key={p.productId} className={p.isActive ? '' : 'row-muted'}>
                  <td>{p.name}</td>
                  <td>{p.category}</td>
                  <td>{p.interestRate.toFixed(2)}%</td>
                  <td>{formatMoney(p.minAmount)} – {formatMoney(p.maxAmount)}</td>
                  <td>{p.maxTenureMonths} mo</td>
                  <td>{p.isActive ? <span className="badge badge-green">Active</span> : <span className="badge badge-grey">Inactive</span>}</td>
                  <td className="actions">
                    <button className="btn btn-ghost btn-sm" onClick={() => startEdit(p)}>Edit</button>
                    <button className="btn btn-danger btn-sm" onClick={() => remove(p)}>Delete</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
