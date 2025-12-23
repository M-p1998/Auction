import { http } from "./http";
import type { BidDto, CreateBidRequest } from "../types/dto";

export async function placeBid(payload: CreateBidRequest): Promise<BidDto> {
  const res = await http.post<BidDto>("/api/bids", payload);
  return res.data;
}

// export async function getMyBids(): Promise<BidDto[]> {
//   const res = await http.get<BidDto[]>("/api/bids/me");
//   return res.data;
// }
