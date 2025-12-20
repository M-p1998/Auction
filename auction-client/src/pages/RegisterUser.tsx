import { useState } from "react";
import { registerUser } from "../api/authClient";
import { useNavigate } from "react-router-dom";
import { validateEmail, validatePassword } from "../services/validators";

export default function RegisterUser() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [msg, setMsg] = useState<string | null>(null);
  const [err, setErr] = useState<string | null>(null);

  const nav = useNavigate();

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setErr(null);
    setMsg(null);

    if (!validateEmail(email)) {
      setErr("Please enter a valid email.");
      return;
    }

    if (!validatePassword(password)) {
      setErr("Password must be 8+ chars and include upper, lower, number, and special char.");
      return;
    }

    try {
      const res = await registerUser({ email, password });
      setMsg(String(res));
      nav("/login/user");
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (ex: any) {
      setErr(ex?.response?.data ?? "Register failed");
    }
  }

  return (
    <div style={{ maxWidth: 420, margin: "40px auto" }}>
      <h2>Register User</h2>
      <form onSubmit={onSubmit}>
        <input value={email} onChange={(e) => setEmail(e.target.value)} placeholder="email" style={{ width: "100%", padding: 10 }} />
        <div style={{ height: 10 }} />
        <input value={password} onChange={(e) => setPassword(e.target.value)} placeholder="password" type="password" style={{ width: "100%", padding: 10 }} />
        <div style={{ height: 10 }} />
        <button style={{ width: "100%", padding: 10 }}>Register</button>
      </form>

      {msg && <p>{msg}</p>}
      {err && <p style={{ color: "tomato" }}>{String(err)}</p>}
    </div>
  );
}
