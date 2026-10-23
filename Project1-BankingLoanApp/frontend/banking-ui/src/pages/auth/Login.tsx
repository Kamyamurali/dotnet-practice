import { useState, type FormEvent } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { authApi } from '../../api/authApi';
import { getErrorMessage } from '../../api/client';
import { useAppDispatch } from '../../store/hooks';
import { loginSuccess } from '../../store/authSlice';
import { Alert } from '../../components/Feedback';

export default function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const location = useLocation();
  const from = (location.state as { from?: string } | null)?.from;

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!email.trim() || !password) {
      setError('Please enter your email and password.');
      return;
    }

    setSubmitting(true);
    try {
      const response = await authApi.login(email.trim(), password);
      dispatch(loginSuccess(response));
      const home = response.user.role === 'Admin' ? '/admin' : '/dashboard';
      navigate(from ?? home, { replace: true });
    } catch (err) {
      setError(getErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="auth">
      <div className="auth__panel">
        <h1>Welcome back</h1>
        <p className="muted">Log in to apply for loans and track your applications.</p>

        <Alert message={error} />

        <form onSubmit={handleSubmit} noValidate>
          <label className="field">
            <span>Email</span>
            <input type="email" value={email} autoComplete="email"
              onChange={(e) => setEmail(e.target.value)} />
          </label>
          <label className="field">
            <span>Password</span>
            <input type="password" value={password} autoComplete="current-password"
              onChange={(e) => setPassword(e.target.value)} />
          </label>
          <button type="submit" className="btn btn-primary btn-block" disabled={submitting}>
            {submitting ? 'Logging in…' : 'Log in'}
          </button>
        </form>

        <p className="auth__switch">
          New here? <Link to="/register">Open an account</Link>
        </p>
      </div>
    </div>
  );
}
