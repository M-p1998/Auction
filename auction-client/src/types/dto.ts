export type AuctionDto = {
  id: string;
  make: string;
  model: string;
  year: number;
  color?: string;
  mileage: number;
  imageUrl: string;
  reservePrice: number;
  auctionEnd: string;     // ISO date string
  createdAt?: string;      
  currentHighBid?: number; 
};

export type CreateAuctionRequest = {
  make: string;
  model: string;
  year: number;
  color?: string;
  mileage: number;
  imageUrl: string;
  reservePrice: number;
  auctionEnd: string; // ISO string
};

export type UpdateAuctionRequest = Partial<CreateAuctionRequest>;

export type CreateBidRequest = {
  auctionId: string;
  amount: number;
};

export type BidDto = {
  id: string;
  auctionId: string;
  amount: number;
  createdAt: string;
  auctionTitle?: string; 
};
