import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "./useAuth";

export default function RequireAdmin() {
  const { isLoggedIn, isAdmin } = useAuth();

  if (!isLoggedIn) {
    alert("You must log in as an admin to access this page.");
    return <Navigate to="/admin/login" replace />;
  }

  if (!isAdmin) {
    alert("Access denied. Admins only.");
    return <Navigate to="/auctions" replace />;
  }

  return <Outlet />;
}
