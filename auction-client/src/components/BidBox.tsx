import { useState } from "react";
import { placeBid } from "../api/bidsClient";

type Props = {
  auctionId: string;
  reservePrice: number;
  currentHighBid?: number;
};

export default function BidBox({ auctionId, reservePrice, currentHighBid }: Props) {
  const [amount, setAmount] = useState<number>(0);
  const [error, setError] = useState<string>("");
  const [saving, setSaving] = useState(false);

  async function submit() {
    setError("");

    if (!amount || amount <= 0) {
      setError("Bid amount must be greater than 0.");
      return;
    }

    if (amount < reservePrice) {
      setError(`Bid must be greater than or equal to reserve price ($${reservePrice}).`);
      return;
    }

    if (typeof currentHighBid === "number" && amount <= currentHighBid) {
      setError(`Bid must be higher than the current bid ($${currentHighBid}).`);
      return;
    }

    try {
      setSaving(true);
      await placeBid({ auctionId, amount });
      alert("Bid placed!");
      setAmount(0);
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (e: any) {
      setError(e?.response?.data?.message ?? "Failed to place bid.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <div style={{ display: "grid", gap: 10, maxWidth: 420 }}>
      <input
        type="number"
        value={amount}
        onChange={(e) => setAmount(Number(e.target.value))}
        placeholder="Enter bid amount"
      />

      {error && <div style={{ color: "tomato" }}>{error}</div>}

      <button onClick={submit} disabled={saving}>
        {saving ? "Placing..." : "Place bid"}
      </button>
    </div>
  );
}
