import { useEffect } from 'react';
import { Link, Navigate, Route, Routes, useNavigate } from 'react-router-dom';
import Navbar from './components/Navbar';
import ProtectedRoute from './components/ProtectedRoute';
import { useAppDispatch, useAppSelector } from './store/hooks';
import { logout } from './store/authSlice';

import Login from './pages/auth/Login';
import Register from './pages/auth/Register';
import Products from './pages/customer/Products';
import Dashboard from './pages/customer/Dashboard';
import ApplyLoan from './pages/customer/ApplyLoan';
import MyApplications from './pages/customer/MyApplications';
import Statement from './pages/customer/Statement';
import History from './pages/customer/History';
import AdminDashboard from './pages/admin/AdminDashboard';
import ReviewApplications from './pages/admin/ReviewApplications';
import ManageProducts from './pages/admin/ManageProducts';
import Repayments from './pages/admin/Repayments';
import Disbursements from './pages/admin/Disbursements';
import Customers from './pages/admin/Customers';

function Home() {
  const user = useAppSelector((state) => state.auth.user);
  if (!user) return <Navigate to="/products" replace />;
  return <Navigate to={user.role === 'Admin' ? '/admin' : '/dashboard'} replace />;
}

export default function App() {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  useEffect(() => {
    const onExpired = () => {
      dispatch(logout());
      navigate('/login', { replace: true });
    };
    window.addEventListener('auth:expired', onExpired);
    return () => window.removeEventListener('auth:expired', onExpired);
  }, [dispatch, navigate]);

  return (
    <>
      <Navbar />
      <main className="main">
        <Routes>
          {/* Public */}
          <Route path="/" element={<Home />} />
          <Route path="/products" element={<Products />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />

          {/* Customers only */}
          <Route element={<ProtectedRoute role="Customer" />}>
            <Route path="/dashboard" element={<Dashboard />} />
            <Route path="/apply/:productId" element={<ApplyLoan />} />
            <Route path="/applications" element={<MyApplications />} />
            <Route path="/applications/:id" element={<Statement />} />
            <Route path="/history" element={<History />} />
          </Route>

          {/* Admins only */}
          <Route element={<ProtectedRoute role="Admin" />}>
            <Route path="/admin" element={<AdminDashboard />} />
            <Route path="/admin/applications" element={<ReviewApplications />} />
            <Route path="/admin/applications/:id" element={<Statement />} />
            <Route path="/admin/products" element={<ManageProducts />} />
            <Route path="/admin/repayments" element={<Repayments />} />
            <Route path="/admin/disbursements" element={<Disbursements />} />
            <Route path="/admin/customers" element={<Customers />} />
          </Route>

          <Route path="*" element={
            <div className="page"><h1>Page not found</h1><Link to="/">Go home</Link></div>
          } />
        </Routes>
      </main>
    </>
  );
}
