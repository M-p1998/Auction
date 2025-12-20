import React, { createContext, useMemo, useState } from "react";

export type Role = "Admin" | "User" | null;

export type AuthState = {
  token: string | null;
  role: Role;
  login: (token: string, role: Exclude<Role, null>) => void;
  logout: () => void;
};

// eslint-disable-next-line react-refresh/only-export-components
export const AuthContext = createContext<AuthState | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setToken] = useState<string | null>(
    () => localStorage.getItem("token")
  );
  const [role, setRole] = useState<Role>(
    () => (localStorage.getItem("role") as Role) ?? null
  );

  const value = useMemo<AuthState>(() => ({
    token,
    role,
    login: (t, r) => {
      localStorage.setItem("token", t);
      localStorage.setItem("role", r);
      setToken(t);
      setRole(r);
    },
    logout: () => {
      localStorage.removeItem("token");
      localStorage.removeItem("role");
      setToken(null);
      setRole(null);
    },
  }), [token, role]);

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}
