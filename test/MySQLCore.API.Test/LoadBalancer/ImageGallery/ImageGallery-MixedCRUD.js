import { fail } from "k6";
import { createGallery, readGallery, updateGallery, deleteGallery, request, pause, thresholds, summaryTrendStats } from "./ImageGallery-PerformanceHelpers.js";

export const options = { vus: Number(__ENV.VUS || 5), duration: __ENV.DURATION || "1m", thresholds, summaryTrendStats };

// Each VU owns its gallery: five reads and three writes per complete workflow.
export default function () {
  const id = createGallery();
  try {
    const initial = readGallery(id);
    if (!initial.ok) fail("Cannot read created gallery.");
    const record = initial.body.record;
    pause();
    readGallery(id);
    request("GET", `/by-name/${encodeURIComponent(record.galleryName)}`, null, "search", b => Array.isArray(b.records) && b.records.some(r => r.imageGalleryId === id));
    pause();
    const marker = `edited-${__VU}-${__ITER}`;
    updateGallery(record, marker);
    // Sequential, private entity: this read must observe our own completed write.
    request("GET", `/${id}`, null, "read-after-update", b => b.record && b.record.galleryPath === `/load-test/${marker}` && b.record.imageFile.length === 1 && b.record.imageFile[0].imageName === `${marker}.jpg`);
    readGallery(id);
    pause();
  } finally {
    deleteGallery(id);
  }
}
