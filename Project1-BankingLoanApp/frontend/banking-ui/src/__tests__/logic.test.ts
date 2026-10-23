import { calculateEmi } from '../utils/emi';
import authReducer, { loginSuccess, logout, emptyAuthState } from '../store/authSlice';
import { validateRegistration } from '../utils/validation';

describe('calculateEmi', () => {
  it('matches the standard EMI formula', () => {
    expect(calculateEmi(12000, 11.25, 12)).toBeCloseTo(1062.03, 1);
  });

  it('splits evenly when the rate is 0', () => {
    expect(calculateEmi(1200, 0, 12)).toBe(100);
  });

  it('returns 0 for invalid input', () => {
    expect(calculateEmi(0, 10, 12)).toBe(0);
    expect(calculateEmi(1000, 10, 0)).toBe(0);
  });
});

describe('authSlice', () => {
  const response = {
    token: 'abc',
    expiresAt: '2099-01-01T00:00:00Z',
    user: { userId: 1, fullName: 'Admin', email: 'admin@bank.com', role: 'Admin' as const, createdAt: '' },
  };

  it('stores the user and token on loginSuccess', () => {
    const state = authReducer(emptyAuthState, loginSuccess(response));
    expect(state.token).toBe('abc');
    expect(state.user?.role).toBe('Admin');
  });

  it('clears everything on logout', () => {
    const loggedIn = authReducer(emptyAuthState, loginSuccess(response));
    expect(authReducer(loggedIn, logout())).toEqual(emptyAuthState);
  });
});

describe('validateRegistration', () => {
  const valid = { fullName: 'Priya Sharma', email: 'priya@example.com', phone: '', password: 'Strong@123', confirm: 'Strong@123' };

  it('accepts a valid form', () => {
    expect(validateRegistration(valid)).toBeNull();
  });

  it('rejects a weak password', () => {
    expect(validateRegistration({ ...valid, password: 'weak', confirm: 'weak' })).toMatch(/at least 8 characters/);
  });

  it('rejects passwords that do not match', () => {
    expect(validateRegistration({ ...valid, confirm: 'Different@1' })).toBe('Passwords do not match.');
  });
});

import { ageOn, checkEligibility } from '../utils/eligibility';

describe('loan eligibility', () => {
  const today = new Date(2026, 9, 23); 

  it('works out age, counting whether the birthday has passed', () => {
    expect(ageOn('2008-10-23', today)).toBe(18);
    expect(ageOn('2008-10-24', today)).toBe(17);
  });

  it('rejects applicants under 18', () => {
    expect(checkEligibility('2010-01-01', 5000, 500, today)).toMatch(/at least 18/);
  });

  it('rejects a monthly payment above 50% of income, and accepts a valid applicant', () => {
    expect(checkEligibility('1998-04-12', 2000, 1200, today)).toMatch(/50%/);
    expect(checkEligibility('1998-04-12', 6000, 1200, today)).toBeNull();
  });
});