import { Routes, Route, Link } from "react-router-dom";
import AuctionsList from "./pages/AuctionsList";
import LoginAdmin from "./pages/LoginAdmin";
import LoginUser from "./pages/LoginUser";
import RegisterUser from "./pages/RegisterUser";
import CreateAuction from "./pages/CreateAuction";
import { RequireAuth } from "./auth/RequireAuth";
import { useAuth } from "./auth/useAuth";

export default function App() {
  const { role, logout, token } = useAuth();

  return (
    <div style={{ maxWidth: 1000, margin: "0 auto", padding: 16 }}>
      <nav style={{ display: "flex", gap: 12, alignItems: "center" }}>
        <Link to="/">Auctions</Link>
        <Link to="/login/user">User Login</Link>
        <Link to="/login/admin">Admin Login</Link>
        <Link to="/register">Register</Link>

        {role === "Admin" && <Link to="/admin/create-auction">Create Auction</Link>}

        <div style={{ marginLeft: "auto" }}>
          {token ? (
            <button onClick={logout}>Logout ({role})</button>
          ) : (
            <span>Not logged in</span>
          )}
        </div>
      </nav>

      <hr />

      <Routes>
        <Route path="/" element={<AuctionsList />} />

        <Route path="/login/user" element={<LoginUser />} />
        <Route path="/login/admin" element={<LoginAdmin />} />
        <Route path="/register" element={<RegisterUser />} />

        <Route element={<RequireAuth role="Admin" />}>
          <Route path="/admin/create-auction" element={<CreateAuction />} />
        </Route>
      </Routes>
    </div>
  );
}
