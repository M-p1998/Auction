// import { useEffect, useState } from "react";
// import { useNavigate, useParams } from "react-router-dom";
// import { getAuctionById, updateAuction } from "../api/auctionsClient";
// import type { CreateAuctionRequest } from "../types/dto";


// type FormState = CreateAuctionRequest;
// type Errors = Partial<Record<keyof FormState, string>>;

// export default function UpdateAuction() {
//   const { id } = useParams();
//   const nav = useNavigate();
//   const [form, setForm] = useState<FormState | null>(null);
//   // const [error, setError] = useState("");
//   const [errors, setErrors] = useState<Errors>({});
//   const [saving, setSaving] = useState(false);

//   useEffect(() => {
//     (async () => {
//       if (!id) return;
//       const a = await getAuctionById(id);
//       setForm({
//         make: a.make,
//         model: a.model,
//         year: a.year,
//         color: a.color ?? "",
//         mileage: a.mileage,
//         imageUrl: a.imageUrl,
//         reservePrice: a.reservePrice,
//         auctionEnd: a.auctionEnd,
//       });
//     })();
//   }, [id]);

//   // function validate(): string {
//   //   if (!form) return "Missing form";
//   //   if (!form.make.trim()) return "Make is required";
//   //   if (!form.model.trim()) return "Model is required";
//   //   if (form.year < 1980 || form.year > new Date().getFullYear() + 1) return "Year looks invalid";
//   //   if (form.mileage < 0) return "Mileage cannot be negative";
//   //   if (form.reservePrice < 0) return "Reserve price cannot be negative";
//   //   if (!form.imageUrl.trim()) return "Image URL is required";

//   //   const end = new Date(form.auctionEnd).getTime();
//   //   if (Number.isNaN(end)) return "Auction end date is invalid";
//   //   if (end <= Date.now()) return "Auction end must be in the future";

//   //   return "";
//   // }
//   function validateForm(): Errors {
//   if (!form) return {};

//   const e: Errors = {};

//   if (!form.make.trim()) e.make = "Make is required";
//   if (!form.model.trim()) e.model = "Model is required";

//   if (form.year < 1980 || form.year > new Date().getFullYear() + 1) {
//     e.year = "Year looks invalid";
//   }

//   if (form.mileage < 0) e.mileage = "Mileage cannot be negative";
//   if (form.reservePrice < 0) e.reservePrice = "Reserve price cannot be negative";
//   if (!form.imageUrl.trim()) e.imageUrl = "Image URL is required";

//   const end = new Date(form.auctionEnd).getTime();
//   if (Number.isNaN(end)) e.auctionEnd = "Invalid date";
//   else if (end <= Date.now()) e.auctionEnd = "End date must be in the future";

//   return e;
// }


//   async function onSave(e: React.FormEvent) {
//   e.preventDefault();
//   if (!id || !form) return;

//   const v = validateForm();
//   setErrors(v);

//   if (Object.keys(v).length > 0) return;

//   try {
//     setSaving(true);
//     await updateAuction(id, form);
//     nav("/auctions");
//   // eslint-disable-next-line @typescript-eslint/no-explicit-any
//   } catch (err: any) {
//     setErrors(err?.response?.data?.message ?? "Update failed");
//   } finally {
//     setSaving(false);
//   }
// }


//   if (!form) return <div className="p-2">Loading...</div>;

//   return (
//     <div className="form-page">
//         <div className="form-card">
//     <h2>Update Auction</h2>

//     <form onSubmit={onSave}>

//   <label>Make</label>
//   <input
//     value={form.make}
//     onChange={e => setForm({ ...form, make: e.target.value })}
//   />
//   {errors.make && <div className="error">{errors.make}</div>}

//   <label>Model</label>
//   <input
//     value={form.model}
//     onChange={e => setForm({ ...form, model: e.target.value })}
//   />
//   {errors.model && <div className="error">{errors.model}</div>}

//   <label>Year</label>
//   <input
//     type="number"
//     value={form.year}
//     onChange={e => setForm({ ...form, year: +e.target.value })}
//   />
//   {errors.year && <div className="error">{errors.year}</div>}

//   <label>Color</label>
//   <input
//     value={form.color}
//     onChange={e => setForm({ ...form, color: e.target.value })}
//   />

//   <label>Mileage</label>
//   <input
//     type="number"
//     value={form.mileage}
//     onChange={e => setForm({ ...form, mileage: +e.target.value })}
//   />
//   {errors.mileage && <div className="error">{errors.mileage}</div>}

//   <label>Image URL</label>
//   <input
//     value={form.imageUrl}
//     onChange={e => setForm({ ...form, imageUrl: e.target.value })}
//   />
//   {errors.imageUrl && <div className="error">{errors.imageUrl}</div>}

//   <label>Reserve Price</label>
//   <input
//     type="number"
//     value={form.reservePrice}
//     onChange={e => setForm({ ...form, reservePrice: +e.target.value })}
//   />
//   {errors.reservePrice && <div className="error">{errors.reservePrice}</div>}

//   <label>Auction End</label>
//   <input
//     type="datetime-local"
//     value={form.auctionEnd.slice(0, 16)}
//     onChange={e => setForm({ ...form, auctionEnd: e.target.value })}
//   />
//   {errors.auctionEnd && <div className="error">{errors.auctionEnd}</div>}


//   <div className="form-actions">
//     <button
//       type="button"
//       className="btn btn-cancel"
//       onClick={() => nav(-1)}
//     >
//       Cancel
//     </button>

//     <button className="btn btn-primary" disabled={saving}>
//       {saving ? "Saving..." : "Save"}
//     </button>
//   </div>
// </form>

//   </div>
//     </div>
//   );
// }





import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getAuctionById, updateAuction } from "../api/auctionsClient";
import type { CreateAuctionRequest } from "../types/dto";

type Errors = Partial<Record<keyof CreateAuctionRequest, string>>;

export default function UpdateAuction() {
  const { id } = useParams();
  const nav = useNavigate();

  const [form, setForm] = useState<CreateAuctionRequest | null>(null);
  const [errors, setErrors] = useState<Errors>({});
  const [saving, setSaving] = useState(false);
  const [apiError, setApiError] = useState("");


  useEffect(() => {
    if (!id) return;

    (async () => {
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


  function parseLocalDateTime(value: string) {
    const [date, time] = value.split("T");
    const [year, month, day] = date.split("-").map(Number);
    const [hour, minute] = time.split(":").map(Number);

    return new Date(year, month - 1, day, hour, minute).getTime();
  }

  function validate(): Errors {
    if (!form) return {};

    const e: Errors = {};

    if (!form.make.trim()) e.make = "Make is required";
    if (!form.model.trim()) e.model = "Model is required";
    if (form.year < 1980) e.year = "Year looks invalid";
    if (!form.color || !form.color.trim()) {
      e.color = "Color is required";
    }

    // if(form.color && form.color.trim().length === 0) e.color = "Color cannot be empty string";  
    // if (form.mileage < 0) e.mileage = "Mileage cannot be negative";
    if (!form.imageUrl.trim()) e.imageUrl = "Image URL required";
    // if (form.reservePrice <= 0) e.reservePrice = "Invalid price";
    if (Number.isNaN(form.mileage) || form.mileage <= 0)
      e.mileage = "Mileage must be greater than 0";

    if (Number.isNaN(form.reservePrice) || form.reservePrice <= 0)
      e.reservePrice = "Reserve price must be greater than 0";

    // const end = new Date(form.auctionEnd).getTime();
    // if (isNaN(end) || end <= Date.now()) {
    //   e.auctionEnd = "End date must be in the future";
    // }
    // const end = new Date(form.auctionEnd).getTime();
    // if (Number.isNaN(end)) e.auctionEnd = "Invalid date";
    // else if (end <= Date.now()) e.auctionEnd = "End date must be in the future";

    const end = parseLocalDateTime(form.auctionEnd);
    if (end <= Date.now()) {
      e.auctionEnd = "End date must be in the future";
    }

    return e;
  }

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!id || !form) return;

    const v = validate();
    setErrors(v);
    if (Object.keys(v).length > 0) return;

    try {
      setSaving(true);
      await updateAuction(id, form);
      nav("/auctions");
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (err: any) {
      setApiError(err?.response?.data?.message ?? "Update failed");
    } finally {
      setSaving(false);
    }
  }

  if (!form) return <div className="p-2">Loading...</div>;

  return (
    <div className="page-container">
      <div className="form-container">
        <h2>Update Auction</h2>

        <form className="auction-form" onSubmit={onSubmit}>
          {([
            ["make", "Make"],
            ["model", "Model"],
            ["year", "Year", "number"],
            ["color", "Color"],
            ["mileage", "Mileage", "number"],
            ["imageUrl", "Image URL"],
            ["reservePrice", "Reserve Price", "number"],
          ] as const).map(([key, label, type]) => (
            <div className="form-group" key={key}>
              <label>{label}</label>
              {/* <input
                type={type ?? "text"}
                value={form[key]}
                onChange={e =>
                  setForm({ ...form, [key]: type === "number" ? +e.target.value : e.target.value })
                }
              /> */}
              <input
                type={type ?? "text"}
                min={type === "number" ? 1 : undefined}
                value={form[key] === 0 ? "" : form[key]}
                onChange={e =>
                  setForm({
                    ...form,
                    [key]:
                      type === "number"
                        ? e.target.value === ""
                          ? NaN
                          : +e.target.value
                        : e.target.value,
                  })
                }
              />
              {errors[key] && <div className="field-error">{errors[key]}</div>}
            </div>
          ))}

          <div className="form-group">
            <label>Auction End</label>
            <input
              type="datetime-local"
              value={form.auctionEnd.slice(0, 16)}
              onChange={e => setForm({ ...form, auctionEnd: e.target.value })}
            />
            {errors.auctionEnd && <div className="field-error">{errors.auctionEnd}</div>}
          </div>

          {apiError && <div className="error">{apiError}</div>}

          <button className="primary-btn" disabled={saving}>
            {saving ? "Saving..." : "Save Changes"}
          </button>
        </form>
      </div>
    </div>
  );
}
