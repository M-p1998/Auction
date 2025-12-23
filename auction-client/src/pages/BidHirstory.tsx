// import { useEffect, useState } from "react";
// import { getMyBids } from "../api/bidsClient";
// import type { BidDto } from "../types/dto";
// import { useNavigate } from "react-router-dom";

// export default function BidHistory() {
//   const [bids, setBids] = useState<BidDto[]>([]);
//   const [loading, setLoading] = useState(true);
//   const nav = useNavigate();

//   useEffect(() => {
//     (async () => {
//       try {
//         const data = await getMyBids();
//         // newest first
//         const sorted = [...data].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
//         setBids(sorted);
//       } finally {
//         setLoading(false);
//       }
//     })();
//   }, []);

//   if (loading) return <div className="p-2">Loading your bids...</div>;

//   return (
//     <div className="p-2">
//       <h2>My Bid History</h2>

//       {bids.length === 0 ? (
//         <div style={{ opacity: 0.85 }}>No bids yet.</div>
//       ) : (
//         <div style={{ display: "grid", gap: 10 }}>
//           {bids.map((b) => (
//             <div key={b.id} className="auction-card" style={{ padding: 12 }}>
//               <div style={{ display: "flex", justifyContent: "space-between", gap: 10, flexWrap: "wrap" }}>
//                 <div>
//                   <div style={{ fontWeight: 800 }}>${b.amount.toLocaleString()}</div>
//                   <div style={{ opacity: 0.8, fontSize: 13 }}>{new Date(b.createdAt).toLocaleString()}</div>
//                 </div>

//                 <button onClick={() => nav(`/auctions/${b.auctionId}`)}>View Auction</button>
//               </div>
//             </div>
//           ))}
//         </div>
//       )}
//     </div>
//   );
// }
