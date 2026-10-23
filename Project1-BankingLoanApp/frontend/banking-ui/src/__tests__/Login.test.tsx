import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { Provider } from 'react-redux';
import { MemoryRouter } from 'react-router-dom';
import { configureStore } from '@reduxjs/toolkit';
import authReducer from '../store/authSlice';
import Login from '../pages/auth/Login';
import { authApi } from '../api/authApi';

// Replace the real API with a fake so no network call happens
jest.mock('../api/authApi', () => ({
  authApi: { login: jest.fn() },
}));
const mockLogin = authApi.login as jest.Mock;

function renderLogin() {
  const store = configureStore({ reducer: { auth: authReducer } });
  render(
    <Provider store={store}>
      <MemoryRouter>
        <Login />
      </MemoryRouter>
    </Provider>
  );
  return store;
}

describe('Login page', () => {
  beforeEach(() => mockLogin.mockReset());

  it('renders the form', () => {
    renderLogin();
    expect(screen.getByLabelText('Email')).toBeInTheDocument();
    expect(screen.getByLabelText('Password')).toBeInTheDocument();
    expect(screen.getByRole('button', { name: 'Log in' })).toBeInTheDocument();
  });

  it('shows an error when fields are empty and does not call the API', async () => {
    renderLogin();
    await userEvent.click(screen.getByRole('button', { name: 'Log in' }));

    expect(screen.getByRole('alert')).toHaveTextContent('Please enter your email and password.');
    expect(mockLogin).not.toHaveBeenCalled();
  });

  it('logs in and saves the user in the Redux store', async () => {
    mockLogin.mockResolvedValue({
      token: 'fake-jwt',
      expiresAt: '2099-01-01T00:00:00Z',
      user: { userId: 2, fullName: 'Priya Sharma', email: 'priya@example.com', role: 'Customer', createdAt: '' },
    });
    const store = renderLogin();

    await userEvent.type(screen.getByLabelText('Email'), 'priya@example.com');
    await userEvent.type(screen.getByLabelText('Password'), 'Customer@123');
    await userEvent.click(screen.getByRole('button', { name: 'Log in' }));

    expect(mockLogin).toHaveBeenCalledWith('priya@example.com', 'Customer@123');
    expect(store.getState().auth.token).toBe('fake-jwt');
    expect(store.getState().auth.user?.fullName).toBe('Priya Sharma');
  });
});
