import { createContext, useContext, useMemo, useState, type ReactNode } from 'react';
import api from '../api/client';
import type { LoginResponse } from '../types';

interface RegisterInput {
  organizationName: string;
  fullName: string;
  email: string;
  password: string;
}

interface AuthState {
  user: LoginResponse | null;
  login: (email: string, password: string) => Promise<void>;
  register: (input: RegisterInput) => Promise<void>;
  logout: () => void;
  hasPermission: (code: string) => boolean;
}

const AuthContext = createContext<AuthState | undefined>(undefined);

const STORAGE_KEY = 'accesshub_user';

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<LoginResponse | null>(() => {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? (JSON.parse(raw) as LoginResponse) : null;
  });

  const login = async (email: string, password: string) => {
    const { data } = await api.post<LoginResponse>('/api/auth/login', { email, password });
    localStorage.setItem('accesshub_token', data.token);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(data));
    setUser(data);
  };

  const register = async (input: RegisterInput) => {
    const { data } = await api.post<LoginResponse>('/api/auth/register', {
      organizationName: input.organizationName,
      fullName: input.fullName,
      email: input.email,
      password: input.password,
    });
    localStorage.setItem('accesshub_token', data.token);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(data));
    setUser(data);
  };

  const logout = () => {
    localStorage.removeItem('accesshub_token');
    localStorage.removeItem(STORAGE_KEY);
    setUser(null);
  };

  const hasPermission = (code: string) =>
    !!user && (user.isSuperAdmin || user.permissions.includes(code));

  const value = useMemo(
    () => ({ user, login, register, logout, hasPermission }),
    [user]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
