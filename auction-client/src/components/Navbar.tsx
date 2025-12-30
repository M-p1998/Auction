import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/useAuth";

export default function Navbar() {
  const { isLoggedIn, isAdmin, logout, email } = useAuth();
  const navigate = useNavigate();


  // function handleCreateClick() {
  //   if (!isAdmin) {
  //     alert("Only admin can create auctions.");
  //     return;
  //   }
  //   navigate("/admin/auctions/create");
  // }
  function handleCreateClick() {
  if (!isAdmin) {
    alert("Please log in as an admin to create auctions.");
    navigate("/admin/login");
    return;
  }

  navigate("/admin/auctions/create");
}

  function handleLogout() {
    logout();
    navigate("/auctions");
  }

  return (
    <nav className="navbar">
      <div className="logo">
        <Link to="/auctions">AuctionApp</Link>
      </div>

      <div className="nav-links">
        {!isLoggedIn && (
          <>
            <Link to="/register">Register</Link>
            <Link to="/login">Login</Link>
            <Link to="/admin/login">Admin</Link>
          </>
        )}
        {isLoggedIn && (
          <>
            <span style={{ fontWeight: 500 }}>
              {isAdmin ? "Admin" : email}
            </span>

            <button className="logout-btn" onClick={handleLogout}>
              Logout
            </button>
          </>
        )}
        <button
          className="create-btn"
          onClick={handleCreateClick}
        >
          Create Auction
        </button>
        {/* <button
          className="create-btn"
          disabled={!isAdmin}
          title={!isAdmin ? "Admin login required" : ""}
          onClick={handleCreateClick}
        >
          Create Auction
        </button> */}


      </div>
    </nav>
  );
}
