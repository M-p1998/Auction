import { useEffect, useMemo, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import type { AuctionDto } from "../types/dto";
import { deleteAuction, getAuctionById } from "../api/auctionsClient";
import { useAuth } from "../auth/useAuth";
import BidBox from "../components/BidBox";
import Countdown from "../components/Countdown";

export default function AuctionDetails() {
  const { id } = useParams();
  const [auction, setAuction] = useState<AuctionDto | null>(null);
  const [loading, setLoading] = useState(true);
  const nav = useNavigate();
  const { isAdmin } = useAuth();
  const { isLoggedIn } = useAuth();

  useEffect(() => {
    (async () => {
      try {
        if (!id) return;
        const data = await getAuctionById(id);
        setAuction(data);
      } finally {
        setLoading(false);
      }
    })();
  }, [id]);

  const isExpired = useMemo(() => {
    if (!auction) return false;
    return new Date(auction.auctionEnd).getTime() <= Date.now();
  }, [auction]);

  // function handleBidSuccess(newHighBid: number) {
  //   setAuction(prev =>
  //     prev
  //       ? { ...prev, currentHighBid: newHighBid }
  //       : prev
  //   );
  // }
  async function handleBidSuccess() {
    if (!id) return;
    const fresh = await getAuctionById(id);
    setAuction(fresh);
  }



  async function onDelete() {
    if (!auction) return;
    if (!isExpired) {
      alert("You can only delete an auction after it expires.");
      return;
    }

    const ok = confirm("Delete this expired auction?");
    if (!ok) return;

    await deleteAuction(auction.id);
    nav("/auctions");
  }

  if (loading) return <div className="p-2">Loading...</div>;
  if (!auction) return <div className="p-2">Not found</div>;

  return (
    <div className="p-2" style={{ display: "grid", gap: 16 }}>
      <button onClick={() => nav("/auctions")} style={{ width: "fit-content" }}>
        ← Back
      </button>

      <div className="auction-card">
        <div className="auction-img-wrap">
          <img className="auction-img" src={auction.imageUrl} alt={`${auction.make} ${auction.model}`} />
        </div>

        <div className="auction-card-body" style={{ display: "grid", gap: 10 }}>
          <div style={{ display: "flex", justifyContent: "space-between", flexWrap: "wrap", gap: 10 }}>
            <div>
              <div style={{ fontSize: 22, fontWeight: 800 }}>
                {auction.year} {auction.make} {auction.model}
              </div>
              <div style={{ opacity: 0.8 }}>
                Color: {auction.color ?? "—"} • Mileage: {auction.mileage.toLocaleString()}
              </div>
            </div>

            <div style={{ textAlign: "right" }}>
              <div style={{ fontSize: 12, opacity: 0.8 }}>Ends in</div>
              <Countdown end={auction.auctionEnd} />
              <div style={{ marginTop: 6, fontSize: 12, opacity: 0.8 }}>
                Reserve: <b>${auction.reservePrice.toLocaleString()}</b>
              </div>
            </div>
          </div>

          {/* Admin buttons */}
          {isAdmin && (
            <div className="btn-row">
              <button onClick={() => nav(`/admin/auctions/${auction.id}/edit`)}>Update</button>
              <button onClick={onDelete} disabled={!isExpired} title={!isExpired ? "Only expired auctions can be deleted" : ""}>
                Delete (expired only)
              </button>
            </div>
          )}
        </div>
      </div>

      {/* Bid section */}
      <div className="auction-card">
        <div className="auction-card-body">
          <h3 style={{ marginTop: 0 }}>Bid</h3>

          {/* {isAdmin ? (
            <div style={{ opacity: 0.85 }}>
              Admin accounts cannot bid.
            </div>
          ) : isExpired ? (
            <div style={{ opacity: 0.85 }}>
              This auction has expired. Bidding is closed.
            </div>
          ) 
          : 
          (
            <BidBox auctionId={auction.id} reservePrice={auction.reservePrice} currentHighBid={auction.currentHighBid} onSuccess={handleBidSuccess} />
          ) 
          } */}

          {isLoggedIn ? (
            <BidBox
              auctionId={auction.id}
              reservePrice={auction.reservePrice}
              currentHighBid={auction.currentHighBid}
              onSuccess={handleBidSuccess}
            />
          ) : (
            <div style={{ opacity: 0.85 }}>
              Please <Link to="/login">login</Link> to place a bid.
            </div>
          )}


        </div>
      </div>
    </div>
  );
}
