import http from "k6/http";
import { check, fail } from "k6";

const apiKey = __ENV.X_API_KEY || __ENV.API_KEY || __ENV.MYSQLCORE_API_KEY || __ENV["X-API-KEY"];
const baseUrl = (__ENV.BASE_URL || "http://localhost:5820").replace(/\/$/, "");
const galleryName = __ENV.GALLERY_NAME;

export const options = {
  vus: Number(__ENV.VUS || 1),
  iterations: Number(__ENV.ITERATIONS || 1),
};

export default function () {
  if (!apiKey) {
    fail("Missing API key. Run with: k6 run -e X_API_KEY=<your-api-key> -e GALLERY_NAME=<name> ImageGallery-GetRecordsByGalleryName.js");
  }

  if (!galleryName || galleryName.length <= 3) {
    fail("GALLERY_NAME must be longer than 3 characters.");
  }

  const encodedGalleryName = encodeURIComponent(galleryName);
  const res = http.get(`${baseUrl}/api/image-gallery/by-name/${encodedGalleryName}`, {
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
