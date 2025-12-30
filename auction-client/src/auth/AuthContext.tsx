import React, { createContext, useMemo, useState, useEffect, useRef } from "react";

export type Role = "Admin" | "User" | null;

export type AuthState = {
  token: string | null;
  role: Role;
  email: string | null;
  login: (token: string, role: Exclude<Role, null>, email: string) => void;
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

  const IDLE_TIMEOUT = 10 * 60 * 1000; // 10 minutes
  const idleTimer = useRef<number | null>(null);

  const [email, setEmail] = useState<string | null>(
  () => localStorage.getItem("email")
);


  function resetIdleTimer() {
    if (idleTimer.current) {
      window.clearTimeout(idleTimer.current);
    }

    idleTimer.current = window.setTimeout(() => {
      localStorage.removeItem("token");
      localStorage.removeItem("role");
      setToken(null);
      setRole(null);
      window.location.href = "/login";
    }, IDLE_TIMEOUT);
  }

  useEffect(() => {
    if (!token) return;

    const events = [
      "mousemove",
      "mousedown",
      "keydown",
      "scroll",
      "touchstart"
    ];

    events.forEach(event =>
      window.addEventListener(event, resetIdleTimer)
    );

    resetIdleTimer();

    return () => {
      if (idleTimer.current) {
        window.clearTimeout(idleTimer.current);
      }

      events.forEach(event =>
        window.removeEventListener(event, resetIdleTimer)
      );
    };
  }, [token]);

  const value = useMemo<AuthState>(() => ({
    token,
    role,
    email,
    login: (t, r, e) => {
      localStorage.setItem("token", t);
      localStorage.setItem("role", r);
      localStorage.setItem("email",e);
      setToken(t);
      setRole(r);
      setEmail(e);
    },
    logout: () => {
      localStorage.removeItem("token");
      localStorage.removeItem("role");
      localStorage.removeItem("email")
      setToken(null);
      setRole(null);
      setEmail(null);
    },
  }), [token, role, email]);

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}
