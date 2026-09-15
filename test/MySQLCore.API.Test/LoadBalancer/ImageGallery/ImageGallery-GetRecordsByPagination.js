import { page, pause, thresholds, summaryTrendStats } from "./ImageGallery-PerformanceHelpers.js";

export const options = {
  vus: Number(__ENV.VUS || 1),
  ...(__ENV.DURATION ? { duration: __ENV.DURATION } : { iterations: Number(__ENV.ITERATIONS || 1) }),
  thresholds,
  summaryTrendStats,
};

export default function () {
  page();
  pause();
}
