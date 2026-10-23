import { useState, type ChangeEvent, type FormEvent } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { authApi } from '../../api/authApi';
import { getErrorMessage } from '../../api/client';
import { useAppDispatch } from '../../store/hooks';
import { loginSuccess } from '../../store/authSlice';
import { Alert } from '../../components/Feedback';
import { emptyRegistration, validateRegistration } from '../../utils/validation';


export default function Register() {
  const [form, setForm] = useState(emptyRegistration);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const update = (e: ChangeEvent<HTMLInputElement>) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    const problem = validateRegistration(form);
    if (problem) {
      setError(problem);
      return;
    }

    setSubmitting(true);
    setError(null);
    try {
      const response = await authApi.register({
        fullName: form.fullName.trim(),
        email: form.email.trim(),
        phone: form.phone.trim() || undefined,
        password: form.password,
      });
      dispatch(loginSuccess(response));     // logged in straight away
      navigate('/dashboard', { replace: true });
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="auth">
      <div className="auth__panel">
        <h1>Open an account</h1>
        <p className="muted">Register as a member to apply for loans online.</p>

        <Alert message={error} />

        <form onSubmit={handleSubmit} noValidate>
          <label className="field">
            <span>Full name</span>
            <input name="fullName" value={form.fullName} onChange={update} autoComplete="name" />
          </label>
          <label className="field">
            <span>Email</span>
            <input name="email" type="email" value={form.email} onChange={update} autoComplete="email" />
          </label>
          <label className="field">
            <span>Phone <em>(optional)</em></span>
            <input name="phone" type="tel" value={form.phone} onChange={update} autoComplete="tel" />
          </label>
          <div className="field-row">
            <label className="field">
              <span>Password</span>
              <input name="password" type="password" value={form.password} onChange={update} autoComplete="new-password" />
            </label>
            <label className="field">
              <span>Confirm password</span>
              <input name="confirm" type="password" value={form.confirm} onChange={update} autoComplete="new-password" />
            </label>
          </div>
          <p className="hint">8+ characters with upper case, lower case, a number and a symbol.</p>

          <button type="submit" className="btn btn-primary btn-block" disabled={submitting}>
            {submitting ? 'Creating account…' : 'Create account'}
          </button>
        </form>

        <p className="auth__switch">
          Already a member? <Link to="/login">Log in</Link>
        </p>
      </div>
    </div>
  );
}
