// src/services/validators.ts
export function validateEmail(email: string) {
  return /^\S+@\S+\.\S+$/.test(email);
}

export function validatePassword(password: string) {
  // matches your Admin#123 format: at least 8 chars, 1 upper, 1 lower, 1 number, 1 special
  return /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$/.test(password);
}
