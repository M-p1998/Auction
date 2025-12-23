import { useEffect, useState } from "react";
import { getAuctions } from "../api/auctionsClient";
import type { AuctionDto } from "../types/dto";
import AuctionCard from "../components/AuctionCard";
import { useAuth } from "../auth/useAuth";
import { useNavigate } from "react-router-dom";

export default function AuctionsList() {
  const [auctions, setAuctions] = useState<AuctionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const { isAdmin } = useAuth();
  const nav = useNavigate();

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
    <div className="auction-grid">
      <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", gap: 12, flexWrap: "wrap" }}>
        <h2 style={{ margin: 0 }}>Auctions</h2>

        {isAdmin && (
          <button onClick={() => nav("/admin/auctions/create")}>
            + Create Auction
          </button>
        )}
      </div>

      <div style={{ marginTop: 16 }} className="auction-grid">
        {auctions.map((a) => (
          <AuctionCard key={a.id} auction={a} />
        ))}
      </div>
    </div>
  );
}
