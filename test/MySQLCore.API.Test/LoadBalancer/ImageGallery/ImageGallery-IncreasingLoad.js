import { page, pause, thresholds, summaryTrendStats } from "./ImageGallery-PerformanceHelpers.js";

export const options = {
  stages: [
    { duration: "15s", target: 1 },
    { duration: "20s", target: 10 },
    { duration: "20s", target: Number(__ENV.VUS || 25) },
    { duration: "30s", target: Number(__ENV.VUS || 25) },
    { duration: "15s", target: 0 },
  ],
  thresholds,
  summaryTrendStats,
};
export default function () { page(); pause(); }
