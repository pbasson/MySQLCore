import { setupGallery, teardownGallery, readGallery, updateGallery, pause, thresholds, summaryTrendStats } from "./ImageGallery-PerformanceHelpers.js";

export const options = { vus: Number(__ENV.VUS || 10), duration: __ENV.DURATION || "1m", thresholds, summaryTrendStats };
export const setup = setupGallery;
export function teardown(record) {
  readGallery(record.imageGalleryId, "verification");
  teardownGallery(record);
}
export default function (record) {
  // All VUs write the same gallery AND existing image row; do not create replacement images.
  updateGallery(record, `writer-${__VU}-${__ITER}`);
  readGallery(record.imageGalleryId);
  // Another writer may legitimately have won; do not assert our last value here.
  pause();
}
