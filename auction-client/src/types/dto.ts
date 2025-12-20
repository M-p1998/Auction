// src/types/dto.ts

export type AuctionDto = {
  id: string;
  reservePrice: number;
  seller: string;
  winner: string | null;
  soldAmount: number;
  currentHighBid: number;
  createdAt: string;
  updatedAt: string;
  auctionEnd: string;
  status: string;

  make: string;
  model: string;
  year: number;
  color: string;
  mileage: number;
  imageUrl: string;
};

export type CreateAuctionDto = {
  make: string;
  model: string;
  year: number;
  color: string;
  mileage: number;
  imageUrl: string;
  reservePrice: number;
  auctionEnd: string; // ISO string
};

export type LoginRequest = { email: string; password: string };
export type LoginResponse = { message: string; token: string };
