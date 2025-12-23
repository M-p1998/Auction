import { Routes, Route, Navigate } from "react-router-dom";

import AuctionsList from "./pages/AuctionsList";
import AuctionDetails from "./pages/AuctionDetails";
import CreateAuction from "./pages/CreateAuction";
import UpdateAuction from "./pages/UpdateAuction";
// import BidHistory from "./pages/BidHistory";

import RequireAuth from "./auth/RequireAuth";
import RequireAdmin from "./RequireAdmin"

import LoginAdmin from "./pages/LoginAdmin";
import LoginUser from "./pages/LoginUser";
import RegisterUser from "./pages/RegisterUser";

export default function App() {
  return (
    <Routes>
      {/* DEBUG ROUTE */}
      <Route path="/debug" element={<div style={{ padding: 40 }}>DEBUG ROUTE WORKS</div>} />

      <Route path="/" element={<Navigate to="/auctions" replace />} />
      <Route path="/auctions" element={<AuctionsList />} />
      <Route path="/auctions/:id" element={<AuctionDetails />} />

      <Route path="/register" element={<RegisterUser />} />
      <Route path="/login" element={<LoginUser />} />
      <Route path="/admin/login" element={<LoginAdmin />} />

      {/* User protected */}
      <Route element={<RequireAuth />}>
        {/* <Route path="/me/bids" element={<BidHistory />} /> */}
      </Route>

      {/* Admin protected */}
      <Route element={<RequireAdmin />}>
        <Route path="/admin/auctions/create" element={<CreateAuction />} />
        <Route path="/admin/auctions/:id/edit" element={<UpdateAuction />} />
      </Route>

      <Route path="*" element={<div className="p-2">Not found</div>} />
    </Routes>
  );
}
