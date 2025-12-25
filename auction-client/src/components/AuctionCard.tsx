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

type Props = {
  auction: AuctionDto;
};

export default function AuctionCard({ auction }: Props) {
  const nav = useNavigate();
  const { isAdmin } = useAuth();
  const isEnded = new Date(auction.auctionEnd) <= new Date();


  return (
    <div className="auction-card">
      {/* TOP BID INFO */}
      <div className="auction-bid-header">
        <small>CURRENT BID:</small>
        <div className="auction-bid-amount">
          ${auction.currentHighBid?.toLocaleString() ?? "0.00"}
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
      </div>

      {/* ACTIONS */}
      <div className="auction-actions">
        <button className="bid-btn">BID</button>
      </div>
    </div>
  );
}
