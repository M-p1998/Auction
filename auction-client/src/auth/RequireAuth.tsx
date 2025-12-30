import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "./useAuth";

export default function RequireAuth() {
  const { isLoggedIn} = useAuth();
  if (!isLoggedIn) {
    alert("Please log in to continue.")
    return <Navigate to="/login" replace />;
  }
  // if (!isAdmin) return <Navigate to="/auctions" replace />;
  return <Outlet />;
}
