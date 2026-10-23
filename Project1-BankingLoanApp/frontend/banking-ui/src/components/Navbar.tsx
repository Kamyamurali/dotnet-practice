import { useState } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { useAppDispatch, useAppSelector } from '../store/hooks';
import { logout } from '../store/authSlice';

interface NavItem {
  to: string;
  label: string;
  end?: boolean; 
}

const customerLinks: NavItem[] = [
  { to: '/dashboard', label: 'Dashboard' },
  { to: '/products', label: 'Loan products' },
  { to: '/applications', label: 'My applications' },
  { to: '/history', label: 'History' },
];

const adminLinks: NavItem[] = [
  { to: '/admin', label: 'Overview', end: true },
  { to: '/admin/applications', label: 'Applications' },
  { to: '/admin/products', label: 'Products' },
  { to: '/admin/repayments', label: 'Repayments' },
  { to: '/admin/disbursements', label: 'Disbursements' },
  { to: '/admin/customers', label: 'Customers' },
];

export default function Navbar() {
  const user = useAppSelector((state) => state.auth.user);
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const [open, setOpen] = useState(false);

  const links: NavItem[] = !user ? [{ to: '/products', label: 'Loan products' }]
    : user.role === 'Admin' ? adminLinks : customerLinks;

  const handleLogout = () => {
    dispatch(logout());
    navigate('/login');
  };

  return (
    <nav className="nav">
      <div className="nav__inner">
        <NavLink to="/" className="brand" onClick={() => setOpen(false)}>
          <span className="brand__mark" aria-hidden="true">M</span>
          <span>Meridian<span className="brand__sub">Loans</span></span>
        </NavLink>

        <button className="nav__toggle" aria-label="Menu" aria-expanded={open} onClick={() => setOpen(!open)}>
          ☰
        </button>

        <div className={`nav__links ${open ? 'is-open' : ''}`}>
          {links.map((link) => (
            <NavLink key={link.to} to={link.to} end={link.end}
              className="nav__link" onClick={() => setOpen(false)}>
              {link.label}
            </NavLink>
          ))}

          <div className="nav__account">
            {user ? (
              <>
                <span className="nav__user">
                  {user.fullName}
                  {user.role === 'Admin' && <span className="nav__role">Admin</span>}
                </span>
                <button className="btn btn-ghost btn-sm" onClick={handleLogout}>Log out</button>
              </>
            ) : (
              <>
                <NavLink to="/login" className="nav__link" onClick={() => setOpen(false)}>Log in</NavLink>
                <NavLink to="/register" className="btn btn-gold btn-sm" onClick={() => setOpen(false)}>Open account</NavLink>
              </>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}
