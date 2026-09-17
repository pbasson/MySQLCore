import { fail, sleep } from "k6";
import { Rate, Trend } from "k6/metrics";
import { page, pause, thresholds, summaryTrendStats } from "./ImageGallery-PerformanceHelpers.js";

const recovered = new Rate("recovered_requests");
const baseline = new Trend("baseline_duration", true);
export const options = {
  setupTimeout: "2m",
  scenarios: {
    spike: { executor: "constant-vus", vus: Number(__ENV.VUS || 25), duration: "30s", gracefulStop: "10s", exec: "spike" },
    // Spike has completely stopped by 40s; allow 20s to settle before measuring.
    recovery: { executor: "constant-vus", vus: 1, startTime: "60s", duration: "30s", exec: "recovery" },
  },
  summaryTrendStats,
  thresholds: {
    ...thresholds,
    "http_req_duration{phase:recovery}": thresholds.http_req_duration,
    "request_failures{phase:recovery}": ["rate<0.01"],
    "http_reqs{phase:recovery}": ["count>=20"],
    recovered_requests: ["rate>=0.95"],
  },
};
export function setup() {
  if (!page("warmup").ok) fail("Seed a gallery first; baseline page must return records.");
  const samples = [];
  for (let i = 0; i < 100; i++) {
    const result = page("baseline");
    if (!result.ok) fail("Baseline failed; cannot assess recovery.");
    samples.push(result.res.timings.duration);
    baseline.add(result.res.timings.duration);
    sleep(0.2);
  }
  samples.sort((a, b) => a - b);
  const p95 = samples[94];
  // Relative to this run, with a noise floor for fast localhost responses.
  const limit = Math.max(p95 * Number(__ENV.RECOVERY_FACTOR || 3), Number(__ENV.RECOVERY_FLOOR_MS || 500));
  console.log(`Baseline p95=${p95.toFixed(2)}ms; at least 95% of recovery requests must succeed within ${limit.toFixed(2)}ms`);
  return { limit };
}
export function spike() { page("spike"); pause(); }
export function recovery(data) {
  const result = page("recovery");
  recovered.add(result.ok && result.res.timings.duration <= data.limit);
  pause();
}
