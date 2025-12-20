import { useEffect, useState } from "react";
import { getAllAuctions } from "../api/auctionsClient";
import type { AuctionDto } from "../types/dto";

export default function AuctionsList() {
  const [items, setItems] = useState<AuctionDto[]>([]);
  const [err, setErr] = useState<string | null>(null);

  useEffect(() => {
    (async () => {
      try {
        setItems(await getAllAuctions());
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
      } catch (ex: any) {
        setErr(ex?.response?.data ?? "Failed to load auctions");
      }
    })();
  }, []);

  return (
    <div style={{ maxWidth: 900, margin: "30px auto" }}>
      <h2>Auctions</h2>
      {err && <p style={{ color: "tomato" }}>{String(err)}</p>}
      <ul>
        {items.map((a) => (
          <li key={a.id}>
            {a.make} {a.model} ({a.year}) - status: {a.status}
          </li>
        ))}
      </ul>
    </div>
  );
}
