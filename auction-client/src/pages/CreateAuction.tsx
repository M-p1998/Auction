import { useState } from "react";
import { createAuction } from "../api/auctionsClient";
import { useNavigate } from "react-router-dom";

type Errors = Partial<Record<
  | "make"
  | "model"
  | "year"
  | "mileage"
  | "reservePrice"
  | "imageUrl"
  | "auctionEnd",
  string
>>;

export default function CreateAuction() {
  const nav = useNavigate();

  const [make, setMake] = useState("");
  const [model, setModel] = useState("");
  const [year, setYear] = useState<number>(2024);
  const [color, setColor] = useState("");
  const [mileage, setMileage] = useState<number>(0);
  const [imageUrl, setImageUrl] = useState("");
  const [reservePrice, setReservePrice] = useState<number>(0);
  const [auctionEnd, setAuctionEnd] = useState(() => {
    const d = new Date();
    d.setDate(d.getDate() + 7); // default = 7 days from now
    return d.toISOString().slice(0, 16);
  });


  const [errors, setErrors] = useState<Errors>({});
  const [serverError, setServerError] = useState<string | null>(null);

  function validate(): boolean {
    const e: Errors = {};

    if (!make) e.make = "Make is required";
    if (!model) e.model = "Model is required";
    if (year < 1980 || year > new Date().getFullYear() + 1)
      e.year = "Enter a valid car year";
    if (mileage < 0) e.mileage = "Mileage cannot be negative";
    if (reservePrice <= 0)
      e.reservePrice = "Reserve price must be greater than 0";
    if (!imageUrl.startsWith("http"))
      e.imageUrl = "Enter a valid image URL";

    const endDate = new Date(auctionEnd);
    if (isNaN(endDate.getTime()) || endDate <= new Date()) {
      e.auctionEnd = "Auction end date must be in the future";
    }

    setErrors(e);
    return Object.keys(e).length === 0;
  }

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setServerError(null);

    if (!validate()) return;

    try {
      await createAuction({
        make,
        model,
        year,
        color,
        mileage,
        imageUrl,
        reservePrice,
        auctionEnd: new Date(auctionEnd).toISOString(),
      });

      nav("/auctions"); 
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (err: any) {
      setServerError(err?.response?.data ?? "Failed to create auction");
    }
  }

  return (
    <div className="page-container">
      <div className="form-container">
        <h2>Create Car Auction</h2>

        <form onSubmit={onSubmit} className="auction-form">
          <div className="form-group">
              <label>Make</label>
              <input
              placeholder="Toyota"
              value={make}
              onChange={e => setMake(e.target.value)}
              />
              {errors.make && <small className="field-error">{errors.make}</small>}
          </div>

          <div className="form-group">
              <label>Model</label>
              <input
              placeholder="Camry"
              value={model}
              onChange={e => setModel(e.target.value)}
              />
              {errors.model && <small className="field-error">{errors.model}</small>}
          </div>
          <div className="form-group">
              <label>Year</label>
              <input
              type="number"
              value={year}
              onChange={e => setYear(+e.target.value)}
              />
              {errors.year && <small className="field-error">{errors.year}</small>}
          </div>
          <div className="form-group">
              <label>Color</label>
              <input
              placeholder="Black"
              value={color}
              onChange={e => setColor(e.target.value)}
              />
          </div>
          <div className="form-group">
              <label>Mileage (miles)</label>
              <input
              type="number"
              value={mileage}
              onChange={e => setMileage(+e.target.value)}
              />
              {errors.mileage && <small className="field-error">{errors.mileage}</small>}
          </div>
          <div className="form-group">
              <label>Image URL</label>
              <input
              placeholder="https://example.com/car.jpg"
              value={imageUrl}
              onChange={e => setImageUrl(e.target.value)}
              />
              {errors.imageUrl && <small className="field-error">{errors.imageUrl}</small>}
          </div>
          <div className="form-group">
              <label>Reserve Price ($)</label>
              <input
              type="number"
              value={reservePrice}
              onChange={e => setReservePrice(+e.target.value)}
              />
              {errors.reservePrice && (
              <small className="field-error">{errors.reservePrice}</small>
              )}
          </div>
          <div className="form-group">
            <label>Auction End</label>
            <input
              type="datetime-local"
              min={new Date().toISOString().slice(0, 16)}
              value={auctionEnd}
              onChange={e => setAuctionEnd(e.target.value)}
            />
          </div>
          {errors.auctionEnd && (
            <small className="field-error">{errors.auctionEnd}</small>
          )}

          <button className="primary-btn">Create Auction</button>

          {serverError && <p className="error">{serverError}</p>}
          

        </form>
          

      </div>
    </div>
  );
}
