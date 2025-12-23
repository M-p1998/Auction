import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "./auth/useAuth";

export default function RequireAdmin() {
  const { isLoggedIn, isAdmin } = useAuth();

  if (!isLoggedIn) return <Navigate to="/login" replace />;
  if (!isAdmin) return <Navigate to="/auctions" replace />;

  return <Outlet />;
}
