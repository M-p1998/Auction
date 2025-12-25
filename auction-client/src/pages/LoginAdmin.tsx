import { useState } from "react";
import { loginAdmin } from "../api/authClient";
import { useAuth } from "../auth/useAuth";
import { useNavigate } from "react-router-dom";
import { validateEmail, validatePassword } from "../services/validators";


export default function LoginAdmin() {
  const [email, setEmail] = useState("admin@auction.com");
  const [password, setPassword] = useState("Admin#123");
  const [err, setErr] = useState<string | null>(null);

  const auth = useAuth();
  const nav = useNavigate();

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setErr(null);

    // validation
    if (!validateEmail(email)) {
      setErr("Please enter a valid email address.");
      return;
    }

    if (!validatePassword(password)) {
      setErr(
        "Password must be at least 8 characters and include uppercase, lowercase, number, and special character."
      );
      return;
    }

    try {
      const res = await loginAdmin({ email, password });
      auth.login(res.token, "Admin");
      nav("/admin/auctions/create");
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (ex: any) {
      setErr(ex?.response?.data ?? "Admin login failed");
    }
  }

  return (
    <div style={{ maxWidth: 420, margin: "40px auto" }}>
      <h2>Admin Login</h2>

      <form onSubmit={onSubmit}>
        <input
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          placeholder="email"
          style={{ width: "100%", padding: 10 }}
        />

        <div style={{ height: 10 }} />

        <input
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          placeholder="password"
          type="password"
          style={{ width: "100%", padding: 10 }}
        />

        <div style={{ height: 10 }} />

        <button style={{ width: "100%", padding: 10 }}>
          Login
        </button>
      </form>

      {err && <p style={{ color: "tomato" }}>{err}</p>}
    </div>
  );
}
