
import { createHttpClient } from "./http";
import type { AuctionDto, CreateAuctionDto } from "../types/dto";

const baseURL = import.meta.env.VITE_GATEWAY_BASE_URL as string;
const http = createHttpClient(baseURL);

export async function getAllAuctions() {
  const res = await http.get<AuctionDto[]>("/api/auctions");
  return res.data;
}

export async function createAuction(payload: CreateAuctionDto) {
  const res = await http.post("/api/auctions", payload);
  return res.data;
}
