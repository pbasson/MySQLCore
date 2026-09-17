import http from "k6/http";
import { check, fail } from "k6";

const apiKey = __ENV.X_API_KEY || __ENV.API_KEY || __ENV.MYSQLCORE_API_KEY || __ENV["X-API-KEY"];
const baseUrl = (__ENV.BASE_URL || "http://localhost:5820").replace(/\/$/, "");

export const options = {
  vus: Number(__ENV.VUS || 500),
  iterations: Number(__ENV.ITERATIONS || 1000),
};

export default function () {
  if (!apiKey) {
    fail("Missing API key. Run with: k6 run -e X_API_KEY=<your-api-key> ImageGallery-GetAllRecords.js");
  }

  const res = http.get(`${baseUrl}/api/image-gallery`, {
    headers: {
      accept: "text/plain",
      "X-API-KEY": apiKey,
    },
  });

  console.log(`Status: ${res.status}`);
  console.log(`Body: ${res.body}`);

  check(res, {
    "status is 200": (r) => r.status === 200,
  });
}
