// import { useEffect, useState } from "react";

// export function Countdown({ end }: { end: string }) {
//   const [time, setTime] = useState("");

//   useEffect(() => {
//     const timer = setInterval(() => {
//       const diff = new Date(end).getTime() - Date.now();

//       if (diff <= 0) {
//         setTime("Ended");
//         clearInterval(timer);
//       } else {
//         const h = Math.floor(diff / 3600000);
//         const m = Math.floor((diff % 3600000) / 60000);
//         const s = Math.floor((diff % 60000) / 1000);
//         setTime(`${h}h ${m}m ${s}s`);
//       }
//     }, 1000);

//     return () => clearInterval(timer);
//   }, [end]);

//   return <div style={{ fontWeight: 700 }}>Ends in: {time}</div>;
// }


import { useEffect, useState } from "react";

export default function Countdown({ end }: { end: string }) {
  const [timeLeft, setTimeLeft] = useState("");

  useEffect(() => {
    const interval = setInterval(() => {
      const diff = new Date(end).getTime() - Date.now();

      if (diff <= 0) {
        setTimeLeft("Auction ended");
        clearInterval(interval);
        return;
      }

      const hours = Math.floor(diff / (1000 * 60 * 60));
      const minutes = Math.floor((diff / (1000 * 60)) % 60);
      const seconds = Math.floor((diff / 1000) % 60);

      setTimeLeft(`${hours}h ${minutes}m ${seconds}s`);
    }, 1000);

    return () => clearInterval(interval);
  }, [end]);

  return <strong>{timeLeft}</strong>;
}



