import { useEffect, useState } from "react";
import { getAuctions } from "../api/auctionsClient";
import type { AuctionDto } from "../types/dto";
import AuctionCard from "../components/AuctionCard";

export default function AuctionsList() {
  const [auctions, setAuctions] = useState<AuctionDto[]>([]);
  const [loading, setLoading] = useState(true);


  useEffect(() => {
    (async () => {
      try {
        const data = await getAuctions();

        // “first come first” (oldest first) OR "newest first" depending what you mean.
        // Most apps want newest first:
        const sorted = [...data];

        // Only sort if createdAt exists. Otherwise keep API order.
        if (sorted.length > 0 && sorted[0].createdAt) {
          sorted.sort((a, b) => {
            const at = new Date(a.createdAt ?? "").getTime();
            const bt = new Date(b.createdAt ?? "").getTime();
            return bt - at; // newest first
          });
        }

        setAuctions(sorted);
      } finally {
        setLoading(false);
      }
    })();
  }, []);

  if (loading) return <div className="p-2">Loading auctions...</div>;

  return (
     <div className="auctions-page">
      {/* HEADER */}
      <div className="auctions-header">
   
        {/* <a className="create-btn" onClick={handleCreateClick}>
          Create Auction
        </a> */}
      </div>

      {/* GRID */}
      <div className="auction-grid">
        {auctions.map((a) => (
          <AuctionCard key={a.id} auction={a} />
        ))}
      </div>
    </div>
  );

  
}
