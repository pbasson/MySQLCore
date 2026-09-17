import http from "k6/http";
import { check, fail, sleep } from "k6";
import { Rate } from "k6/metrics";

const apiKey = __ENV.X_API_KEY || __ENV.API_KEY || __ENV.MYSQLCORE_API_KEY || __ENV["X-API-KEY"];
const baseUrl = (__ENV.BASE_URL || "http://localhost:5820").replace(/\/$/, "");
export const requestFailures = new Rate("request_failures");
export const summaryTrendStats = ["avg", "min", "med", "max", "p(95)", "p(99)"];
// Loose local-development regression guards, not production SLAs.
export const thresholds = {
  http_req_duration: [`p(95)<${__ENV.P95_MS || 2000}`, `p(99)<${__ENV.P99_MS || 5000}`],
  http_req_failed: ["rate<0.01"],
  http_reqs: ["count>0"],
  request_failures: ["rate<0.01"],
  checks: ["rate>0.99"],
};

export function request(method, path, payload, operation, valid = () => true, phase = "workload") {
  if (!apiKey) fail("Missing API key: set X_API_KEY (or API_KEY/MYSQLCORE_API_KEY).");
  const res = http.request(method, `${baseUrl}/api/image-gallery${path}`, payload == null ? null : JSON.stringify(payload), {
    headers: { accept: "text/plain", "Content-Type": "application/json", "X-API-KEY": apiKey },
    timeout: __ENV.REQUEST_TIMEOUT || "10s",
    tags: { name: operation, operation, phase },
    responseCallback: http.expectedStatuses(200),
  });
  let body = null;
  try { body = res.json(); } catch (_) { /* Count invalid JSON as a failed request. */ }
  let validResponse = false;
  try { validResponse = res.status === 200 && body !== null && Boolean(valid(body)); } catch (_) { /* Malformed response shape. */ }
  const ok = check(res, { [operation + " succeeded"]: () => validResponse }, { phase });
  requestFailures.add(!ok, { operation, phase });
  return { res, body, ok };
}

export function page(phase = "workload") {
  const n = Number(__ENV.PAGE || 1);
  if (!Number.isInteger(n) || n <= 0) fail("PAGE must be a positive integer.");
  return request("GET", `/by-page/${n}`, null, "page", b => Array.isArray(b.records) && b.records.length > 0, phase);
}
export function pause() { sleep(Number(__ENV.THINK_TIME || 0.2)); }
export function createGallery(phase = "workload") {
  const payload = {
    galleryName: `k6-${Date.now()}-${__VU}-${typeof __ITER === "undefined" ? "setup" : __ITER}-${Math.random().toString(36).slice(2, 8)}`,
    galleryPath: "/load-test",
    imageFile: [{ imageName: "load-test-image.jpg", imagePosition: 1 }],
  };
  const created = request("POST", "/create", payload, "create", b => b.success === true && b.id > 0, phase);
  if (!created.ok) fail("Cannot create test gallery; inspect create failures (an interrupted response may leave a record).");
  return created.body.id;
}
export function readGallery(id, phase = "workload") {
  return request("GET", `/${id}`, null, "read", b => b.record && b.record.imageGalleryId === id && b.record.imageFile && b.record.imageFile.length === 1, phase);
}
export function updateGallery(record, marker, phase = "workload") {
  return request("PUT", "/update", {
    imageGalleryId: record.imageGalleryId,
    galleryName: record.galleryName,
    galleryPath: `/load-test/${marker}`,
    imageFile: record.imageFile.map(f => ({ ...f, imageName: `${marker}.jpg` })),
  }, "update", b => b.success === true && b.id === record.imageGalleryId, phase);
}
export function deleteGallery(id, phase = "workload") {
  const result = request("DELETE", `/delete/${id}`, null, "delete", b => b === true, phase);
  if (!result.ok) console.error(`Cleanup failed for test gallery ${id}; remove it manually.`);
}
export function setupGallery() {
  const id = createGallery("fixture");
  console.log(`Test-owned gallery ID: ${id}`);
  const result = readGallery(id, "fixture");
  if (!result.ok) { deleteGallery(id, "fixture"); fail("Cannot read test fixture."); }
  return result.body.record;
}
export function teardownGallery(record) { deleteGallery(record.imageGalleryId, "fixture"); }
