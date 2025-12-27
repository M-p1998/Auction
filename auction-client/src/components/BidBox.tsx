// import { useState } from "react";
// import { placeBid } from "../api/bidsClient";

// type Props = {
//   auctionId: string;
//   reservePrice: number;
//   currentHighBid?: number;
//   onCancel: () => void;
//   onSuccess?: () => void;
// };

// export default function BidBox({ auctionId, reservePrice, currentHighBid, onCancel,onSuccess}: Props) {
//   // const [amount, setAmount] = useState<number>(0);
//   const [amount, setAmount] = useState<number | "">("");
//   const [error, setError] = useState<string>("");
//   const [saving, setSaving] = useState(false);

//   async function submit() {
//     setError("");

//     if (!amount || amount <= 0) {
//       setError("Bid amount must be greater than 0.");
//       return;
//     }

//     if (amount < reservePrice) {
//       setError(`Bid must be greater than or equal to reserve price ($${reservePrice}).`);
//       return;
//     }

//     if (typeof currentHighBid === "number" && amount <= currentHighBid) {
//       setError(`Bid must be higher than the current bid ($${currentHighBid}).`);
//       return;
//     }

//     try {
//       setSaving(true);
//       await placeBid({ auctionId, amount });
//       // alert("Bid placed!");
//       // setAmount(0);
//       // onSuccess();
//     // eslint-disable-next-line @typescript-eslint/no-explicit-any
//     } catch (e: any) {
//       setError(e?.response?.data?.message ?? "Failed to place bid.");
//     } finally {
//       setSaving(false);
//     }
//   }

//   return (
//     <div style={{ display: "grid", gap: 10, maxWidth: 420 }}>
//       <input
//         type="number"
//         value={amount}
//         onChange={(e) => setAmount(Number(e.target.value))}
//         placeholder="Enter bid amount"
//       />
//       {/* <input
//         type="number"
//         min={Math.max(reservePrice, (currentHighBid ?? 0) + 1)}
//         step={100}
//         value={amount}
//         onChange={(e) => {
//           const value = e.target.value;
//           setAmount(value === "" ? "" : Number(value));
//         }}
//         placeholder={`Min $${Math.max(
//           reservePrice,
//           (currentHighBid ?? 0) 
//         ).toLocaleString()}`}
//       /> */}

//       {error && <div style={{ color: "tomato" }}>{error}</div>}

//       <div style={{ display: "flex", gap: 8 }}>
//         <button
//           className="secondary-btn"
//           onClick={onCancel}
//           disabled={saving}
//         >
//           Cancel
//         </button>

//       <button className="primary-btn" onClick={submit} disabled={saving}>
//         {saving ? "Placing..." : "Place bid"}
//       </button>
//     </div>
//     </div>
//   );
// }


// import { useMemo, useState } from "react";
// import { placeBid } from "../api/bidsClient";

// type Props = {
//   auctionId: string;
//   reservePrice: number;
//   currentHighBid?: number;
//   onCancel: () => void;
//   onSuccess?: () => void;
// };

// export default function BidBox({
//   auctionId,
//   reservePrice,
//   currentHighBid,
//   onCancel,
//   onSuccess,
// }: Props) {
//   const [amountText, setAmountText] = useState<string>("");
//   const [error, setError] = useState<string>("");
//   const [saving, setSaving] = useState(false);

//   const minBid = useMemo(() => {
//     const high = currentHighBid ?? 0;
//     return Math.max(reservePrice, high + 1);
//   }, [reservePrice, currentHighBid]);

//   function handleChange(value: string) {
//     // allow empty
//     if (value === "") {
//       setAmountText("");
//       return;
//     }

//     // digits only
//     const digitsOnly = value.replace(/[^\d]/g, "");

//     // prevent leading zeros (but allow "0" while typing)
//     const normalized = digitsOnly.replace(/^0+(?=\d)/, "");

//     setAmountText(normalized);
//   }

//   async function submit() {
//     setError("");

//     if (amountText.trim() === "") {
//       setError("Enter a bid amount.");
//       return;
//     }

//     const amount = Number(amountText);

//     if (!Number.isFinite(amount) || amount <= 0) {
//       setError("Bid amount must be greater than 0.");
//       return;
//     }

//     if (amount < minBid) {
//       setError(`Bid must be at least $${minBid.toLocaleString()}.`);
//       return;
//     }

//     try {
//       setSaving(true);
//       await placeBid({ auctionId, amount });
//       setAmountText("");
//       onSuccess?.();
//     // eslint-disable-next-line @typescript-eslint/no-explicit-any
//     } catch (e: any) {
//       setError(e?.response?.data?.message ?? "Failed to place bid.");
//     } finally {
//       setSaving(false);
//     }
//   }

//   return (
//     <div className="bidbox">
//       <div className="bidbox-row">
//         <input
//           className="bidbox-input"
//           inputMode="numeric"
//           value={amountText}
//           onChange={(e) => handleChange(e.target.value)}
//           placeholder={`Min $${minBid.toLocaleString()}`}
//           disabled={saving}
//         />

//         <button className="bidbox-btn secondary" onClick={onCancel} disabled={saving}>
//           Cancel
//         </button>

//         <button className="bidbox-btn primary" onClick={submit} disabled={saving}>
//           {saving ? "Placing..." : "Place bid"}
//         </button>
//       </div>

//       {error && <div className="bidbox-error">{error}</div>}
//     </div>
//   );
// }



import { useState } from "react";
import { placeBid } from "../api/bidsClient";

type Props = {
  auctionId: string;
  reservePrice: number;
  currentHighBid?: number;
  onCancel: () => void;
  onSuccess?: () => void;
};

export default function BidBox({
  auctionId,
  reservePrice,
  currentHighBid,
  onCancel,
  onSuccess,
}: Props) {
  const minBid = Math.max(reservePrice, (currentHighBid ?? 0) + 1);

  const [amount, setAmount] = useState<number | "">("");
  const [error, setError] = useState("");
  const [saving, setSaving] = useState(false);

  async function submit() {
    setError("");

    if (amount === "" || amount < minBid) {
      setError(`Bid must be at least $${minBid.toLocaleString()}`);
      return;
    }

    try {
      setSaving(true);
      await placeBid({ auctionId, amount });
      onSuccess?.();
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (e: any) {
      setError(e?.response?.data?.message ?? "Failed to place bid");
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="bidbox">
      <input
        type="number"
        min={minBid}
        // step={100}
        value={amount}
        placeholder={`Min $${minBid.toLocaleString()}`}
        onChange={(e) =>
          setAmount(e.target.value === "" ? "" : Number(e.target.value))
        }
        className="bidbox-input"
      />

      {error && <div className="bid-error">{error}</div>}

      <div className="bidbox-actions">
        <button className="bidbox btn-secondary" onClick={onCancel} disabled={saving}>
          Cancel
        </button>

        <button className="bidbox btn-primary" onClick={submit} disabled={saving}>
          {saving ? "Placing..." : "Place bid"}
        </button>
      </div>
    </div>
  );
}
