import { http } from "./http";
import type { AuctionDto, CreateAuctionRequest, UpdateAuctionRequest } from "../types/dto";

export async function getAuctions(): Promise<AuctionDto[]> {
  const res = await http.get<AuctionDto[]>("/api/auctions");
  return res.data;
}

export async function getAuctionById(id: string): Promise<AuctionDto> {
  const res = await http.get<AuctionDto>(`/api/auctions/${id}`);
  return res.data;
}

export async function createAuction(payload: CreateAuctionRequest): Promise<AuctionDto> {
  const res = await http.post<AuctionDto>("/api/auctions", payload);
  return res.data;
}

export async function updateAuction(id: string, payload: UpdateAuctionRequest): Promise<AuctionDto> {
  const res = await http.put<AuctionDto>(`/api/auctions/${id}`, payload);
  return res.data;
}

export async function deleteAuction(id: string): Promise<void> {
  await http.delete(`/api/auctions/${id}`);
}
