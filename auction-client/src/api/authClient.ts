// src/api/authClient.ts
import { createHttpClient } from "./http";

const baseURL = import.meta.env.VITE_GATEWAY_BASE_URL as string;
const http = createHttpClient(baseURL);

export type LoginRequest = {
  email: string;
  password: string;
};

export type LoginResponse = {
  message: string;
  token: string;
};

export async function loginAdmin(payload: LoginRequest) {
  const res = await http.post<LoginResponse>(
    "/api/auth/login-admin",
    payload
  );
  return res.data;
}

export async function loginUser(payload: LoginRequest) {
  const res = await http.post<LoginResponse>(
    "/api/auth/login-user",
    payload
  );
  return res.data;
}

export async function registerUser(payload: LoginRequest) {
  const res = await http.post<string>(
    "/api/auth/register-user",
    payload
  );
  return res.data;
}
