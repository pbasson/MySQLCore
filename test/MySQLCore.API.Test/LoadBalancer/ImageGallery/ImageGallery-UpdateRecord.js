import http from "k6/http";
import { check, fail } from "k6";

const apiKey = __ENV.X_API_KEY || __ENV.API_KEY || __ENV.MYSQLCORE_API_KEY || __ENV["X-API-KEY"];
const baseUrl = (__ENV.BASE_URL || "http://localhost:5820").replace(/\/$/, "");

export const options = {
  vus: Number(__ENV.VUS || 1),
  iterations: Number(__ENV.ITERATIONS || 1),
};

function getPayload() {
  if (__ENV.UPDATE_IMAGE_GALLERY_JSON) {
    try {
      return JSON.parse(__ENV.UPDATE_IMAGE_GALLERY_JSON);
    } catch (error) {
      fail(`UPDATE_IMAGE_GALLERY_JSON is not valid JSON: ${error.message}`);
    }
  }

  const imageGalleryId = Number(__ENV.IMAGE_GALLERY_ID || __ENV.ID);
  if (!Number.isInteger(imageGalleryId) || imageGalleryId <= 0) {
    fail("IMAGE_GALLERY_ID must be a positive integer.");
  }

  return {
    imageGalleryId,
    galleryName: __ENV.GALLERY_NAME || `Updated Load Test Gallery ${imageGalleryId}`,
    galleryPath: __ENV.GALLERY_PATH || "/load-test/updated",
    imageFile: [
      {
        imageFileId: Number(__ENV.IMAGE_FILE_ID || 0),
        imageGalleryId,
        imageName: __ENV.IMAGE_NAME || "updated-load-test-image.jpg",
        imagePosition: Number(__ENV.IMAGE_POSITION || 1),
      },
    ],
  };
}

export default function () {
  if (!apiKey) {
    fail("Missing API key. Run with: k6 run -e X_API_KEY=<your-api-key> -e IMAGE_GALLERY_ID=<id> ImageGallery-UpdateRecord.js");
  }

  const payload = JSON.stringify(getPayload());
  const res = http.put(`${baseUrl}/api/image-gallery/update`, payload, {
    headers: {
      accept: "text/plain",
      "Content-Type": "application/json",
      "X-API-KEY": apiKey,
    },
  });

  console.log(`Status: ${res.status}`);
  console.log(`Body: ${res.body}`);

  check(res, {
    "status is 200": (r) => r.status === 200,
  });
}
