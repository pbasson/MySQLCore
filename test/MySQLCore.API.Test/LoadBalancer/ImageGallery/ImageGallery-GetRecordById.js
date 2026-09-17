import http from "k6/http";
import { check, fail } from "k6";

const apiKey = __ENV.X_API_KEY || __ENV.API_KEY || __ENV.MYSQLCORE_API_KEY || __ENV["X-API-KEY"];
const baseUrl = (__ENV.BASE_URL || "http://localhost:5820").replace(/\/$/, "");
const imageGalleryId = Number(__ENV.IMAGE_GALLERY_ID || __ENV.ID);

export const options = {
  vus: Number(__ENV.VUS || 1),
  iterations: Number(__ENV.ITERATIONS || 1),
};

export default function () {
  if (!apiKey) {
    fail("Missing API key. Run with: k6 run -e X_API_KEY=<your-api-key> -e IMAGE_GALLERY_ID=<id> ImageGallery-GetRecordById.js");
  }

  if (!Number.isInteger(imageGalleryId) || imageGalleryId <= 0) {
    fail("IMAGE_GALLERY_ID must be a positive integer.");
  }

  const res = http.get(`${baseUrl}/api/image-gallery/${imageGalleryId}`, {
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
