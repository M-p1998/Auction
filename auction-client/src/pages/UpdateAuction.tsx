import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getAuctionById, updateAuction } from "../api/auctionsClient";
import type { CreateAuctionRequest } from "../types/dto";

type FormState = CreateAuctionRequest;

export default function UpdateAuction() {
  const { id } = useParams();
  const nav = useNavigate();
  const [form, setForm] = useState<FormState | null>(null);
  const [error, setError] = useState("");
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    (async () => {
      if (!id) return;
      const a = await getAuctionById(id);
      setForm({
        make: a.make,
        model: a.model,
        year: a.year,
        color: a.color ?? "",
        mileage: a.mileage,
        imageUrl: a.imageUrl,
        reservePrice: a.reservePrice,
        auctionEnd: a.auctionEnd,
      });
    })();
  }, [id]);

  function validate(): string {
    if (!form) return "Missing form";
    if (!form.make.trim()) return "Make is required";
    if (!form.model.trim()) return "Model is required";
    if (form.year < 1980 || form.year > new Date().getFullYear() + 1) return "Year looks invalid";
    if (form.mileage < 0) return "Mileage cannot be negative";
    if (form.reservePrice < 0) return "Reserve price cannot be negative";
    if (!form.imageUrl.trim()) return "Image URL is required";

    const end = new Date(form.auctionEnd).getTime();
    if (Number.isNaN(end)) return "Auction end date is invalid";
    if (end <= Date.now()) return "Auction end must be in the future";

    return "";
  }

  async function onSave() {
    if (!id || !form) return;
    setError("");

    const msg = validate();
    if (msg) {
      setError(msg);
      return;
    }

    try {
      setSaving(true);
      await updateAuction(id, form);
      nav(`/auctions/${id}`);
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (e: any) {
      setError(e?.response?.data?.message ?? "Update failed");
    } finally {
      setSaving(false);
    }
  }

  if (!form) return <div className="p-2">Loading...</div>;

  return (
    <div className="p-2" style={{ maxWidth: 600 }}>
      <h2>Update Auction</h2>

      <div style={{ display: "grid", gap: 10 }}>
        <input value={form.make} onChange={(e) => setForm({ ...form, make: e.target.value })} placeholder="Make" />
        <input value={form.model} onChange={(e) => setForm({ ...form, model: e.target.value })} placeholder="Model" />
        <input type="number" value={form.year} onChange={(e) => setForm({ ...form, year: Number(e.target.value) })} placeholder="Year" />
        <input value={form.color ?? ""} onChange={(e) => setForm({ ...form, color: e.target.value })} placeholder="Color" />
        <input type="number" value={form.mileage} onChange={(e) => setForm({ ...form, mileage: Number(e.target.value) })} placeholder="Mileage" />
        <input value={form.imageUrl} onChange={(e) => setForm({ ...form, imageUrl: e.target.value })} placeholder="Image URL" />
        <input type="number" value={form.reservePrice} onChange={(e) => setForm({ ...form, reservePrice: Number(e.target.value) })} placeholder="Reserve Price" />
        <input value={form.auctionEnd} onChange={(e) => setForm({ ...form, auctionEnd: e.target.value })} placeholder="Auction End (ISO)" />

        {error && <div style={{ color: "tomato" }}>{error}</div>}

        <div className="btn-row">
          <button onClick={() => nav(-1)}>Cancel</button>
          <button onClick={onSave} disabled={saving}>
            {saving ? "Saving..." : "Save"}
          </button>
        </div>
      </div>
    </div>
  );
}
