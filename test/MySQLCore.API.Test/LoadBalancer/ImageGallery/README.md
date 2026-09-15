# ImageGallery k6 scenarios

These are standalone k6 scripts against the existing API. Run from this directory.
Start the normal local API, MySQL, Redis and required messaging dependencies first;
use a disposable development database and its configured API key. No production
code or project structure is changed by this suite.

```bash
cd test/MySQLCore.API.Test/LoadBalancer/ImageGallery
export BASE_URL=http://localhost:5820
export X_API_KEY='<your-local-api-key>'
```

`API_KEY`, `MYSQLCORE_API_KEY` and `X-API-KEY` remain supported aliases. Do not commit
keys. Reads require a populated page (`PAGE=1` by default). The existing create
script can seed one gallery if needed; it prints the returned ID:

```bash
k6 run ImageGallery-CreateRecord.js
```

## Existing coverage and gaps

| Scenario | Existing coverage | Extension |
| --- | --- | --- |
| Baseline | By-ID, by-name, pagination, create and update default to one VU/one iteration; usable as smoke tests, insufficient latency samples | Reuse pagination with `DURATION`, percentiles, failure thresholds and no per-request console output |
| Increasing concurrency | GetAllRecords defaults to 500 VUs/1,000 shared iterations, a fixed load rather than an increasing ramp | IncreasingLoad ramps 1 → 10 → 25 VUs, holds, then ramps down |
| Mixed CRUD | Independent create/read/update scripts; no delete or linked workflow | MixedCRUD owns a gallery per iteration and performs five reads and three writes |
| Same-entity contention | By-ID/update can share an ID with VU overrides; update defaults imageFileId to zero, potentially replacing images | Contention creates one shared gallery/image, preserves both IDs, and runs concurrent writes and reads |
| Spike/recovery | None | SpikeRecovery measures a baseline, applies an abrupt spike, allows settling, and gates recovery |

Other existing endpoint scripts retain their defaults. The existing GetAllRecords
fixed-load test is not duplicated. Its per-response logging can distort results;
the new performance workflows do not log response bodies.

## Run each scenario

```bash
# 1. Baseline: one VU for a minute (existing script)
k6 run -e DURATION=1m --summary-export=baseline-summary.json ImageGallery-GetRecordsByPagination.js

# 2. Increasing load: 100 seconds, peak defaults to 25 VUs
k6 run --out json=increasing-samples.json --summary-export=increasing-summary.json ImageGallery-IncreasingLoad.js

# 3. Mixed CRUD: five VUs for a minute
k6 run --out json=mixed-samples.json --summary-export=mixed-summary.json ImageGallery-MixedCRUD.js

# 4. Shared gallery and image contention: ten VUs for a minute
k6 run --out json=contention-samples.json --summary-export=contention-summary.json ImageGallery-Contention.js

# 5. Spike and recovery: about 110 seconds plus request time
k6 run --out json=spike-samples.json --summary-export=spike-summary.json ImageGallery-SpikeRecovery.js
```

Output files are local run artifacts; do not commit them. `VUS` changes baseline,
CRUD and contention concurrency, the ramp peak, or spike concurrency. `DURATION`
changes baseline, CRUD and contention duration. Ramp and spike schedules are
explicitly defined in their files so phase timings are easy to understand.
`THINK_TIME` defaults to 0.2 seconds between iterations/actions; the spike baseline
uses that same pacing by default. Use defaults when comparing baseline/recovery.
`REQUEST_TIMEOUT` defaults to `10s`.

CRUD and contention create only test-owned galleries and delete them through the
API. Mixed CRUD uses `finally`; contention uses `teardown`. Hard interruption,
setup failure or ambiguous create timeouts can still leave test data. Contention
prints its fixture ID, and failed deletes print the affected ID. Review galleries
with the `k6-` prefix and `/load-test` path after an interrupted run. Writes also
create outbox messages; deleting galleries does not remove those messages or
cached search results. Use a disposable stack/reset it between comparable runs.

## Metrics and interpretation

All five scenarios show `http_req_duration` (average, median, maximum, p95, p99),
`http_reqs` (count and requests/second), and `http_req_failed`. `request_failures`
also counts invalid JSON, unexpected status and unsuccessful application results
(e.g. HTTP 200 with `false` from delete). `checks` shows response assertions.
A threshold failure makes k6 exit nonzero. Timeout failures may have misleadingly
short duration values: always inspect failure rates alongside latency.

The default gates are deliberately loose local-development regression guards:
p95 < 2,000 ms, p99 < 5,000 ms, HTTP and semantic failure rates < 1%, checks > 99%.
They are not production SLAs or a capacity claim. Adjust `P95_MS` and `P99_MS`
against repeated runs on the same machine/data. There is no minimum throughput
SLA: these are closed-loop VU tests, so request rate depends on latency and pacing.
Short smoke runs cannot provide reliable tail percentiles.

- **Baseline:** record latency and requests/second for later comparisons. Repeated
  pagination reads are mostly a warm-cache API baseline, not a database benchmark.
- **Increasing load:** correlate sample timestamps and `vus` with latency, failures
  and requests/second in each stage. A flat throughput curve with rising latency
  suggests saturation. The aggregate percentiles can hide a bad peak stage.
- **Mixed CRUD:** inspect samples by `operation` (`create`, `read`, `search`,
  `update`, `read-after-update`, `delete`). `iterations` counts completed workflows,
  whereas `http_reqs` counts requests. The read after update checks persisted API
  values in a sequential private-entity workflow; delete checks the returned bool.
- **Contention:** inspect update latency, failures, API errors and MySQL diagnostics
  for deadlocks/timeouts. All VUs use the same gallery ID and existing image ID.
  Reads and a final teardown read require the gallery and exactly one image to
  remain. Another writer can win, so reading your own last value is not required.
  This exercises concurrent database writes; it does not prove serializability or
  detect every lost update. `fixture` and `verification` phase tags separate setup
  and teardown requests from workload samples.
- **Spike/recovery:** setup warms a page then collects 100 single-VU baseline
  samples. A 25-VU spike runs for 30s, with at most 10s graceful drain. Recovery
  starts at 60s and runs one VU for 30s, leaving at least 20s to settle.
  `baseline_duration` reports baseline tails; `http_req_duration{phase:recovery}`
  reports recovery tails. At least 95% of recovery requests must succeed within
  `max(baseline p95 × 3, 500ms)` via `recovered_requests`. The factor/floor are
  configurable using `RECOVERY_FACTOR`/`RECOVERY_FLOOR_MS`; the floor accommodates
  localhost timing noise. Recovery also requires at least 20 requests and has its own latency/failure gates. All
  requests, including spike requests, remain subject to the overall gates.
  This tests settling after load is removed, not sustained low load during settling.

JSON samples carry stable `operation`/`name` and `phase` tags rather than unique
IDs in the request name. Filter by phase and divide request counts by that phase's
elapsed seconds for phase throughput; the overall summary rate includes setup
and idle recovery time. Inspect API logs and existing tracing alongside k6;
client latency alone cannot identify a database lock or attribute a bottleneck.

## Analysis findings and validation limits

No production behavior was modified. Source inspection found:

- Create/update/delete invalidate the all-records cache; update/delete also
  invalidate by-ID. Pagination and gallery-name cache entries are not invalidated,
  so reads can be stale until expiry. Pagination load primarily tests the cache
  path. A cache-miss read racing an update can also refill the by-ID cache with an
  older value; this is a source-level race risk, not a reproduced runtime failure.
- Gallery/image updates have no configured optimistic concurrency token. Writers
  can overwrite each other's fields without an explicit stale-write conflict.
  The contention test intentionally reports request health without claiming
  lost-update protection.
- The delete semaphore belongs to each repository instance, and the repository
  is scoped per request. It therefore does not serialize deletes across HTTP
  requests. This is not itself evidence of a database locking failure.

The default API at localhost:5820 was unreachable during implementation, including
an outside-sandbox check. No actual MySQL deadlock, timeout, throughput limit or
latency result could be established. `k6 inspect` validates every script. A
separate temporary HTTP stub passed all five workflows at two VUs (shortened
baseline/ramp/CRUD/contention runs, full spike/recovery timing). Fixture cleanup
left only the original seed. An injected HTTP-200/false delete response correctly
failed the semantic thresholds with k6 exit code 99. These checks exercise script
flow/response contracts; stub results are not ImageGallery performance measurements.
