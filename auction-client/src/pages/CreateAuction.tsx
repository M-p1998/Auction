import { useState } from "react";
import { createAuction } from "../api/auctionsClient";

export default function CreateAuction() {
  const [make, setMake] = useState("Toyota");
  const [model, setModel] = useState("Camry");
  const [year, setYear] = useState(2020);
  const [color, setColor] = useState("Black");
  const [mileage, setMileage] = useState(20000);
  const [imageUrl, setImageUrl] = useState("img.jpg");
  const [reservePrice, setReservePrice] = useState(10000);
  const [auctionEnd, setAuctionEnd] = useState<string>(() =>
    new Date(Date.now() + 7 * 86400000).toISOString());

  const [msg, setMsg] = useState<string | null>(null);
  const [err, setErr] = useState<string | null>(null);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setErr(null);
    setMsg(null);

    try {
      await createAuction({
        make, model, year, color, mileage, imageUrl, reservePrice,
        auctionEnd: auctionEnd,
      });
      setMsg("Auction created!");
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (ex: any) {
      setErr(ex?.response?.data ?? "Create failed (are you logged in as Admin?)");
    }
  }

  return (
    <div style={{ maxWidth: 520, margin: "40px auto" }}>
      <h2>Create Auction (Admin)</h2>
      <form onSubmit={onSubmit}>
        <input value={make} onChange={(e) => setMake(e.target.value)} placeholder="Make" style={{ width: "100%", padding: 10 }} />
        <div style={{ height: 10 }} />
        <input value={model} onChange={(e) => setModel(e.target.value)} placeholder="Model" style={{ width: "100%", padding: 10 }} />
        <div style={{ height: 10 }} />
        <input value={year} onChange={(e) => setYear(Number(e.target.value))} placeholder="Year" style={{ width: "100%", padding: 10 }} />
        <div style={{ height: 10 }} />
        <input value={color} onChange={(e) => setColor(e.target.value)} placeholder="Color" style={{ width: "100%", padding: 10 }} />
        <div style={{ height: 10 }} />
        <input value={mileage} onChange={(e) => setMileage(Number(e.target.value))} placeholder="Mileage" style={{ width: "100%", padding: 10 }} />
        <div style={{ height: 10 }} />
        <input value={imageUrl} onChange={(e) => setImageUrl(e.target.value)} placeholder="ImageUrl" style={{ width: "100%", padding: 10 }} />
        <div style={{ height: 10 }} />
        <input value={reservePrice} onChange={(e) => setReservePrice(Number(e.target.value))} placeholder="ReservePrice" style={{ width: "100%", padding: 10 }} />
        <div style={{ height: 10 }} />
        <input value={auctionEnd} onChange={(e) => setAuctionEnd(e.target.value)} placeholder="AuctionEnd ISO" style={{ width: "100%", padding: 10 }} />
        <div style={{ height: 10 }} />
        <button style={{ width: "100%", padding: 10 }}>Create</button>
      </form>

      {msg && <p>{msg}</p>}
      {err && <p style={{ color: "tomato" }}>{String(err)}</p>}
    </div>
  );
}
