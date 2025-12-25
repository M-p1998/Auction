import { Link } from "react-router-dom";

export default function Navbar() {
  return (
    <nav className="navbar">
      <div className="logo">AuctionApp</div>

      <div className="nav-links">
        <Link to="/auctions">Auctions</Link>
        <Link to="/login">Login</Link>
        <Link to="/admin/login">Admin</Link>
      </div>
    </nav>
  );
}
