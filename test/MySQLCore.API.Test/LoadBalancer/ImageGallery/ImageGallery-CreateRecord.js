import http from "k6/http";
import { check, fail } from "k6";

const apiKey = __ENV.X_API_KEY || __ENV.API_KEY || __ENV.MYSQLCORE_API_KEY || __ENV["X-API-KEY"];
const baseUrl = (__ENV.BASE_URL || "http://localhost:5820").replace(/\/$/, "");

export const options = {
  vus: Number(__ENV.VUS || 1),
  iterations: Number(__ENV.ITERATIONS || 1),
};

function getPayload() {
  if (__ENV.CREATE_IMAGE_GALLERY_JSON) {
    try {
      return JSON.parse(__ENV.CREATE_IMAGE_GALLERY_JSON);
    } catch (error) {
      fail(`CREATE_IMAGE_GALLERY_JSON is not valid JSON: ${error.message}`);
    }
  }

  return {
    galleryName: __ENV.GALLERY_NAME || `Load Test Gallery ${Date.now()}`,
    galleryPath: __ENV.GALLERY_PATH || "/load-test",
    imageFile: [
      {
        imageName: __ENV.IMAGE_NAME || "load-test-image.jpg",
        imagePosition: Number(__ENV.IMAGE_POSITION || 1),
      },
    ],
  };
}

export default function () {
  if (!apiKey) {
    fail("Missing API key. Run with: k6 run -e X_API_KEY=<your-api-key> ImageGallery-CreateRecord.js");
  }

  const payload = JSON.stringify(getPayload());
  const res = http.post(`${baseUrl}/api/image-gallery/create`, payload, {
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
