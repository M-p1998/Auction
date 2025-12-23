// import { useContext } from "react";
// import { AuthContext } from "./AuthContext";

// export function useAuth() {
//   const ctx = useContext(AuthContext);

//   const isLoggedIn = !!ctx?.user;
//   const roles = ctx?.user?.roles ?? [];
//   const isAdmin = roles.includes("Admin");

//   return { ...ctx, isLoggedIn, isAdmin };
// }


import { useContext } from "react";
import { AuthContext } from "./AuthContext";

export function useAuth() {
  const ctx = useContext(AuthContext);

  if (!ctx) {
    throw new Error("useAuth must be used within an AuthProvider");
  }

  const isLoggedIn = !!ctx.token;
  const isAdmin = ctx.role === "Admin";

  return {
    ...ctx,
    isLoggedIn,
    isAdmin,
  };
}
