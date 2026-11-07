# STAGE ACCEPTANCE — P07 / Engineer-defined raw baselines (backend)

## Source lock
- **Backend:** `AeroTech.ECTM.V1`, uncommitted on top of `3ef5fd9` (P06).
- **Frontend:** `d4178af`, not touched.
- **Authority:** `docs/ECTM-P07-Baseline-Pack-v2.0` and the owner rulings of 2026-11-07 (see DECISIONS.md).
- **Migration `AddBaselines`:**
  - It adds 4 tables: Baselines, BaselineParameters, BaselinePrimarySelections and BaselineParameterFlights.
  - It is applied to `DotAirEctmV1`.
- **BASELINE_RULES v2:** published on `DotAirEctmV1` (id `1568577849164562432`). v1 is the seed draft that the host creates at startup.
- **Routes:** 13 new, 76 in total. Job kind `BASELINE_EVALUATION`.

## What the owner can click (pack v2.0 docs/07 demo, through the API)
1. `POST engines/{id}/baselines/prefill-file` with the CSV or XLSX of the first flight after shop and the engine position. It returns grouped takeoff and cruise suggestions and stores nothing.
2. `POST engines/{id}/baselines` with the edited or manual values. Then `POST baselines/{id}/publish` with the confirmation, which queues the evaluation job.
3. A second, manual baseline for the same measurement. `PUT engines/{id}/baselines/primary` picks one of the two.
4. Accept a flight in P06. The job writes its comparisons, read with `GET flights/{id}/baseline-comparisons` and `GET engines/{id}/baseline-trend`.
5. HOLD the flight: it shows NOT_FINAL at once. After the job its rows are withdrawn. RELEASE + ACCEPT evaluates it again.
6. `clone`, `retire` and `reconcile`.

The acceptance test `BaselineWalkthroughTests` runs steps 1–6 on real files 9072 and 26.

## Results
| Gate | Result |
|---|---|
| Independent frozen fixture (8 cases, SHA-256 equal to the pack manifest) | PASS |
| New conformance tests (Baselines: 35) | PASS |
| Conformance, full | 377 / 382. The 5 failures are environmental: `samples/…/2026-10-02_EP-DOA_6950.csv` is open in Excel (IOException). They are the P04–P06 corpus tests; P07 code is not involved. Rerun them after closing Excel. |
| Acceptance, full (throwaway databases) | 82 / 82 PASS, including the walkthrough |
| B07-01 … B07-19, B07-21 | PASS (TEST-PLAN.md) |
| B07-20 browser E2E | NOT_RUN: frontend agent |
| P06 goldens | Parity 8/8 PASS. The 102-file corpus is BLOCKED by the locked file. |

## Verdict
- **SOFTWARE_VERIFIED:**
  - Backend: PASS.
  - Whole P07: PENDING, because the frontend and the connected demo are the frontend agent's step.
- **ENGINEERING_APPROVED:** PENDING. The tolerances are CANDIDATE; correction is P08.
- **OPERATIONAL_READY:** PENDING. Security is deferred by the owner.

## Deviations from the pack
- The corrected columns are returned as null but are not stored in P07; P08/P09 add them to the same table.
- The client does not send `unit`, `statistic` or `valueOrigin`; the server derives them, so they cannot be forged.
- The baseline register R-P08 is `GET engines/{id}/baselines`, with no separate route (pack docs/05).
- Rulings 1 and 3 (Consumers, ApiResult) stand as in P00.
