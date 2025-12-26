// import { useNavigate } from "react-router-dom";
// import Countdown from "./Countdown";
// import type { AuctionDto } from "../types/dto";

// type Props = {
//   auction: AuctionDto;
// };

// export default function AuctionCard({ auction }: Props) {
//   const nav = useNavigate();

//   return (
//     <div
//       className="auction-card"
//       onClick={() => nav(`/auctions/${auction.id}`)}
//       style={{ cursor: "pointer" }}
//     >
//       {/* Image */}
//       <div className="auction-image-wrapper">
//         <img
//           className="auction-image"
//           src={auction.imageUrl}
//           alt={`${auction.year} ${auction.make} ${auction.model}`}
//         />
//       </div>

//       {/* Body */}
//       <div className="auction-body">
//         <div className="auction-title">
//           {auction.year} {auction.make} {auction.model}
//         </div>

//         <div className="auction-meta">
//           Mileage: {auction.mileage.toLocaleString()} miles
//         </div>

//         <div className="auction-footer">
//           <div>
//             Reserve: ${auction.reservePrice.toLocaleString()}
//           </div>
//             Ends in: <Countdown end={auction.auctionEnd} />
//         </div>
//       </div>
//     </div>
//   );
// }



import { useNavigate } from "react-router-dom";
import Countdown from "./Countdown";
import type { AuctionDto } from "../types/dto";
import { useAuth } from "../auth/useAuth";
import { useState } from "react";
import BidBox from "./BidBox";


type Props = {
  auction: AuctionDto;
  onBidPlaced?: () => void;
};

export default function AuctionCard({ auction }: Props) {
  const nav = useNavigate();
  const { isAdmin } = useAuth();
  const isEnded = new Date(auction.auctionEnd) <= new Date();
  const hasBid = (auction.currentHighBid ?? 0) > 0;
  const [showBidBox, setShowBidBox] = useState(false);
  // const [showBid, setShowBid] = useState(false);

  const [showBid, setShowBid] = useState(false);

  function toggleBid() {
    setShowBid((v) => !v);
  }


  function onBidPlaced() {
    throw new Error("Function not implemented.");
  }

  return (
    <div className="auction-card">

      <div className="auction-bid-header">
        <small>CURRENT BID:</small>
        <div className="auction-bid-amount">
          ${auction.currentHighBid?.toLocaleString() ?? "0"}
        </div>
        <div className="auction-countdown">
          <Countdown end={auction.auctionEnd} />
        </div>
      </div>

      {/* IMAGE */}
      <div
        className="auction-image-wrapper"
        onClick={() => nav(`/auctions/${auction.id}`)}
      >
        <img
          className="auction-image"
          src={auction.imageUrl}
          alt={auction.model}
        />
      </div>

      {/* BODY */}
      <div className="auction-body">
        <div className="auction-title">
          {auction.year} {auction.make} {auction.model}
        </div>
        <div className="auction-lot">
          Mileage: {auction.mileage} miles
        </div>
        <div className="auction-lot">
          Color: {auction.color || "N/A"}
        </div>

        <div className="auction-lot">
          Reserve Price: ${auction.reservePrice.toLocaleString()}
        </div>
      
      {/* WINNER / NO BID LOGIC */}
      {isEnded && (
        <div className="auction-lot auction-result">
          {hasBid ? (
            <>
              <span className="label">Winner:</span>
              <span className="value">{auction.winner}</span>
            </>
          ) : (
            <span className="no-bid">No bids placed</span>
          )}
        </div>
      )}
      </div>

      {/* ACTIONS */}
      <div className="auction-actions">
         {!isAdmin && (
          <button
            className="auction-btn bid-btn"
            disabled={isEnded}
            // onClick={() => nav(`/auctions/${auction.id}`)}
            onClick={toggleBid}
          >
            {/* {isEnded ? "Ended" : "Bid"} */}
            {isEnded ? "Ended" : showBid ? "Cancel" : "Bid"}
          </button>
        )}

        {/* <button
          className="auction-btn bid-btn"
          disabled={isEnded}
          // eslint-disable-next-line @typescript-eslint/no-explicit-any
          onClick={() => setShowBidBox((v: any) => !v)}
        >
          {isEnded ? "Ended" : showBidBox ? "Cancel" : "Bid"}
        </button>
        {!isAdmin && !isEnded && showBidBox && (
        <div className="inline-bid">
          <BidBox
            auctionId={auction.id}
            reservePrice={auction.reservePrice}
            currentHighBid={auction.currentHighBid}
            onSuccess={() => setShowBidBox(false)}
          />
        </div>
      )} */}

      {/* <div className="auction-actions">
        {!isAdmin && !isEnded && !showBid && (
          <button
            className="auction-btn bid-btn"
            onClick={() => setShowBid(true)}
          >
            Bid
          </button>
        )}

        {!isAdmin && !isEnded && showBid && (
          <BidBox
            auctionId={auction.id}
            reservePrice={auction.reservePrice}
            currentHighBid={auction.currentHighBid}
            onCancel={() => setShowBid(false)}
            onSuccess={() => setShowBid(false)}
          />
        )}

        {isEnded && (
          <button className="auction-btn bid-btn" disabled>
            Ended
          </button>
        )}
      </div> */}



        {isAdmin && (
          <>
            <button
              className="auction-btn admin-btn"
              onClick={() => nav(`/admin/auctions/${auction.id}/edit`)}
            >
              Update
            </button>

            <button
              className="auction-btn delete-btn"
              onClick={() => nav(`/admin/auctions/${auction.id}/delete`)}
            >
              Delete
            </button>
          </>
        )}
      </div>

      {!isAdmin && !isEnded && showBid && (
        <div className="auction-inline-bid">
          <BidBox
            auctionId={auction.id}
            reservePrice={auction.reservePrice}
            currentHighBid={auction.currentHighBid}
            onCancel={() => setShowBid(false)}
            onSuccess={() => {
              setShowBid(false);
              onBidPlaced?.();
            }}
          />
        </div>
      )}
    </div>
  );
}
