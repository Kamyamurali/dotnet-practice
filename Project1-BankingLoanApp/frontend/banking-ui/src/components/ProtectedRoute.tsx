import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAppSelector } from '../store/hooks';
import type { Role } from '../types';

interface Props {
  role?: Role;   
}


export default function ProtectedRoute({ role }: Props) {
  const user = useAppSelector((state) => state.auth.user);
  const location = useLocation();

  if (!user) {
    return <Navigate to="/login" replace state={{ from: location.pathname }} />;
  }
  if (role && user.role !== role) {
    return <Navigate to={user.role === 'Admin' ? '/admin' : '/dashboard'} replace />;
  }
  return <Outlet />;
}
