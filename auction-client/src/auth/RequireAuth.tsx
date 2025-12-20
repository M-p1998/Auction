import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../auth/useAuth";

export function RequireAuth({ role }: { role?: "Admin" | "User" }) {
  const { token, role: currentRole } = useAuth();

  if (!token) return <Navigate to="/login/user" replace />;
  if (role && currentRole !== role) return <Navigate to="/" replace />;

  return <Outlet />;
}
